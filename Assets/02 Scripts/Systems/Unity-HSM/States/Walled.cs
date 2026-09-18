using HSM;
using UnityEngine;

public class Walled : State
{
    readonly PlayerContext ctx;
    public readonly WallRun WallRun;
    public readonly WallSlide WallSlide;

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
        base.OnEnter();
    }

    protected override void OnUpdate(float deltaTime)
    {
        //base.OnUpdate(deltaTime);
        Vector3 wallNormal = ctx.wallRight ? ctx.rightWallhit.normal : ctx.leftWallhit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal,ctx.body.transform.up);

        // reverse forward if in wrong direction. Might want to totaly change this. no garente that forward is side to side on every model
        if((ctx.body.transform.forward - wallForward).magnitude>(ctx.body.transform.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }
        ctx.velocity.y = 0;
        
        float targetx = wallForward.x * ctx.currentMoveSpeed;
        float targetz = wallForward.z * ctx.currentMoveSpeed;

        targetx = Mathf.MoveTowards(ctx.velocity.x, targetx, ctx.accel*deltaTime);
        targetz = Mathf.MoveTowards(ctx.velocity.z, targetz, ctx.accel*deltaTime);


        ctx.velocity.x = targetx;
        ctx.velocity.z = targetz;
    }
    
}
