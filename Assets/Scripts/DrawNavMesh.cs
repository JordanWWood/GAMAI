using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[ExecuteInEditMode]
public class DrawNavMesh : MonoBehaviour {
    private NavMeshTriangulation _triangulation;
    public Boolean showMesh = false;
    public bool showText = false;

    private static readonly Queue<List<NavNode>> _executionQueue = new Queue<List<NavNode>>();
    
    public static void Enqueue(List<NavNode> list) {
        lock (_executionQueue) {
            _executionQueue.Enqueue(list);
        }
    }

    
    // Start is called before the first frame update
    void Start() {
        if (showMesh) {
            Mesh mesh = new Mesh {vertices = _triangulation.vertices, triangles = _triangulation.indices};
            GetComponent<MeshFilter>().mesh = mesh;
        }
    }
    
    private void Update() {
        _triangulation = NavMesh.CalculateTriangulation();
    }

    private void OnDrawGizmosSelected() {
        for (int i = 0; i < _triangulation.indices.Length - 1; i++) {
            Vector3 point1;
            Vector3 point2;

            if ((i + 1) % 3 == 0 && i != 0) {
                point1 = _triangulation.vertices[_triangulation.indices[i]];
                point2 = _triangulation.vertices[_triangulation.indices[i - 2]];
            }
            else {
                point1 = _triangulation.vertices[_triangulation.indices[i]];
                point2 = _triangulation.vertices[_triangulation.indices[i + 1]];
            }
            
            var halfway = (point1 + point2) / 2;
            Handles.DrawLine(point1, point2);

            if (!showText) continue;
            Handles.Label(point1, $"{point1}");
            Handles.Label(halfway, Vector3.Distance(point1, point2) + "");
        }

        lock (_executionQueue) {
            while (_executionQueue.Count > 0) {
                var list = _executionQueue.Dequeue();
                for (int i = 0; i + 1 < list.Count; i++)
                    Debug.DrawLine(list[i].Location, list[i + 1].Location, Color.cyan, .1f);
            }
        }
    }
}