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
        playerForward = ctx.body.transform.forward.normalized;

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
        ctx.velocity = new Vector3(localVelocity.x, 0 , localVelocity.z);
    }
    
    protected override void OnUpdate(float deltaTime)
    {
        RaycastHit hit = ctx.wallHit;

        Vector3 surfaceTangent = Vector3.ProjectOnPlane(playerForward,hit.normal).normalized;
        surfaceTangent.y = 0;
        surfaceTangent = surfaceTangent.normalized;

        playerForward = surfaceTangent;

        
        

        ctx.body.transform.forward = surfaceTangent; // turn body to face forward along the walls rotation

        ctx.velocity = surfaceTangent * ctx.currentMoveSpeed;
    }
    
}
