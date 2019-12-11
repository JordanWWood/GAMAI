using UnityEngine;

public class WanderGoal : GoalBase {
    public WanderGoal(NavigationType navType) : base(new SteeringBehaviour()) {
        var steeringBehaviour = SteeringBehaviours;
        steeringBehaviour.Seek = false;
        steeringBehaviour.EnableObjectAvoidance = true;
        steeringBehaviour.EnableWallAvoidance = true;
        steeringBehaviour.Wander = true;

        SteeringBehaviours = steeringBehaviour;
    }

    public override (bool, Vector3) CalculateTarget(Vector3 pos, int progress, bool forceRecalc) {
        return (false, Vector3.zero);
    }
}