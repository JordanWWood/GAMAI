using Unity.Entities;
using UnityEngine;

public class DOTSBootstrap : MonoBehaviour {
    public static DOTSBootstrap instance { get; private set; }
    public EntityManager EntityManager { get; private set; }
    
    private void Awake() {
        Debug.Assert(instance == null, "Multiple instances of the \"DOTSBootstrap\"");
        instance = this;
        this.EntityManager = World.Active.EntityManager;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
}
