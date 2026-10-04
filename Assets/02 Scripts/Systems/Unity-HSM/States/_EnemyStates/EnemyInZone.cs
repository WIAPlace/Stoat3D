using HSM;
using UnityEngine;

public class EnemyInZone : State
{
    readonly EnemyContext ctx;
    public EnemyAlerted EnemyAlerted;
    public EnemyPursue EnemyPursue;
    public EnemyWindUp EnemyWindUp;
    public EnemyAttack EnemyAttack;

    public EnemyInZone(StateMachine m,State parent, EnemyContext ctx) : base(m, parent) 
    {
        this.ctx = ctx;
        EnemyAlerted = new EnemyAlerted(m,this,ctx);
        EnemyPursue = new EnemyPursue(m,this,ctx);
        EnemyWindUp = new EnemyWindUp(m,this,ctx);
        EnemyAttack = new EnemyAttack(m,this,ctx);
    }

    protected override State GetInitialState()=>EnemyAlerted;

    protected override State GetTransition()
    {
        if(!ctx.inZone) return ((EnemyRoot)Parent).EnemyWander;

        return base.GetTransition();
    }

    
    
}
