using UnityEngine;
using HSM;

public class Attack : State
{
    readonly PlayerContext ctx;
    public readonly BasicAttack BasicAttack;
    
    public Attack(StateMachine m, State parent, PlayerContext ctx) : base(m, parent) {
        this.ctx = ctx;
        BasicAttack = new BasicAttack(m, this, ctx);
    }

    protected override State GetTransition()
    {
        return base.GetTransition();
    }

    protected override State GetInitialState() => BasicAttack;

    protected override void OnEnter()
    {
        ctx.attacking = false;
        
    }
}
