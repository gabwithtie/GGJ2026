using UnityEngine;
using UnityEngine.InputSystem;

namespace GabUnity
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        private float laneWidth => LaneManager.LaneWidth;

        [Header("Lane Settings")]
        [SerializeField] private float laneSwitchSpeed = 15.0f;

        [Header("Jump Settings (Quadratic)")]
        [SerializeField] private float jumpHeight = 2.5f;
        [SerializeField] private float timeToJumpApex = 0.4f;
        [Range(1f, 10f)][SerializeField] private float upwardGravityMultiplier = 2f;
        [Range(1f, 10f)][SerializeField] private float downwardGravityMultiplier = 4f;

        [Header("Detection")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundDistance = 0.2f;
        [SerializeField] private LayerMask groundMask;

        private Rigidbody rb;
        private int currentLane = 0;
        private float targetXPosition;
        [SerializeField] private bool isGrounded;

        // Calculated variables
        private float gravity;
        private float initialJumpVelocity;

        private void OnValidate()
        {
            // This allows you to tweak values in the inspector and see results immediately
            CalculateJumpPhysics();
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            
            targetXPosition = transform.position.x;
            CalculateJumpPhysics();
        }

        private void CalculateJumpPhysics()
        {
            // Formula derived from: h = (v^2) / (2g)
            // gravity = -(2 * height) / (time to apex^2)
            gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            initialJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        }

        private void Update()
        {
            HandleLaneMovement();
        }

        private void OnCollisionStay(Collision collision)
        {
            isGrounded = true;
        }

        private void OnCollisionExit(Collision collision)
        {
            isGrounded = false;
        }

        private void FixedUpdate()
        {
            ApplyCustomGravity();
        }

        private void HandleLaneMovement()
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Lerp(pos.x, targetXPosition, Time.deltaTime * laneSwitchSpeed);
            transform.position = pos;
        }

        private void ApplyCustomGravity()
        {
            if (isGrounded && rb.linearVelocity.y < 0)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, -0.1f, rb.linearVelocity.z);
                return;
            }

            // Quadratic feel: Fall faster than you rise
            float multiplier = (rb.linearVelocity.y > 0) ? upwardGravityMultiplier : downwardGravityMultiplier;
            rb.AddForce(Vector3.up * (gravity * multiplier), ForceMode.Acceleration);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            Vector2 input = context.ReadValue<Vector2>();

            // X-Axis: Lane Swapping
            if (input.x < -0.1f && currentLane > LaneManager.MinLane) currentLane--;
            else if (input.x > 0.1f && currentLane < LaneManager.MaxLane) currentLane++;

            targetXPosition = currentLane * laneWidth;

            // Y-Axis: Instant Fall (Subway Surfers "Slam" mechanic)
            if (input.y < -0.1f && !isGrounded)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, -initialJumpVelocity * 1.5f, rb.linearVelocity.z);
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed && isGrounded)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, initialJumpVelocity, rb.linearVelocity.z);
            }
        }
    }
}