using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct GoalComponent : IComponentData {
    public int index;
    public int progress;

    public float3 PreviousTarget;
    public Vector3 CurrentTarget;
}