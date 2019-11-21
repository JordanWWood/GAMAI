using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

public struct GoalComponent : IComponentData {
    public int index;
    public int progress;
}