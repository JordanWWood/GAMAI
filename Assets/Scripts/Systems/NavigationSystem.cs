﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using PriorityQueues;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;

public class NavigationSystem : ComponentSystem {
    private static readonly ConcurrentDictionary<Vector3, NavNode> _graph = new ConcurrentDictionary<Vector3, NavNode>();

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
                _graph.TryAdd(from, new NavNode() {
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
                _graph.TryAdd(to, new NavNode() {
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

    private class AgentComparison : IComparable<AgentComparison> {
        public AiAgentComponent agent;
        
        public int CompareTo(AgentComparison other) {
            return other == null ? 1 : agent.DeferredFrames.CompareTo(other.agent.DeferredFrames);
        }
    }
    
    protected override void OnUpdate() {
        var totalCalculated = 0;
        Entities.ForEach((ref AiAgentComponent aiAgent, ref Translation translation) => {
            var deferredFrames = aiAgent.DeferredFrames;
            if (deferredFrames > 0) {
                aiAgent.DeferredFrames--;
                return;
            }

            if (totalCalculated > 6) {
                var newDeferredFrames = totalCalculated % 6;
                aiAgent.DeferredFrames = newDeferredFrames;
                
                return;
            }
            
            var graph = new Dictionary<Vector3, NavNode>(_graph);
            var navigation = new AStar(graph);
            var route = navigation.CalculateRoute(translation.Value, new Vector3(-28.1f, 0, -28.4f));

            totalCalculated++;
        });
    }
}