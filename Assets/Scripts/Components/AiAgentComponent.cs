using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct AiAgentComponent : IComponentData {
    public int NavigationIndex;
    public int NavigationTotal;
    public int DeferredFrames;
    
    public bool DestinationReached;
    public Vector3 PrevPos;
    public float TimeSinceLastStuckCheck;
    public float TimeStuck;
}