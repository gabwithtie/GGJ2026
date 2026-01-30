using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using System.Linq;
using GabUnity;

namespace GabUnity
{

    [RequireComponent(typeof(UnitIdentifier))]
    public class TriggerDamager : MonoBehaviour
    {
        [SerializeField] private float damage = 1;

        private UnitIdentifier mUnitController;

        public float GetDamage() => damage;

        private void Awake()
        {
            mUnitController = GetComponent<UnitIdentifier>();
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider.attachedRigidbody == null)
                return;

            if (collider.attachedRigidbody.TryGetComponent(out HealthObject hitable))
            {
                hitable.TakeDamage(damage, mUnitController);
            }
        }
    }
}
