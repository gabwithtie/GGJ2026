using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(Collider))]
    public class TriggerForcer : MonoBehaviour
    {
        [SerializeField] private Vector3 local_acceleration;

        private bool insomething;
        private Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        private void OnTriggerStay(Collider other)
        {
            insomething = true;
        }

        private void FixedUpdate()
        {
            if (insomething)
            {
                _collider.attachedRigidbody.AddForce(local_acceleration, ForceMode.Acceleration);
            }

            insomething = false; 
        }
    }
}
