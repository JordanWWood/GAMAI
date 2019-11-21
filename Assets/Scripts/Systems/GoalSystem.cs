using System.Collections.Generic;
using Unity.Entities;

public class GoalSystem : ComponentSystem {
    private Dictionary<int, List<KeyValuePair<int, GoalBase>>> _entityGoalMap = new Dictionary<int, List<KeyValuePair<int, GoalBase>>>();
    private List<GoalBase> _goals = new List<GoalBase>();

    protected override void OnStartRunning() { }

    protected override void OnUpdate() {
        Entities.ForEach((ref GoalComponent goalComponent) => {
            if (!_entityGoalMap.ContainsKey(goalComponent.index)) _entityGoalMap.Add(goalComponent.index, new List<KeyValuePair<int, GoalBase>>());
        });
    }
}