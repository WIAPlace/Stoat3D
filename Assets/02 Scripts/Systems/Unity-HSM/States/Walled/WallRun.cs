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

    public WallRun(StateMachine m, State parent, PlayerContext ctx) : base(m, parent) {
        this.ctx = ctx;
    }

    protected override State GetTransition()
    {
        if (ctx.jumpPressed)
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

        playerForward = ctx.body.transform.forward.normalized;
        stuck=false;
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
    }
}
