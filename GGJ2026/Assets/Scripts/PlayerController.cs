using UnityEngine;
using UnityEngine.InputSystem;

namespace GabUnity
{
    [RequireComponent(typeof(Rigidbody), typeof(GroundChecker))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Steering Settings")]
        [SerializeField] private float steeringSpeed = 20.0f;
        [SerializeField] private float horizontalDamping = 5.0f;
        private float maxHorizontalRange => ((float)LaneManager.MaxLane + 0.5f) * LaneManager.LaneWidth;

        [Header("Obstacle Avoidance")]
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private float sideRayLength = 0.8f; // Distance from center to check
        [SerializeField] private float rayHeightOffset = 0.5f; // Cast above ground level

        [Header("Motorcycle Leaning")]
        [SerializeField] private Transform visuals;
        [SerializeField] private float maxLeanAngle = 35.0f;
        [SerializeField] private float leanSpeed = 10.0f;

        [Header("Jump Settings (Quadratic)")]
        [SerializeField] private float jumpHeight = 2.5f;
        [SerializeField] private float timeToJumpApex = 0.4f;
        [Range(1f, 10f)][SerializeField] private float upwardGravityMultiplier = 2f;
        [Range(1f, 10f)][SerializeField] private float downwardGravityMultiplier = 4f;

        private Rigidbody rb;
        private Vector2 inputDirection;
        private float currentHorizontalVelocity;
        private GroundChecker isGrounded;

        private float gravity;
        private float initialJumpVelocity;

        private void OnValidate() => CalculateJumpPhysics();

        private void Awake() => isGrounded = GetComponent<GroundChecker>();

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            CalculateJumpPhysics();
        }

        private void CalculateJumpPhysics()
        {
            gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            initialJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        }

        private void Update() => HandleVisualLean();

        private void FixedUpdate()
        {
            HandleSmoothSteering();
            ApplyCustomGravity();
        }

        private void HandleSmoothSteering()
        {
            float targetVelocityX = inputDirection.x * steeringSpeed;

            // --- Obstacle Avoidance Logic ---
            targetVelocityX = CheckForObstacles(targetVelocityX);

            currentHorizontalVelocity = Mathf.Lerp(currentHorizontalVelocity, targetVelocityX, Time.fixedDeltaTime * horizontalDamping);

            Vector3 nextPos = rb.position + new Vector3(currentHorizontalVelocity * Time.fixedDeltaTime, 0, 0);
            nextPos.x = Mathf.Clamp(nextPos.x, -maxHorizontalRange, maxHorizontalRange);

            rb.MovePosition(nextPos);
        }

        private float CheckForObstacles(float targetVel)
        {
            // If not moving horizontally, no need to check
            if (Mathf.Abs(targetVel) < 0.01f) return targetVel;

            // Determine ray direction based on movement
            Vector3 rayDir = (targetVel > 0) ? Vector3.right : Vector3.left;
            Vector3 rayOrigin = transform.position + Vector3.up * rayHeightOffset;

            // Draw ray for debugging in Scene View
            Debug.DrawRay(rayOrigin, rayDir * sideRayLength, Color.red);

            if (Physics.Raycast(rayOrigin, rayDir, out RaycastHit hit, sideRayLength, obstacleLayer))
            {
                // We hit something! Kill the velocity in that direction
                return 0;
            }

            return targetVel;
        }

        private void HandleVisualLean()
        {
            if (visuals == null) return;
            float leanFactor = -currentHorizontalVelocity / steeringSpeed;
            float targetAngle = leanFactor * maxLeanAngle;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            visuals.localRotation = Quaternion.Slerp(visuals.localRotation, targetRotation, Time.deltaTime * leanSpeed);
        }

        private void ApplyCustomGravity()
        {
            if (isGrounded.Grounded && rb.linearVelocity.y <= 0)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, -0.1f, rb.linearVelocity.z);
                return;
            }

            float multiplier = (rb.linearVelocity.y > 0) ? upwardGravityMultiplier : downwardGravityMultiplier;
            rb.AddForce(Vector3.up * (gravity * multiplier), ForceMode.Acceleration);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            inputDirection = context.ReadValue<Vector2>();
            if (inputDirection.y < -0.1f && !isGrounded.Grounded)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, -initialJumpVelocity * 1.5f, rb.linearVelocity.z);
            }
        }

        public void ForceJump(float forcemult) => rb.linearVelocity = new Vector3(rb.linearVelocity.x, initialJumpVelocity * forcemult, rb.linearVelocity.z);

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed && isGrounded.Grounded)
            {
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, initialJumpVelocity, rb.linearVelocity.z);
            }
        }
    }
}