using UnityEngine;

namespace HSM{
    public class Fall : State
    {
        readonly PlayerContext ctx;

        public Fall(StateMachine machine, State parent, PlayerContext ctx) : base(machine, parent)
        {
            this.ctx = ctx;
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

            if(ctx.velocity.x < .1f)
            {
                ctx.velocity.x = 0;
            }
            if(ctx.velocity.z < .1f)
            {
                ctx.velocity.z = 0;
            }

            // turn visual player to dir of cam.
            // will probably be taken off of this parent state, and only put on sertain child states.
            ctx.TurnToForward(deltaTime);
        }
    }
}
