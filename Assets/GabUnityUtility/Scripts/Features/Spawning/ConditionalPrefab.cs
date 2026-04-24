using UnityEngine;

namespace GabUnity
{
    public class ConditionalPrefab : MonoBehaviour
    {
        [Header("Child References")]
        [Tooltip("The main object that will be enabled permanently upon activation.")]
        [SerializeField] private GameObject mainObject;
        [Tooltip("The preview object to show before activation.")]
        [SerializeField] private GameObject previewObject;

        [Header("Bounds & Placement (OBB)")]
        [Tooltip("Center offset of the bounds relative to this transform's local space.")]
        [SerializeField] private Vector3 boundsCenter = Vector3.zero;
        [Tooltip("Total size of the bounding box.")]
        [SerializeField] private Vector3 boundsSize = Vector3.one;
        [Tooltip("Layers that will block this prefab from being interacted with/placed.")]
        [SerializeField] private LayerMask blockingLayers;

        [Header("Animation Settings")]
        [Tooltip("How fast the main object scales up to its original size when enabled.")]
        [SerializeField] private float scaleSpeed = 15f;

        // State tracking
        private bool _isMainActivated = false;
        private bool _isScaling = false;
        private Vector3 _targetScale;

        private void Start()
        {
            // Safely initialize the main object
            if (mainObject != null)
            {
                _targetScale = mainObject.transform.localScale;
                mainObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning($"ConditionalPrefab on {gameObject.name} is missing a Main Object reference.");
            }

            // Safely initialize the preview object
            if (previewObject != null)
            {
                previewObject.SetActive(false);
            }
        }

        private void Update()
        {
            // Handle the scale interpolation
            if (_isScaling && mainObject != null)
            {
                mainObject.transform.localScale = Vector3.Lerp(
                    mainObject.transform.localScale,
                    _targetScale,
                    Time.deltaTime * scaleSpeed
                );

                // Snap to target and stop calculating once it's visually close enough
                if (Vector3.Distance(mainObject.transform.localScale, _targetScale) < 0.01f)
                {
                    mainObject.transform.localScale = _targetScale;
                    _isScaling = false;
                }
            }
        }

        /// <summary>
        /// Checks if there is any collider overlapping the Oriented Bounding Box.
        /// </summary>
        public bool IsBlocked()
        {
            // Convert the local center to world space
            Vector3 worldCenter = transform.TransformPoint(boundsCenter);

            // CheckBox uses half-extents
            Vector3 halfExtents = boundsSize * 0.5f;

            // Check if any colliders on the blocking layers overlap this OBB
            // We ignore triggers so things like sight-ranges don't block placement
            return Physics.CheckBox(worldCenter, halfExtents, transform.rotation, blockingLayers, QueryTriggerInteraction.Ignore);
        }

        /// <summary>
        /// Toggles the preview object on or off. 
        /// Will be ignored if the main prefab has already been activated.
        /// </summary>
        public void EnablePreview(bool enable)
        {
            // Lock out preview changes if the main object is already alive
            if (_isMainActivated) return;

            // Optional: You could also force the preview off if IsBlocked() is true, 
            // but usually it's good to show the preview so the player knows where they are aiming.
            if (previewObject != null)
            {
                previewObject.SetActive(enable);
            }
        }

        /// <summary>
        /// Activates the main prefab, starts the scale animation, 
        /// and permanently disables the preview.
        /// </summary>
        public void EnableMainPrefab()
        {
            // Prevent double-activation
            if (_isMainActivated) return;

            // Prevent interaction if the bounds are blocked by an obstacle
            if (IsBlocked())
            {
                Debug.Log($"Cannot activate {gameObject.name}. The placement area is blocked.");
                return;
            }

            _isMainActivated = true;

            // Turn off preview immediately
            if (previewObject != null)
            {
                previewObject.SetActive(false);
            }

            // Turn on main object and start the scale up effect
            if (mainObject != null)
            {
                mainObject.transform.localScale = Vector3.zero;
                mainObject.SetActive(true);
                _isScaling = true;
            }
        }

        private void OnDrawGizmos()
        {
            // Visual feedback: Draw Red if blocked, Cyan if clear
            bool isBlocked = IsBlocked();
            Gizmos.color = isBlocked ? new Color(1, 0, 0, 0.5f) : new Color(0, 1, 1, 0.5f);

            // Set the Gizmo matrix to match the transform so the box rotates with the object (OBB)
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

            // Draw the box using local coordinates
            Gizmos.DrawWireCube(boundsCenter, boundsSize);

            // Draw a slightly transparent solid cube for better visibility
            Gizmos.color = isBlocked ? new Color(1, 0, 0, 0.2f) : new Color(0, 1, 1, 0.2f);
            Gizmos.DrawCube(boundsCenter, boundsSize);
        }
    }
}