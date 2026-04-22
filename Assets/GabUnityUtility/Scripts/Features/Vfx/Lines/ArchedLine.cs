using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(LineRenderer))]
    public class ArchedLine : MonoBehaviour
    {
        private LineRenderer lineRenderer;

        [Header("Positions")]
        [SerializeField] private Vector3 pos_a;
        [SerializeField] private Vector3 pos_b;

        [Header("Curve Settings")]
        [SerializeField] private int vertexes = 20; // Higher = smoother curve
        [SerializeField] private float archHeight = 2f;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            // Ensure the LineRenderer uses world space if positions are provided in world space
            lineRenderer.useWorldSpace = true;
        }

        public void SetStart(Vector3 pos) => SetStartEnd(pos, pos_b);
        public void SetEnd(Vector3 pos) => SetStartEnd(pos_a, pos);

        public void SetStartEnd(Vector3 start, Vector3 end)
        {
            pos_a = start;
            pos_b = end;
            UpdateLine();
        }

        void UpdateLine()
        {
            if (lineRenderer == null) return;

            lineRenderer.positionCount = vertexes;

            // Define the Control Point for the Quadratic Bezier
            // Midpoint + (Local Up * Height)
            Vector3 midPoint = (pos_a + pos_b) / 2f;
            Vector3 controlPoint = midPoint + (transform.up * archHeight);

            for (int i = 0; i < vertexes; i++)
            {
                // T is the normalized progress (0 to 1) along the line
                float t = i / (float)(vertexes - 1);

                // Quadratic Bezier Formula: (1-t)^2*P0 + 2(1-t)t*P1 + t^2*P2
                Vector3 point = CalculateQuadraticBezierPoint(t, pos_a, controlPoint, pos_b);

                lineRenderer.SetPosition(i, point);
            }
        }

        private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            // Standard Bézier formula
            // B(t) = (1-t)²P0 + 2(1-t)tP1 + t²P2
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;

            Vector3 p = uu * p0; // (1-t)^2 * P0
            p += 2 * u * t * p1; // 2 * (1-t) * t * P1
            p += tt * p2;        // t^2 * P2

            return p;
        }

        // Context menu to preview in editor
        [ContextMenu("Refresh Line")]
        private void Refresh() => UpdateLine();
    }
}