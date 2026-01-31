using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GabUnity
{
    public class DisableOnUnitNear : MonoBehaviour
    {
        [SerializeField] private List<GameObject> to_disable = new();

        private UnitIdentifier latest;

        private void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger)
                return;

            if (other.attachedRigidbody == null)
                return;

            if (other.attachedRigidbody.gameObject.TryGetComponent(out UnitIdentifier player) == false)
                return;

            latest = player;
            to_disable.ForEach(x => x.SetActive(false));
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.isTrigger)
                return;

            if (other.attachedRigidbody == null)
                return;

            if (other.attachedRigidbody.gameObject.TryGetComponent(out UnitIdentifier player) == false)
                return;

            if (latest != player)
                return;

            to_disable.ForEach(x => x.SetActive(true));
        }
    }
}