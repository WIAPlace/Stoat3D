using UnityEngine;

public interface IHitAble 
{
    public void OnHit(Vector3 hitFrom, float force, float verticalForce, float damage);
}
