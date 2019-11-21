using System;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

// Incredibly basic system to update the location based on the current velocity and making sure the agent isnt stuck
public class MovementSystem : ComponentSystem {
    protected override void OnUpdate() {
        Entities.ForEach(
            (ref Translation translation, ref SteeringComponent steeringComponent,
                ref AiAgentComponent aiAgentComponent) => {
//                if (aiAgentComponent.TimeSinceLastStuckCheck >= 3) {
//                    aiAgentComponent.PrevPos = translation.Value;
//                    aiAgentComponent.TimeSinceLastStuckCheck = 0;
//                }
//                
//                if (aiAgentComponent.TimeStuck >= 2) {
//                    aiAgentComponent.DestinationReached = true;
//                    aiAgentComponent.TimeSinceLastStuckCheck = float.MaxValue;
//                }
//                
//                if (Math.Abs(aiAgentComponent.PrevPos.x - translation.Value.x) < .5f &&
//                    Math.Abs(aiAgentComponent.PrevPos.y - translation.Value.y) < .5f &&
//                    Math.Abs(aiAgentComponent.PrevPos.y - translation.Value.y) < .5f)
//                    aiAgentComponent.TimeStuck += Time.deltaTime;

                translation.Value += steeringComponent.Velocity * (Time.deltaTime * 60);
                aiAgentComponent.TimeSinceLastStuckCheck += Time.deltaTime;
            });
    }
}