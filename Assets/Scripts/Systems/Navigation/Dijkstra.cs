using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Dijkstra : NavigationBase {
    public override List<NavNode> CalculateRoute(Vector3 start, Vector3 target, Dictionary<Vector3, NavNode> graph) {
        var minPriorityQueue = new List<NavNode>();
        var route = new List<NavNode>();

        var minCost = new Dictionary<NavNode, float?>();
        var nearest = new Dictionary<NavNode, NavNode>();
        
        // Hashset uses a hashing algorithm to search for objects within the Set. This is way faster than doing '.Contains' on a List 
        var visited = new HashSet<NavNode>();
        
        // Create nodes at both the start and end locations so they are navigable if not directly on a node
        CreateNodeAtLocation(start, graph);
        CreateNodeAtLocation(target, graph);

        minPriorityQueue.Add(graph[start]);
        var visitedNodes = 0;
        do {
            visitedNodes++;
            minPriorityQueue = minPriorityQueue.OrderBy(x => {
                var successNode = minCost.TryGetValue(x, out var minCostNode);
                return (successNode ? minCostNode : (float?) null);
            }).ToList();
            var node = minPriorityQueue.First();
            minPriorityQueue.Remove(node);

            foreach (var edge in node.Edges.OrderBy(x => x.Cost)) {
                var child = graph[edge.To];

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
            if (node.Equals(graph[target])) break;
        } while (minPriorityQueue.Any());

        Debug.Log("Total visited nodes = " + visitedNodes);
        route.Add(graph[target]);
        return BuildPath(route, graph[target], nearest);
    }
    
    private List<NavNode> BuildPath(List<NavNode> list, NavNode node, Dictionary<NavNode, NavNode> nearest, int recursions = 0) {
        var success = nearest.TryGetValue(node, out _);
        if (!success) return list;
        if (recursions == 1000) return list;
        list.Add(nearest[node]);

        return BuildPath(list, nearest[node], nearest, recursions + 1);
    }

    private void CreateNodeAtLocation(Vector3 location, Dictionary<Vector3, NavNode> graph) {
        if (graph.ContainsKey(location)) return;

        var distances = graph.Select(node =>
            new KeyValuePair<float, NavNode>(Vector3.Distance(location, node.Value.Location), node.Value)).ToList();
        distances = distances.OrderBy(pair => pair.Key).Take(1).ToList();

        graph.Add(location, new NavNode() {
            Location = location,
            Edges = new HashSet<NavEdge> {
                new NavEdge(location, distances[0].Value.Location, distances[0].Key, distances[0].Key, int.MaxValue)
            },
            Index = int.MaxValue
        });

        graph.Remove(distances[0].Value.Location);
        graph.Add(distances[0].Value.Location, (NavNode) distances[0].Value.Clone());
        graph[distances[0].Value.Location].Edges.Add(new NavEdge(distances[0].Value.Location, location,
            distances[0].Key, distances[0].Key, int.MaxValue - 1));
    }
}