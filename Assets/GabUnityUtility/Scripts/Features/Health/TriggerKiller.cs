using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(UnitIdentifier))]
    public class TriggerKiller : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("How long a unit can stay in the zone before dying.")]
        [SerializeField] private float KillTime = 3.0f;

        private UnitIdentifier mUnitController;

        // Track active "Kill" coroutines indexed by the HealthObject they are targeting
        private Dictionary<HealthObject, Coroutine> _activeTimers = new Dictionary<HealthObject, Coroutine>();

        private void Awake()
        {
            mUnitController = GetComponent<UnitIdentifier>();
        }

        public void KillRigidbody(Rigidbody target)
        {
            if (target.TryGetComponent(out HealthObject hitable))
            {
                // If we aren't already tracking this unit, start the kill timer
                if (!_activeTimers.ContainsKey(hitable))
                {
                    Coroutine killRoutine = StartCoroutine(KillAfterDelay(hitable));
                    _activeTimers.Add(hitable, killRoutine);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.attachedRigidbody == null)
                return;

            KillRigidbody(other.attachedRigidbody);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.attachedRigidbody == null)
                return;

            if (other.attachedRigidbody.TryGetComponent(out HealthObject hitable))
            {
                // If the unit leaves the zone, stop the timer and remove it from the tracking list
                if (_activeTimers.TryGetValue(hitable, out Coroutine activeRoutine))
                {
                    StopCoroutine(activeRoutine);
                    _activeTimers.Remove(hitable);
                }
            }
        }

        private IEnumerator KillAfterDelay(HealthObject hitable)
        {
            yield return new WaitForSeconds(KillTime);

            if (hitable != null)
            {
                hitable.Kill(mUnitController);
            }

            // Cleanup dictionary after the kill is executed
            _activeTimers.Remove(hitable);
        }

        private void OnDisable()
        {
            // Safety: Stop all timers if the trigger object is disabled/destroyed
            foreach (var routine in _activeTimers.Values)
            {
                if (routine != null) StopCoroutine(routine);
            }
            _activeTimers.Clear();
        }
    }
}