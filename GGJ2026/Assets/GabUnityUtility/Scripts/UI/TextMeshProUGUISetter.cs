using TMPro;
using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextMeshProUGUISetter : MonoBehaviour
    {
        private TextMeshProUGUI _TextMeshProUGUI;

        private void Awake()
        {
            _TextMeshProUGUI = GetComponent<TextMeshProUGUI>();
        }

        public void SetText(string _value)
        {
            _TextMeshProUGUI.SetText(_value);
        }


        public void SetText(float _value)
        {
            _TextMeshProUGUI.SetText(_value.ToString());
        }

        public void SetText(int _value)
        {
            _TextMeshProUGUI.SetText(_value.ToString());
        }
    }

}