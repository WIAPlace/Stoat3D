using UnityEngine;

namespace HSM {
    public class Move : State {
        readonly PlayerContext ctx;

        public Move(StateMachine m, State parent, PlayerContext ctx) : base(m, parent) {
            this.ctx = ctx;
        }

        protected override State GetTransition() {
            if (!ctx.grounded) return ((PlayerRoot)Parent).Airborne;
            
            return Mathf.Abs(ctx.move.magnitude) <= 0.01f ? ((Grounded)Parent).Idle : null;
        }

        protected override void OnUpdate(float deltaTime)
        {
            //Debug.Log("Walk");
            float targetx = ctx.move.x * ctx.moveSpeed;
            float targetz = ctx.move.z * ctx.moveSpeed;
            ctx.velocity.x = targetx;
            ctx.velocity.z = targetz;

            // turn visual player to forward
            ctx.TurnToForward(deltaTime);
        }
    }
}