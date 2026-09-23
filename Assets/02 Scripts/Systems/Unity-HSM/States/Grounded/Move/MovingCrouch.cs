using HSM;
using UnityEngine;

public class MovingCrouch : State
{
    readonly PlayerContext ctx;

    public MovingCrouch(StateMachine m, State parent, PlayerContext ctx) : base(m, parent) {
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
        
        if(ctx.sprinting && ctx.crouching)
        {
            return ((Move)Parent).Slide;
        }
        if (ctx.sprinting)
        {
            return ((Move)Parent).Run;
        }
        if (!ctx.crouching)
        {
            return ((Move)Parent).Walk;
        }
        if (ctx.jumpPressed) { // Crouch Jump
            //ctx.jumpPressed = false;
            ctx.velocity.y = ctx.jumpForce*ctx.crouchJumpMod;
            ctx.jumpPressed = false;

            return ((PlayerRoot)Parent.Parent.Parent).Airborne;
        }

        return null;
    }

    protected override void OnEnter()
    {
        if(ctx.crouching)
        {
            ctx.currentMoveSpeed = ctx.moveSpeed * ctx.crouchMod;
            ctx.simpleJump = false;
        }
    }
    protected override void OnExit()
    {
        ctx.simpleJump = true;
    }


}
