using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace GabUnity
{
    public class UnitDetector : MonoBehaviour
    {
        private List<UnitIdentifier> inside = new();

        [SerializeField] private UnityEvent<UnitIdentifier> OnEnter;
        [SerializeField] private UnityEvent<UnitIdentifier> OnExit;

        [SerializeField] private List<int> teamid_whitelist = new();

        private Action<UnitIdentifier> EnterAction;
        private Action<UnitIdentifier> ExitAction;
        private Action<UnitIdentifier> NoneAction;

        public void DoOnAll(Action<UnitIdentifier> action)
        {
            ValidateList();

            foreach (var unit in inside)
            {
                action(unit);
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

        public bool Contains(UnitIdentifier unit)
        {
            return inside.Contains(unit);
        }

        public UnitIdentifier GetClosest(Vector3 worldpos)
        {
            ValidateList();

            UnitIdentifier closest = null;
            float closest_dist_sqr = float.MaxValue;
            foreach (var unit in inside)
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

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger)
                return;

            if (other.attachedRigidbody == null)
                return;

            if (other.attachedRigidbody.gameObject.TryGetComponent(out UnitIdentifier player) == false)
                return;

            if (inside.Contains(player))
                return;

            if(teamid_whitelist.Count > 0 && !teamid_whitelist.Contains(player.TeamId))
                return;

            inside.Add(player);
            OnEnter.Invoke(player);

            EnterAction?.Invoke(player);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (other.isTrigger)
                return;

            if (other.attachedRigidbody == null)
                return;

            if (other.attachedRigidbody.gameObject.TryGetComponent(out UnitIdentifier player) == false)
                return;

            if (!inside.Contains(player))
                return;

            inside.Remove(player);
            OnExit.Invoke(player);

            ExitAction?.Invoke(player);

            inside.RemoveAll(unit => unit == null);
            if (inside.Count == 0)
                NoneAction?.Invoke(null);
        }

        private bool ValidateList()
        {
            var removed = inside.RemoveAll(unit => unit == null);

            if (removed > 0 && inside.Count == 0)
            {
                NoneAction?.Invoke(null);
            }

            return removed > 0;
        }
    }
}