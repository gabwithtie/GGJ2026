using UnityEngine;

namespace GabUnity
{
    [ExecuteAlways] // This makes the script run in Edit Mode
    [RequireComponent(typeof(LineRenderer))]
    public class GuidedPointerQuad : GuidedInteractable
    {
        [Header("Quad Settings")]
        [Tooltip("Width (X) and Height (Y) of the interactable area.")]
        [SerializeField] private Vector2 quadSize = new Vector2(1f, 1f);

        [Header("Visuals")]
        [SerializeField] private bool showOutlineOnlyOnHover = false;

        private LineRenderer _lineRenderer;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();

            // Basic LineRenderer setup
            _lineRenderer.positionCount = 5;
            _lineRenderer.loop = true;

            // Ensure the line is set to use World Space so our TransformPoint logic works
            _lineRenderer.useWorldSpace = true;
        }

        private void Start()
        {
            UpdateLineRenderer();
        }

        protected override void Update()
        {
            // Only run guidance logic and base interaction logic during Play Mode
            if (Application.isPlaying)
            {
                CalculateAndReportGuidance();
                base.Update();

                if (showOutlineOnlyOnHover)
                {
                    bool isHovered = GuidedPointer.Instance != null &&
                                     GuidedPointer.Instance.IsGuided &&
                                     GuidedPointer.Instance.GuidingObject == gameObject;
                    _lineRenderer.enabled = isHovered;
                }
            }
            else
            {
                // In Edit Mode, always show the line and keep it updated to the transform
                _lineRenderer.enabled = true;
                UpdateLineRenderer();
            }
        }

        private void CalculateAndReportGuidance()
        {
            if (MainCamera.Cam == null || GuidedPointer.Instance == null) return;

            Plane quadPlane = new Plane(transform.forward, transform.position);
            Ray ray = MainCamera.Cam.ScreenPointToRay(GuidedPointer.MouseScreenPosition);

            if (quadPlane.Raycast(ray, out float enter))
            {
                Vector3 worldHitPoint = ray.GetPoint(enter);
                Vector3 localPoint = transform.InverseTransformPoint(worldHitPoint);

                float halfW = quadSize.x * 0.5f;
                float halfH = quadSize.y * 0.5f;

                float clampedX = Mathf.Clamp(localPoint.x, -halfW, halfW);
                float clampedY = Mathf.Clamp(localPoint.y, -halfH, halfH);

                Vector3 clampedLocalPos = new Vector3(clampedX, clampedY, 0);
                Vector3 worldSnapPos = transform.TransformPoint(clampedLocalPos);

                GuidedPointer.Instance.ReportSnapCandidate(worldSnapPos, gameObject);
            }
        }

        private void UpdateLineRenderer()
        {
            if (_lineRenderer == null) _lineRenderer = GetComponent<LineRenderer>();

            float halfW = quadSize.x * 0.5f;
            float halfH = quadSize.y * 0.5f;

            // Update the 5 positions (4 corners + 1 to close the loop)
            _lineRenderer.SetPosition(0, transform.TransformPoint(new Vector3(-halfW, -halfH, 0)));
            _lineRenderer.SetPosition(1, transform.TransformPoint(new Vector3(-halfW, halfH, 0)));
            _lineRenderer.SetPosition(2, transform.TransformPoint(new Vector3(halfW, halfH, 0)));
            _lineRenderer.SetPosition(3, transform.TransformPoint(new Vector3(halfW, -halfH, 0)));
            _lineRenderer.SetPosition(4, transform.TransformPoint(new Vector3(-halfW, -halfH, 0)));
        }

        // This is called automatically when you change values in the Inspector
        private void OnValidate()
        {
            UpdateLineRenderer();
        }
    }
}