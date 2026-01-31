using TMPro;
using UnityEngine;


namespace GabUnity
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class CurrencyText : MonoBehaviour
    {
        private TextMeshProUGUI _textMeshPro;

        [SerializeField] private string prefix = "";
        [SerializeField] private CurrencyInfo currencyInfo;

        private void Start()
        {
            _textMeshPro = GetComponent<TextMeshProUGUI>();
        }

        // Update is called once per frame
        void Update()
        {
            _textMeshPro.text = $"{prefix}{CurrencyManager.GetAmount(currencyInfo)}";
        }
    }
}
