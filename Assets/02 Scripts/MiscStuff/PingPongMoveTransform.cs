using UnityEngine;

public class PingPongMoveTransform : MonoBehaviour
{
    public float moveSpeed;
    public float length;
    private Vector3 startPosition;
    float t;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        t+=Time.deltaTime*moveSpeed;
        float mod = Mathf.PingPong(t,length);

        
        transform.position = new Vector3(startPosition.x,startPosition.y,startPosition.z+mod); 
    }
}
