using UnityEngine;

public class ShowHitNormal : MonoBehaviour
{
    public bool isOn;
    public LineRenderer lineRenderer;
    public LineRenderer dirLineRenderer;
    public LayerMask hitMask;
    public InputReader input;
    public float rayDistance = 100f;
    public float normalLineLength = 1f;
    

    void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        if (dirLineRenderer == null)
        {
            dirLineRenderer = GetComponent<LineRenderer>();
        }
        
        // Ensure the line has two points (start and end)
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 2;
        dirLineRenderer.enabled = false;
        dirLineRenderer.positionCount = 2;
        input.InteractEvent += OnInteract;
    }

    void OnInteract()
    {
        if(isOn){
            // Create a ray from the center of the camera forward (or use any origin/direction)
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayDistance,hitMask))
            {
                Vector3 rayDir = ray.direction;
                Vector3 incomingDir = rayDir.normalized;
                Vector3 surfaceTangent = Vector3.ProjectOnPlane(incomingDir,hit.normal).normalized;
                surfaceTangent.y = 0;
                surfaceTangent = surfaceTangent.normalized;

                // Line Render Stuff For Visual Debugging

                // Enable the line renderer if it was hidden
                lineRenderer.enabled = true;
                dirLineRenderer.enabled = true;

                // used for making it so the line isnt clipping inside of the object
                // used only on dirLine because the other one isnt inside of an object
                Vector3 visualPadding = hit.normal * 0.001f; 

                // Point 0: The exact hit point on the surface
                lineRenderer.SetPosition(0, hit.point);
                dirLineRenderer.SetPosition(0, hit.point+visualPadding);

                // Point 1: Extending outward in the direction of the surface normal
                Vector3 normalEndPoint = hit.point + (hit.normal * normalLineLength);
                Vector3 endDirPoint = hit.point + (surfaceTangent * normalLineLength);

                lineRenderer.SetPosition(1, normalEndPoint);
                dirLineRenderer.SetPosition(1,endDirPoint+visualPadding);
            }
            else
            {
                // Hide the line if nothing is hit
                lineRenderer.enabled = false;
                dirLineRenderer.enabled = false;
            }
        }
    }


}
