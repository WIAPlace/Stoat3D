using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;
using HSM;
using UnityEngine.AI;

// State machine driver for basic enemys caus behavior systems is annoying
public class EnemyStateDriver : MonoBehaviour
{
    public EnemyContext ctx = new EnemyContext();
    StateMachine machine;
    State root;


    void Awake()
    {
        root = new EnemyRoot(null, ctx);
        var builder = new StateMachineBuilder(root);
        machine = builder.Build();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        machine.Tick(Time.deltaTime);

        ctx.currentLeaf = machine.Root.Leaf();
        var path = StatePath(ctx.currentLeaf);
        ctx.debugCurrentLeaf = path;
    }

    public void OnZoneTriggered(bool context)
    {
        
    }

    // Misc /////////////////////////////////////////////////////////////////////////////////////////////////////////////
    static string StatePath(State s) {
        return string.Join(" > ", s.PathToRoot().Reverse().Select(n => n.GetType().Name));
    }
    
    private void OnDrawGizmosSelected()
    {
        if (ctx.WanderZone != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(ctx.WanderZone.position, ctx.WanderRadius);
        }
    }
}

// Enemy Context //////////////////////////////////////////////////////////////////////////////////////////////////////////
[Serializable]
public class EnemyContext
{
    public string debugCurrentLeaf;

    [Header("Wander")]
    public float WanderRadius;
    public float WanderTime;
    
    [Header("Alerted")]
    public float AlertTime;

    [Header("Attack")]
    public float WindUpTime;
    public float AttackTime;

    [Header("Bools")]
    public bool inZone;

    // refrences;
    public NavMeshAgent Agent;
    public State currentLeaf;
    public Transform WanderZone;
    public GameObject Target;
}
