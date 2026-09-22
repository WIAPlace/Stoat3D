using HSM;
using UnityEngine;

public class Walled : State
{
    readonly PlayerContext ctx;
    public readonly WallRun WallRun;
    public readonly WallSlide WallSlide;
    public readonly WallJump WallJump;
    

    public Walled(StateMachine m, State parent, PlayerContext ctx) : base(m, parent) {
        this.ctx = ctx;
        WallRun = new WallRun(m, this, ctx);
        WallSlide = new WallSlide(m, this, ctx);
        WallJump = new WallJump(m, this, ctx);
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
        

        Vector3 targetPosition = ctx.wallHit.point + (ctx.wallHit.normal * ctx.wallSnapLength);
        ctx.externalPush = targetPosition - ctx.body.transform.position;
        base.OnEnter();
    }

    protected override void OnExit()
    {
        // Store your target world-space velocity 
        Vector3 targetWorldVelocity = ctx.velocity;

        // Convert the world vector back into the body's local space
        Vector3 localVelocity = ctx.body.transform.InverseTransformDirection(targetWorldVelocity);

        // Assign the calculated local x and z components back to ctx.velocity
        //ctx.velocity = new Vector3(localVelocity.x, 0 , localVelocity.z);
        ctx.velocity = localVelocity;
    }
    
    protected override void OnUpdate(float deltaTime)
    {
        
    }
    
}
