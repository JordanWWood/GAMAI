using UnityEngine;

public class WanderGoal : GoalBase {
    public WanderGoal(NavigationType navType, Vector3 target) : base(navType,
        new SteeringBehaviour(), target) {
        var steeringBehaviour = SteeringBehaviours;
        steeringBehaviour.Seek = false;
        steeringBehaviour.EnableObjectAvoidance = true;
        steeringBehaviour.EnableWallAvoidance = true;
        steeringBehaviour.Wander = true;

        SteeringBehaviours = steeringBehaviour;
    }
}