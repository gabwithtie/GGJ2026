using UnityEngine;
using UnityEngine.InputSystem;

namespace GabUnity
{
    public class GuidedPointer : MonoSingleton<GuidedPointer>
    {
        [Header("Settings")]
        public float SnapSmoothing = 20f;

        [Header("Projection Plane")]
        public float PlaneDist = 10;

        // The 'Raw' mouse position projected onto the world plane
        public static Vector3 RawWorldPosition => Instance._actualWorldPosition;
        // The 'Lerped/Snapped' position
        public static Vector3 WorldPosition => Instance._guidedWorldPosition;
        // Helper for lines to get the current hardware mouse position
        public static Vector2 MouseScreenPosition { get; private set; }

        public bool IsGuided { get; private set; }
        public GameObject GuidingObject { get; private set; }

        private Vector3 _actualWorldPosition;
        private Vector3 _guidedWorldPosition;
        private Vector3 _bestSnapTarget;

        // This is now tracking PIXEL distance, not world units
        private float _bestSnapScreenDistance = float.MaxValue;
        private GameObject _bestSnapObject;

        private void Update()
        {
            UpdateActualMousePosition();

            if (_bestSnapScreenDistance < float.MaxValue)
            {
                IsGuided = true;
                GuidingObject = _bestSnapObject;
                _guidedWorldPosition = Vector3.Lerp(_guidedWorldPosition, _bestSnapTarget, Time.deltaTime * SnapSmoothing);
            }
            else
            {
                IsGuided = false;
                GuidingObject = null;
                _guidedWorldPosition = Vector3.Lerp(_guidedWorldPosition, _actualWorldPosition, Time.deltaTime * SnapSmoothing);
            }

            // Reset for next frame
            _bestSnapScreenDistance = float.MaxValue;
            _bestSnapObject = null;
        }

        private void UpdateActualMousePosition()
        {
            if (Pointer.current == null) return;

            MouseScreenPosition = Pointer.current.position.ReadValue();
            Ray ray = MainCamera.Cam.ScreenPointToRay(MouseScreenPosition);

            var cam_forward = MainCamera.Cam.transform.forward;
            Plane interactionPlane = new Plane(-cam_forward, MainCamera.Cam.transform.position + (cam_forward * PlaneDist));

            if (interactionPlane.Raycast(ray, out float enter))
            {
                _actualWorldPosition = ray.GetPoint(enter);
            }
        }

        /// <summary>
        /// Logic now uses screenDistance to decide which candidate wins.
        /// </summary>
        public void ReportSnapCandidate(Vector3 worldSnapPos, float screenDistance, GameObject source)
        {
            if (Vector3.SqrMagnitude(worldSnapPos - MainCamera.Cam.transform.position) < PlaneDist * PlaneDist && screenDistance < _bestSnapScreenDistance)
            {
                _bestSnapScreenDistance = screenDistance;
                _bestSnapTarget = worldSnapPos;
                _bestSnapObject = source;
            }
        }
    }
}