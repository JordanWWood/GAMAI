using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

//public class SteeringSystem : JobComponentSystem {
//    struct SteeringJob : IJobForEach<AiAgentComponent, SteeringComponent> {
//        [ReadOnly] public float DeltaTime;
//
//        public void Execute(ref AiAgentComponent aiAgent, ref SteeringComponent steering) {
//            
//        }
//    }
//    
//    protected override JobHandle OnUpdate(JobHandle inputDeps) {
//        var job = new SteeringJob {
//            DeltaTime = Time.deltaTime
//        };
//
//        return job.Schedule(this, inputDeps);
//    }
//}