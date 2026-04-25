using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(LineRenderer))]
    public class LaserRenderer : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        [SerializeField] private Transform start_cap;
        [SerializeField] private Transform end_cap;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();

            // Ensure we have exactly 2 points for a laser beam
            _lineRenderer.positionCount = 2;
        }

        /// <summary>
        /// Updates the start and end positions of the laser beam in world space.
        /// </summary>
        /// <param name="start">The origin of the laser.</param>
        /// <param name="end">The hit point or max range point of the laser.</param>
        public void SetLaserEndpoints(Vector3 start, Vector3 end)
        {
            if (start_cap != null)
                start_cap.position = start;
            if (end_cap != null)
                end_cap.position = end;

            // Standard safety check in case the method is called before Awake
            if (_lineRenderer == null) _lineRenderer = GetComponent<LineRenderer>();

            _lineRenderer.SetPosition(0, start);
            _lineRenderer.SetPosition(1, end);
        }

        /// <summary>
        /// Call this to hide the laser when nothing is being hit or the laser is "off".
        /// </summary>
        public void SetVisible(bool visible)
        {
            if (_lineRenderer == null) _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.enabled = visible;
        }
    }
}