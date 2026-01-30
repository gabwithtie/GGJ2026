using System;
using System.Collections.Generic;
using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(UnitIdentifier))]
    public class StatusHolder : MonoBehaviour
    {
        [SerializeField] private SerializableDictionary<StatusInfo, List<IStatusGiver>> statuses = new(null);

        private Action<StatusInfo> OnAdd;
        private Action<StatusInfo> OnRemove;
        private Action<StatusInfo, int> OnChangeStacks;

        public void RegisterAddAction(Action<StatusInfo> action)
        {
            OnAdd += action;
        }

        public void RegisterRemoveAction(Action<StatusInfo> action)
        {
            OnRemove += action;
        }

        public void RegisterChangeStacksAction(Action<StatusInfo, int> action)
        {
            OnChangeStacks += action;
        }

        public bool HasStatus(StatusInfo statusInfo)
        {
            return statuses.ContainsKey(statusInfo);
        }

        public int GetStatusStacks(StatusInfo statusInfo)
        {
            if (statuses.ContainsKey(statusInfo))
            {
                int totalStacks = 0;
                foreach (var giver in statuses[statusInfo])
                {
                    totalStacks += Mathf.Max(giver.Stacks, 1);
                }
                return totalStacks;
            }
            return 0;
        }

        public void AddStatus(StatusInfo statusInfo, IStatusGiver giver)
        {
            if (statuses.ContainsKey(statusInfo))
            {
                if (statuses[statusInfo].Contains(giver) == false)
                    statuses[statusInfo].Add(giver);
            }
            else
            {
                statuses[statusInfo] = new List<IStatusGiver>() { giver };
            }

            OnChangeStacks?.Invoke(statusInfo, GetStatusStacks(statusInfo));
            OnAdd?.Invoke(statusInfo);
        }
        public void RemoveStatus(StatusInfo statusInfo, IStatusGiver giver)
        {
            if (statuses.ContainsKey(statusInfo))
            {
                statuses[statusInfo].Remove(giver);

                OnChangeStacks?.Invoke(statusInfo, GetStatusStacks(statusInfo));

                if (statuses[statusInfo].Count == 0)
                {
                    statuses.Remove(statusInfo);
                    OnRemove?.Invoke(statusInfo);
                }
            }
        }
    }
}