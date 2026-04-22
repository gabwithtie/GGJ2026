using UnityEngine;

namespace GabUnity
{
    public class GuidedPointerUI : MonoBehaviour
    {
        public RectTransform Reticle;
        public Vector2 Offset;

        private Camera _cam;

        private void Start()
        {
            _cam = Camera.main;
            if (Reticle == null) Reticle = GetComponent<RectTransform>();
        }

        private void LateUpdate()
        {
            if (GuidedPointer.Instance == null) return;

            // Convert the Guided World Position to Screen Space
            Vector3 screenPos = _cam.WorldToScreenPoint(GuidedPointer.WorldPosition);

            // If the point is behind the camera, hide it or ignore it
            if (screenPos.z < 0)
            {
                return;
            }

            Reticle.position = (Vector2)screenPos + Offset;
        }
    }
}