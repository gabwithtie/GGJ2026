using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GabUnity;

namespace GabUnity
{

    [RequireComponent(typeof(UnitIdentifier))]
    public class TriggerKiller : MonoBehaviour
    {
        private UnitIdentifier mUnitController;


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
                hitable.Kill(mUnitController);
            }
        }
    }
}
