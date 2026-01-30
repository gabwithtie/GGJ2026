using TMPro;
using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class CurrencyChangeText : MonoBehaviour
    {
        [SerializeField] private string mainText = "";
        [SerializeField] private bool show_currency_name;
        [SerializeField] private CurrencyChangeInfo spendInfo;

        private TextMeshProUGUI _textMeshPro;

        private void Start()
        {
            _textMeshPro = GetComponent<TextMeshProUGUI>();

            var amount_string = "";

            foreach (var item in spendInfo.Costs)
            {
                var substring = item.amount > 0 ? $" +{item.amount}" : $"{item.amount}";

                if (show_currency_name)
                    amount_string += $"({substring} {item.currencyInfo.CurrencyName})";
            }

            _textMeshPro.text = $"{mainText} {amount_string}";
        }
    }

}