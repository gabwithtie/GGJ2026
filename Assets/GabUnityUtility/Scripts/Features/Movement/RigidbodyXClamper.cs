using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodyXClamper : MonoBehaviour
    {
        [Header("X Axis Limits")]
        [SerializeField] private float minX = -5f;
        [SerializeField] private float maxX = 5f;

        [Header("Settings")]
        [Tooltip("If true, velocity will be killed when hitting a limit to prevent 'bouncing' or jitter.")]
        [SerializeField] private bool stopVelocityAtLimit = true;

        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            Vector3 pos = _rb.position;
            Vector3 vel = _rb.linearVelocity;

            // Check Minimum Limit
            if (pos.x < minX)
            {
                pos.x = minX;
                if (stopVelocityAtLimit && vel.x < 0)
                {
                    vel.x = 0;
                }
            }
            // Check Maximum Limit
            else if (pos.x > maxX)
            {
                pos.x = maxX;
                if (stopVelocityAtLimit && vel.x > 0)
                {
                    vel.x = 0;
                }
            }

            // Apply the clamped position and adjusted velocity back to the Rigidbody
            _rb.position = pos;
            if (stopVelocityAtLimit)
            {
                _rb.linearVelocity = vel;
            }
        }

        // Visualize the limits in the Scene View
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector3 center = transform.position;

            // Draw vertical lines representing the boundaries
            Gizmos.DrawLine(new Vector3(minX, center.y - 1, center.z), new Vector3(minX, center.y + 1, center.z));
            Gizmos.DrawLine(new Vector3(maxX, center.y - 1, center.z), new Vector3(maxX, center.y + 1, center.z));

            // Draw a horizontal connector
            Gizmos.DrawLine(new Vector3(minX, center.y, center.z), new Vector3(maxX, center.y, center.z));
        }
    }
}