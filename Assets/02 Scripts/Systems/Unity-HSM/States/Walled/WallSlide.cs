using HSM;
using UnityEngine;

public class WallSlide : State
{
    readonly PlayerContext ctx;

    public WallSlide(StateMachine m, State parent, PlayerContext ctx) : base(m, parent) {
        this.ctx = ctx;
    }

    protected override State GetTransition()
    {
        return base.GetTransition();
    }
    
    protected override void OnEnter()
    {
        base.OnEnter();
    }

    protected override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
    }
}
