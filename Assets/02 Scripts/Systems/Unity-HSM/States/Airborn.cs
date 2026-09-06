using UnityEngine;

namespace HSM {
    public class Airborne : State {
        readonly PlayerContext ctx;

        public Airborne(StateMachine m, State parent, PlayerContext ctx) : base(m, parent) {
            this.ctx = ctx;
            Add(new ColorPhaseActivity(ctx.renderer){
                enterColor = Color.red, // runs while Airborne is activating
            });
        }
        
        protected override State GetTransition() => ctx.grounded ? ((PlayerRoot)Parent).Grounded : null;

        protected override void OnEnter() {
            // TODO: Update Animator through ctx.anim
            //Debug.Log("Entered Airborn");
        }
        protected override void OnUpdate(float deltaTime)
        {
            if(ctx.grounded && ctx.velocity.y < 0) // if on ground reset gravity
            {
                ctx.velocity.y = 0;
            }
            //Debug.Log("grav");
            ctx.velocity.y += -ctx.gravForce * deltaTime; // apply gravity

            // Match the player body's Y rotation to the camera target's Y rotation
            // Maybe change this to be changing a value rather than changing it directly in the state itself.
            //Vector3 targetRotation = new Vector3(0, ctx.cinCamTransform.eulerAngles.y, 0);
            //ctx.controller.transform.rotation = Quaternion.Euler(targetRotation);
        }
    }
}
