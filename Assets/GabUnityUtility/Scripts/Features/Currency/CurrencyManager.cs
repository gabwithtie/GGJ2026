using UnityEngine;

namespace GabUnity
{
    public class CurrencyManager : Manager_Base<CurrencyManager>
    {
        [SerializeField] private SerializableDictionary<CurrencyInfo, int> currencyamounts = new(null);

        public static int GetAmount(CurrencyInfo currencyInfo)
        {
            if (currencyInfo == null)
                return 0;

            if (Instance.currencyamounts.ContainsKey(currencyInfo))
                return Instance.currencyamounts[currencyInfo];
            return 0;
        }

        public static void Add(CurrencyInfo currencyInfo, int amount)
        {
            if (Instance.currencyamounts.ContainsKey(currencyInfo))
                Instance.currencyamounts[currencyInfo] += amount;
            else
                Instance.currencyamounts[currencyInfo] = amount;

            if (Instance.currencyamounts[currencyInfo] > currencyInfo.Max)
                Instance.currencyamounts[currencyInfo] = currencyInfo.Max;
        }

        public static bool Spend(CurrencyChangeInfo changeInfo)
        {
            foreach (var change in changeInfo.Costs)
            {
                // We check the negative amount because change.amount is likely stored as a positive cost
                // Assuming CheckSpend(info, amount) expects the cost as a positive integer
                if (CheckSpend(change.currencyInfo, -change.amount) == false)
                {
                    // FAILURE DETECTED: Notify the scene
                    NotifySpendFailed(change.currencyInfo);
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
            {
                // FAILURE DETECTED: Single item spend failure
                NotifySpendFailed(currencyInfo);
                return false;
            }

            Instance.currencyamounts[currencyInfo] -= amount;
            return true;
        }

        /// <summary>
        /// Finds all OnSpendFailedHandler components in the scene and triggers them.
        /// </summary>
        private static void NotifySpendFailed(CurrencyInfo info)
        {
            // FindObjectsByType is the modern, faster replacement for FindObjectsOfType
            OnSpendFailedHandler[] handlers = Object.FindObjectsByType<OnSpendFailedHandler>(FindObjectsSortMode.None);

            foreach (var handler in handlers)
            {
                handler.OnSpendFailed(info);
            }
        }
    }
}