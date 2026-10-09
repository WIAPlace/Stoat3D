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

        // Get Transition Occurs from top of the states to the bottom
        protected override State GetTransition() {
            if (ctx.jumpPressed && ctx.simpleJump) { // Jump
                //ctx.jumpPressed = false;
                ctx.velocity.y = ctx.jumpForce;
                ctx.jumpPressed = false;

                return ((PlayerRoot)Parent).Airborne;
            }
            return ctx.grounded ? null : ((PlayerRoot)Parent).Airborne;
        }
        protected override void OnEnter()
        {
            ctx.simpleJump = true; // make sure this is true by default in case it some how got left off;
        }
        protected override void OnExit()
        {
            ctx.activePlatform = null;
            ctx.tempWorldMovment = ctx.worldMovement;
            ctx.worldMovement = Vector3.zero;

            ctx.FullHight();
        }

        // On Update Occurs from the bottom of the tree to the top
        protected override void OnUpdate(float deltaTime)
        {
            if(ctx.grounded && ctx.velocity.y < 0) // if on ground reset gravity
            {
                ctx.velocity.y = 0;
            }
            //Debug.Log("Move");
            ctx.velocity.y += -ctx.gravForce * deltaTime; // apply gravity

            HandleMovingPlatform();
        }

        void HandleMovingPlatform()
        {
            if(ctx.groundHit.collider != null && ctx.groundHit.collider.CompareTag(ctx.moveingPlatTag))
            {
                Transform currentPlatform = ctx.groundHit.transform;
                //Debug.Log("first if passed");
                if (currentPlatform == ctx.activePlatform)
                {
                    // Calculate how much the platform moved since the last frame
                    ctx.worldMovement = currentPlatform.position - ctx.platformLastPosition;
                    //Debug.Log("On Existing Plat");
                }
                else
                {
                    // Player just stepped onto a new platform
                    ctx.activePlatform = currentPlatform;
                    ctx.worldMovement = Vector3.zero;
                    //Debug.Log("On new Plat");
                }

                // Update the position record for the next frame
                ctx.platformLastPosition = currentPlatform.position;
                return;
            }
            // If we are not on a moving platform, clear variables
            ctx.activePlatform = null;
            ctx.worldMovement = Vector3.zero;
            //Debug.Log("Failed");
        }
    }
}