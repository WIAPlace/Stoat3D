using HSM;
using UnityEngine;

public class EnemyRoot : State
{
    readonly EnemyContext ctx;
    public EnemyWander EnemyWander;
    public EnemyInZone EnemyInZone;

    public EnemyRoot(StateMachine m, EnemyContext ctx) : base(m, null) 
    {
        this.ctx = ctx;
        EnemyWander = new EnemyWander(m,this,ctx);
        EnemyInZone = new EnemyInZone(m,this,ctx);
    }

    protected override State GetInitialState()=>EnemyWander;

    
}
