using UnityEngine;
using System.Collections.Generic;

namespace GabUnity
{
    [RequireComponent(typeof(LineRenderer))]
    public class HolePuncher : GuidedInteractable
    {
        [Header("Hole Settings")]
        [SerializeField] private Vector2 holeSize = new Vector2(1f, 1f);
        [SerializeField] private float minSegmentSize = 0.01f;

        [Header("Effects")]
        [Tooltip("A unit cube prefab that is spawned in the empty space of the hole.")]
        [SerializeField] private GameObject destroyedPrefab;

        [Header("Filtering")]
        [Tooltip("Only children with this tag will be subdivided. Leave empty to attempt all.")]
        [SerializeField] private string segmentTag = "WallSegment";

        private LineRenderer _lineRenderer;
        private Collider[] _childColliders;

        private void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 5;
            _lineRenderer.loop = true;
            _lineRenderer.enabled = false;

            RefreshColliders();
        }

        protected override void Update()
        {
            ReportToPointer();
            base.Update();

            if (GuidedPointer.Instance.IsGuided && GuidedPointer.Instance.GuidingObject == gameObject)
            {
                _lineRenderer.enabled = true;
                UpdateLineRendererBox(GuidedPointer.WorldPosition);
            }
            else
            {
                _lineRenderer.enabled = false;
            }
        }

        private void ReportToPointer()
        {
            if (MainCamera.Cam == null) return;

            Ray ray = MainCamera.Cam.ScreenPointToRay(GuidedPointer.MouseScreenPosition);
            RaycastHit closestHit = new RaycastHit();
            float minDistance = float.MaxValue;
            bool hitAny = false;

            foreach (var col in _childColliders)
            {
                if (col != null && col.Raycast(ray, out RaycastHit hit, 100f))
                {
                    if (hit.distance < minDistance)
                    {
                        minDistance = hit.distance;
                        closestHit = hit;
                        hitAny = true;
                    }
                }
            }

            if (hitAny)
            {
                GuidedPointer.Instance.ReportSnapCandidate(closestHit.point, gameObject);
            }
        }

        public void HandlePunchHole()
        {
            Vector3 worldPos = GuidedPointer.WorldPosition;
            Vector3 localCenter = transform.InverseTransformPoint(worldPos);

            float hMinX = localCenter.x - (holeSize.x * 0.5f);
            float hMaxX = localCenter.x + (holeSize.x * 0.5f);
            float hMinY = localCenter.y - (holeSize.y * 0.5f);
            float hMaxY = localCenter.y + (holeSize.y * 0.5f);

            List<Transform> currentSegments = new List<Transform>();
            float thickness = 0.1f; // Default fallback thickness

            foreach (Transform child in transform)
            {
                if (!child.gameObject.activeInHierarchy) continue;

                if (string.IsNullOrEmpty(segmentTag) || child.CompareTag(segmentTag))
                {
                    currentSegments.Add(child);
                    // Capture the thickness of the segments being punched
                    thickness = child.localScale.z;
                }
            }

            int subdividedCount = 0;
            foreach (Transform segment in currentSegments)
            {
                if (TrySubdivide(segment, hMinX, hMaxX, hMinY, hMaxY))
                {
                    subdividedCount++;
                }
            }

            if (subdividedCount > 0)
            {
                SpawnDestroyedVisual(worldPos, thickness);
                RefreshColliders();
            }
        }

        private void SpawnDestroyedVisual(Vector3 worldPosition, float thickness)
        {
            if (destroyedPrefab == null) return;

            // Instantiate at the punch location with the wall's rotation
            GameObject go = Instantiate(destroyedPrefab, worldPosition, transform.rotation);

            // Scale the unit cube to match the hole dimensions and wall thickness
            go.transform.localScale = new Vector3(holeSize.x, holeSize.y, thickness);
        }

        private bool TrySubdivide(Transform segment, float hMinX, float hMaxX, float hMinY, float hMaxY)
        {
            Vector3 pos = segment.localPosition;
            Vector3 scale = segment.localScale;

            float sMinX = pos.x - (scale.x * 0.5f);
            float sMaxX = pos.x + (scale.x * 0.5f);
            float sMinY = pos.y - (scale.y * 0.5f);
            float sMaxY = pos.y + (scale.y * 0.5f);

            if (hMinX >= sMaxX || hMaxX <= sMinX || hMinY >= sMaxY || hMaxY <= sMinY)
                return false;

            float iMinX = Mathf.Clamp(hMinX, sMinX, sMaxX);
            float iMaxX = Mathf.Clamp(hMaxX, sMinX, sMaxX);
            float iMinY = Mathf.Clamp(hMinY, sMinY, sMaxY);
            float iMaxY = Mathf.Clamp(hMaxY, sMinY, sMaxY);

            CreateFragment(segment.gameObject, sMinX, iMinX, sMinY, sMaxY, scale.z, pos.z);
            CreateFragment(segment.gameObject, iMaxX, sMaxX, sMinY, sMaxY, scale.z, pos.z);
            CreateFragment(segment.gameObject, iMinX, iMaxX, iMaxY, sMaxY, scale.z, pos.z);
            CreateFragment(segment.gameObject, iMinX, iMaxX, sMinY, iMinY, scale.z, pos.z);

            Destroy(segment.gameObject);
            return true;
        }

        private void CreateFragment(GameObject prototype, float xMin, float xMax, float yMin, float yMax, float thickness, float zPos)
        {
            float width = xMax - xMin;
            float height = yMax - yMin;

            if (width < minSegmentSize || height < minSegmentSize) return;

            GameObject fragment = Instantiate(prototype, transform);
            fragment.transform.localPosition = new Vector3((xMin + xMax) * 0.5f, (yMin + yMax) * 0.5f, zPos);
            fragment.transform.localScale = new Vector3(width, height, thickness);
            fragment.transform.localRotation = Quaternion.identity;
        }

        public void RefreshColliders()
        {
            _childColliders = GetComponentsInChildren<Collider>();
        }

        private void UpdateLineRendererBox(Vector3 worldCenter)
        {
            Vector3 localCenter = transform.InverseTransformPoint(worldCenter);
            float hW = holeSize.x * 0.5f;
            float hH = holeSize.y * 0.5f;
            float z = localCenter.z - 0.01f;

            _lineRenderer.SetPosition(0, transform.TransformPoint(new Vector3(localCenter.x - hW, localCenter.y - hH, z)));
            _lineRenderer.SetPosition(1, transform.TransformPoint(new Vector3(localCenter.x - hW, localCenter.y + hH, z)));
            _lineRenderer.SetPosition(2, transform.TransformPoint(new Vector3(localCenter.x + hW, localCenter.y + hH, z)));
            _lineRenderer.SetPosition(3, transform.TransformPoint(new Vector3(localCenter.x + hW, localCenter.y - hH, z)));
            _lineRenderer.SetPosition(4, transform.TransformPoint(new Vector3(localCenter.x - hW, localCenter.y - hH, z)));
        }
    }
}