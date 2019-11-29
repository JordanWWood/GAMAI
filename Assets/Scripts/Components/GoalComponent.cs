using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct GoalComponent : IComponentData {
    public int index;
    public int progress;

    public Vector3 CurrentTarget;
}