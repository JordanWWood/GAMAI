using UnityEngine;

//TODO navigation type enum enable/disable steering behaviour, flocking, etc
public abstract class GoalBase {
    public NavigationType NavType { get; protected set; }
    public SteeringBehaviours SteeringBehaviours { get; protected set; }
    public Vector3 Target;

    public GoalBase(NavigationType navType, SteeringBehaviours steeringBehaviours, Vector3 target) {
        NavType = navType;
        SteeringBehaviours = steeringBehaviours;
        Target = target;
    }
}

public struct SteeringBehaviours {
    public bool EnableGeneralSteeringBehaviour;
    public bool EnableFlocking;
    public bool EnableAvoidance;
}