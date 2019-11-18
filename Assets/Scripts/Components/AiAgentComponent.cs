using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct AiAgentComponent : IComponentData {
    public int NavigationIndex;
    public int NavigationTotal;
    public int DeferredFrames;
    
    public bool destinationReached;
}