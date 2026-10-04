using UnityEngine;
using Unity.Behavior;

public class OnZoneEnterAlertEnemy : MonoBehaviour
{
    [SerializeField]private EnemyStateDriver[] agents;
    [SerializeField] private LayerMask playerMask;

    void OnTriggerEnter(Collider other)
    {
        if ((playerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            foreach(EnemyStateDriver agent in agents)
            {
                agent.OnZoneTriggered(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if ((playerMask.value & (1 << other.gameObject.layer)) != 0)
        {
               foreach(EnemyStateDriver agent in agents)
            {
                agent.OnZoneTriggered(false);
            } 
        }
    }
}
