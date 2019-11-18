using System;
using System.Collections.Generic;
using UnityEngine;

public struct NavNode {
    public int Index;
    public Vector3 Location;
    public List<NavEdge> Edges;
}