using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using System.Collections;
//using UnityUtils;

namespace HSM {
    public class PlayerStateDriver : MonoBehaviour {
        public PlayerContext ctx = new PlayerContext();
        public InputReader input;
        public GameObject body;
        public Transform groundCheck;
        public float groundRadius = 0.2f;
        public LayerMask groundMask;
        public bool drawGizmos = true;
        string lastPath;

        CinemachineOrbitalFollow orb;

        CharacterController controller;
        Transform cam;
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

            cam = ctx.cinCamTransform;
            
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

            ctx.grounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundMask);

            machine.Tick(Time.deltaTime);

            ctx.currentLeaf = machine.Root.Leaf();
            var path = StatePath(ctx.currentLeaf);
            ctx.debugCurrentLeaf = path;

            if (path != lastPath) {
                //Debug.Log("State"+ path);
                lastPath = path;
            }

            float gravVel = ctx.velocity.y; // maintain gravity
            // Move in the direction of the controller.
            Vector3 horizontalVel = (body.transform.right * ctx.velocity.x) + (body.transform.forward * ctx.velocity.z);

            Vector3 tempVel = new Vector3(horizontalVel.x,gravVel,horizontalVel.z);

            controller.Move(tempVel * Time.deltaTime);
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


        // Events /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SetUpEvents()
        {
            input.MoveEvent += HandleMove;
            input.JumpEvent += HandleJump;
            input.JumpCancelledEvent += HandleJumpCancelled;
        }
        private void EndEvents()
        {
            input.MoveEvent -= HandleMove;
            input.JumpEvent -= HandleJump;
            input.JumpCancelledEvent -= HandleJumpCancelled;
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
        
    }

    // Player Context //////////////////////////////////////////////////////////////////////////////////////////////////////////
    [Serializable]
    public class PlayerContext {
        [Header("Game Variables")]
        public Vector3 move;
        public Vector3 velocity;
        public bool grounded;
        public float moveSpeed = 6f;
        public float accel = 40f;
        public float decel = 50f;
        public float jumpForce = 7f;
        public bool jumpPressed;
        public float gravForce = 9.81f;
        public float drag = 1;

        [Header("Visual Variables")]
        [Tooltip("Speed the visual gameobject turns")]public float turnSpeed = 5;

        [Header("Refrences")]
        public GameObject body;
        public CharacterController controller;
        public CinemachineCamera cinCam;
        public Transform cinCamTransform => cinCam.transform;
        public Animator anim;
        //public Rigidbody rb;
        public Renderer renderer;
        
        [Header("Debug")]
        public State currentLeaf;
        public string debugCurrentLeaf;

        public void TurnToForward(float tickTime)
        {
            // Get the forward direction of the target transform
            Vector3 targetDir = new Vector3(cinCamTransform.forward.x,0,cinCamTransform.forward.z);
            
            // Create the target rotation looking in that direction
            Quaternion targetRotation = Quaternion.LookRotation(targetDir);
            
            // Smoothly rotate toward the target rotation
            body.transform.rotation = Quaternion.Slerp(body.transform.rotation, targetRotation, turnSpeed * tickTime);
        }
    }
}