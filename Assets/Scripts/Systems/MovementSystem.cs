using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

// Incredibly basic system to update the location based on the current velocity
public class MovementSystem : ComponentSystem {
    protected override void OnUpdate() {
        Entities.ForEach(
            (ref Translation translation, ref SteeringComponent steeringComponent, ref AiAgentComponent aiAgentComponent) => {
                translation.Value += steeringComponent.Velocity;
            });
    }
}