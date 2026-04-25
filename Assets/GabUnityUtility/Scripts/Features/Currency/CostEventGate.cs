using UnityEngine;
using UnityEngine.Events;

namespace GabUnity
{
    public class CostEventGate : MonoBehaviour
    {
        [SerializeField] private UnityEvent onBuySuccess;
        [SerializeField] private UnityEvent onBuyFail;

        [SerializeField] private CurrencyChangeInfo costInfo;

        public void TryBuy()
        {
            if(CurrencyManager.Spend(costInfo))
            {
                onBuySuccess.Invoke();
            }
            else
            {
                onBuyFail.Invoke();
            }
        }
    }
}
