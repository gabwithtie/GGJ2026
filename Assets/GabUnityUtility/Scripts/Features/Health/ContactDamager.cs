using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using System.Linq;
using GabUnity;

namespace GabUnity
{

    [RequireComponent(typeof(UnitIdentifier))]
    public class ContactDamager : MonoBehaviour
    {
        [SerializeField] private float damage = 1;
        [SerializeField] private float cooldown = 0.5f;

        private UnitIdentifier mUnitController;

        private List<(HealthObject what, float time_left)> damaged_recently = new();

        public float GetDamage() => damage;

        private void Awake()
        {
            mUnitController = GetComponent<UnitIdentifier>();
        }

        private void Update()
        {
            for (int i = 0; i < damaged_recently.Count; i++)
            {
                var (what, time_left) = damaged_recently[i];
                damaged_recently[i] = (what, time_left - Time.deltaTime);
            }

            damaged_recently.RemoveAll(item => item.time_left <= 0);
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collision.collider.attachedRigidbody == null)
                return;

            if (collision.collider.attachedRigidbody.TryGetComponent(out HealthObject hitable))
            {
                if (damaged_recently.Any(item => item.what == hitable))
                    return;

                hitable.TakeDamage(damage, mUnitController);
                damaged_recently.Add((hitable, cooldown));
            }
        }
    }
}
