using UnityEngine;
using UnityEngine.InputSystem;

namespace GabUnity
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Steering Settings")]
        [SerializeField] private float steeringSpeed = 20.0f;
        [SerializeField] private float horizontalDamping = 5.0f;
        private float maxHorizontalRange => ((float)LaneManager.MaxLane + 0.5f) * LaneManager.LaneWidth;

        [Header("Motorcycle Leaning")]
        [SerializeField] private Transform visuals; // Assign your mesh child object here
        [SerializeField] private float maxLeanAngle = 35.0f;
        [SerializeField] private float leanSpeed = 10.0f;

        [Header("Jump Settings (Quadratic)")]
        [SerializeField] private float jumpHeight = 2.5f;
        [SerializeField] private float timeToJumpApex = 0.4f;
        [Range(1f, 10f)][SerializeField] private float upwardGravityMultiplier = 2f;
        [Range(1f, 10f)][SerializeField] private float downwardGravityMultiplier = 4f;

        [Header("Detection")]
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float groundCheckRadius = 0.25f;

        private Rigidbody rb;
        private Vector2 inputDirection;
        private float currentHorizontalVelocity;
        private float currentLeanVelocity;
        [SerializeField] private bool isGrounded;

        private float gravity;
        private float initialJumpVelocity;

        private void OnValidate() => CalculateJumpPhysics();

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate; // Important for smooth visuals

            CalculateJumpPhysics();
        }

        private void CalculateJumpPhysics()
        {
            gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            initialJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        }

        private void Update()
        {
            HandleVisualLean();
        }

        private void FixedUpdate()
        {
            HandleSmoothSteering();
            ApplyCustomGravity();
        }

        private void OnCollisionStay(Collision collision)
        {
            isGrounded = true;
        }

        private void OnCollisionExit(Collision collision)
        {
            isGrounded = false;
        }

        private void HandleSmoothSteering()
        {
            // 1. Calculate target velocity based on input
            float targetVelocityX = inputDirection.x * steeringSpeed;

            // 2. Smoothly move current velocity toward target
            currentHorizontalVelocity = Mathf.Lerp(currentHorizontalVelocity, targetVelocityX, Time.fixedDeltaTime * horizontalDamping);

            // 3. Apply velocity while clamping position to the road boundaries
            Vector3 nextPos = rb.position + new Vector3(currentHorizontalVelocity * Time.fixedDeltaTime, 0, 0);
            nextPos.x = Mathf.Clamp(nextPos.x, -maxHorizontalRange, maxHorizontalRange);

            rb.MovePosition(nextPos);
        }

        private void HandleVisualLean()
        {
            if (visuals == null) return;

            // Calculate lean amount based on current horizontal movement (-1 to 1)
            float leanFactor = -currentHorizontalVelocity / steeringSpeed; // Negative because tilting left leans left
            float targetAngle = leanFactor * maxLeanAngle;

            // Smoothly rotate the visual child object
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            visuals.localRotation = Quaternion.Slerp(visuals.localRotation, targetRotation, Time.deltaTime * leanSpeed);
        }

        private void ApplyCustomGravity()
        {
            if (isGrounded && rb.linearVelocity.y <= 0)
            {
                rb.linearVelocity = new Vector3(0, -0.1f, 0); // Stick to ground
                return;
            }

            float multiplier = (rb.linearVelocity.y > 0) ? upwardGravityMultiplier : downwardGravityMultiplier;
            rb.AddForce(Vector3.up * (gravity * multiplier), ForceMode.Acceleration);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            inputDirection = context.ReadValue<Vector2>();

            // Slam mechanic: Downward swipe/key
            if (inputDirection.y < -0.1f && !isGrounded)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, -initialJumpVelocity * 1.5f, rb.linearVelocity.z);
            }
        }

        public void ForceJump(float forcemult)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, initialJumpVelocity * forcemult, rb.linearVelocity.z);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed && isGrounded)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, initialJumpVelocity, rb.linearVelocity.z);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.1f, groundCheckRadius);
        }
    }
}