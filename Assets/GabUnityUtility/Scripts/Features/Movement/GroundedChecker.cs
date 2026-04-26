using UnityEngine;
using UnityEngine.Events;

namespace GabUnity
{
    [RequireComponent(typeof(Rigidbody))]
    public class GroundedChecker : MonoBehaviour
    {
        [Header("Detection Settings")]
        [Tooltip("How far below the object to check for ground.")]
        [SerializeField] private float groundCheckDistance = 0.1f;
        [Tooltip("The radius of the detection sphere (should match the width of your object).")]
        [SerializeField] private float checkRadius = 0.45f;
        [SerializeField] private LayerMask groundLayer = ~0;

        [Header("Events")]
        [Tooltip("Fires once the moment the object touches the ground.")]
        public UnityEvent OnNewlyGrounded;

        private bool _isGrounded;
        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            CheckGroundStatus();
        }

        private void CheckGroundStatus()
        {
            // Calculate the bottom of the object based on current position
            Vector3 origin = transform.position + Vector3.up * checkRadius;

            // Perform a SphereCast downwards
            bool hit = Physics.SphereCast(origin, checkRadius, Vector3.down, out _, groundCheckDistance, groundLayer);

            // Logic for "Newly Grounded"
            if (hit && !_isGrounded)
            {
                OnNewlyGrounded?.Invoke();
            }

            _isGrounded = hit;
        }

        private void OnDrawGizmosSelected()
        {
            // Visualize the ground check area in the editor
            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Vector3 origin = transform.position + Vector3.up * checkRadius;
            Vector3 target = origin + Vector3.down * groundCheckDistance;

            Gizmos.DrawWireSphere(origin, checkRadius);
            Gizmos.DrawWireSphere(target, checkRadius);
        }
    }
}