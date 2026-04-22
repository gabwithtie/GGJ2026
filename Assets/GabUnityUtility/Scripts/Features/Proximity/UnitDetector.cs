using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace GabUnity
{
    public class UnitDetector : MonoBehaviour
    {
        private List<UnitIdentifier> inside = new();
        // New list to store only those currently visible
        private List<UnitIdentifier> visibleUnits = new();

        [Header("Detection Settings")]
        [SerializeField] private List<int> teamid_whitelist = new();

        [Header("Line of Sight Settings")]
        [SerializeField] private bool requireLineOfSight = false;
        [SerializeField] private LayerMask losLayerMask;
        [SerializeField] private float losUpdateRate = 0.1f; // Check visibility 10 times a second
        [SerializeField] private Vector3 eyeOffset = Vector3.up; // Offset from turret pivot

        [SerializeField] private UnityEvent<UnitIdentifier> OnEnter;
        [SerializeField] private UnityEvent<UnitIdentifier> OnExit;

        private Action<UnitIdentifier> EnterAction;
        private Action<UnitIdentifier> ExitAction;
        private Action<UnitIdentifier> NoneAction;

        private float _nextLoSTime;

        private void Update()
        {
            if (requireLineOfSight && Time.time >= _nextLoSTime)
            {
                UpdateVisibility();
                _nextLoSTime = Time.time + losUpdateRate;
            }
        }

        private void UpdateVisibility()
        {
            visibleUnits.Clear();
            Vector3 origin = transform.position + eyeOffset;

            foreach (var unit in inside)
            {
                if (unit == null) continue;

                Vector3 targetPos = unit.transform.position + Vector3.up; // Aim for chest/head
                Vector3 direction = targetPos - origin;
                float distance = direction.magnitude;

                // Raycast to check if anything in losLayerMask blocks the view
                if (!Physics.Raycast(origin, direction, distance, losLayerMask))
                {
                    visibleUnits.Add(unit);
                }
            }

            if (visibleUnits.Count == 0 && inside.Count > 0)
            {
                NoneAction?.Invoke(null);
            }
        }

        public void DoOnAll(Action<UnitIdentifier> action)
        {
            ValidateList();
            // Use visibleUnits if LoS is enabled, otherwise use everyone inside the trigger
            var targets = requireLineOfSight ? visibleUnits : inside;

            foreach (var unit in targets)
            {
                action(unit);
            }
        }

        public UnitIdentifier GetClosest(Vector3 worldpos)
        {
            ValidateList();
            var targets = requireLineOfSight ? visibleUnits : inside;

            UnitIdentifier closest = null;
            float closest_dist_sqr = float.MaxValue;
            foreach (var unit in targets)
            {
                float dist_sqr = (unit.transform.position - worldpos).sqrMagnitude;
                if (dist_sqr < closest_dist_sqr)
                {
                    closest = unit;
                    closest_dist_sqr = dist_sqr;
                }
            }
            return closest;
        }

        // ... existing OnTriggerEnter/Exit logic remains the same
        private void OnCollisionEnter(Collision collision) => OtherEnter(collision.collider);
        private void OnCollisionExit(Collision collision) => OtherExit(collision.collider);
        protected virtual void OnTriggerEnter(Collider other)=> OtherEnter(other);

        protected virtual void OnTriggerExit(Collider other) => OtherExit(other);   

        private void OtherEnter(Collider other)
        {
            if (other.isTrigger || other.attachedRigidbody == null) return;

            if (other.attachedRigidbody.gameObject.TryGetComponent(out UnitIdentifier player))
            {
                if (inside.Contains(player)) return;
                if (teamid_whitelist.Count > 0 && !teamid_whitelist.Contains(player.TeamId)) return;

                inside.Add(player);

                OnEnter?.Invoke(player);
                EnterAction?.Invoke(player);
            }
        }

        private void OtherExit(Collider other)
        {
            if (other.isTrigger || other.attachedRigidbody == null) return;

            if (other.attachedRigidbody.gameObject.TryGetComponent(out UnitIdentifier player))
            {
                if (!inside.Contains(player)) return;

                inside.Remove(player);
                visibleUnits.Remove(player); // Ensure they are removed from visibility too

                if (OnExit != null)
                    OnExit.Invoke(player);

                ExitAction?.Invoke(player);

                if (inside.Count == 0) NoneAction?.Invoke(null);
            }
        }

        private bool ValidateList()
        {
            int removed = inside.RemoveAll(unit => unit == null);
            visibleUnits.RemoveAll(unit => unit == null);

            if (removed > 0 && inside.Count == 0)
            {
                NoneAction?.Invoke(null);
            }
            return removed > 0;
        }

        private void OnDrawGizmosSelected()
        {
            if (requireLineOfSight)
            {
                Gizmos.color = Color.red;
                Vector3 origin = transform.position + eyeOffset;
                foreach (var unit in visibleUnits)
                {
                    Gizmos.DrawLine(origin, unit.transform.position + Vector3.up);
                }
            }
        }

        public void RegisterEnterAction(Action<UnitIdentifier> action)
        {
            EnterAction += action;
        }

        public void RegisterExitAction(Action<UnitIdentifier> action)
        {
            ExitAction += action;
        }

        public void RegisterNoneAction(Action<UnitIdentifier> action)
        {
            NoneAction += action;
        }
    }
}