

using UnityEngine;

public class RandomRoam : GoalBase {
    public RandomRoam() : base(new SteeringBehaviour()) {
        SteeringBehaviours = new SteeringBehaviour {
            EnableObjectAvoidance = true, 
            EnableWallAvoidance = true, 
            Seek = true, 
            Wander = false
        };
        
        Navigation = new AStar();
    }

    public override (bool, Vector3) CalculateTarget(Vector3 pos, int progress, bool forceRecalc) {
        if (forceRecalc) {
            CurrentTarget = EntityBootstrap.Instance.RandomNavmeshLocation(50);
            return (false, CurrentTarget);
        }

        if (progress >= 1)
            return (false, CurrentTarget);

        CurrentTarget = EntityBootstrap.Instance.RandomNavmeshLocation(50);
        return (false, CurrentTarget);
    }
}