using UnityEngine;
using UnityEngine.Events;

namespace GabUnity
{
    [RequireComponent(typeof(ClosestUnitSelector))]

    public class ClosestStatusGiver : MonoBehaviour, IStatusGiver
    {
        [SerializeField] private StatusInfo statusInfo;
        [SerializeField] private int stacks = 1;

        [SerializeField] private UnityEvent<UnitIdentifier> onChangeTarget;

        private ClosestUnitSelector detector;

        int IStatusGiver.Stacks => stacks;

        StatusHolder previous_closest = null;

        private void Awake()
        {
            detector = GetComponent<ClosestUnitSelector>();
        }

        private void Start()
        {
            detector.RegisterOnChangeClosest((new_closest) =>
            {
                if (this.enabled == false)
                    return;

                if (previous_closest != null)
                {
                    previous_closest.RemoveStatus(statusInfo, this);
                }

                if (new_closest == null)
                {
                    onChangeTarget.Invoke(null);
                    previous_closest = null;
                    return;
                }

                if (!new_closest.TryGetComponent(out StatusHolder new_statusHolder))
                    return;

                new_statusHolder.AddStatus(statusInfo, this);
                previous_closest = new_statusHolder;
                onChangeTarget.Invoke(new_closest);
            });
        }

        private void OnDisable()
        {
            if(previous_closest == null)
                return;

            previous_closest.RemoveStatus(statusInfo, this);
            previous_closest = null;

            onChangeTarget.Invoke(null);
        }
    }
}
