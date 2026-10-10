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
        ctx.isHit = true;
        StartCoroutine(KnockbackRoutine(hitDir,force,verticalForce,1));
    }

    private IEnumerator KnockbackRoutine(Vector3 direction, float distance, float arcHeight, float duration)
    {
        // 1. Disable the NavMeshAgent so physics/math can take over
        if (ctx.Agent != null && ctx.Agent.enabled)
        {
            ctx.Agent.enabled = false;
        }

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + (direction.normalized * distance);
        float elapsed = 0f;

        // Ensure target is valid or clamped if needed, or just move freely in air
        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // Linear interpolation for horizontal movement
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);

            // Parabolic arc offset using a sine wave or parabola formula (4 * h * t * (1 - t))
            float heightOffset = 4f * arcHeight * t * (1f - t);
            currentPos.y += heightOffset;

            transform.position = currentPos;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        ctx.isHit=false;
        // 2. Re-enable the NavMeshAgent and warp to the landed position
        if (ctx.Agent != null)
        {
            ctx.Agent.enabled = true;
            // Warp ensures the agent locks back onto the nearest point on the NavMesh
            ctx.Agent.Warp(transform.position);
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

    [Header("Hit")]
    public float stunTime;
    public float verticalVelocity;

    [Header("Bools")]
    public bool inZone;
    public bool isHit;



    // refrences;
    public NavMeshAgent Agent;
    public State currentLeaf;
    public Transform WanderZone;
    public GameObject Target;


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
}
