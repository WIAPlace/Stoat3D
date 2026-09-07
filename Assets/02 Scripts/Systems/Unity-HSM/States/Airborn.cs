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

            // turn visual player to dir of cam.
            // will probably be taken off of this parent state, and only put on sertain child states.
            ctx.TurnToForward(deltaTime);
        }
    }
}
