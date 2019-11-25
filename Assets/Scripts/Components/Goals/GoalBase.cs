using System;
using System.Linq;
using UnityEngine;

//TODO navigation type enum enable/disable steering behaviour, flocking, etc
public abstract class GoalBase : IComparable<GoalBase> {
    public NavigationType NavType { get; protected set; }
    public SteeringBehaviour SteeringBehaviours { get; protected set; }
    public Vector3 Target;
    public int Priority;

    public GoalBase(NavigationType navType, SteeringBehaviour steeringBehaviours, Vector3 target) {
        NavType = navType;
        SteeringBehaviours = steeringBehaviours;
        Target = target;
    }

    public int CompareTo(GoalBase other) {
        return other == null ? 1 : Priority.CompareTo(other.Priority);
    }
}

public struct SteeringBehaviour {
    public bool Seek;
    public bool EnableWallAvoidance;
    public bool EnableObjectAvoidance;
    public bool Wander;
}