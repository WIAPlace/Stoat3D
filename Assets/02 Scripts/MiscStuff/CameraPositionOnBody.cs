using UnityEngine;

public class CameraPositionOnBody : MonoBehaviour
{
    [SerializeField] private Transform body;

    void Update()
    {
        transform.position = body.position;
    }
}
