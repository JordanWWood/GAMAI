using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class PenetrationConstraintSystem : ComponentSystem {
    protected override void OnUpdate() {
        // Allow this to be disabled for demonstration purposes.
        if (EntityBootstrap.Instance.disableCollisionConstraint) return;
        
        // This is *SLOW*. This SHOULD BE IMPROVED with a quad tree to search for nearby entities
        Entities.ForEach((Entity e, ref AiAgentComponent aiAgentComponent, ref Translation translation, ref LocalToWorld localToWorld) => {
            Vector3 currentPos = translation.Value;

            Entities.ForEach((Entity e1, ref AiAgentComponent aiAgentComponent1, ref Translation translation1, ref LocalToWorld localToWorld1) => {
                Vector3 otherCurrentPos = translation1.Value;
                if (currentPos == otherCurrentPos) return;

                var diff = currentPos - otherCurrentPos;
                var dist = diff.magnitude;
                var overlap = 1 - dist;

                if (!(overlap >= 0)) return;
                currentPos += diff / dist * overlap;
                currentPos.y = 1;
            });
            
            translation.Value = currentPos;
        });
    }
}
