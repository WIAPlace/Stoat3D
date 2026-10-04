using System.Threading;
using HSM;
using UnityEngine;

public class EnemyAttack : State
{
    readonly EnemyContext ctx;
    float timer;

    public EnemyAttack(StateMachine m,State parent, EnemyContext ctx) : base(m, parent) 
    {
        this.ctx = ctx;
    }

    protected override State GetTransition()
    {
        if(timer>=ctx.AttackTime)return ((EnemyInZone)Parent).EnemyPursue;
        return base.GetTransition();
    }

    protected override void OnEnter()
    {
        Debug.Log("Enemy Attacks");
        timer = 0;
    }

    protected override void OnUpdate(float deltaTime)
    {
        timer+=deltaTime;
    }
}
