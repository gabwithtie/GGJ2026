using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GabUnity
{
    public class PointerCatcher : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
    {
        [SerializeField] private bool PassToRigidbody = true;

        [SerializeField] private UnityEvent<Vector3> OnClick;
        [SerializeField] private UnityEvent<Vector3> OnEnter;
        [SerializeField] private UnityEvent<Vector3> OnMove;
        [SerializeField] private UnityEvent<Vector3> OnExit;
        [SerializeField] private UnityEvent<Vector3> OnDown;
        [SerializeField] private UnityEvent<Vector3> OnDrag;
        [SerializeField] private UnityEvent<Vector3> OnUp;

        Vector2 _pixel_pos_held;

        Collider _collider;
        Rigidbody _rbody;
        PointerCatcher _rbodyCatcher;

        bool _pointerhovering = false;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _rbody = GetComponent<Rigidbody>();

            if (_rbody == null && _collider != null)
                _rbody = _collider.attachedRigidbody;

            if (_rbody != null)
                _rbodyCatcher = _rbody.GetComponent<PointerCatcher>();

            if(_rbodyCatcher == this)
                _rbodyCatcher = null;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            Vector3 clickPosition = eventData.pointerCurrentRaycast.worldPosition;
            OnDrag.Invoke(clickPosition);

            if (PassToRigidbody && _rbodyCatcher != null)
                _rbodyCatcher.OnDrag.Invoke(clickPosition);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            // 1. Retrieve the world position from the event's raycast result
            Vector3 clickPosition = eventData.pointerCurrentRaycast.worldPosition;
            OnClick.Invoke(clickPosition);

            if (PassToRigidbody && _rbodyCatcher != null)
                _rbodyCatcher.OnClick.Invoke(clickPosition);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            Vector3 clickPosition = eventData.pointerCurrentRaycast.worldPosition;
            OnDown.Invoke(clickPosition);

            _pixel_pos_held = eventData.position;

            if (PassToRigidbody && _rbodyCatcher != null)
                _rbodyCatcher.OnDown.Invoke(clickPosition);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            Vector3 clickPosition = eventData.pointerCurrentRaycast.worldPosition;
            OnEnter.Invoke(clickPosition);

            if (PassToRigidbody && _rbodyCatcher != null)
                _rbodyCatcher.OnEnter.Invoke(clickPosition);

            _pointerhovering = true;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (PassToRigidbody && _rbodyCatcher != null && _rbodyCatcher._pointerhovering)
                return; //prevent double signaling from parent rigidbody

            Vector3 clickPosition = eventData.pointerCurrentRaycast.worldPosition;
            OnExit.Invoke(clickPosition);

            if (PassToRigidbody && _rbodyCatcher != null)
                _rbodyCatcher.OnExit.Invoke(clickPosition);

            _pointerhovering = false;
        }

        void IPointerMoveHandler.OnPointerMove(PointerEventData eventData)
        {
            Vector3 clickPosition = eventData.pointerCurrentRaycast.worldPosition;
            OnMove.Invoke(clickPosition);

            if (PassToRigidbody && _rbodyCatcher != null)
                _rbodyCatcher.OnMove.Invoke(clickPosition);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            Vector3 clickPosition = eventData.pointerCurrentRaycast.worldPosition;
            OnUp.Invoke(clickPosition);

            if (PassToRigidbody && _rbodyCatcher != null)
                _rbodyCatcher.OnUp.Invoke(clickPosition);
        }
    }
}