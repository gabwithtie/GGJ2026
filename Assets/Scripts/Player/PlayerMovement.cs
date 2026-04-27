using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GabUnity
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Base Movement")]
        [SerializeField] private float moveSpeed = 6.0f;
        public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
        [SerializeField] private float SprintMult = 2.0f;
        [SerializeField] private float SpeedChangeRate = 10.0f;
        [SerializeField] private float RotationSmoothTime = 0.12f;

        [Header("Ground Detection")]
        [SerializeField] private LayerMask GroundLayers;
        [SerializeField] private Transform GroundedReference;
        [SerializeField] private float GroundedRadius = 0.28f;
        [SerializeField] private float GroundedOffset = -0.14f;
        [SerializeField] private float GroundedOffsetMax = 0f;

        [Header("Jump & Physics")]
        [SerializeField] private float JumpPower = 10f;
        [SerializeField] private float GravityForce = -18.0f;
        [SerializeField] private int MaxJumps = 2;

        [SerializeField] private UnityEvent OnJump;
        [SerializeField] private UnityEvent OnGround;
        [SerializeField] private UnityEvent OnMoveGround;
        [SerializeField] private UnityEvent OnMoveStopGround;
        [SerializeField] private UnityEvent OffGround;

        [Header("Movement Locking (For Rolling/Actions)")]
        [SerializeField] private Animator PlayerAnimator;
        [SerializeField] private string LockedAnimationTag;

        // Output Properties
        public float VerticalVelocity => _rb != null ? _rb.linearVelocity.y : 0f;
        public bool Grounded => _isGrounded;
        public float AnimationBlend { get; private set; }

        private Rigidbody _rb;
        private PlayerCharacterInput _input;
        private bool _isGrounded;
        private int _jumpCount;
        private float _timeSinceJump;
        private float _rotationVelocity;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.useGravity = false;
            _rb.freezeRotation = true;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
            _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            _input = GetComponent<PlayerCharacterInput>();
            if (GroundedReference == null) GroundedReference = transform;
        }

        private void Update()
        {
            _timeSinceJump += Time.deltaTime;

            if (IsMovementLocked()) return;

            HandleRotation();

            if (_input.Jump)
            {
                PerformJump();
            }
        }

        private void FixedUpdate()
        {
            CheckSurfaces();
            ApplyMovement();
            ApplyGravity();
        }

        private void CheckSurfaces()
        {
            // Setup Ground Sphere Position
            float yg = Mathf.Min(GroundedReference.position.y + GroundedOffset, transform.position.y + GroundedOffsetMax);
            Vector3 groundSpherePos = new Vector3(transform.position.x, yg, transform.position.z);

            // Overlap detection
            Collider[] floorColliders = Physics.OverlapSphere(groundSpherePos, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
            bool foundGround = floorColliders.Length > 0;

            UpdateGroundingState(foundGround);
        }

        private void UpdateGroundingState(bool grounded)
        {
            if (grounded && !_isGrounded) OnGround.Invoke();
            if (!grounded && _isGrounded) 
                OffGround.Invoke();


            _isGrounded = grounded;
            if (grounded && _timeSinceJump > 0.2f) _jumpCount = 0;
        }

        private void ApplyMovement()
        {
            if (IsMovementLocked())
            {
                // Use force to slow down horizontal movement, preserving vertical velocity
                Vector3 currentHorizontalVelLock = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
                Vector3 stopForce = -currentHorizontalVelLock * SpeedChangeRate;
                _rb.AddForce(stopForce, ForceMode.Acceleration);
                AnimationBlend = Mathf.Lerp(AnimationBlend, 0, Time.fixedDeltaTime * SpeedChangeRate);
                return;
            }

            float targetSpeed = _input.Sprint ? moveSpeed * SprintMult : moveSpeed;
            if (_input.Move == Vector2.zero)
            {
                targetSpeed = 0f;

                if (Grounded)
                    OnMoveStopGround.Invoke();
            }
            else
            {
                if (Grounded)
                    OnMoveGround.Invoke();
            }

            AnimationBlend = Mathf.Lerp(AnimationBlend, targetSpeed, Time.fixedDeltaTime * SpeedChangeRate);
            Vector3 finalMoveDir = GetCameraRelativeInput();

            // Calculate horizontal velocity change
            Vector3 targetVelocity = finalMoveDir * targetSpeed;
            Vector3 currentHorizontalVel = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
            Vector3 velocityChange = targetVelocity - currentHorizontalVel;

            _rb.AddForce(velocityChange * SpeedChangeRate, ForceMode.Acceleration);
        }

        private void ApplyGravity()
        {
            _rb.AddForce(Vector3.up * GravityForce, ForceMode.Acceleration);
        }

        private Vector3 GetCameraRelativeInput()
        {
            if (MainCamera.Instance == null) return Vector3.zero;

            Transform camTransform = MainCamera.Instance.gameObject.transform;
            Vector3 camForward = camTransform.forward;
            Vector3 camRight = camTransform.right;

            camForward.y = 0;
            camRight.y = 0;

            camForward.Normalize();
            camRight.Normalize();

            return (camForward * _input.Move.y + camRight * _input.Move.x).normalized;
        }

        private void HandleRotation()
        {
            if (_input.Move == Vector2.zero) return;

            Vector3 moveDir = GetCameraRelativeInput();

            if (moveDir != Vector3.zero)
            {
                float targetRotationY = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotationY, ref _rotationVelocity, RotationSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }

        private void PerformJump()
        {
            if (!_isGrounded && _jumpCount >= MaxJumps) return;

            float jumpImpulse = JumpPower;
            jumpImpulse -= _rb.linearVelocity.y; // Add extra "kick" if falling to ensure consistent height

            Vector3 jumpDir = Vector3.up * jumpImpulse;
            _rb.AddForce(jumpDir, ForceMode.VelocityChange);

            _jumpCount++;
            _timeSinceJump = 0;
            UpdateGroundingState(false);
            OnJump.Invoke();
        }

        private bool IsMovementLocked()
        {
            if (PlayerAnimator == null) return false;
            for (int i = 0; i < PlayerAnimator.layerCount; i++)
            {
                if (PlayerAnimator.GetCurrentAnimatorStateInfo(i).IsTag(LockedAnimationTag)) return true;
            }
            return false;
        }

        private void OnDrawGizmos()
        {
            if (GroundedReference != null)
            {
                float yg = Mathf.Min(GroundedReference.position.y + GroundedOffset, transform.position.y + GroundedOffsetMax);
                Vector3 groundSpherePos = new Vector3(transform.position.x, yg, transform.position.z);
                Gizmos.color = _isGrounded ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);
                Gizmos.DrawSphere(groundSpherePos, GroundedRadius);
            }
        }
    }
}