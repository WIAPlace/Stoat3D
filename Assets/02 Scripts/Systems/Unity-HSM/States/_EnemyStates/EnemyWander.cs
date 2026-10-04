using HSM;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWander : State
{
    readonly EnemyContext ctx;
    public float wanderTimer;

    public EnemyWander(StateMachine m,State parent, EnemyContext ctx) : base(m, parent) 
    {
        this.ctx = ctx;
    }

    protected override State GetTransition()
    {
        if(ctx.inZone) return ((EnemyRoot)Parent).EnemyInZone;

        return base.GetTransition();
    }
    protected override void OnEnter()
    {
        wanderTimer = 5;
    }
    protected override void OnExit()
    {
        ctx.Agent.ResetPath();
    }

    protected override void OnUpdate(float deltaTime)
    {
        wanderTimer+=deltaTime;
        if(wanderTimer >= ctx.WanderTime)
        {
            Vector3 randomSpherePoint = ctx.WanderZone.position + (Random.insideUnitSphere * ctx.WanderRadius);
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomSpherePoint, out hit, ctx.WanderRadius, NavMesh.AllAreas))
            {
                // 3. Set the agent's destination to the valid NavMesh point
                ctx.Agent.SetDestination(hit.position);
                //Debug.Log(hit.position);
            }
            else
            {
                Debug.LogWarning("Could not find a valid NavMesh point within the sphere.");
            }
            wanderTimer = 0;
        }
    }
}
