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

    [Range(.1f, 1.0f)] public float Speed;

    public static EntityBootstrap Instance;

    void Start() {
        Instance = this;

        _entityManager = World.Active.EntityManager;

        var sourceEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(AgentPrefab, World.Active);
        var sourceCollider = _entityManager.GetComponentData<PhysicsCollider>(sourceEntity).Value;
        if (!createAgentsOnStart) return;
        for (int i = 0; i < 100; i++) {
            Vector3 loc = RandomNavmeshLocation(40);
            SpawnAgentEntity(new float3(loc.x, 1f, loc.z), sourceEntity, sourceCollider);
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

    private void SpawnAgentEntity(float3 position, Entity entity, BlobAssetReference<Unity.Physics.Collider> collider) {
        var instance = _entityManager.Instantiate(entity);
        _entityManager.AddComponent<GoalComponent>(instance);
        _entityManager.AddComponent<AiAgentComponent>(instance);
        _entityManager.AddComponent<SteeringComponent>(instance);
        _entityManager.AddComponent<RotationEulerXYZ>(instance);
        
        SetEntityComponentData(instance, position, agentMesh, agentMaterial, collider);
    }

    private void SetEntityComponentData(Entity entity, float3 spawnPosition, Mesh mesh, Material material,
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

//        var newCollider = Unity.Physics.CapsuleCollider.Create(
//            new CapsuleGeometry {
//                Radius = .5f,
//                Vertex0 = new float3(0, .25f, 0),
//                Vertex1 = new float3(0, .75f, 0)
//            }, new CollisionFilter {
//                BelongsTo = 2,
//                CollidesWith = ~0u,
//                GroupIndex = 0
//            });
        _entityManager.SetComponentData(entity, new PhysicsCollider {
            Value = sourceCollider
        });
        Debug.Log(sourceCollider.IsCreated + " | " + sourceCollider.Value.CollisionType);

        _entityManager.AddBuffer<BufferedNavNode>(entity);
    }
}