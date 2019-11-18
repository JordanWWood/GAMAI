using UnityEngine;

//TODO navigation type enum enable/disable steering behaviour, flocking, etc
public abstract class GoalBase {
    public NavigationType NavType { get; protected set; }
    public SteeringBehaviour SteeringBehaviours { get; protected set; }
    public Vector3 Target;

    public GoalBase(NavigationType navType, SteeringBehaviour steeringBehaviours, Vector3 target) {
        NavType = navType;
        SteeringBehaviours = steeringBehaviours;
        Target = target;
    }
}

public struct SteeringBehaviour {
    public bool EnableGeneralSteeringBehaviour;
    public bool EnableAvoidance;
}