using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public struct NavNode : ICloneable {
    public int Index;
    public Vector3 Location;
    public HashSet<NavEdge> Edges;

    public NavNode(NavNode d) {
        Index = d.Index;
        Location = d.Location;
        Edges = new HashSet<NavEdge>(d.Edges);
    }

    public object Clone() {
        var clone = CloneFromCopyConstructor(this);
        return clone;
    }

    private static object CloneFromCopyConstructor(object d) {
        if (d == null) return null;
        var t = d.GetType();
        return (from ci in t.GetConstructors() let pi = ci.GetParameters() where pi.Length == 1 && pi[0].ParameterType == t select ci.Invoke(new object[] {d})).FirstOrDefault();
    }
}