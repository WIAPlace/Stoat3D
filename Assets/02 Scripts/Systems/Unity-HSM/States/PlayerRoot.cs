using UnityEngine;
using HSM;
namespace HSM {
    public class PlayerRoot : State {
        public readonly Grounded Grounded;
        public readonly Airborne Airborne;
        public readonly Walled Walled;
        public readonly LedgeGrab LedgeGrab;
        readonly PlayerContext ctx;

        public PlayerRoot(StateMachine m, PlayerContext ctx) : base(m, null) {
            this.ctx = ctx;
            Grounded = new Grounded(m, this, ctx);
            Airborne = new Airborne(m, this, ctx);
            Walled = new Walled(m, this, ctx);
            LedgeGrab = new LedgeGrab(m, this, ctx);
        }
        
        protected override State GetInitialState() => Grounded;
        //protected override State GetTransition() => ctx.grounded ? null : Airborne;
        /*
        protected override State GetTransition()
        {
            if(!ctx.grounded && ctx.currentLeaf != Airborne.Leaf())
            {
                return Airborne;
            }
            else if(ctx.grounded && ctx.currentLeaf != Grounded.Leaf())
            {
                return Grounded;
            }
            else return null;
        }
        */
    }
}
