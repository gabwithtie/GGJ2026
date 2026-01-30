using UnityEngine;

namespace GabUnity
{
    public class CurrencyManager : Manager_Base<CurrencyManager>
    {
        [SerializeField] private SerializableDictionary<CurrencyInfo, int> currencyamounts = new(null);

        public static int GetAmount(CurrencyInfo currencyInfo)
        {
            if(Instance.currencyamounts.ContainsKey(currencyInfo))
                return Instance.currencyamounts[currencyInfo];
            return 0;
        }

        public static void Add(CurrencyInfo currencyInfo, int amount)
        {
            if(Instance.currencyamounts.ContainsKey(currencyInfo))
                Instance.currencyamounts[currencyInfo] += amount;
            else
                Instance.currencyamounts[currencyInfo] = amount;
        }

        public static bool Spend(CurrencyChangeInfo changeInfo)
        {
            foreach (var change in changeInfo.Costs)
            {
                if (CheckSpend(change.currencyInfo, -change.amount) == false)
                {
                    return false;
                }
            }

            foreach (var change in changeInfo.Costs)
            {
                Spend(change.currencyInfo, -change.amount);
            }

            return true;
        }

        public static bool CheckSpend(CurrencyInfo currencyInfo, int amount)
        {
            if (Instance.currencyamounts.ContainsKey(currencyInfo) == false || Instance.currencyamounts[currencyInfo] < amount)
                return false;

            return true;
        }

        public static bool Spend(CurrencyInfo currencyInfo, int amount)
        {
            if (Instance.currencyamounts.ContainsKey(currencyInfo) == false || Instance.currencyamounts[currencyInfo] < amount)
                return false;
            
            Instance.currencyamounts[currencyInfo] -= amount;
            return true;
        }
    }
}