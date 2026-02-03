using UnityEngine;
using UnityEngine.InputSystem;

namespace GabUnity
{
    [RequireComponent(typeof(Rigidbody), typeof(GroundChecker), typeof(HealthObject))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Forward Movement (Physics)")]
        [SerializeField] private float baseSpeed = 15.0f;
        [SerializeField] private float sprintMultiplier = 2.0f;
        [SerializeField] private float accelerationForce = 50.0f; // How much force to apply to reach target speed
        private bool isSprinting = false;

        [Header("Steering (Physics)")]
        [SerializeField] private float steeringForce = 40.0f;
        [SerializeField] private float horizontalDrag = 5.0f; // Helps stop sliding when input is released
        private float maxHorizontalRange => ((float)LaneManager.MaxLane + 0.5f) * LaneManager.LaneWidth;

        [Header("Motorcycle Visuals")]
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
        private GroundChecker isGrounded;
        private HealthObject healthobject;

        private float gravity;
        private float initialJumpVelocity;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            healthobject = GetComponent<HealthObject>();
            isGrounded = GetComponent<GroundChecker>();
        }

        private void Start()
        {
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
            healthobject.Heal(3.0f * Time.deltaTime);
        }

        private void FixedUpdate()
        {
            if (!healthobject.Alive) return;

            ApplyForwardForce();
            ApplySteeringForce();
            ApplyCustomGravity();
        }

        private void ApplyForwardForce()
        {
            float targetSpeed = isSprinting ? baseSpeed * sprintMultiplier : baseSpeed;

            // Calculate velocity difference
            float currentZVel = rb.linearVelocity.z;
            float velDiff = targetSpeed - currentZVel;

            // Apply force proportional to the difference to reach target speed
            float force = velDiff * accelerationForce;
            rb.AddForce(Vector3.forward * force, ForceMode.Acceleration);
        }

        private void ApplySteeringForce()
        {
            // 1. Calculate Target Horizontal Velocity
            float targetXVel = inputDirection.x * steeringForce;

            // 2. Prevent moving out of lanes via force suppression
            if ((transform.position.x > maxHorizontalRange && targetXVel > 0) ||
                (transform.position.x < -maxHorizontalRange && targetXVel < 0))
            {
                targetXVel = 0;
            }

            // 3. Apply Horizontal Force
            float velDiff = targetXVel - rb.linearVelocity.x;
            rb.AddForce(Vector3.right * velDiff, ForceMode.VelocityChange);
        }

        private void HandleVisualLean()
        {
            if (visuals == null) return;
            // Lean is based on current X velocity relative to steering potential
            float leanFactor = -rb.linearVelocity.x / steeringForce;
            float targetAngle = Mathf.Clamp(leanFactor * maxLeanAngle, -maxLeanAngle, maxLeanAngle);

            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            visuals.localRotation = Quaternion.Slerp(visuals.localRotation, targetRotation, Time.deltaTime * leanSpeed);
        }

        private void ApplyCustomGravity()
        {
            if (isGrounded.Grounded && rb.linearVelocity.y <= 0)
            {
                // Snap to ground slightly to prevent "bouncing" while driving
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, -0.5f, rb.linearVelocity.z);
                return;
            }

            float multiplier = (rb.linearVelocity.y > 0) ? upwardGravityMultiplier : downwardGravityMultiplier;
            rb.AddForce(Vector3.up * (gravity * multiplier), ForceMode.Acceleration);
        }

        // --- Input Callbacks ---
        public void OnMove(InputAction.CallbackContext context) => inputDirection = context.ReadValue<Vector2>();
        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.performed) isSprinting = true;
            if (context.canceled) isSprinting = false;
        }
        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed && isGrounded.Grounded)
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, initialJumpVelocity, rb.linearVelocity.z);
        }
    }
}