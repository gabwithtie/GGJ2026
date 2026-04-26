using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GabUnity
{

    public class DoOnUnitNear : MonoBehaviour
    {
        [SerializeField] private UnityEvent onEnter;
        [SerializeField] private UnityEvent onExit;

        [SerializeField] private bool whitelist_only = false;
        [SerializeField] private int whitelist = 0;

        private void OnTriggerEnter(Collider other)
        {
            if(whitelist_only)
            {
                var basis = other.gameObject;

                if (other.attachedRigidbody != null)
                    basis = other.attachedRigidbody.gameObject;

                if (!basis.TryGetComponent(out UnitIdentifier unit))
                    return;

                if(unit.TeamId != whitelist)
                    return;

                onEnter.Invoke();
            }
            else
            {
                    onEnter.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (whitelist_only)
            {
                var basis = other.gameObject;

                if (other.attachedRigidbody != null)
                    basis = other.attachedRigidbody.gameObject;

                if (!basis.TryGetComponent(out UnitIdentifier unit))
                    return;

                if (unit.TeamId != whitelist)
                    return;

                onExit.Invoke();
            }
            else
            {
                onExit.Invoke();
            }
        }
    }
}