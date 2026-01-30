using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GabUnity
{
    [RequireComponent(typeof(Collider))]
    public class PointerCatcher : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField] private UnityEvent<Vector3> OnClick;
        [SerializeField] private UnityEvent<Vector3> OnDown;
        [SerializeField] private UnityEvent<Vector3> OnDrag;
        [SerializeField] private UnityEvent<Vector3> OnUp;

        Vector2 _pixel_pos_held;

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            Vector3 clickPosition = eventData.pointerCurrentRaycast.worldPosition;
            OnDrag.Invoke(clickPosition);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            // 1. Retrieve the world position from the event's raycast result
            Vector3 clickPosition = eventData.pointerCurrentRaycast.worldPosition;
            OnClick.Invoke(clickPosition);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            Vector3 clickPosition = eventData.pointerCurrentRaycast.worldPosition;
            OnDown.Invoke(clickPosition);

            _pixel_pos_held = eventData.position;
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            Vector3 clickPosition = eventData.pointerCurrentRaycast.worldPosition;
            OnUp.Invoke(clickPosition);
        }
    }
}