using UnityEngine;

namespace GabUnity
{
    [CreateAssetMenu(menuName = "GabUnity/Currency/Currency Info")]
    public class CurrencyInfo : ScriptableObject
    {
        [SerializeField] private string currencyName;
        public string CurrencyName => currencyName;

        [SerializeField] private Sprite currencyIcon;
        public Sprite CurrencyIcon => currencyIcon;
    }
}
