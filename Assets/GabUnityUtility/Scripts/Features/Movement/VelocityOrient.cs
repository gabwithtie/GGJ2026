using UnityEngine;

namespace GabUnity
{
    public class VelocityOrient : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("How fast it rotates to face the new direction")]
        public float RotationSpeed = 10f;
        [Tooltip("Minimum distance moved to trigger a rotation update (prevents jitter)")]
        public float MinMoveDistance = 0.001f;
        [Tooltip("Flip the direction to face forward along velocity instead of opposite")]
        public bool FaceOppositeVelocity = true;

        private Vector3 _lastPosition;
        private Vector3 _currentVelocity;

        private void Start()
        {
            _lastPosition = transform.position;
        }

        private void LateUpdate()
        {
            // 1. Calculate the movement vector since the last frame
            Vector3 movementDelta = transform.position - _lastPosition;

            // 2. Only update if we've moved enough to establish a clear direction
            if (movementDelta.magnitude > MinMoveDistance)
            {
                // Calculate direction
                Vector3 targetDirection = movementDelta.normalized;

                // If we want to face OPPOSITE the velocity
                if (FaceOppositeVelocity)
                {
                    targetDirection = -targetDirection;
                }

                // 3. Create the target rotation
                // We use Vector3.up as the world-up to keep the object upright
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

                // 4. Smoothly rotate toward the target
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    Time.deltaTime * RotationSpeed
                );
            }

            // 5. Store current position for the next frame
            _lastPosition = transform.position;
        }
    }
}