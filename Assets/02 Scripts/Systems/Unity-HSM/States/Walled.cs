using HSM;
using UnityEngine;

public class Walled : State
{
    readonly PlayerContext ctx;
    public readonly WallRun WallRun;
    public readonly WallSlide WallSlide;
    public Vector3 playerForward;

    public Walled(StateMachine m, State parent, PlayerContext ctx) : base(m, parent) {
        this.ctx = ctx;
        WallRun = new WallRun(m, this, ctx);
        WallSlide = new WallSlide(m, this, ctx);
    }
    protected override State GetInitialState() => WallRun;
    //protected override State GetTransition(){}


    protected override State GetTransition()
    {
        //return base.GetTransition();
        if (ctx.grounded)
        {
            return ((PlayerRoot)Parent).Grounded;
        }
        return ctx.walled ? null : ((PlayerRoot)Parent).Airborne;
    }
    
    protected override void OnEnter()
    {
        playerForward = ctx.body.transform.forward;
        base.OnEnter();
    }

    protected override void OnUpdate(float deltaTime)
    {
        
    }
    
}
