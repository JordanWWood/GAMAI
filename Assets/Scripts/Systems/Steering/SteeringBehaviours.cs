using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEditor;
using UnityEngine;
using Collider = Unity.Physics.Collider;
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
        // IDE comment to disable warnings
        // ReSharper disable Unity.InefficientMultiplicationOrder
        var feelers = new[] {
            position + (currentVelocity.normalized * distance),
            position + (Quaternion.AngleAxis(-45, Vector3.up) * currentVelocity.normalized) * (distance / 2),
            position + (Quaternion.AngleAxis(45, Vector3.up) * currentVelocity.normalized) * (distance / 2)
        };

        var avoidance = new Vector3();
        foreach (var feeler in feelers) {
            var distanceToIntersect = float.MaxValue;
            var closestPoint = new Vector3();
            var (hit, normal, hitLoc, _) = Raycast(new float3(position.x, position.y, position.z),
                new float3(feeler.x, feeler.y, feeler.z), 4, pWorld);
            if (!hit) continue;

            var vecDistance = Vector3.Distance(hitLoc, position);
            if (vecDistance < distanceToIntersect) {
                closestPoint = hitLoc;
                distanceToIntersect = vecDistance;
            }

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (distanceToIntersect == float.MaxValue) continue;

            var depth = feeler - closestPoint;
            avoidance = normal * depth.magnitude;
        }

        return avoidance * steeringForce;
    }

    public static Vector3 ObstacleAvoidance(Vector3 currentVelocity, Vector3 position, float steeringForce,
        float distance, PhysicsWorld pWorld) {
        var rot = Quaternion.LookRotation(currentVelocity.normalized, Vector3.up);
        var ahead = position + (currentVelocity.normalized * distance);
        var start = position + (currentVelocity.normalized * -.5f);
        var startLeft = start + (rot * new Vector3(-.55f, 0, 0));
        var aheadLeft = ahead + (rot * new Vector3(-.55f, 0, 0));
        var startRight = start + (rot * new Vector3(.55f, 0, 0));
        var aheadRight = ahead + (rot * new Vector3(.55f, 0, 0));
        
        var (leftHit, _, _, leftObjectPos) = Raycast(startLeft, aheadLeft, 2, pWorld);
        var (rightHit, _, _, rightObjectPos) = Raycast(startRight, aheadRight, 2, pWorld);
        var (topHit, _, _, topObjectPos) = Raycast(aheadLeft, aheadRight, 2, pWorld);
        if (!leftHit && !rightHit && ! topHit) {
            return new Vector3();
        }

        var avoidance = ahead;
        if (leftHit) avoidance -= leftObjectPos;
        else if (rightHit) avoidance -= rightObjectPos;
        else avoidance -= topObjectPos;
        

        avoidance.y = 0;

        return avoidance * steeringForce;
    }

    private static (bool hit, Vector3 normal, Vector3 hitLoc, Vector3 objectPos) Raycast(float3 rayFrom, float3 rayTo, uint collideLayer,
        PhysicsWorld pWorld) {
        var input = new RaycastInput() {
            Start = rayFrom,
            End = rayTo,
            Filter = new CollisionFilter() {
                BelongsTo = ~0u,
                CollidesWith = collideLayer,
                GroupIndex = 0
            }
        };

        var haveHit = pWorld.CollisionWorld.CastRay(input, out var hit);

        if (!haveHit) {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(rayFrom, rayTo);
            });
            return (false, new Vector3(), new Vector3(), new Vector3());
        }

        if (hit.RigidBodyIndex == -1) return (false, new Vector3(), new Vector3(), new Vector3());
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(rayFrom, rayTo);
        });

        var normal = hit.SurfaceNormal;
        var hitLoc = hit.Position;
        return (true, normal, hitLoc, pWorld.Bodies[hit.RigidBodyIndex].WorldFromBody.pos);
    }
    
}