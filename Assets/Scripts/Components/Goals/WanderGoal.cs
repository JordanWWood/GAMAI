using UnityEngine;

public class WanderGoal : GoalBase {
    public WanderGoal(NavigationType navType, SteeringBehaviour steeringBehaviours, Vector3 target) : base(navType,
        steeringBehaviours, target) {
        var steeringBehaviour = SteeringBehaviours;
        steeringBehaviour.Seek = false;
        steeringBehaviour.Wander = true;

        SteeringBehaviours = steeringBehaviour;
    }
}