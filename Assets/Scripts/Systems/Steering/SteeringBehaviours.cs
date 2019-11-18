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

        var steering = Vector3.ClampMagnitude((intendedVelocity - currentVelocity), steeringForce);
        return Vector3.ClampMagnitude(currentVelocity + steering, maxSpeed);
    }

    public static Vector3 Avoid(Vector3 currentVelocity, Vector3 position, float maxSpeed, float steeringForce,
        float distance, PhysicsWorld pWorld) {
        var to = (position + (currentVelocity * distance));

        var result = Raycast(new float3(position.x, position.y, position.z), new float3(to.x, to.y, to.z), pWorld);
        if (!result.Item1) return new Vector3();
        
        var avoidance = new Vector3 {x = to.x - result.Item2.x, z = to.z - result.Item2.z};
        avoidance.Normalize();
        avoidance *= steeringForce;

        return avoidance;
    }

    private static (bool, Vector3, Vector3) Raycast(float3 rayFrom, float3 rayTo, PhysicsWorld pWorld) {
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
        
        //Debug.Log($"Raycast hit {haveHit} {hit.Position}");
        //UnityMainThreadDispatcher.Instance().Enqueue(() => Debug.DrawLine(rayFrom, rayTo));

        if (!haveHit) return (false, new Vector3(), new Vector3());
        var objectLoc = pWorld.Bodies[hit.RigidBodyIndex].WorldFromBody.pos;
        var hitLoc = hit.Position;

        //Debug.Log($"Raycast hit {haveHit} {hitLoc}");
        
        return (true, objectLoc, hitLoc);
    }
}