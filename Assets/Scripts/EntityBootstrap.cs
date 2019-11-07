using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

public class EntityBootstrap : MonoBehaviour {
    private static EntityManager _entityManager;
    
    // Agent
    public Mesh agentMesh;
    public Material agentMaterial;

    // Start is called before the first frame update
    void Start() {
        _entityManager = World.Active.EntityManager;

        for (int i = 0; i < 100; i++)
            SpawnAgentEntity(new float3(Random.Range(-28, 28), 1f, Random.Range(-20, 28)));
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