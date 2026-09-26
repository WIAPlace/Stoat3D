using HSM;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;

public class WallRun : State
{
    readonly PlayerContext ctx;

    private float initialVelocity;
    public float checkIntervals = 1f;
    public float tolerance = .1f;
    public Vector3 playerForward;
    private bool jumpPressed = false;
    private float jumpSuppresed = 0;
    private float gravity;


    public WallRun(StateMachine m, State parent, PlayerContext ctx) : base(m, parent) {
        this.ctx = ctx;
    }

    protected override State GetTransition()
    {
        if (jumpPressed&&jumpSuppresed > .1f)
        {
            return ((Walled)Parent).WallJump;
        }
        
        return base.GetTransition();
    }
    
    protected override void OnEnter()
    {
        initialVelocity = ctx.currentVelocityMag;
        ctx.lastPosition = ctx.body.transform.position;

        playerForward = ctx.visualBody.transform.forward.normalized;
        jumpPressed = false;
        jumpSuppresed = 0;
        gravity = 0;

        

    }

    protected override void OnUpdate(float deltaTime)
    {
        float desiredSpeed;
        if(Mathf.Abs(ctx.move.x)>0.01f || Mathf.Abs(ctx.move.z)>0.01f)
        {
            desiredSpeed = initialVelocity;
        }
        else
        {
            desiredSpeed = 0;
        }

        if (gravity < ctx.gravForce)
        {
            gravity-=ctx.wallFallSpeed*deltaTime;
        }

        // find the surface tangent
        RaycastHit hit = ctx.wallHit;
        Vector3 surfaceTangent = Vector3.ProjectOnPlane(playerForward,hit.normal).normalized;
        surfaceTangent.y = 0;
        surfaceTangent = surfaceTangent.normalized;

        playerForward = surfaceTangent;
    
        ctx.body.transform.forward = surfaceTangent; // turn body to face forward along the walls rotation


        // rate of change based off of if you are moving forward fast enough
        float rateOfChange;
        if(desiredSpeed >= initialVelocity)rateOfChange = ctx.accel;
        else rateOfChange = ctx.decel;
        
        ctx.currentMoveSpeed = Mathf.MoveTowards(ctx.currentMoveSpeed,desiredSpeed,rateOfChange*deltaTime);
        // the desired velocity at this current velocity
        ctx.velocity = surfaceTangent * ctx.currentMoveSpeed;
        ctx.velocity.y = gravity;


        // hold gate for jump pressed;
        if (ctx.jumpPressed && !jumpPressed)
        {
            jumpPressed = true;
        }
        if(jumpSuppresed<.2f)jumpSuppresed += deltaTime;
    }
}
