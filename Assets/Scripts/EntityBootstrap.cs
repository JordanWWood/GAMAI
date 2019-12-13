using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
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

    public bool disableCollisionConstraint = false;
    public static EntityBootstrap Instance;

    void Start() {
        Instance = this;

        _entityManager = World.Active.EntityManager;

        var sourceEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(AgentPrefab, World.Active);
        var sourceCollider = _entityManager.GetComponentData<PhysicsCollider>(sourceEntity).Value;
        if (!createAgentsOnStart) return;
        for (int i = 0; i < 100; i++) {
            Vector3 loc = RandomNavmeshLocation(40);
            SpawnAgentEntity(i, new float3(loc.x, 1f, loc.z), sourceEntity, sourceCollider);
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

    private void SpawnAgentEntity(int id, float3 position, Entity entity, BlobAssetReference<Unity.Physics.Collider> collider) {
        var instance = _entityManager.Instantiate(entity);
        _entityManager.AddComponent<GoalComponent>(instance);
        _entityManager.AddComponent<AiAgentComponent>(instance);
        _entityManager.AddComponent<SteeringComponent>(instance);
        _entityManager.AddComponent<RotationEulerXYZ>(instance);
        
        SetEntityComponentData(id, instance, position, agentMesh, agentMaterial, collider);
    }

    private void SetEntityComponentData(int id, Entity entity, float3 spawnPosition, Mesh mesh, Material material,
        BlobAssetReference<Unity.Physics.Collider> sourceCollider) {
        _entityManager.SetSharedComponentData(entity, new RenderMesh {
            material = material,
            mesh = mesh
        });

        _entityManager.SetComponentData(entity, new Translation {
            Value = spawnPosition
        });

        _entityManager.SetComponentData(entity, new AiAgentComponent {
            DestinationReached = true,
            TimeSinceLastStuckCheck = float.MaxValue
        });
        
        _entityManager.SetComponentData(entity, new PhysicsCollider {
            Value = sourceCollider
        });
        
        _entityManager.SetComponentData(entity, new GoalComponent() {
            index = id,
            progress = 0
        });

        _entityManager.AddBuffer<BufferedGoal>(entity);
        _entityManager.AddBuffer<BufferedNavNode>(entity);
        
        var bufferFromEntity = _entityManager.GetBuffer<BufferedGoal>(entity);
        bufferFromEntity.Add(new BufferedGoal {
            Goal = new ExampleGoal()
        });
    }
}

