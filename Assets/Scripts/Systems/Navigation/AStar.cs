using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStar : NavigationBase {
    public AStar(Dictionary<Vector3, NavNode> graph) : base(graph) { }

    public override List<NavNode> CalculateRoute(Vector3 start, Vector3 target) {
        var minPriorityQueue = new List<NavNode>();
        var route = new List<NavNode>();

        var minCost = new Dictionary<NavNode, float?>();
        var nearest = new Dictionary<NavNode, NavNode>();
        var visited = new List<NavNode>();
        
        // Create nodes at both the start and end locations so they are navigable if not directly on a node
        CreateNodeAtLocation(start);
        CreateNodeAtLocation(target);

        minPriorityQueue.Add(_graph[start]);
        var visitedNodes = 0;
        do {
            visitedNodes++;
            minPriorityQueue = minPriorityQueue.OrderBy(x => {
                var successNode = minCost.TryGetValue(x, out var minCostNode) ? minCostNode : null;
                return (successNode ?? 0) + Vector3.Distance(x.Location, target);
            }).ToList();
            var node = minPriorityQueue.First();
            minPriorityQueue.Remove(node);

            foreach (var edge in node.Edges.OrderBy(x => x.Cost)) {
                var child = _graph[edge.To];

                if (visited.Contains(child)) continue;

                var successChild = minCost.TryGetValue(child, out var minCostChild) ? minCostChild : null;
                var successNode = minCost.TryGetValue(node, out var minCostNode) ? minCostNode : null;

                if (successChild != null && !(successNode + edge.Cost < successChild)) continue;

                minCost[child] = (successNode ?? 0) + edge.Cost;
                nearest[child] = node;
                
                if (!minPriorityQueue.Contains(child))
                    minPriorityQueue.Add(child);
            }

            visited.Add(node);
            if (node == _graph[target]) break;
        } while (minPriorityQueue.Any());
        
        route.Add(_graph[target]);
        return BuildPath(route, _graph[target], nearest);
    }
    
    private List<NavNode> BuildPath(List<NavNode> list, NavNode node, Dictionary<NavNode, NavNode> nearest, int recursions = 0) {
        var success = nearest.TryGetValue(node, out _);
        if (!success) return list;
        if (recursions == 1000) return list;
        list.Add(nearest[node]);

        return BuildPath(list, nearest[node], nearest, recursions + 1);
    }

    private void CreateNodeAtLocation(Vector3 location) {
        var distances = _graph.Select(node =>
            new KeyValuePair<float, NavNode>(Vector3.Distance(location, node.Value.Location), node.Value)).ToList();
        distances = distances.OrderBy(pair => pair.Key).Take(1).ToList();

        _graph.Add(location, new NavNode() {
            Location = location,
            Edges = new List<NavEdge> {
                new NavEdge(location, distances[0].Value.Location, distances[0].Key, distances[0].Key, int.MaxValue)
            },
            Index = int.MaxValue
        });
    }
}