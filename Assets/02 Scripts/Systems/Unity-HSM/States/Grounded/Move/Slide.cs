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
            return ((PlayerRoot)Parent.Parent).Airborne;
        }
        
        // Idle
        if(Mathf.Abs(ctx.move.magnitude) <= 0.01f)
        {
            return ((Grounded)Parent.Parent).Idle;
        }
        if(Mathf.Abs(ctx.velocity.x) <= 0.01f && Mathf.Abs(ctx.velocity.z) <=0.1f) 
        {
            return ((Grounded)Parent.Parent).Idle;
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
