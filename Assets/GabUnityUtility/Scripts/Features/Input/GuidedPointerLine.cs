using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(LineRenderer))]
    public class GuidedPointerLine : MonoBehaviour
    {
        public Transform PointA;
        public Transform PointB;
        [Tooltip("The detection radius in PIXELS.")]
        public float ScreenDetectionRadius = 50.0f;

        private LineRenderer _lineRenderer;

        private void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 2;
        }

        private void Update()
        {
            if (PointA == null || PointB == null) return;

            _lineRenderer.SetPosition(0, PointA.position);
            _lineRenderer.SetPosition(1, PointB.position);

            // 1. Convert 3D world points to 2D Screen points
            Vector3 screenA = MainCamera.Cam.WorldToScreenPoint(PointA.position);
            Vector3 screenB = MainCamera.Cam.WorldToScreenPoint(PointB.position);

            // Skip if the points are behind the camera
            if (screenA.z < 0 || screenB.z < 0) return;

            // 2. Project the mouse position onto the 2D line segment formed by A and B
            Vector2 mousePos = GuidedPointer.MouseScreenPosition;

            // Get the 't' value (0 to 1) representing the closest point on the segment in screen space
            float t = GetClosestTOnSegment(screenA, screenB, mousePos);

            // 3. Find the 2D distance for the "Snap Winner" calculation
            Vector2 closestScreenPoint = Vector2.Lerp((Vector2)screenA, (Vector2)screenB, t);
            float screenDist = Vector2.Distance(mousePos, closestScreenPoint);

            if (screenDist < ScreenDetectionRadius)
            {
                // 4. Calculate the 3D world position corresponding to that 2D 't'
                Vector3 worldSnapPos = Vector3.Lerp(PointA.position, PointB.position, t);

                // Report back using pixel distance
                GuidedPointer.Instance.ReportSnapCandidate(worldSnapPos, screenDist, gameObject);
            }
        }

        /// <summary>
        /// Calculates the normalized position (0.0 to 1.0) along segment AB 
        /// that is closest to point P.
        /// </summary>
        private float GetClosestTOnSegment(Vector2 a, Vector2 b, Vector2 p)
        {
            Vector2 ab = b - a;
            float sqrLen = ab.sqrMagnitude;
            if (sqrLen == 0) return 0; // A and B are on the same pixel

            // Standard scalar projection formula: t = ( (P-A) . (B-A) ) / |B-A|^2
            float t = Vector2.Dot(p - a, ab) / sqrLen;
            return Mathf.Clamp01(t);
        }
    }
}