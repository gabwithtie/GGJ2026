using UnityEngine;
using UnityEngine.Events;

namespace GabUnity
{
    public class OnSpendFailedHandler : MonoBehaviour
    {
        [Header("Events")]
        [Tooltip("Fires when CurrencyManager detects a failed transaction.")]
        public UnityEvent<CurrencyInfo> OnSpendFailedEvent;

        /// <summary>
        /// This is called automatically by CurrencyManager.NotifySpendFailed.
        /// </summary>
        /// <param name="info">The specific currency that was insufficient.</param>
        public void OnSpendFailed(CurrencyInfo info)
        {
            // Trigger the UnityEvent to notify any listeners in the Inspector
            OnSpendFailedEvent?.Invoke(info);
        }
    }
}