using UnityEngine;

public class ExampleGoal : GoalBase {
    public ExampleGoal() : base(new SteeringBehaviour()) { }

    public override (bool, Vector3) CalculateTarget(Vector3 pos, int progress, bool forceRecalc) {
        return (true, Vector3.zero);
    }
}