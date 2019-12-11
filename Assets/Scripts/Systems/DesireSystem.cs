using Unity.Entities;

public class DesireSystem : ComponentSystem {
    
    
    protected override void OnUpdate() {
        Entities.ForEach((Entity e, ref AiAgentComponent aiAgentComponent, ref DesireComponent desireComponent) => {
            
        });
    }
}