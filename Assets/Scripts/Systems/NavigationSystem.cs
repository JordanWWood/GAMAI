using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AI;

public class NavigationSystem : ComponentSystem {
    private Dictionary<Vector3, NavNode> graph = new Dictionary<Vector3, NavNode>();

    protected override void OnStartRunning() {
        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();

        // Build Graph from NavMesh
        for (int i = 0; i < triangulation.indices.Length - 1; i++) {
            Vector3 from;
            Vector3 to;

            // If we are on the 3rd indice we close the tri instead of continuing to the next indice
            if ((i + 1) % 3 == 0 && i != 0) {
                from = triangulation.vertices[triangulation.indices[i]];
                to = triangulation.vertices[triangulation.indices[i - 2]];
            } else {
                from = triangulation.vertices[triangulation.indices[i]];
                to = triangulation.vertices[triangulation.indices[i + 1]];
            }

            if (!graph.ContainsKey(from)) {
                graph.Add(from, new NavNode() {
                    Index = triangulation.indices[i],
                    Location = from,
                    Edges = new List<NavEdge> {
                        new NavEdge(from, to, Vector3.Distance(from, to), Vector3.Distance(from, to), i)
                    }
                });
            } else {
                NavNode node = graph[from];
                node.Edges.Add(new NavEdge(from, to, Vector3.Distance(from, to), Vector3.Distance(from, to), i));
            }

            if (!graph.ContainsKey(to)) {
                graph.Add(to, new NavNode() {
                    Index = triangulation.indices[i + 1],
                    Location = to,
                    Edges = new List<NavEdge> {
                        new NavEdge(to, from, Vector3.Distance(from, to), Vector3.Distance(from, to), i)
                    }
                });
            } else {
                NavNode node = graph[to];
                node.Edges.Add(new NavEdge(to, from, Vector3.Distance(from, to), Vector3.Distance(from, to), i));
            }
        }
        
        var dijkstra = new Dijkstra(graph);
        var route = dijkstra.CalculateRoute(new Vector3(25.47f, 0, -8), new Vector3(-25.47f, 0, 8));
        
        Debug.Log($"DrawNavMesh path set {route.Count}");
        DrawNavMesh.Path = route;
    }

    protected override void OnUpdate() {
        // TODO
    }
}