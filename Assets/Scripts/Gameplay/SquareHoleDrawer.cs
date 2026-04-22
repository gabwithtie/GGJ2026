using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(LineRenderer))]
    public class SquareHoleDrawer : MonoBehaviour
    {
        [Header("Anchor Reset Settings")]
        [SerializeField] private Transform distanceReference;
        [SerializeField] private float maxAnchorDistance = 10f;

        private LineRenderer _lineRenderer;
        private int _clickCount = 0;
        private Vector3 _firstClickPosition;
        private HoleSubdivider _activeSubdivider;

        private void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 5;
            _lineRenderer.enabled = false;
        }

        private void Update()
        {
            if (_clickCount == 1 && distanceReference != null)
            {
                float dist = Vector3.Distance(distanceReference.position, _firstClickPosition);
                if (dist > maxAnchorDistance)
                {
                    ResetDrawer();
                }
            }
        }

        public void OnHoverHit(Vector3 hitPosition, GameObject hitObject)
        {
            if (hitObject.transform.parent == null)
                return;

            if (_clickCount != 1 || _activeSubdivider == null)
            {
                _lineRenderer.enabled = false;
                return;
            }

            if (hitObject.transform.parent.GetComponent<HoleSubdivider>() != _activeSubdivider)
            {
                _lineRenderer.enabled = false;
                return;
            }

            _lineRenderer.enabled = true;
            UpdateLineRendererBox(_firstClickPosition, hitPosition);
        }

        public void OnClickHit(Vector3 hitPosition, GameObject hitObject)
        {
            if (hitObject.transform.parent == null)
                return;

            HoleSubdivider subdivider = hitObject.transform.parent.GetComponent<HoleSubdivider>();
            if (subdivider == null) return;

            if (_clickCount == 0)
            {
                _firstClickPosition = hitPosition;
                _activeSubdivider = subdivider;
                _clickCount = 1;
            }
            else
            {
                if (subdivider == _activeSubdivider)
                {
                    _activeSubdivider.CutHole(_firstClickPosition, hitPosition);
                }

                ResetDrawer();
            }
        }

        private void UpdateLineRendererBox(Vector3 pA, Vector3 pB)
        {
            Transform targetTransform = _activeSubdivider.transform;
            Vector3 localA = targetTransform.InverseTransformPoint(pA);
            Vector3 localB = targetTransform.InverseTransformPoint(pB);

            // Cut along the XY plane. We use localA.z as the constant depth 
            // so the drawn line is perfectly flush with the wall surface you clicked.
            float zDepth = localA.z;

            Vector3 c0 = new Vector3(localA.x, localA.y, zDepth);
            Vector3 c1 = new Vector3(localA.x, localB.y, zDepth);
            Vector3 c2 = new Vector3(localB.x, localB.y, zDepth);
            Vector3 c3 = new Vector3(localB.x, localA.y, zDepth);

            _lineRenderer.SetPosition(0, targetTransform.TransformPoint(c0));
            _lineRenderer.SetPosition(1, targetTransform.TransformPoint(c1));
            _lineRenderer.SetPosition(2, targetTransform.TransformPoint(c2));
            _lineRenderer.SetPosition(3, targetTransform.TransformPoint(c3));
            _lineRenderer.SetPosition(4, targetTransform.TransformPoint(c0));
        }

        private void ResetDrawer()
        {
            _clickCount = 0;
            _activeSubdivider = null;
            _lineRenderer.enabled = false;
        }
    }
}