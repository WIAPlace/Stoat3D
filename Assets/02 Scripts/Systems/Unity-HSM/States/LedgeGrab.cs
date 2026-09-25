using UnityEngine;
using HSM;


public class LedgeGrab : State {
    readonly PlayerContext ctx;
    
    public LedgeGrab(StateMachine m, State parent, PlayerContext ctx) : base(m, parent) {
        this.ctx = ctx;
    }

    protected override State GetTransition()
    {
        if (ctx.crouching)
        {
            return ((PlayerRoot)Parent).Airborne;
        }
        if (ctx.jumpPressed)
        {
            ctx.velocity = (ctx.body.transform.right * ctx.move.x * ctx.horizontalLedgeJumpForce) + 
                            (ctx.body.transform.forward * ctx.move.z * ctx.horizontalLedgeJumpForce);
            ctx.velocity.y = ctx.jumpForce * ctx.verticalLedgeJumpMod;
            return ((PlayerRoot)Parent).Airborne;
        }
        // if jump, jump
        return null;
    }

    protected override void OnEnter()
    {
        
        ctx.lastLedge = ctx.ledgeHit.transform;

        ctx.velocity = Vector3.zero;
    }

    protected override void OnExit()
    {
        ctx.ledgeGrabbed = false;

        // Store your target world-space velocity 
        Vector3 targetWorldVelocity = ctx.velocity;

        // Convert the world vector back into the body's local space
        Vector3 localVelocity = ctx.body.transform.InverseTransformDirection(targetWorldVelocity);

        // Assign the calculated local x and z components back to ctx.velocity
        //ctx.velocity = new Vector3(localVelocity.x, 0 , localVelocity.z);
        ctx.velocity = localVelocity;

        ctx.LastLedgeReset();
    }

    protected override void OnUpdate(float deltaTime)
    {
        Vector3 directionToLedge = ctx.currLedge.position - ctx.body.transform.position;
        float ledgeDist = Vector3.Distance(ctx.body.transform.position, ctx.currLedge.position);

        if (ledgeDist > 1f)
        {   
            if(ctx.velocity.magnitude<ctx.moveToLedgeSpeed)
                ctx.velocity += directionToLedge.normalized * ctx.moveToLedgeSpeed * deltaTime;
        }
        else
        {
            if (ctx.velocity.magnitude != 0)
            {
                ctx.velocity = Vector3.zero;
            }
        }
    }

    
    
}
