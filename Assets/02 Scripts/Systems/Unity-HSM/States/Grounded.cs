using UnityEngine;

namespace HSM {
    public class Grounded : State {
        readonly PlayerContext ctx;
        public readonly Idle Idle;
        public readonly Move Move;

        public Grounded(StateMachine m, State parent, PlayerContext ctx) : base(m, parent) {
            this.ctx = ctx;
            Idle = new Idle(m, this, ctx);
            Move = new Move(m, this, ctx);
            Add(new ColorPhaseActivity(ctx.renderer){
                enterColor = Color.yellow,  // runs while Grounded is activating
            });
        }
        
        protected override State GetInitialState() => Idle;

        protected override State GetTransition() {
            if (ctx.jumpPressed) { // Jump
                //ctx.jumpPressed = false;
                ctx.velocity.y = ctx.jumpForce;
                ctx.jumpPressed = false;

                return ((PlayerRoot)Parent).Airborne;
            }
            return ctx.grounded ? null : ((PlayerRoot)Parent).Airborne;
        }
        protected override void OnEnter()
        {
            //Debug.Log("Entered Grounded");
        }

        protected override void OnUpdate(float deltaTime)
        {
            if(ctx.grounded && ctx.velocity.y < 0) // if on ground reset gravity
            {
                ctx.velocity.y = 0;
            }
            //Debug.Log("Move");
            ctx.velocity.y += -ctx.gravForce * deltaTime; // apply gravity

            // Match the player body's Y rotation to the camera target's Y rotation
            // Maybe change this to be changing a value rather than changing it directly in the state itself.
            //Vector3 targetRotation = new Vector3(0, ctx.cinCamTransform.eulerAngles.y, 0);
            //ctx.controller.transform.rotation = Quaternion.Euler(targetRotation);
        }
    }
}