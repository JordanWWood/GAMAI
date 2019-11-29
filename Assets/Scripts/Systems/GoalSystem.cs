using System.Collections.Generic;
using PriorityQueues;
using Unity.Entities;
using Unity.Transforms;

public class GoalSystem : ComponentSystem {
    private Dictionary<int, PriorityQueue<GoalBase>> _entityGoalMap = new Dictionary<int, PriorityQueue<GoalBase>>();
    private List<GoalBase> _goals = new List<GoalBase> { };

    protected override void OnStartRunning() { }

    protected override void OnUpdate() {
        Entities.ForEach((ref GoalComponent goalComponent, ref SteeringComponent steeringComponent,
            ref AiAgentComponent aiAgent, ref Translation translation) => {
            if (!_entityGoalMap.ContainsKey(goalComponent.index)) {
                _entityGoalMap.Add(goalComponent.index, new PriorityQueue<GoalBase>());
                _entityGoalMap[goalComponent.index].Enqueue(new RandomRoam(NavigationType.AStar));
            }

            var goal = _entityGoalMap[goalComponent.index].Peek();
            steeringComponent.SteeringBehaviour = goal.SteeringBehaviours;
            var (complete, target) = goal.CalculateTarget(translation.Value, goalComponent.progress, aiAgent.DestinationReached);
            goalComponent.CurrentTarget = target;
            goalComponent.progress++;

            if (complete) _entityGoalMap[goalComponent.index].Dequeue();
        });
    }
}