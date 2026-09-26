using System;
using HSM;
using UnityEngine;

public class InformParentOfTrigger : MonoBehaviour
{
    [SerializeField] private PlayerStateDriver psd;

    void OnTriggerEnter(Collider other)
    {
        psd.OnTriggerZoneEnter(other);
    }
}
