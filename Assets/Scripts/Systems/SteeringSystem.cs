using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(NavigationSystem))]
public class SteeringSystem : JobComponentSystem {

    private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
    
    [BurstCompile]
    private struct SteeringJob : IJobForEachWithEntity<Translation, SteeringComponent, LocalToWorld, AiAgentComponent> {
        [ReadOnly] public BufferFromEntity<BufferedNavNode> routes;
        public EntityCommandBuffer.Concurrent entityCommandBuffer;

        public void Execute(Entity entity, int index, ref Translation translation, ref SteeringComponent steeringComponent, ref LocalToWorld localToWorld, ref AiAgentComponent aiAgent) {
            var nodes = routes[entity];
            if (nodes.Length <= 1) return;

            float steeringForce = .005f;
            float maxSpeed = .1f;
            
            var currentVelocity = new Vector3(steeringComponent.Velocity.x, steeringComponent.Velocity.y, steeringComponent.Velocity.z);

            var target = new Vector3(nodes[aiAgent.NavigationIndex].Node.x, 0, nodes[aiAgent.NavigationIndex].Node.z);
            var location = new Vector3(localToWorld.Position.x, 0, localToWorld.Position.z);
            var intendedVelocity = (target - location);
            intendedVelocity.Normalize();
            intendedVelocity *= maxSpeed;

            var steering = Vector3.ClampMagnitude((intendedVelocity - currentVelocity), steeringForce);
            var newDirection = Vector3.ClampMagnitude(currentVelocity + steering, maxSpeed);

            if (Vector3.Distance(target, location) <= .25f) {
                bool destinationReached = aiAgent.destinationReached;
                if (aiAgent.NavigationIndex + 1 >= nodes.Length) destinationReached = true;
                
                entityCommandBuffer.SetComponent(index, entity, new AiAgentComponent() {
                    NavigationIndex = aiAgent.NavigationIndex + 1,
                    DeferredFrames = aiAgent.DeferredFrames,
                    destinationReached = destinationReached,
                    NavigationTotal = aiAgent.NavigationTotal
                });
            }
            
            entityCommandBuffer.SetComponent(index, entity, new SteeringComponent { Velocity = new float3(newDirection.x, newDirection.y, newDirection.z)});
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var job = new SteeringJob {
            routes = GetBufferFromEntity<BufferedNavNode>(),
            entityCommandBuffer = _entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent()
        };

        var jobHandle = job.Schedule(this, inputDeps);
        _entityCommandBufferSystem.AddJobHandleForProducer(jobHandle);
        
        return jobHandle;
    }

    protected override void OnCreate() {
        _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
}