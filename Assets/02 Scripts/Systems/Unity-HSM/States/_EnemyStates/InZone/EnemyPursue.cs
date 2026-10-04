using HSM;
using UnityEngine;

public class EnemyPursue : State
{
    readonly EnemyContext ctx;
    float timer = 0;

    public EnemyPursue(StateMachine m,State parent, EnemyContext ctx) : base(m, parent) 
    {
        this.ctx = ctx;
    }

    protected override State GetTransition()
    {
        if(HasReachedStoppingDist()) return ((EnemyInZone)Parent).EnemyWindUp;
        return base.GetTransition();
    }

    private bool HasReachedStoppingDist()
    {
        if(ctx.Agent.pathPending) return false;

        if(ctx.Agent.remainingDistance <= ctx.Agent.stoppingDistance)
        {
            if(!ctx.Agent.hasPath || ctx.Agent.velocity.sqrMagnitude == 0f)
            {
                return true;
            }
        }
        return false;
    }

    protected override void OnEnter()
    {
        ctx.Agent.destination = ctx.Target.transform.position;
        timer = 0; 
    }

    protected override void OnUpdate(float deltaTime)
    {
        timer += deltaTime;
        if(timer >= .2f)
        {
            ctx.Agent.destination = ctx.Target.transform.position;
            timer = 0;
        }
    }
}
