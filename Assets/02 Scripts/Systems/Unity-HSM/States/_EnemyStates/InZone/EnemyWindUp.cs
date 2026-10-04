using System.Threading;
using HSM;
using UnityEngine;

public class EnemyWindUp : State
{
    readonly EnemyContext ctx;
    float timer = 0;

    public EnemyWindUp(StateMachine m,State parent, EnemyContext ctx) : base(m, parent) 
    {
        this.ctx = ctx;
    }

    protected override State GetTransition()
    {
        if(timer >= ctx.WindUpTime) return ((EnemyInZone)Parent).EnemyAttack;
        return base.GetTransition();
    }

    protected override void OnEnter()
    {
        timer = 0;
        Debug.Log("Winding Up To Attack");
    }

    protected override void OnUpdate(float deltaTime)
    {
        timer += deltaTime;
    }

}
