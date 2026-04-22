using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace GabUnity
{
    // By inheriting from UnityEvent<Vector3, GameObject>, this ensures 
    // the event shows up perfectly in the Unity Inspector.
    [System.Serializable]
    public class ClickHitEvent : UnityEvent<Vector3, GameObject> { }

    public class PlayerClickHandler : MonoBehaviour
    {
        [Header("Raycast Settings (Fallback)")]
        [SerializeField] private LayerMask collisionLayer;
        [SerializeField] private float rayDistance = 100f;

        public ClickHitEvent OnClickHit;
        public ClickHitEvent OnHoverHit;

        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        public void OnClickTarget(InputAction.CallbackContext context)
        {
            if (!context.started) return;

            // 1. Check if the GuidedPointer is currently snapped to something
            if (GuidedPointer.Instance.IsGuided)
            {
                // Fire event using the guided position and the object that guided it
                OnClickHit.Invoke(GuidedPointer.WorldPosition, GuidedPointer.Instance.GuidingObject);
                return;
            }

            // 2. Fallback: Standard Raycast if not guided
            if (Pointer.current == null) return;
            Vector2 pointerPos = Pointer.current.position.ReadValue();
            Ray ray = _mainCamera.ScreenPointToRay(pointerPos);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, rayDistance, collisionLayer))
            {
                OnClickHit.Invoke(hitInfo.point, hitInfo.collider.gameObject);
            }
        }

        private void LateUpdate()
        {
            // 1. Check if the GuidedPointer is currently snapped to something
            if (GuidedPointer.Instance.IsGuided)
            {
                // Fire event using the guided position and the object that guided it
                OnHoverHit.Invoke(GuidedPointer.WorldPosition, GuidedPointer.Instance.GuidingObject);
                return;
            }

            // 2. Fallback: Standard Raycast if not guided
            if (Pointer.current == null) return;
            Vector2 pointerPos = Pointer.current.position.ReadValue();
            Ray ray = _mainCamera.ScreenPointToRay(pointerPos);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, rayDistance, collisionLayer))
            {
                OnHoverHit.Invoke(hitInfo.point, hitInfo.collider.gameObject);
            }
        }
    }
}