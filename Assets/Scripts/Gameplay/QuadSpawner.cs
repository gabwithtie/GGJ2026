using System.Collections.Generic;
using UnityEngine;

namespace GabUnity
{
    public class QuadSpawner : MonoBehaviour
    {
        [Header("Targeting Settings")]
        [SerializeField] private string validTargetTag = "SpawnableSurface";

        [Header("Spawn Settings")]
        [SerializeField] private GameObject quadPrefab;
        [SerializeField] private GameObject previewPrefab;
        [SerializeField] private float interpolationSpeed = 15f;
        [SerializeField] private float bridgeWidth = 1.0f;

        [Header("Anchor Reset Settings")]
        [SerializeField] private Transform distanceReference;
        [SerializeField] private float maxAnchorDistance = 10f;

        private HashSet<GameObject> _usedObjects = new HashSet<GameObject>();
        private int _clickCount = 0;
        private Vector3 _firstClickPosition;
        private GameObject _firstClickObject;
        private GameObject _previewInstance;

        // Target values for smooth interpolation
        private Vector3 _targetScale;
        private Quaternion _targetRotation;

        private void Update()
        {
            // Reset the state if the reference (player) moves too far from the anchor
            if (_clickCount == 1 && distanceReference != null)
            {
                float dist = Vector3.Distance(distanceReference.position, _firstClickPosition);
                if (dist > maxAnchorDistance)
                {
                    ResetFirstAnchor();
                }
            }

            // Smoothly interpolate the preview if it exists
            if (_previewInstance != null && _previewInstance.activeSelf)
            {
                _previewInstance.transform.localScale = Vector3.Lerp(
                    _previewInstance.transform.localScale,
                    _targetScale,
                    Time.deltaTime * interpolationSpeed
                );

                _previewInstance.transform.rotation = Quaternion.Slerp(
                    _previewInstance.transform.rotation,
                    _targetRotation,
                    Time.deltaTime * interpolationSpeed
                );
            }
        }

        public void OnHoverHit(Vector3 hitPosition, GameObject hitObject)
        {
            // Show preview only if we have the first anchor set
            if (_clickCount != 1)
            {
                if (_previewInstance != null) _previewInstance.SetActive(false);
                return;
            }

            if (validTargetTag.Length > 0 && !hitObject.CompareTag(validTargetTag))
            {
                if (_previewInstance != null) _previewInstance.SetActive(false);
                return;
            }

            if (_previewInstance == null && previewPrefab != null)
            {
                _previewInstance = Instantiate(previewPrefab);
                // Snap to initial state to avoid a weird lerp from (0,0,0)
                CalculateGeometry(_firstClickPosition, hitPosition, out Vector3 s, out Quaternion r);
                _previewInstance.transform.position = _firstClickPosition;
                _previewInstance.transform.localScale = s;
                _previewInstance.transform.rotation = r;
            }

            if (_previewInstance != null)
            {
                _previewInstance.SetActive(true);
                // Update targets for the Update loop to Lerp towards
                CalculateGeometry(_firstClickPosition, hitPosition, out _targetScale, out _targetRotation);
            }
        }

        public void OnClickHit(Vector3 hitPosition, GameObject hitObject)
        {
            if (validTargetTag.Length > 0 && !hitObject.CompareTag(validTargetTag)) return;
            if (_usedObjects.Contains(hitObject)) return;

            if (_clickCount == 0)
            {
                _firstClickPosition = hitPosition;
                _firstClickObject = hitObject;
                _clickCount = 1;
            }
            else
            {
                if (quadPrefab != null)
                {
                    GameObject spawnedQuad = Instantiate(quadPrefab);
                    CalculateGeometry(_firstClickPosition, hitPosition, out Vector3 s, out Quaternion r);
                    spawnedQuad.transform.position = _firstClickPosition;
                    spawnedQuad.transform.localScale = s;
                    spawnedQuad.transform.rotation = r;
                }

                _usedObjects.Add(_firstClickObject);
                _usedObjects.Add(hitObject);
                ResetFirstAnchor();
            }
        }

        private void ResetFirstAnchor()
        {
            _clickCount = 0;
            _firstClickObject = null;
            if (_previewInstance != null)
            {
                Destroy(_previewInstance);
                _previewInstance = null;
            }
        }

        private void CalculateGeometry(Vector3 pA, Vector3 pB, out Vector3 scale, out Quaternion rotation)
        {
            Vector3 delta = pB - pA;

            // Point representing the projection on the XZ plane to find depth
            Vector3 xAyB = new Vector3(pA.x, pB.y, pB.z);

            // Width is horizontal difference, Y scale is locked at 1
            float depth = (pA - xAyB).magnitude;
            scale = new Vector3(bridgeWidth, 1f, depth);

            // Calculate X-axis tilt to bridge vertical gap
            var r_cross = Vector3.Cross(delta, Vector3.up);

            rotation = Quaternion.LookRotation(delta, Vector3.Cross(delta, r_cross));
        }
    }
}