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
        if (ctx.jumpPressed) { // slide Jump
            //ctx.jumpPressed = false;
            ctx.velocity *=ctx.slideJumpMod;
            ctx.velocity.y = ctx.jumpForce;
            ctx.jumpPressed = false;

            return ((PlayerRoot)Parent.Parent.Parent).Airborne;
        }
        
        if(!ctx.sprinting && !ctx.crouching)
        {
            return ((Move)Parent).Walk;
        }

        if (ctx.currentVelocityMag <= ctx.moveSpeed * ctx.crouchMod && !ctx.sprinting)
        {  // if just crouching and moving walk forward  and dont just come to a stop before continuing to walk forward
            return ((Move)Parent).MovingCrouch;
            // could turn sprinting to false instead of looking if its false if we want to set sprinting as a toggle.
        }
        /*
        if (ctx.sprinting && !ctx.crouching)
        {
            return ((Move)Parent).Run;
        }
        if (ctx.crouching && !ctx.sprinting)
        {
            return ((Move)Parent).MovingCrouch;
        }
        */
        return null;
    }
    

    protected override void OnEnter()
    {
        ctx.currentMoveSpeed = 0;
        
        ctx.simpleJump = false;
    }
    protected override void OnExit()
    {
        ctx.simpleJump = true;
    }
}
