using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dijkstra : NavigationBase {
    public Dijkstra(Dictionary<Vector3, NavNode> graph) : base(graph) { }

    public override List<NavNode> CalculateRoute(Vector3 start, Vector3 target) {
        var minPriorityQueue = new List<NavNode>();
        var route = new List<NavNode>();

        // Create nodes at both the start and end locations so they are navigable if not directly on a node
        createNodeAtLocation(start);
        createNodeAtLocation(target);

        minPriorityQueue.Add(_graph[start]);
        do {
            minPriorityQueue = minPriorityQueue.OrderBy(x => x.MinCost).ToList();
            var node = minPriorityQueue.First();
            minPriorityQueue.Remove(node);

            foreach (var edge in node.Edges.OrderBy(x => x.Cost)) {
                var child = _graph[edge.To];

                if (child.Visited) continue;
                if (child.MinCost == null || node.MinCost + edge.Cost < child.MinCost) {
                    child.MinCost = node.MinCost + edge.Cost;
                    child.Nearest = node;
                    if (!minPriorityQueue.Contains(child))
                        minPriorityQueue.Add(child);
                }
            }

            node.Visited = true;
            if (node == _graph[target]) break;
        } while (minPriorityQueue.Any());

        route.Add(_graph[target]);
        return BuildPath(route, _graph[target]);
    }
    
    private List<NavNode> BuildPath(List<NavNode> list, NavNode node, int recursions = 0) {
        if (node.Nearest == null) return list;
        if (recursions == 1000) return list;
        list.Add(node.Nearest);

        return BuildPath(list, node.Nearest, recursions + 1);
    }

    private void createNodeAtLocation(Vector3 location) {
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

        distances[0].Value.Edges.Add(new NavEdge(distances[0].Value.Location, location, distances[0].Key,
            distances[0].Key, int.MaxValue));
    }
}