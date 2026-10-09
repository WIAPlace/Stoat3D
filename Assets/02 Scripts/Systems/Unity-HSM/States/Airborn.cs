using UnityEditor;
using UnityEngine;

namespace HSM {
    public class Airborne : State {
        readonly PlayerContext ctx;
        public readonly State Jump;
        public readonly State Fall; 
        
        


        public Airborne(StateMachine m, State parent, PlayerContext ctx) : base(m, parent) {
            this.ctx = ctx;

            Jump = new Jump(m, this, ctx);
            Fall = new Fall(m, this, ctx);

            Add(new ColorPhaseActivity(ctx.renderer){
                enterColor = Color.red, // runs while Airborne is activating
            });
        }
        //protected override State GetInitialState() => Jump;

        protected override State GetTransition()
        {
            if(ctx.crouching && ctx.grounded)
            {
                return ((PlayerRoot)Parent).Grounded.Move.Slide;
            }
            if (ctx.grounded)
            {
                return ((PlayerRoot)Parent).Grounded;
            }
            if (ctx.ledgeGrabbed)
            {
                return ((PlayerRoot)Parent).LedgeGrab;
            }
            if (ctx.walled )
            {
                return ((PlayerRoot)Parent).Walled;
            }
            return null;
        } 

        protected override void OnEnter() {
            
            // Store your target world-space velocity 
            if(ctx.tempWorldMovment.magnitude > 0){
                Vector3 targetWorldVelocity = ctx.tempWorldMovment/Time.deltaTime;
                ctx.tempWorldMovment = Vector3.zero;
                // Convert the world vector back into the body's local space
                Vector3 localVelocity = ctx.body.transform.InverseTransformDirection(targetWorldVelocity);
                
                // Assign the calculated local x and z components back to ctx.velocity
                //ctx.velocity = new Vector3(localVelocity.x, 0 , localVelocity.z);
                ctx.velocity += localVelocity;
            }


            ctx.useInitialForward = true;
            ctx.forwardDir = ctx.body.transform.forward;
            ctx.rightDir = ctx.body.transform.right;
        }
        protected override void OnExit()
        {
            ctx.useInitialForward = false;
        }
        protected override void OnUpdate(float deltaTime)
        {
            //Debug.Log(ctx.velocity + "  (Update)");
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

            // turn visual player to forward
            //ctx.TurnToForward(deltaTime);
            ctx.TurnMechanicalBodyForward(deltaTime);
            ctx.TurnVisualBodyToMoveForward(deltaTime);
        }
    }
}
