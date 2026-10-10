using System.Threading;
using HSM;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHit :State
{
    readonly EnemyContext ctx;


    public EnemyHit(StateMachine m,State parent, EnemyContext ctx) : base(m, parent) 
    {
        this.ctx = ctx;
    }

    protected override State GetTransition()
    {
        if (!ctx.isHit)
        {
            if(ctx.inZone)return ((EnemyRoot)Parent).EnemyInZone;
            else return ((EnemyRoot)Parent).EnemyWander;
        }
        return base.GetTransition();
    }

    protected override void OnEnter()
    {

    }
    protected override void OnExit()
    {
        
    }
}
