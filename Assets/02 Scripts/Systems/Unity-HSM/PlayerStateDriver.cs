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
            input.MoveEvent += HandleMove;


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
            input.MoveEvent -= HandleMove;
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
            Vector3 horizontalVel = (controller.transform.right * ctx.velocity.x) + (controller.transform.forward * ctx.velocity.z);

            ctx.velocity = new Vector3(horizontalVel.x,gravVel,horizontalVel.z);

            controller.Move(ctx.velocity * Time.deltaTime);
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

        // Handle Events //////////////////////////////////////////////////////////////////////////////////////////////////////
        private void HandleMove(Vector2 moveInput) // Move
        {
            ctx.move.x = moveInput.x;
            ctx.move.z = moveInput.y;

            ctx.move.Normalize();
        }
        IEnumerator DisableRecentering()
        {
            yield return new WaitForSeconds(1f);
            orb.HorizontalAxis.Recentering.Enabled = false;
        }
    }

    // Player Context //////////////////////////////////////////////////////////////////////////////////////////////////////////
    [Serializable]
    public class PlayerContext {
        public Vector3 move;
        public Vector3 velocity;
        public bool grounded;
        public float moveSpeed = 6f;
        public float accel = 40f;
        public float jumpSpeed = 7f;
        public bool jumpPressed;
        public float gravForce = 9.81f;
        //public float gravMulti = 2.0f;

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
    }
}