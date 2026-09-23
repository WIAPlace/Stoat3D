using HSM;
using UnityEngine;

public class IdleCrouch : State
{
    readonly PlayerContext ctx;

    public IdleCrouch(StateMachine m, State parent, PlayerContext ctx) : base(m, parent) {
        this.ctx = ctx;
    }
    protected override State GetTransition()
    {
        if (ctx.jumpPressed) { // Crouch Jump
            //ctx.jumpPressed = false;
            ctx.velocity.y = ctx.jumpForce*ctx.crouchJumpMod;
            ctx.jumpPressed = false;

            return ((PlayerRoot)Parent.Parent.Parent).Airborne;
        }

        return ctx.crouching ? null : ((Idle)Parent).Stand;
    }
    protected override void OnEnter()
    {
        if(ctx.crouching)
        {
            ctx.simpleJump = false;
        }
    }
    protected override void OnExit()
    {
        ctx.simpleJump = true;
    }
}
