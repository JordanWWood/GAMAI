using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;


[UpdateAfter(typeof(NavigationSystem))]
public class SteeringSystem : JobComponentSystem {
    [BurstCompile]
    private struct SteeringJob : IJobForEachWithEntity<Translation, SteeringComponent, LocalToWorld, AiAgentComponent> {
        [ReadOnly] public BufferFromEntity<BufferedNavNode> Routes;
        [ReadOnly] public PhysicsWorld World;
        public EntityCommandBuffer.Concurrent EntityCommandBuffer;

        public void Execute(Entity entity, int index, ref Translation translation, ref SteeringComponent steeringComponent, ref LocalToWorld localToWorld, ref AiAgentComponent aiAgent) {
            var nodes = Routes[entity];
            if (nodes.Length <= 1) return;

            const float seekForce = .0025f;
            const float wAvoidForce = .03f;
            const float oAvoidForce = 1.0f;
            const float maxSpeed = .1f;

            var currentVelocity = new Vector3(steeringComponent.Velocity.x, steeringComponent.Velocity.y, steeringComponent.Velocity.z);
            var target = new Vector3(nodes[aiAgent.NavigationIndex].Node.x, 1, nodes[aiAgent.NavigationIndex].Node.z);
            var location = new Vector3(localToWorld.Position.x, 1, localToWorld.Position.z);

            var steering = SteeringBehaviours.Seek(currentVelocity, location, target, maxSpeed, seekForce);
            steering += SteeringBehaviours.WallAvoidance(currentVelocity, translation.Value, maxSpeed, wAvoidForce, 2.0f, World);
            steering += SteeringBehaviours.ObstacleAvoidance(currentVelocity, translation.Value, oAvoidForce, 2.0f, World);

            var newDirection = Vector3.ClampMagnitude(currentVelocity + steering, maxSpeed);
            
            if (Vector3.Distance(target, location) <= .5f) {
                bool destinationReached = aiAgent.DestinationReached;
                if (aiAgent.NavigationIndex + 1 >= nodes.Length) destinationReached = true;
                
                EntityCommandBuffer.SetComponent(index, entity, new AiAgentComponent() {
                    NavigationIndex = aiAgent.NavigationIndex + 1,
                    DeferredFrames = aiAgent.DeferredFrames,
                    DestinationReached = destinationReached,
                    NavigationTotal = aiAgent.NavigationTotal
                });
            }
            
            EntityCommandBuffer.SetComponent(index, entity, new SteeringComponent { Velocity = new float3(newDirection.x, newDirection.y, newDirection.z)});
        }
    }

    private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

    protected override JobHandle OnUpdate(JobHandle inputDeps) {
        var physicsWorldSystem = World.Active.GetExistingSystem<BuildPhysicsWorld>();

        var job = new SteeringJob {
            Routes = GetBufferFromEntity<BufferedNavNode>(),
            EntityCommandBuffer = _entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            World = physicsWorldSystem.PhysicsWorld
        };

        var jobHandle = job.Schedule(this, inputDeps);
        _entityCommandBufferSystem.AddJobHandleForProducer(jobHandle);
        
        return jobHandle;
    }

    protected override void OnCreate() {
        _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
}