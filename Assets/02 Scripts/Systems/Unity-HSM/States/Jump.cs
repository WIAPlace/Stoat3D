using UnityEngine;

namespace HSM{
    public class Jump : State
    {
        readonly PlayerContext ctx;

        public Jump(StateMachine machine, State parent, PlayerContext ctx) : base(machine, parent)
        {
            this.ctx = ctx;
        }

        protected override State GetTransition(){
            if (!ctx.jumpPressed)
            {
                return ((Airborne)Parent).Fall;
            }
            else return null;
        }

        protected override void OnEnter()
        {
            
        }
    }
}
