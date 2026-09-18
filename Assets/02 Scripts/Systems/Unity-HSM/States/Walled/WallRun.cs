using HSM;
using UnityEngine;

public class WallRun : State
{
    readonly PlayerContext ctx;

    private Vector3 initialVelocity;

    public WallRun(StateMachine m, State parent, PlayerContext ctx) : base(m, parent) {
        this.ctx = ctx;
    }

    protected override State GetTransition()
    {
        return base.GetTransition();
    }
    
    protected override void OnEnter()
    {
        initialVelocity = ctx.velocity;
    }

    protected override void OnUpdate(float deltaTime)
    {
        ctx.currentMoveSpeed = ctx.moveSpeed*ctx.sprintMod;
    }
}
