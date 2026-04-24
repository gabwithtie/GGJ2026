using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace GabUnity
{
    public class GuidedInteractable : MonoBehaviour
    {
        [Header("Guided Events")]
        public UnityEvent OnHoverEnter;
        public UnityEvent OnHoverExit;
        public UnityEvent OnClick;

        private bool _wasHovered;

        protected virtual void Update()
        {
            if (GuidedPointer.Instance == null) return;

            // Check if the GuidedPointer is currently locked onto THIS gameObject
            bool isCurrentlyHovered = GuidedPointer.Instance.IsGuided &&
                                      GuidedPointer.Instance.GuidingObject == gameObject;

            // 1. Handle Hover Enter
            if (isCurrentlyHovered && !_wasHovered)
            {
                OnHoverEnter?.Invoke();
            }
            // 2. Handle Hover Exit
            else if (!isCurrentlyHovered && _wasHovered)
            {
                OnHoverExit?.Invoke();
            }

            // 3. Handle Click (Only registers if this object is currently hovered)
            if (isCurrentlyHovered)
            {
                if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
                {
                    OnClick?.Invoke();
                }
            }

            _wasHovered = isCurrentlyHovered;
        }

        protected virtual void OnDisable()
        {
            // Failsafe to ensure we trigger an exit if the object gets turned off while hovered
            if (_wasHovered)
            {
                OnHoverExit?.Invoke();
                _wasHovered = false;
            }
        }
    }
}