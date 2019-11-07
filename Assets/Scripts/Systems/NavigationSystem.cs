using Unity.Entities;
using UnityEngine.AI;

public class NavigationSystem : ComponentSystem {
    private NavMeshTriangulation _triangulation;
    
    protected override void OnStartRunning() {
        _triangulation = NavMesh.CalculateTriangulation();
    }

    protected override void OnUpdate() {
        Entities.ForEach((ref AiAgentComponent aiAgentComponent) => {
            
        });
    }
}