using HSM;
using UnityEngine;

public class Slide : State
{
    readonly PlayerContext ctx;

    float raycastLength = .5f;
    

    public Slide(StateMachine m, State parent, PlayerContext ctx) : base(m, parent) {
        this.ctx = ctx;
    }

    protected override State GetTransition() { 
        
        if (!ctx.grounded) {
            return ((PlayerRoot)Parent.Parent).Airborne;
        }
        
        // Idle
        if(Mathf.Abs(ctx.move.magnitude) <= 0.01f)
        {
            return ((Grounded)Parent.Parent).Idle;
        }
        
        if(Mathf.Abs(ctx.velocity.x) <= 0.01f && Mathf.Abs(ctx.velocity.z) <=0.1f) 
        {
            return ((Grounded)Parent.Parent).Idle;
        } 
        if (ctx.jumpPressed) { // slide Jump
            //ctx.jumpPressed = false;
            ctx.velocity *=ctx.slideJumpMod;
            ctx.velocity.y = ctx.jumpForce;
            ctx.jumpPressed = false;

            return ((PlayerRoot)Parent.Parent.Parent).Airborne;
        }
        
        if(!ctx.sprinting && !ctx.crouching)
        {
            return ((Move)Parent).Walk;
        }

        if (ctx.currentVelocityMag <= ctx.moveSpeed * ctx.crouchMod && !ctx.sprinting)
        {  // if just crouching and moving walk forward  and dont just come to a stop before continuing to walk forward
            return ((Move)Parent).MovingCrouch;
            // could turn sprinting to false instead of looking if its false if we want to set sprinting as a toggle.
        }
        return null;
    }
    

    protected override void OnEnter()
    {
        
        ctx.currentMoveSpeed = 0;
        
        ctx.simpleJump = false;
    }
    protected override void OnExit()
    {
        ctx.simpleJump = true;
    }
    protected override void OnUpdate(float deltaTime)
    {
        // Fire a ray downwards to get the ground normal
        if (Physics.Raycast(ctx.body.transform.position, Vector3.down, out RaycastHit hit, raycastLength))
        {
            Vector3 groundNormal = hit.normal;

            Vector3 moveDirection = (ctx.visualBody.transform.forward*ctx.move.z) + (ctx.visualBody.transform.right*ctx.move.x);

            // Project your input movement onto the slope plane
            Vector3 slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, groundNormal).normalized;

            // Check if we are actually tracking a slope angle
            float angle = Vector3.Angle(groundNormal, Vector3.up);

            if (angle > 0 && moveDirection.magnitude > 0)
            {
                // slopeMoveDirection.y will be positive moving up, negative moving down
                // Subtracting it from 1.0f means:
                // Uphill (pos Y) -> multiplier < 1 (slower)
                // Downhill (neg Y) -> multiplier > 1 (faster)
                float speedMultiplier = 1.0f - (slopeMoveDirection.y * ctx.slopeSlideInfluence);
                
                // Clamp multiplier so the player never stops completely or zooms too fast
                //speedMultiplier = Mathf.Clamp(speedMultiplier, 0.5f, 1.5f);

                ctx.currentMoveSpeed = ctx.moveSpeed * speedMultiplier;
            }
        }

        // Return flat ground movement if no slope/ground detected
        ctx.currentMoveSpeed = 0;
    }
}
