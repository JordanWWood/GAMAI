using System;
using Unity.Entities;
using UnityEngine;

public class Obstacle : GameObjectEntity {
    private EntityManager em;
    
    public void Start() {
        em = World.Active.EntityManager;
        em.AddComponent<ObstacleComponent>(Entity);
        em.AddComponent<BoundingBoxComponent>(Entity);

        em.SetName(Entity, "Obstacle");
    }

    public void Update() {
    }
}