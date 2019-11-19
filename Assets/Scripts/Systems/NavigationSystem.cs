using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;

[UpdateAfter(typeof(GoalSystem))]
public class NavigationSystem : ComponentSystem {
    private static readonly Dictionary<Vector3, NavNode> _graph = new Dictionary<Vector3, NavNode>();

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

            if (!_graph.ContainsKey(from)) {
                _graph.Add(from, new NavNode() {
                    Index = triangulation.indices[i],
                    Location = from,
                    Edges = new List<NavEdge> {
                        new NavEdge(from, to, Vector3.Distance(from, to), Vector3.Distance(from, to), i)
                    }
                });
            } else {
                NavNode node = _graph[from];
                NavEdge navEdge = new NavEdge(from, to, Vector3.Distance(from, to), Vector3.Distance(from, to), i);
                if (!node.Edges.Contains(navEdge))
                    node.Edges.Add(navEdge);
            }

            if (!_graph.ContainsKey(to)) {
                _graph.Add(to, new NavNode() {
                    Index = triangulation.indices[i + 1],
                    Location = to,
                    Edges = new List<NavEdge> {
                        new NavEdge(to, from, Vector3.Distance(from, to), Vector3.Distance(from, to), i)
                    }
                });
            } else {
                NavNode node = _graph[to];
                NavEdge navEdge = new NavEdge(to, from, Vector3.Distance(from, to), Vector3.Distance(from, to), i);
                if (!node.Edges.Contains(navEdge))
                    node.Edges.Add(navEdge);
            }
        }
    }

    protected override void OnUpdate() {
        var totalCalculated = 0;
        Entities.ForEach((Entity e, ref AiAgentComponent aiAgent, ref Translation translation) => {
            if (!aiAgent.DestinationReached) return;
            
//            var deferredFrames = aiAgent.DeferredFrames;
//            if (deferredFrames > 0) {
//                aiAgent.DeferredFrames--;
//                return;
//            }
//
//            if (totalCalculated > 3) {
//                var newDeferredFrames = totalCalculated % 3;
//                aiAgent.DeferredFrames = newDeferredFrames;
//
//                totalCalculated++;
//                return;
//            }
//            
            var graph = new Dictionary<Vector3, NavNode>(_graph);
            var navigation = new AStar(graph);
            var route = navigation.CalculateRoute(translation.Value, EntityBootstrap.Instance.RandomNavmeshLocation(32));
            route.Reverse();

            var bufferFromEntity = EntityManager.GetBuffer<BufferedNavNode>(e);
            bufferFromEntity.Clear();
            foreach (var node in route) bufferFromEntity.Add(new BufferedNavNode { Node = node.Location });

            aiAgent.DestinationReached = false;
            aiAgent.NavigationIndex = 1;
            
            aiAgent.DeferredFrames++;
            totalCalculated++;
        });
    }
}