using UnityEngine;
using HSM;

public class BasicAttack : State
{
    readonly PlayerContext ctx;
    
    float timer = 0;
    bool attacked;

    public BasicAttack(StateMachine m, State parent, PlayerContext ctx) : base(m, parent) {
        this.ctx = ctx;
    }

    protected override State GetTransition()
    {
        if(timer >= ctx.coolDown)
        {
            if(ctx.grounded) return ((PlayerRoot)Parent.Parent).Grounded;
            else return ((PlayerRoot)Parent.Parent).Airborne;
        }
        else return null;
    }

    protected override void OnEnter()
    {
        ctx.velocity = Vector3.zero;
        timer = 0;
        attacked = false;

    }

    protected override void OnExit()
    {
        base.OnExit();
    }

    protected override void OnUpdate(float deltaTime)
    {
        timer += deltaTime;
        if(!attacked && timer >= ctx.attackTime)
        {
            attacked = true;
            
            Collider[] hitTargets = Physics.OverlapSphere(ctx.attackPoint.position,ctx.atkRange,ctx.attackLayer);

            foreach(Collider target in hitTargets)
            {
                if(target.gameObject.TryGetComponent<IHitAble>(out IHitAble hitAble)) 
                    hitAble.OnHit(ctx.visualBody.transform.position, ctx.atkForce,ctx.vertAtkForce,ctx.atkdamage);
                
            }
        }       
    }
}
