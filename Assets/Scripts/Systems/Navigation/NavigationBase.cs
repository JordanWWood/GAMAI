using System.Collections.Generic;
using UnityEngine;

public abstract class NavigationBase {
    public abstract List<NavNode> CalculateRoute(Vector3 start, Vector3 target, Dictionary<Vector3, NavNode> graph);
}