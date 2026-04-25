using UnityEngine;
using UnityEngine.Events;

namespace GabUnity
{
    public class SimpleRayChecker : MonoBehaviour
    {
        [Header("Ray Settings")]
        [SerializeField] private Vector3 rayDirection = Vector3.forward;
        [SerializeField] private float maxDistance = 10f;
        [SerializeField] private LayerMask detectionLayer = ~0; // Default to 'Everything'

        [Header("Events")]
        [Tooltip("Invoked every frame with the world position of the raycast hit.")]
        public UnityEvent<Vector3, Vector3> OnHitPosition;

        [Tooltip("Invoked only if the object hit has a Rigidbody component.")]
        public UnityEvent<Rigidbody> OnHitRigidbody;

        private void Update()
        {
            PerformRaycast();
        }

        private void PerformRaycast()
        {
            // Convert local direction to world space
            Vector3 worldDirection = transform.TransformDirection(rayDirection.normalized);

            Ray ray = new Ray(transform.position, worldDirection);

            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, detectionLayer))
            {
                // Invoke the position event
                OnHitPosition?.Invoke(transform.position, hit.point);

                // Check for and invoke the Rigidbody event
                if (hit.rigidbody != null)
                {
                    OnHitRigidbody?.Invoke(hit.rigidbody);
                }
            }
            else
            {
                OnHitPosition?.Invoke(transform.position, transform.position + (worldDirection * maxDistance));
            }
        }

        private void OnDrawGizmos()
        {
            // Calculate direction and end point for visualization
            Vector3 worldDirection = transform.TransformDirection(rayDirection.normalized);
            Vector3 endPoint = transform.position + (worldDirection * maxDistance);

            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position, worldDirection * maxDistance);

            // Draw a small wire sphere at the max distance to show the "limit"
            Gizmos.DrawWireSphere(endPoint, 0.1f);
        }
    }
}