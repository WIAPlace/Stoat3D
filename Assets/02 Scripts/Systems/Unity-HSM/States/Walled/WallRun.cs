using HSM;
using UnityEngine;

public class WallRun : State
{
    readonly PlayerContext ctx;

    private float initialVelocity;
    private float timer=0;
    public float checkIntervals = 1f;
    public float tolerance = .1f;
    private bool stuck=false;
    public Vector3 playerForward;
    private bool jumpPressed = false;
    private float jumpSuppresed = 0;


    public WallRun(StateMachine m, State parent, PlayerContext ctx) : base(m, parent) {
        this.ctx = ctx;
    }

    protected override State GetTransition()
    {
        if (jumpPressed&&jumpSuppresed > .1f)
        {
            return ((Walled)Parent).WallJump;
        }
        if (stuck)
        {
            return ((Walled)Parent).WallSlide;
        }
        return base.GetTransition();
    }
    
    protected override void OnEnter()
    {
        initialVelocity = ctx.currentVelocityMag;
        ctx.lastPosition = ctx.body.transform.position;

        playerForward = ctx.visualBody.transform.forward.normalized;
        stuck=false;
        jumpPressed = false;
        jumpSuppresed = 0;
    }

    protected override void OnUpdate(float deltaTime)
    {
        ctx.currentMoveSpeed = initialVelocity;

        // Stuck Timer 
        timer+=deltaTime;
        if (timer >= checkIntervals)
        {
            float distanceMoved = Vector3.Distance(ctx.body.transform.position, ctx.lastPosition);
            if (distanceMoved < tolerance)
            {
                // The object has barely moved – it is stuck!
                //Debug.Log("Object is stuck!");
                stuck=true;
            }
            ctx.lastPosition = ctx.body.transform.position;
            timer = 0;
        }

        RaycastHit hit = ctx.wallHit;
        Vector3 surfaceTangent = Vector3.ProjectOnPlane(playerForward,hit.normal).normalized;
        surfaceTangent.y = 0;
        surfaceTangent = surfaceTangent.normalized;

        playerForward = surfaceTangent;
    
        ctx.body.transform.forward = surfaceTangent; // turn body to face forward along the walls rotation

        ctx.velocity = surfaceTangent * ctx.currentMoveSpeed;

        if (ctx.jumpPressed)
        {
            jumpPressed = true;
        }
        jumpSuppresed += deltaTime;
    }
}
