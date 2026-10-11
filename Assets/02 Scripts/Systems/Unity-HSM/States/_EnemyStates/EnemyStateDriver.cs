using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;
using HSM;
using UnityEngine.AI;

// State machine driver for basic enemys caus behavior systems is annoying
public class EnemyStateDriver : MonoBehaviour, IHitAble
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
        ctx.rb = GetComponent<Rigidbody>();
        ctx.Agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {

        machine.Tick(Time.deltaTime);

        ctx.currentLeaf = machine.Root.Leaf();
        var path = StatePath(ctx.currentLeaf);
        ctx.debugCurrentLeaf = path;
    }
    private void FixedUpdate()
    {
        if (ctx.shouldApplyForce)
        {
            ctx.rb.AddForce(ctx.hitForce,ForceMode.Impulse);
            ctx.shouldApplyForce = false;
            ctx.hitForce = Vector3.zero;
        }
    }

    public void OnZoneTriggered(bool context)
    {
        ctx.inZone = context;
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

    public void OnHit(Vector3 hitFrom, float force, float verticalForce, float damage)
    {
        // hit from is where the player is, force is how much force will be applided and damage is damage.
        Vector3 hitDir = (transform.position - hitFrom).normalized;
        hitDir.y = 0;
        hitDir = hitDir*force;
        hitDir.y = verticalForce;

        ctx.hitForce = hitDir;
        
        ctx.isHit = true;

        // Cancel any previous instance running on this context before starting a new one
        if (ctx._cts != null)
        {
            ctx._cts.Cancel();
        }
        //StartCoroutine(KnockbackRoutine(hitDir,force,verticalForce,1));
    }

    private void OnDestroy()
    {
        // Always clean up and trigger cancellation when the object is destroyed
        ctx._cts?.Cancel();
        ctx._cts?.Dispose();
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

    [Header("Hit")]
    public float stunTime;
    public float verticalVelocity;
    public Vector3 hitForce;
    public float MaxKnockbackTime;
    public float StillThreshold = 0.05f;
    public bool shouldApplyForce = false;

    [Header("Bools")]
    public bool inZone;
    public bool isHit;



    // refrences;
    public NavMeshAgent Agent;
    public Rigidbody rb;
    public State currentLeaf;
    public Transform WanderZone;
    public GameObject Target;
    public CancellationTokenSource _cts;


    public bool IsGrounded()
    {
        // Use a small downwards raycast or check if the agent position matches the underlying navmesh
        NavMeshHit hit;
        // Check a tiny radius around the bottom of the object
        if (NavMesh.SamplePosition(Agent.gameObject.transform.position, out hit, 0.3f, NavMesh.AllAreas))
        {
            // If the distance to the nearest point on the navmesh is negligible, we are grounded
            return Mathf.Abs(Agent.gameObject.transform.position.y - hit.position.y) < 0.1f;
        }
        return false;
    }

    public static async Awaitable WaitUntilAsync(Func<bool> condition, CancellationToken cancellationToken)
    {
        while (!condition())
        {
            // Safely exit if the target object is destroyed or operation is canceled
            cancellationToken.ThrowIfCancellationRequested(); 
            
            // Wait for the next frame before checking the condition again
            await Awaitable.NextFrameAsync(cancellationToken); 
        }
    }
}
