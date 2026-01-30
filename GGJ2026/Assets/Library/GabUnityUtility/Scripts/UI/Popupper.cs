using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GabUnity
{
    public class Popupper : MonoSingleton<Popupper>
    {
        [SerializeField] private TextMeshProUGUI textmesh;
        [SerializeField] private UnityEvent onHide;
        [SerializeField] private UnityEvent onShow;

        public void Hide()
        {
            onHide.Invoke();
        }

        public void Show()
        {
            onShow.Invoke();
        }

        public static void Popup(string _value)
        {
            Instance.Show();
            Instance.textmesh.SetText(_value);
        }
    }
}
