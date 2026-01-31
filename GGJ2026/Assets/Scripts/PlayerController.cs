using UnityEngine;
using UnityEngine.InputSystem;

namespace GabUnity
{
    [RequireComponent(typeof(Rigidbody), typeof(GroundChecker))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Steering Settings")]
        [SerializeField] private float steeringSpeed = 20.0f;
        [SerializeField] private float horizontalDamping = 10.0f; // Higher damping for snappier kinematic feel
        private float maxHorizontalRange => ((float)LaneManager.MaxLane + 0.5f) * LaneManager.LaneWidth;

        [Header("Obstacle Avoidance")]
        [SerializeField] private LayerMask obstacleLayer;
        [SerializeField] private float sideRayLength = 0.8f;
        [SerializeField] private float rayHeightOffset = 0.5f;

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
        private float currentXVelocity; // Tracked for leaning visuals
        private float targetXPos;       // The "Virtual" X we want to be at
        private GroundChecker isGrounded;

        private float gravity;
        private float initialJumpVelocity;

        private void OnValidate() => CalculateJumpPhysics();

        private void Awake() => isGrounded = GetComponent<GroundChecker>();

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = false; // We use MovePosition on a non-kinematic RB for better collision interaction
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            targetXPos = transform.position.x;
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
            HandleKinematicSteering();
            ApplyCustomGravity();
        }

        private void HandleKinematicSteering()
        {
            // 1. Move the target X position based on input
            float inputX = inputDirection.x;

            // 2. Obstacle Check: If we are trying to move into a wall, don't move the targetX
            if (IsBlockingSteer(inputX))
            {
                inputX = 0;
            }

            targetXPos += inputX * steeringSpeed * Time.fixedDeltaTime;
            targetXPos = Mathf.Clamp(targetXPos, -maxHorizontalRange, maxHorizontalRange);

            // 3. Smoothly move the Rigidbody to the target X
            Vector3 currentPos = rb.position;
            float newX = Mathf.Lerp(currentPos.x, targetXPos, Time.fixedDeltaTime * horizontalDamping);

            // Calculate current velocity for the lean effect
            currentXVelocity = (newX - currentPos.x) / Time.fixedDeltaTime;

            // 4. Perform the actual Rigidbody Move
            rb.MovePosition(new Vector3(newX, currentPos.y, currentPos.z));
        }

        private bool IsBlockingSteer(float inputX)
        {
            if (Mathf.Abs(inputX) < 0.01f) return false;

            Vector3 rayDir = (inputX > 0) ? Vector3.right : Vector3.left;
            Vector3 rayOrigin = transform.position + Vector3.up * rayHeightOffset;

            Debug.DrawRay(rayOrigin, rayDir * sideRayLength, Color.red);
            return Physics.Raycast(rayOrigin, rayDir, sideRayLength, obstacleLayer);
        }

        private void HandleVisualLean()
        {
            if (visuals == null) return;

            // Lean is now based on how fast the MovePosition is actually shifting the player
            float leanFactor = -currentXVelocity / steeringSpeed;
            float targetAngle = Mathf.Clamp(leanFactor * maxLeanAngle, -maxLeanAngle, maxLeanAngle);

            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            visuals.localRotation = Quaternion.Slerp(visuals.localRotation, targetRotation, Time.deltaTime * leanSpeed);
        }

        private void ApplyCustomGravity()
        {
            // We keep Y-axis physics dynamic so the player can fall and jump naturally
            if (isGrounded.Grounded && rb.linearVelocity.y <= 0)
            {
                rb.linearVelocity = new Vector3(0, -0.1f, 0);
                return;
            }

            float multiplier = (rb.linearVelocity.y > 0) ? upwardGravityMultiplier : downwardGravityMultiplier;
            rb.AddForce(Vector3.up * (gravity * multiplier), ForceMode.Acceleration);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            inputDirection = context.ReadValue<Vector2>();
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