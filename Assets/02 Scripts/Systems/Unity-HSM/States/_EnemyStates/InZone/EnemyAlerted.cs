using HSM;
using UnityEngine;

// the reaction to the player entering the zone;
public class EnemyAlerted : State
{
    readonly EnemyContext ctx;
    private float alertTimer;

    public EnemyAlerted(StateMachine m,State parent, EnemyContext ctx) : base(m, parent) 
    {
        this.ctx = ctx;
    }


    protected override State GetTransition()
    {
        if(alertTimer >= ctx.AlertTime) return ((EnemyInZone)Parent).EnemyPursue;
        return base.GetTransition();
    }

    protected override void OnEnter()
    {
        alertTimer = 0;
    }

    protected override void OnUpdate(float deltaTime)
    {
        alertTimer += deltaTime;
    }
}
