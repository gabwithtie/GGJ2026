using UnityEngine;

namespace GabUnity
{
       public class CurrencyChanger : MonoBehaviour
    {
        [SerializeField] private CurrencyInfo currencyInfo;
        [SerializeField] private int change;
        [SerializeField] private bool only_once = true;

        bool alr_added = false;

        public void AddCurrency()
        {
            if (only_once && !alr_added)
                CurrencyManager.Add(currencyInfo, change);

            alr_added = true;
        }
    }
}
