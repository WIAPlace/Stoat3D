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

        protected override State GetTransition() => ctx.grounded ? ((PlayerRoot)Parent).Grounded : null;

        protected override void OnEnter() {
            //Debug.Log(ctx.velocity);
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
        }
    }
}
