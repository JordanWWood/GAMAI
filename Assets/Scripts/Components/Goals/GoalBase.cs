using System;
using System.Linq;
using UnityEngine;

//TODO navigation type enum enable/disable steering behaviour, flocking, etc
public abstract class GoalBase : IComparable<GoalBase> {
    public SteeringBehaviour SteeringBehaviours { get; protected set; }
    protected Vector3 CurrentTarget;
    public NavigationBase Navigation;
    public int Priority;

    public GoalBase(SteeringBehaviour steeringBehaviours) {
        SteeringBehaviours = steeringBehaviours;
    }

    public abstract (bool, Vector3) CalculateTarget(Vector3 pos, int progress, bool forceRecalc);

    public int CompareTo(GoalBase other) {
        return other == null ? -1 : Priority.CompareTo(other.Priority);
    }
}

public struct SteeringBehaviour {
    public bool Seek;
    public bool EnableWallAvoidance;
    public bool EnableObjectAvoidance;
    public bool Wander;
}