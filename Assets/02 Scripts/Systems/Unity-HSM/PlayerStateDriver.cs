using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using System.Collections;
using UnityEditor.Experimental.GraphView;
//using UnityUtils;

namespace HSM {
    public class PlayerStateDriver : MonoBehaviour {
        public PlayerContext ctx = new PlayerContext();
        public InputReader input;
        public GameObject body;
        public Transform groundCheck;
        public float groundRadius = 0.2f;
        public LayerMask groundMask;
        public LayerMask wallMask;
        public bool drawGizmos = true;
        string lastPath;
        float cyoteTimer=0;

        

        CinemachineOrbitalFollow orb;

        CharacterController controller;
        StateMachine machine;
        State root;

        

        // Awake //////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Awake() {
            //rb = gameObject.GetOrAdd<Rigidbody>();
            //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            //ctx.rb = rb;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            

            ctx.body=body;
            controller = body.GetOrAddComponent<CharacterController>();
            ctx.controller = controller;
            
            //ctx.anim = GetComponentInChildren<Animator>();
            //ctx.renderer = GetComponent<Renderer>();

            root = new PlayerRoot(null, ctx);
            var builder = new StateMachineBuilder(root);
            machine = builder.Build();

            // Set Up Events 
            SetUpEvents();
    
            // fallback: create a groundCheck just below the collider's bounds
            if (groundCheck == null) {
                var col = body.GetComponent<Collider>();
                var t = new GameObject("groundCheck").transform;
                t.SetParent(transform, false);
                var y = col ? (-col.bounds.extents.y + 0.01f) : -0.5f;
                t.localPosition = new Vector3(0, y, 0);
                groundCheck = t;
            }

            //ctx.cinCam.ForceCameraPosition()
        }
        void Start()
        {
            orb = ctx.cinCam.GetComponent<CinemachineOrbitalFollow>();    
            orb.HorizontalAxis.Recentering.Enabled = true;
            StartCoroutine(DisableRecentering());
        }

        // Destroy //////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnDestroy()
        {
            EndEvents();
        }

        // Update //////////////////////////////////////////////////////////////////////////////////////////////////////////
        void Update() {
            ctx.externalPush = Vector3.zero;

            ctx.grounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundMask);

        
            CheckForWallRun();


            machine.Tick(Time.deltaTime);

            ctx.currentLeaf = machine.Root.Leaf();
            var path = StatePath(ctx.currentLeaf);
            ctx.debugCurrentLeaf = path;

            if (path != lastPath) {
                //Debug.Log("State"+ path);
                lastPath = path;
            }

            float gravVel = ctx.velocity.y; // maintain gravity
            Vector3 horizontalVel;

            if(!ctx.walled){
                // Move in the direction of the controller.
                horizontalVel = (body.transform.right * ctx.velocity.x) + (body.transform.forward * ctx.velocity.z);
            }
            else
            {
                //horizontalVel = (body.transform.right * ctx.velocity.x) + (body.transform.forward * ctx.velocity.z);
                horizontalVel = ctx.velocity;
            }

            Vector3 tempVel = new Vector3(horizontalVel.x,gravVel,horizontalVel.z);

            controller.Move((tempVel * Time.deltaTime) + ctx.externalPush);

