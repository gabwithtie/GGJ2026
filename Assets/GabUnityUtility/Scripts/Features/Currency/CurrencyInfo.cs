using UnityEngine;

namespace GabUnity
{
    [CreateAssetMenu(menuName = "GabUnity/Currency/Currency Info")]
    public class CurrencyInfo : ScriptableObject
    {
        [SerializeField] private string currencyName;
        [SerializeField] private int max;

        public int Max => max;
        public string CurrencyName => currencyName;

        [SerializeField] private Sprite currencyIcon;
        public Sprite CurrencyIcon => currencyIcon;
    }
}
