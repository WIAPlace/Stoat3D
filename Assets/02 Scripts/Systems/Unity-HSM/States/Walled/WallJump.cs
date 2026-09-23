using UnityEngine;
using HSM;
public class WallJump :  State
{
    readonly PlayerContext ctx;

    public WallJump(StateMachine m, State parent, PlayerContext ctx) : base(m, parent) {
        this.ctx = ctx;
    }

    protected override void OnEnter()
    {
        Vector3 hitNormal = ctx.wallHit.normal;

        // Find a rotation axis perpendicular to the normal and up (e.g., the right vector relative to the hit)
        Vector3 rotationAxis = Vector3.Cross(hitNormal, Vector3.up);
        if (rotationAxis == Vector3.zero)
        {
            rotationAxis = Vector3.right; // Fallback if normal is perfectly straight up/down
        }
        // Rotate the normal 45 degrees upward around that axis
        Vector3 tiltedNormal = Quaternion.AngleAxis(ctx.wallJumpAngle, rotationAxis) * hitNormal;

        // Alter velocity to make jump at more of an angle.
        Vector3 newVel = ctx.velocity;

        newVel.x*=ctx.wallJumpMoveEffect;
        newVel.z*=ctx.wallJumpMoveEffect;
        newVel += tiltedNormal * ctx.wallJumpForceMod;

        ctx.velocity = newVel;
    }

    protected override void OnUpdate(float deltaTime)
    {
        if(ctx.grounded && ctx.velocity.y < 0) // if on ground reset gravity
        {
            ctx.velocity.y = 0;
        }
        //Debug.Log("grav");
        ctx.velocity.y += -ctx.gravForce * deltaTime; // apply gravity
            
        ctx.velocity.x /= 1f + ctx.drag * deltaTime;
        ctx.velocity.z /= 1f + ctx.drag * deltaTime;

        if(Mathf.Abs(ctx.velocity.x) < .01f)
        {
            ctx.velocity.x = 0;
        }
        if(Mathf.Abs(ctx.velocity.z) < .01f )
        {
           ctx.velocity.z = 0;
        }
    }
    

}
