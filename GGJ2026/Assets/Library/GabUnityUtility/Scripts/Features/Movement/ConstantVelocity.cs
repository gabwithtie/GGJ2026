using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(Rigidbody))]
    public class ConstantVelocity : MonoBehaviour
    {
        [SerializeField] private Vector3 constant_vel;
        [SerializeField] private float acceleration = 10f;
        [SerializeField] private bool ignore_y;

        private Rigidbody m_rigidbody;

        private void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            // 1. Calculate the velocity change required to reach the target
            Vector3 velocityError = constant_vel - m_rigidbody.linearVelocity;

            // 2. If the error is negligible, don't apply force
            if (velocityError.sqrMagnitude < 0.001f) return;

            // 3. Calculate the force needed to bridge that gap in one physics step
            // Force = Mass * (DeltaVelocity / DeltaTime)
            Vector3 forceNeeded = m_rigidbody.mass * (velocityError / Time.fixedDeltaTime);

            // 4. Clamp the force magnitude based on the maximum allowed acceleration
            // MaxForce = Mass * Acceleration
            float maxForce = m_rigidbody.mass * acceleration;
            Vector3 clampedForce = Vector3.ClampMagnitude(forceNeeded, maxForce);

            if (ignore_y)
                clampedForce.y = 0;

            // 5. Apply the force
            m_rigidbody.AddForce(clampedForce, ForceMode.Force);
        }
    }
}