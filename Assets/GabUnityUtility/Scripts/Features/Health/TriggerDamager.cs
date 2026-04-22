using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GabUnity
{
    public class TriggerDamager : MonoBehaviour
    {
        [SerializeField] private float damage = 1;
        [SerializeField] private float cooldown = 0.5f;

        private UnitIdentifier mUnitController;

        private List<(HealthObject what, float time_left)> damaged_recently = new();

        [SerializeField] private UnityEvent<float> additionalEvents;

        public float GetDamage() => damage;

        private void Awake()
        {
            var collider = GetComponent<Collider>();

            if (collider == null)
            {
                Debug.LogError("TriggerDamager requires a Collider component.");
                enabled = false;
                return;
            }

            if (collider.attachedRigidbody == null)
            {
                Debug.LogError("TriggerDamager requires a Rigidbody component attached to the same GameObject.");
                enabled = false;
                return;
            }

            collider.attachedRigidbody.TryGetComponent(out mUnitController);
        }

        private void OnTriggerEnter(Collider other)
        {
            if(this.enabled == false)
                return;

            if (other.attachedRigidbody == null)
                return;

            if (other.attachedRigidbody.TryGetComponent(out HealthObject hitable))
            {
                hitable.TakeDamage(damage, mUnitController);

                additionalEvents.Invoke(damage);
            }
        }
    }
}