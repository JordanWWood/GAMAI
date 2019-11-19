using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

public static class SteeringBehaviours {
    public static Vector3 Seek(Vector3 currentVelocity, Vector3 location, Vector3 target, float maxSpeed,
        float steeringForce) {
        var intendedVelocity = (target - location);
        intendedVelocity.Normalize();
        intendedVelocity *= maxSpeed;

        return Vector3.ClampMagnitude((intendedVelocity - currentVelocity), steeringForce);
    }

    public static Vector3 WallAvoidance(Vector3 currentVelocity, Vector3 position, float maxSpeed, float steeringForce,
        float distance, PhysicsWorld pWorld) {
        
        Vector3[] feelers = new[] {
            position + (currentVelocity * distance),
            position + ((Quaternion.AngleAxis(-45, Vector3.up) * currentVelocity) * (distance/2)), 
            position + ((Quaternion.AngleAxis(45, Vector3.up) * currentVelocity) * (distance/2))
        };
        
        Vector3 avoidance = new Vector3();
        foreach (var feeler in feelers) {
            float distanceToIntersect = float.MaxValue;
            Vector3 closestPoint = new Vector3();
            var result = Raycast(new float3(position.x, position.y, position.z), new float3(feeler.x, feeler.y, feeler.z), pWorld);
            if (!result.Item1) continue;

            var vecDistance = Vector3.Distance(result.Item3, position);
            if (vecDistance < distanceToIntersect) {
                closestPoint = result.hitLoc;
                distanceToIntersect = vecDistance;
            }
            
            if (distanceToIntersect == float.MaxValue) continue;
            
            var depth = feeler - closestPoint;
            avoidance = result.normal * depth.magnitude;
        }

        return avoidance;
    }

    private static (bool hit, Vector3 normal, Vector3 hitLoc) Raycast(float3 rayFrom, float3 rayTo, PhysicsWorld pWorld) {
        RaycastInput input = new RaycastInput() {
            Start = rayFrom,
            End = rayTo,
            Filter = new CollisionFilter() {
                BelongsTo = ~0u,
                CollidesWith = ~0u,
                GroupIndex = 0
            }
        };

        bool haveHit = pWorld.CollisionWorld.CastRay(input, out var hit);

        if (!haveHit) {
            UnityMainThreadDispatcher.Instance().Enqueue(() => Debug.DrawLine(rayFrom, rayTo, Color.white));
            return (false, new Vector3(), new Vector3());
        }
        if (hit.RigidBodyIndex == -1) return (false, new Vector3(), new Vector3());
        
        UnityMainThreadDispatcher.Instance().Enqueue(() => Debug.DrawLine(rayFrom, rayTo, Color.red));

        var normal = hit.SurfaceNormal;
        var hitLoc = hit.Position;
        return (true, normal, hitLoc);
    }
}