using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GabUnity
{
    public class OnHoverButton : Selectable, IPointerClickHandler
    {
        [SerializeField] private UnityEvent<bool> onHover;
        [SerializeField] private UnityEvent onEnter;
        [SerializeField] private UnityEvent onExit;
        [SerializeField] private UnityEvent onClick;



        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            onHover.Invoke(true);
            onEnter.Invoke();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            onHover.Invoke(false);
            onExit.Invoke();
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            onClick.Invoke();
        }
    }
}