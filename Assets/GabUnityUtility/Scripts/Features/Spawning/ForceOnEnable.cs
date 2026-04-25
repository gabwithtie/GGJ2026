using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(Rigidbody))]
    public class ForceOnEnable : MonoBehaviour
    {
        [Header("Explosion Settings")]
        [SerializeField] private Vector3 forceDirection = Vector3.back;
        [SerializeField] private float explosionForce = 5f;
        [SerializeField] private float forceRandomness = 2f;

        [Header("Torque Settings")]
        [SerializeField] private float maxTorque = 10f;

        private Rigidbody _rb;

        private void OnEnable()
        {
            _rb = GetComponent<Rigidbody>();

            ApplyInitialForces();
        }

        private void ApplyInitialForces()
        {
            // 1. Calculate a randomized force vector
            // We use transform.TransformDirection so 'Vector3.back' means 'away from the wall' 
            // regardless of how the wall is rotated.
            Vector3 worldDirection = transform.TransformDirection(forceDirection.normalized);
            float finalForce = explosionForce + Random.Range(-forceRandomness, forceRandomness);

            // Apply the linear push
            _rb.AddForce(worldDirection * finalForce, ForceMode.Impulse);

            // 2. Apply a completely randomized torque (spin)
            Vector3 randomTorque = new Vector3(
                Random.Range(-maxTorque, maxTorque),
                Random.Range(-maxTorque, maxTorque),
                Random.Range(-maxTorque, maxTorque)
            );

            _rb.AddTorque(randomTorque, ForceMode.Impulse);
        }
    }
}