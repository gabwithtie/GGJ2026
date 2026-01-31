using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TargetZRigidbody : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float target_z = 0;
    [SerializeField] private float acceleration = 2;
    [SerializeField] private bool frontonly = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (frontonly && target_z - transform.position.z < 0)
            return;

        rb.AddForce(Vector3.forward * Mathf.Sign(target_z - transform.position.z) * acceleration, ForceMode.Acceleration);
    }
}
