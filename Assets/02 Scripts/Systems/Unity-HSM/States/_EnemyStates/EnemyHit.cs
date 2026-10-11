using System.Threading;
using HSM;
using UnityEngine;

public class EnemyHit :State
{
    readonly EnemyContext ctx;
    private bool running;


    public EnemyHit(StateMachine m,State parent, EnemyContext ctx) : base(m, parent) 
    {
        this.ctx = ctx;
    }

    protected override State GetTransition()
    {
        
        if (!running)
        {
            if(ctx.inZone)return ((EnemyRoot)Parent).EnemyInZone;
            else return ((EnemyRoot)Parent).EnemyWander;
        }
        if(ctx.isHit) return ((EnemyRoot)Parent).EnemyHit;
        
        return base.GetTransition();
    }

    protected override void OnEnter()
    {
        ctx._cts?.Cancel();
        //ctx._cts?.Dispose();
        ctx.Agent.enabled = false;
        ctx.rb.useGravity = true;
        ctx.rb.isKinematic = false;
        running = true;
        ctx.isHit = false;
        
        ApplyKnockbackForce();

    }
    protected override void OnExit()
    {
        running = false;
        ctx._cts?.Cancel();
    }
    protected override void OnUpdate(float deltaTime)
    {
        

    }

    private async void ApplyKnockbackForce()
    {
        // Cancel any previous instance running on this context before starting a new one
        if (ctx._cts != null)
        {
            ctx._cts.Cancel();
            ctx._cts.Dispose();
        }
        ctx._cts = new CancellationTokenSource();

        try
        {
            ctx.shouldApplyForce = true;
            await Awaitable.FixedUpdateAsync(ctx._cts.Token);
            
            float knockbackTime = Time.time;
            //Debug.Log("Before: " + knockbackTime);
            
            // This will throw OperationCanceledException instantly if Cancel() is called while waiting
            await EnemyContext.WaitUntilAsync(
                () => ctx.rb.linearVelocity.magnitude < ctx.StillThreshold || (Time.time > knockbackTime + ctx.MaxKnockbackTime && ctx.IsGrounded()), 
                ctx._cts.Token
            );
            
            //Debug.Log("After: " + Time.time);
            await Awaitable.WaitForSecondsAsync(0.25f, ctx._cts.Token);

            // This section only runs if NO cancellation happened
            ResetPhysicsAndAgent();
            running = false;
        }
        catch (System.OperationCanceledException)
        {
            //Debug.Log("The ApplyKnockbackForce operation was cancelled.");
            // CRITICAL: If cancelled mid-wait, your enemy might be left floating/moving.
            if(!ctx.isHit)ResetPhysicsAndAgent();
            running = false; 
        }
        finally
        {
            // Clean up the token source safely
            ctx._cts?.Dispose();
            ctx._cts = null;
        }

    }
    // Helper method to avoid repeating code
    void ResetPhysicsAndAgent()
    {
        if (ctx.rb != null)
        {
            ctx.rb.linearVelocity = Vector3.zero;
            ctx.rb.angularVelocity = Vector3.zero;
            ctx.rb.useGravity = false;
            ctx.rb.isKinematic = true;
        }
        if (ctx.Agent != null)
        {
            ctx.Agent.Warp(ctx.Agent.gameObject.transform.position);
            ctx.Agent.enabled = true;
        }
    }
}
