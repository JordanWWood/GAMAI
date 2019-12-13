using System.Collections.Generic;
using PriorityQueues;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class GoalSystem : ComponentSystem {
    private EntityManager _entityManager = World.Active.EntityManager;
    public static readonly Dictionary<int, PriorityQueue<GoalBase>> EntityGoalMap = new Dictionary<int, PriorityQueue<GoalBase>>();
    protected override void OnStartRunning() { }

    protected override void OnUpdate() {
        Entities.ForEach((Entity e, ref GoalComponent goalComponent, ref SteeringComponent steeringComponent,
            ref AiAgentComponent aiAgent, ref Translation translation) => {
            
            // Assign a default goal so any given agent always has something to do.
            // This is intended to be overriden by goals with a higher priority
            if (!EntityGoalMap.ContainsKey(goalComponent.index)) {
                EntityGoalMap.Add(goalComponent.index, new PriorityQueue<GoalBase>());
                EntityGoalMap[goalComponent.index].Enqueue(new RandomRoam());
            }

            var goalsToAdd = _entityManager.GetBuffer<BufferedGoal>(e);
            foreach (var bufferedGoal in goalsToAdd)
                EntityGoalMap[goalComponent.index].Enqueue(bufferedGoal.Goal);

            var goal = EntityGoalMap[goalComponent.index].Peek();
            steeringComponent.SteeringBehaviour = goal.SteeringBehaviours;
            var (complete, target) = goal.CalculateTarget(translation.Value, goalComponent.progress, aiAgent.DestinationReached);
            var results = goalComponent.PreviousTarget != new float3(target);
            if (results.x || results.y || results.z) {
                // Force the Navigation system to recalculate the route since the target has changed.
                // This is to allow the goal to be changed/overriden by one with higher priority
                goalComponent.PreviousTarget = new float3(target);
                aiAgent.DestinationReached = true;
            }

            goalComponent.Behaviour = goal.SteeringBehaviours;
            goalComponent.CurrentTarget = target;
            goalComponent.progress++;

            if (complete) EntityGoalMap[goalComponent.index].Dequeue();
        });
    }
}

