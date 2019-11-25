using System.Collections.Generic;
using PriorityQueues;
using Unity.Entities;

public class GoalSystem : ComponentSystem {
    private Dictionary<int, PriorityQueue<GoalBase>> _entityGoalMap = new Dictionary<int, PriorityQueue<GoalBase>>();
    private List<GoalBase> _goals = new List<GoalBase> {
    };

    protected override void OnStartRunning() { }

    protected override void OnUpdate() {
        Entities.ForEach((ref GoalComponent goalComponent, ref SteeringComponent steeringComponent) => {
            if (!_entityGoalMap.ContainsKey(goalComponent.index)) _entityGoalMap.Add(goalComponent.index, new PriorityQueue<GoalBase>());

            steeringComponent.SteeringBehaviour = _entityGoalMap[goalComponent.index].Peek().SteeringBehaviours;
        });
    }
}