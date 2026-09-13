using System.ComponentModel;
using System.Diagnostics;
using UnityEngine;

namespace HSM {
    public class Move : State {
        readonly PlayerContext ctx;
        public readonly Run Run;
        public readonly Walk Walk;
        public readonly MovingCrouch MovingCrouch;
        public readonly Slide Slide;

        public Move(StateMachine m, State parent, PlayerContext ctx) : base(m, parent) {
            this.ctx = ctx;
            Run = new Run(m, this, ctx);
            Walk = new Walk(m, this, ctx);
            MovingCrouch = new MovingCrouch(m, this, ctx);
            Slide = new Slide(m, this, ctx);
        }

        protected override State GetInitialState() => Walk;

        protected override State GetTransition() {
            
            if (!ctx.grounded) {
                return ((PlayerRoot)Parent).Airborne;
            }
            
            return Mathf.Abs(ctx.move.magnitude) <= 0.01f ? ((Grounded)Parent).Idle : null;
        }

        protected override void OnUpdate(float deltaTime)
        {
            //Debug.Log("Walk");
            float rateOfChange = 1f;

            float targetx = ctx.move.x * ctx.currentMoveSpeed;
            float targetz = ctx.move.z * ctx.currentMoveSpeed;

            // Rate of Change
            if(ctx.currentMoveSpeed >= ctx.moveSpeed) rateOfChange = ctx.accel;
            else if(ctx.currentMoveSpeed < ctx.moveSpeed)
            {
                if(ctx.sprinting && ctx.crouching) rateOfChange = ctx.slideDecel;
                else rateOfChange = ctx.decel;
            }

            targetx = Mathf.MoveTowards(ctx.velocity.x, targetx, rateOfChange*deltaTime);
            targetz = Mathf.MoveTowards(ctx.velocity.z, targetz, rateOfChange*deltaTime);


            ctx.velocity.x = targetx;
            ctx.velocity.z = targetz;


            // turn visual player to forward
            ctx.TurnToForward(deltaTime);
        }
    }
}