using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;
using Collider = UnityEngine.Collider;
using Material = UnityEngine.Material;
using Random = UnityEngine.Random;

public class EntityBootstrap : MonoBehaviour {
    private static EntityManager _entityManager;
    
    // Agent
    public Mesh agentMesh;
    public Material agentMaterial;
    public GameObject AgentPrefab;
    
    public bool createAgentsOnStart = true;

    [Range(.1f, 1.0f)]
    public float Speed;

    public static EntityBootstrap Instance;
    
    void Start() {
        Instance = this;
        
        _entityManager = World.Active.EntityManager;

        Entity sourceEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(AgentPrefab, World.Active);
        BlobAssetReference<Unity.Physics.Collider> sourceCollider = _entityManager.GetComponentData<PhysicsCollider>(sourceEntity).Value;
        if (!createAgentsOnStart) return;
        for (int i = 0; i < 10; i++) {
            Vector3 loc = RandomNavmeshLocation(28);
            SpawnAgentEntity(new float3(loc.x, 1f, loc.z), sourceCollider);
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

    private void SpawnAgentEntity(float3 position, BlobAssetReference<Unity.Physics.Collider> collider) {
        Entity e = _entityManager.CreateEntity(
            typeof(Translation),
            typeof(LocalToWorld),
            typeof(RenderMesh),
            typeof(Scale),
            typeof(GoalComponent),
            typeof(AiAgentComponent),
            typeof(SteeringComponent),
            typeof(PhysicsCollider),
            typeof(RotationEulerXYZ));
        
        SetEntityComponentData(e, position, agentMesh, agentMaterial, collider);
        _entityManager.SetComponentData(e, new Scale { Value = 1f });
    }

    private void SetEntityComponentData(Entity entity, float3 spawnPosition, Mesh mesh, Material material, BlobAssetReference<Unity.Physics.Collider> sourceCollider) {
        _entityManager.SetSharedComponentData(entity, new RenderMesh {
            material = material,
            mesh = mesh
        });
        
        _entityManager.SetComponentData(entity, new Translation {
            Value = spawnPosition
        });
        
        _entityManager.SetComponentData(entity, new AiAgentComponent {
            DestinationReached = true
        });
        
        Debug.Log(sourceCollider.Value.Filter.BelongsTo);
        _entityManager.SetComponentData(entity, new PhysicsCollider { Value = sourceCollider });
        _entityManager.AddBuffer<BufferedNavNode>(entity);
    }
}