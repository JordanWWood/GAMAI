using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EntityBootstrap : MonoBehaviour {
    private static EntityManager _entityManager;
    
    // Agent
    public Mesh agentMesh;
    public Material agentMaterial;
    public bool createAgentsOnStart = true;
    
    
    void Start() {
        _entityManager = World.Active.EntityManager;

        if (!createAgentsOnStart) return;
        for (int i = 0; i < 10; i++) {
            Vector3 loc = RandomNavmeshLocation(28);
            SpawnAgentEntity(new float3(loc.x, 1f, loc.z));
        }
    }
    
    public Vector3 RandomNavmeshLocation(float radius) {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1)) {
            finalPosition = hit.position;            
        }
        return finalPosition;
    }

    private void SpawnAgentEntity(float3 position) {
        Entity e = _entityManager.CreateEntity(
            typeof(Translation),
            typeof(LocalToWorld),
            typeof(RenderMesh),
            typeof(Scale),
            typeof(GoalComponent),
            typeof(AiAgentComponent));
        SetEntityComponentData(e, position, agentMesh, agentMaterial);
        _entityManager.SetComponentData(e, new Scale { Value = 1f });
    }

    private void SetEntityComponentData(Entity entity, float3 spawnPosition, Mesh mesh, Material material) {
        _entityManager.SetSharedComponentData<RenderMesh>(entity, new RenderMesh() {
            material = material,
            mesh = mesh
        });
        
        _entityManager.SetComponentData<Translation>(entity, new Translation() {
            Value = spawnPosition
        });
    }
}