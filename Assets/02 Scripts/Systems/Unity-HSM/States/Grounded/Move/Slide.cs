using HSM;
using UnityEngine;

public class Slide : State
{
    readonly PlayerContext ctx;

    public Slide(StateMachine m, State parent, PlayerContext ctx) : base(m, parent) {
        this.ctx = ctx;
    }

    protected override State GetTransition() { 
        
        if (!ctx.grounded) {
            return ((PlayerRoot)Parent).Airborne;
        }
        
        if(Mathf.Abs(ctx.move.magnitude) <= 0.01f)
        {
            return ((Grounded)Parent).Idle;
        }
        
        if(!ctx.sprinting && !ctx.crouching)
        {
            return ((Move)Parent).Walk;
        }
        if (ctx.sprinting && !ctx.crouching)
        {
            return ((Move)Parent).Run;
        }
        if (ctx.crouching && !ctx.sprinting)
        {
            return ((Move)Parent).MovingCrouch;
        }

        return null;
    }
    

    protected override void OnEnter()
    {
        if(ctx.sprinting && ctx.crouching)
        {
            ctx.currentMoveSpeed = 0;
        }
    }
}
