using System.Collections.Generic;
using UnityEngine;

public abstract class NavigationBase {
    protected Dictionary<Vector3, NavNode> _graph;

    public NavigationBase(Dictionary<Vector3, NavNode> graph) {
        _graph = graph;
    }

    public abstract List<NavNode> CalculateRoute(Vector3 start, Vector3 target);
}