using UnityEngine;

namespace GabUnity
{
    [CreateAssetMenu(menuName = "GabUnity/Currency/Spend Info")]
    public class CurrencyChangeInfo : ScriptableObject
    {
        [System.Serializable]
        public struct CurrencyAmount
        {
            public CurrencyInfo currencyInfo;
            public int amount;
        }

        [SerializeField] private CurrencyAmount[] costs;
        public CurrencyAmount[] Costs => costs;
    }
}