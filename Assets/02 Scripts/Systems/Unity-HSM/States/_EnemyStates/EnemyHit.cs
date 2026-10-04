using HSM;
using UnityEngine;

public class EnemyHit :State
{
    readonly EnemyContext ctx;

    public EnemyHit(StateMachine m,State parent, EnemyContext ctx) : base(m, parent) 
    {
        this.ctx = ctx;
    }
}
