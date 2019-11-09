using System;
using System.Collections.Generic;
using UnityEngine;

public class NavNode {
    public int Index;
    public Vector3 Location;
    public List<NavEdge> Edges;
    
    // Processing Variables
    public bool Visited = false;
    public float? MinCost = null;
    public NavNode Nearest;
}