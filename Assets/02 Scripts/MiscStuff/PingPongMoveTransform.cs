using UnityEngine;

public class PingPongMoveTransform : MonoBehaviour
{
    public Vector3 vectorMulti;
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
        t+=Time.deltaTime*(moveSpeed/length);
        float linearT = Mathf.PingPong(t, 1f); 

        // 2. Smooth the value using standard cubic smoothing (S-curve)
        float smoothT = Mathf.SmoothStep(0f, 1f, linearT);

        // 3. Scale the smoothed value by your desired length
        float mod = smoothT * length;
        
        transform.position = new Vector3(startPosition.x + (vectorMulti.x*mod),
                                         startPosition.y + (vectorMulti.y*mod),
                                         startPosition.z + (vectorMulti.z*mod)); 
    }
}
