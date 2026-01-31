using UnityEngine;
using UnityEngine.Events;

namespace GabUnity
{
    [RequireComponent(typeof(StatusHolder))]
    public class StatusActioner : MonoBehaviour
    {
        private StatusHolder statusHolder;

        [SerializeField] private StatusInfo targetstatus;
        [SerializeField] private UnityEvent OnAddStatus;
        [SerializeField] private UnityEvent OnRemoveStatus;
        [SerializeField] private UnityEvent<int> OnChangeStacks;

        private void Awake()
        {
            statusHolder = GetComponent<StatusHolder>();

            if(targetstatus == null)
                Debug.LogError(Equals(gameObject.name, this) + " has no target status assigned.");
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            statusHolder.RegisterAddAction((status) =>
            {
                if (status != targetstatus)
                    return;

                OnAddStatus.Invoke();
            });

            statusHolder.RegisterRemoveAction((status) =>
            {
                if (status != targetstatus)
                    return;

                OnRemoveStatus.Invoke();
            });

            statusHolder.RegisterChangeStacksAction((status, stacks) =>
            {
                if (status != targetstatus)
                    return;

                OnChangeStacks.Invoke(stacks);
            });
        }
    }
}