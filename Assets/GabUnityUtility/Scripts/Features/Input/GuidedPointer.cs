using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace GabUnity
{
    // Ensure the Pointer updates its state before interactables read it
    [DefaultExecutionOrder(-50)]
    public class GuidedPointer : MonoSingleton<GuidedPointer>
    {
        [Header("Projection Plane")]
        public float PlaneDist = 10;
        [Tooltip("The detection radius in normalized screen position.")]
        public float ScreenDetectionRadius = 0.1f;
        public LayerMask blockers;

        // The 'Lerped/Snapped' position
        public static Vector3 WorldPosition => Instance._guidedWorldPosition;
        // Helper for lines to get the current hardware mouse position
        public static Vector2 MouseScreenPosition { get; private set; }

        public bool IsGuided { get; private set; }
        public GameObject GuidingObject { get; private set; }

        private Vector3 _guidedWorldPosition;
        private Vector3 _bestSnapTarget;

        // This is now tracking PIXEL distance, not world units
        private float _bestSnapScreenDistanceSqr = float.MaxValue;
        private GameObject _bestSnapObject;

        private void Update()
        {
            UpdateActualMousePosition();

            if (_bestSnapScreenDistanceSqr < float.MaxValue)
            {
                IsGuided = true;
                GuidingObject = _bestSnapObject;
                _guidedWorldPosition = _bestSnapTarget;
            }
            else
            {
                IsGuided = false;
                GuidingObject = null;
            }

            // Reset for next frame
            _bestSnapScreenDistanceSqr = float.MaxValue;
            _bestSnapObject = null;
        }

        private void UpdateActualMousePosition()
        {
            if (Pointer.current == null || MainCamera.Cam == null) return;

            MouseScreenPosition = Pointer.current.position.ReadValue();
            Ray ray = MainCamera.Cam.ScreenPointToRay(MouseScreenPosition);

            var cam_forward = MainCamera.Cam.transform.forward;
            Plane interactionPlane = new Plane(-cam_forward, MainCamera.Cam.transform.position + (cam_forward * PlaneDist));

            if (interactionPlane.Raycast(ray, out float enter))
            {
                _guidedWorldPosition = ray.GetPoint(enter);
            }
        }

        public void ReportSnapCandidate(Vector3 worldSnapPos, GameObject source)
        {
            if (MainCamera.Cam == null) return;
            if (!Instance.isActiveAndEnabled) return;

            // 1. Convert 3D world points to 2D Screen points
            Vector3 screenp = MainCamera.Cam.WorldToScreenPoint(worldSnapPos);

            // Skip if the points are behind the camera
            if (screenp.z < 0) return;

            var delta = MouseScreenPosition - (Vector2)screenp;
            delta /= new Vector2(Screen.width, Screen.height); // Normalize by screen size

            float screenDistSqr = Vector2.SqrMagnitude(delta);

            if (screenDistSqr > ScreenDetectionRadius * ScreenDetectionRadius)
            {
                return;
            }

            // 1. Basic distance and best-score check
            float distToCamSqr = Vector3.SqrMagnitude(worldSnapPos - MainCamera.Cam.transform.position);

            if (distToCamSqr < PlaneDist * PlaneDist && screenDistSqr < _bestSnapScreenDistanceSqr)
            {
                // 2. Line of Sight Check (Raycast)
                Vector3 origin = MainCamera.Cam.transform.position;
                Vector3 direction = worldSnapPos - origin;
                float maxDistance = direction.magnitude;

                bool hit_correct = true;
                bool hit_close = true;

                if (Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance, blockers, QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider.gameObject != source)
                        hit_correct = false;

                    if (hit.distance < maxDistance - 0.05f)
                        hit_close = false;
                }

                if (hit_correct || hit_close)
                {
                    _bestSnapScreenDistanceSqr = screenDistSqr;
                    _bestSnapTarget = worldSnapPos;
                    _bestSnapObject = source;
                }
            }
        }
    }
}