            // Debug
            ctx.currentVelocityMag = (int)(tempVel.magnitude*100)/100;
            ctx.debugCurrentDir = tempVel;
        }
        void LateUpdate()
        {
            UpdateVisualPosition();
        }

        // Misc /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void OnDrawGizmosSelected() {
            if (!drawGizmos || groundCheck == null) return;

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        }

        static string StatePath(State s) {
            return string.Join(" > ", s.PathToRoot().Reverse().Select(n => n.GetType().Name));
        }

        // Weird start up stuff ////////////////////////////////////////////////////////////////////////////////////////////////
        IEnumerator DisableRecentering()
        {
            yield return new WaitForSeconds(1f);
            orb.HorizontalAxis.Recentering.Enabled = false;
        }

        private void UpdateVisualPosition() // keep visuals in line with the body
        {

            ctx.visualBody.transform.position = Vector3.MoveTowards(ctx.visualBody.transform.position,ctx.body.transform.position,ctx.updateTime*Time.deltaTime);
            //ctx.visualBody.transform.position = ctx.body.transform.position;
            ctx.cameraPosition.position = Vector3.MoveTowards(ctx.cameraPosition.position,ctx.body.transform.position,ctx.updateTime*Time.deltaTime);
            //ctx.cameraPosition.position = ctx.body.transform.position;
        }

        private void CheckForWallRun()
        {
            // start by assuming we arnt touching a wall.
            if(ctx.wallLeft) ctx.wallLeft = false;
            if(ctx.wallRight) ctx.wallRight = false;

            if(ctx.grounded) return;
            
            Vector3 currentNormal = Vector3.up;
            Ray tempRay=default;
            RaycastHit tempHit;
            
            

            // if left was hit
            Ray ray = new Ray(ctx.body.transform.position, -ctx.body.transform.right);
            if(Physics.Raycast(ray,out tempHit, ctx.wallRayDistance, wallMask)) // Left
            {
                //Debug.Log("Left Hit");
                ctx.wallLeft = true; 
                //ctx.rayDirection = ray.direction;
                currentNormal = tempHit.normal;
                tempRay = ray;
                ctx.wallHit = tempHit;
            }

            // if right was hit
            ray = new Ray(ctx.body.transform.position, ctx.body.transform.right);
            if(Physics.Raycast(ray,out tempHit, ctx.wallRayDistance, wallMask)) // Right
            {
                //Debug.Log("Right Hit");
                ctx.wallRight = true; 
                //ctx.rayDirection = ray.direction;
                currentNormal = tempHit.normal;
                tempRay = ray;
                ctx.wallHit = tempHit;
            }

            // Check if the current normal is moving;
            if(currentNormal != Vector3.up && tempRay.direction != Vector3.zero)
            {
                bool currentToLast = true;
                float dot = Mathf.Abs(Vector3.Dot(tempRay.direction.normalized, currentNormal));
                ctx.debugDot = dot;
                if(dot < ctx.wallDegreeThreshold)
                {
                    //Debug.Log("doted");
                    if(ctx.walled)ctx.wallHit = ctx.previousHit;
                    else
                    {   // ignore if it is perpindicular to begin with
                        currentToLast = false;
                        ctx.wallRight=false;
                        ctx.wallLeft=false;
                    }
                }
                

                if(currentToLast){  
                    ctx.lastHitNormal = currentNormal;
                }
                ctx.previousHit = ctx.wallHit;
            }

            // Walled Bool
            if(ctx.wallRight && ctx.wallLeft) // temp solution in case something is weird.
            {
                ctx.walled = false;
                cyoteTimer = 0;
            }
            else if(ctx.wallRight || ctx.wallLeft)
            {
                ctx.walled = true;
                cyoteTimer = 0;
            }
            else
            {
                if(ctx.walled && cyoteTimer <= ctx.cyoteTime)
                {
                    cyoteTimer += Time.deltaTime;
                    //Debug.Log("CyoteTime: " + cyoteTimer);
                    ctx.walled = true;
                }
                else{ 
                    ctx.walled = false;
                    cyoteTimer = 0;
                }
            }

        }

        // Gizmos /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            
            if(ctx.currentVelocityMag > .01f) Gizmos.DrawLine(ctx.body.transform.position, ctx.body.transform.position + ctx.debugCurrentDir*2);
        }


        // Events /////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void SetUpEvents()
        {
            input.MoveEvent += HandleMove;
            input.JumpEvent += HandleJump;
            input.JumpCancelledEvent += HandleJumpCancelled;
            input.SprintEvent += SprintEvent;
            input.SprintCancelledEvent += SprintEventCancelled;
            input.CrouchEvent += CrouchEvent;
            input.CrouchCancelledEvent += CrouchEventCancelled;
        }
        private void EndEvents()
        {
            input.MoveEvent -= HandleMove;
            input.JumpEvent -= HandleJump;
            input.JumpCancelledEvent -= HandleJumpCancelled;
            input.SprintEvent -= SprintEvent;
            input.SprintCancelledEvent -= SprintEventCancelled;
            input.CrouchEvent -= CrouchEvent;
            input.CrouchCancelledEvent -= CrouchEventCancelled;
        }
        
        // Handle Events //////////////////////////////////////////////////////////////////////////////////////////////////////
        private void HandleMove(Vector2 moveInput) // Move
        {
            ctx.move.x = moveInput.x;
            ctx.move.z = moveInput.y;

            ctx.move.Normalize();
        }

        private void HandleJump()
        {
            if(!ctx.jumpPressed){
                ctx.jumpPressed = true;
            }
        }
        private void HandleJumpCancelled()
        {
            if(ctx.jumpPressed){
                ctx.jumpPressed = false;
            }
        }
        
        private void SprintEvent()
        {
            if (!ctx.sprinting)
            {
                ctx.sprinting = true;
            }
        }
        private void SprintEventCancelled()
        {
            if (ctx.sprinting)
            {
                ctx.sprinting = false;
            }
        }

        private void CrouchEvent()
        {
            if (!ctx.crouching)
            {
                ctx.crouching = true;
            }
        }
        private void CrouchEventCancelled()
        {
            if (ctx.crouching)
            {
                ctx.crouching = false;
            }
        }
        
    }

    // Player Context //////////////////////////////////////////////////////////////////////////////////////////////////////////
    [Serializable]
    public class PlayerContext {
        [Header("Game Variables")]
        public Vector3 move;
        public Vector3 velocity;

        public float moveSpeed = 6f;
        public float currentMoveSpeed = 0f;

        public float accel = 40f;
        public float decel = 50f;
        public float slideDecel = 10f;

        public float jumpForce = 7f;
        
        public float gravForce = 9.81f;
        public float drag = 1;

        public float cyoteTime = .3f;

        [Header("Wall Stuff")]
        public float wallRayDistance;
        public float wallDegreeThreshold = .1f;
        public float wallSnapLength = .7f;
        public float wallJumpForceMod;
        public float wallJumpAngle = 45f;
        //[HideInInspector] public Vector3 rayDirection;
        public RaycastHit wallHit;
        public RaycastHit previousHit; 
        


        [Header("State Modifiers")]
        public float sprintMod = 2;
        public float crouchMod = .5f;

        [Header("Visual Variables")]
        [Tooltip("Speed the visual gameobject turns")]public float turnSpeed = 5;
        public float updateTime = 1;

        // Bools
        [Header("Bools")]
        public bool jumpPressed;
        public bool grounded;
        public bool walled;
        public bool sprinting;
        public bool crouching;
        public bool wallLeft;
        public bool wallRight;
        
        
        [Header("Refrences")]
        public GameObject body;
        public GameObject visualBody;
        public Transform cameraPosition;
        public CharacterController controller;
        public CinemachineCamera cinCam;
        public Transform cinCamTransform => cinCam.transform;
        public Animator anim;
        //public Rigidbody rb;
        public Renderer renderer;
        [HideInInspector] public Vector3 lastPosition;
        [HideInInspector] public Vector3 lastHitNormal;
        [HideInInspector] public Vector3 externalPush;
        
        [Header("Debug")]
        public float currentVelocityMag;
        public State currentLeaf;
        public string debugCurrentLeaf;
        public Vector3 debugCurrentDir;
        public float debugDot;
        

        public void TurnToForward(float tickTime)
        {
            /////////////////////////// Real Body
            // Get the forward direction of the target transform
            Vector3 targetDir = new Vector3(cinCamTransform.forward.x,0,cinCamTransform.forward.z);
            
            // Create the target rotation looking in that direction
            Quaternion targetRotation = Quaternion.LookRotation(targetDir);
            
            // Smoothly rotate toward the target rotation
            body.transform.rotation = Quaternion.Slerp(body.transform.rotation, targetRotation, turnSpeed * tickTime);

            ///////////////////////// Visual
            /// get the movment direction from the normals of the players velocity
            Vector3 targetVisualDir = velocity.normalized;
            targetVisualDir = (cinCamTransform.forward * targetVisualDir.z) + (cinCamTransform.right * targetVisualDir.x);
            targetVisualDir.y = 0;

            if(targetVisualDir.magnitude > .01f){
                // Create the target rotation looking in that direction
                Quaternion targetVisualRot = Quaternion.LookRotation(targetVisualDir);

                // Smoothly rotate toward the target rotation
                visualBody.transform.rotation = Quaternion.Slerp(visualBody.transform.rotation, targetVisualRot, turnSpeed * tickTime);
            }
        }
    
    }
}