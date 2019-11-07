using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[ExecuteInEditMode]
public class DrawNavMesh : MonoBehaviour {
    private NavMeshTriangulation _triangulation;
    public Boolean showMesh = false;

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
            Handles.Label(halfway, Vector3.Distance(point1, point2) + "");
        }
    }
}