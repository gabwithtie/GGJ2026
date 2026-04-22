using UnityEngine;

namespace GabUnity
{
    public class PlayerAnimation : MonoBehaviour
    {
        [Header("Audio")]
        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Header("Procedural Lean Settings")]
        public Transform VisualContainer;
        public float TiltSpeed = 10f;

        [Header("Ground Turn Lean")]
        public float TurnLeanIntensity = 0.5f;
        public float MaxTurnLean = 15f;

        [Header("Landing Roll")]
        public float RollThreshold = -12f;
        private bool _wasGrounded;

        private Animator _animator;
        private PlayerMovement _movement;
        private PlayerCharacterInput _input;

        private int
            _animIDSpeed, _animIDGrounded,
            _animIDJump, _animIDMotionSpeed,
            _animIDRoll;

        private float _lastRotationY;
        private float _leanAmount;

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _movement = transform.parent.GetComponent<PlayerMovement>();
            _input = transform.parent.GetComponent<PlayerCharacterInput>();

            if (VisualContainer == null) VisualContainer = transform;

            AssignAnimationIDs();
            _lastRotationY = transform.parent.eulerAngles.y;
            _wasGrounded = _movement.Grounded;
        }

        private void Update()
        {
            if (_animator == null) return;

            _animator.SetBool(_animIDRoll, false);

            // --- LANDING ROLL LOGIC ---
            if (!_wasGrounded && _movement.Grounded)
            {
                if (_movement.VerticalVelocity < RollThreshold)
                {
                    _animator.SetBool(_animIDRoll, true);
                }
            }
            _wasGrounded = _movement.Grounded;

            // --- STANDARD MOVEMENT LOGIC ---
            _animator.SetFloat(_animIDSpeed, _movement.AnimationBlend);
            _animator.SetFloat(_animIDMotionSpeed, _input.analogMovement ? _input.Move.magnitude : 1f);
            _animator.SetBool(_animIDGrounded, _movement.Grounded);

            if (_movement.Grounded)
            {
                _animator.SetBool(_animIDJump, false);
                if (_input.Jump) _animator.SetBool(_animIDJump, true);
            }

            HandleProceduralLean();
        }

        private void HandleProceduralLean()
        {
            float targetZLean = 0f;

            if (_movement.Grounded)
            {
                float currentRotationY = transform.parent.eulerAngles.y;
                float deltaRotation = Mathf.DeltaAngle(_lastRotationY, currentRotationY);
                if (_movement.AnimationBlend > 0.1f)
                {
                    float rawLean = -deltaRotation * TurnLeanIntensity * _movement.AnimationBlend;
                    targetZLean = Mathf.Clamp(rawLean, -MaxTurnLean, MaxTurnLean);
                }
                _lastRotationY = currentRotationY;
            }

            _leanAmount = Mathf.Lerp(_leanAmount, targetZLean, Time.deltaTime * TiltSpeed);

            // 1. Extract the current world rotation from the matrix
            Quaternion worldRotation = transform.parent.rotation;

            // 2. Define your local lean offset
            Quaternion localLean = Quaternion.Euler(0, 0, _leanAmount);

            // 3. Multiply World * Local to apply the lean relative to the object's current orientation
            Quaternion targetRotation = worldRotation * localLean;

            VisualContainer.rotation = Quaternion.Slerp(
                    VisualContainer.rotation,
                    targetRotation,
                    Time.deltaTime * TiltSpeed
                );
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDRoll = Animator.StringToHash("Roll");
        }

        // --- AUDIO EVENTS (Ensure these match your Animation Clips) ---
        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f && FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.position, FootstepAudioVolume);
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f && LandingAudioClip != null)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.position, FootstepAudioVolume);
            }
        }
    }
}