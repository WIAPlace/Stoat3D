using UnityEngine;
using Unity.Behavior;

public class OnZoneEnterAlertBehaviors : MonoBehaviour
{
    [SerializeField]private BehaviorGraphAgent[] agents;
    [SerializeField] private LayerMask playerMask;

    void OnTriggerEnter(Collider other)
    {
        if ((playerMask.value & (1 << other.gameObject.layer)) != 0)
        {
            foreach(BehaviorGraphAgent agent in agents)
            {
                agent.SetVariableValue("EnteredZone",true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if ((playerMask.value & (1 << other.gameObject.layer)) != 0)
        {
               foreach(BehaviorGraphAgent agent in agents)
            {
                agent.SetVariableValue("EnteredZone",false);
            } 
        }
    }
}
