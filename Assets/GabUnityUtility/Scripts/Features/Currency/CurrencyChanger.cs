using UnityEngine;

namespace GabUnity
{
       public class CurrencyChanger : MonoBehaviour
    {
        [SerializeField] private CurrencyInfo currencyInfo;
        [SerializeField] private int change;
        public void AddCurrency()
        {
            CurrencyManager.Add(currencyInfo, change);
        }
    }
}
