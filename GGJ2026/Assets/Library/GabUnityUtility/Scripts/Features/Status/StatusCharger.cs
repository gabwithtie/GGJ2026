using UnityEngine;
using UnityEngine.Events;

namespace GabUnity
{
    [RequireComponent(typeof(StatusHolder))]
    public class StatusCharger : MonoBehaviour
    {
        private StatusHolder statusHolder;

        [SerializeField] private StatusInfo targetstatus;
        [SerializeField] private UnityEvent OnFullCharge;
        [SerializeField] private UnityEvent<float> OnChargeChangeRatio;

        [Header("Values")]
        [SerializeField] private float max_charge = 5;
        [SerializeField] private float charge_per_second_per_stack = 1;
        [SerializeField] private float current_charge = 0;


        private void Awake()
        {
            statusHolder = GetComponent<StatusHolder>();
        }
        
        public void ResetCharge()
        {
            current_charge = 0;
            OnChargeChangeRatio.Invoke(0);
        }

        private void Update()
        {
            var stacks = statusHolder.GetStatusStacks(targetstatus);

            current_charge += charge_per_second_per_stack * stacks * Time.deltaTime;

            OnChargeChangeRatio.Invoke(Mathf.Clamp01(current_charge / max_charge));

            if (current_charge >= max_charge)
            {
                OnFullCharge.Invoke();
            }
        }

    }
}
