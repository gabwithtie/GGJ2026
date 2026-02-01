using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GabUnity
{
    public class ComicScroller : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("References")]
        [SerializeField] private ScrollRect scrollRect;

        [Header("Settings")]
        [SerializeField] private float scrollSpeed = 0.05f;
        [SerializeField] private bool pauseOnHover = true;
        [SerializeField] private bool loop = true;

        private bool isPaused = false;

        private void Start()
        {
            if (scrollRect == null)
                scrollRect = GetComponent<ScrollRect>();
        }

        private void Update()
        {
            if (isPaused || scrollRect == null) return;

            // Calculate the new scroll position
            float newPos = scrollRect.horizontalNormalizedPosition + (scrollSpeed * Time.deltaTime);

            if (loop)
            {
                // Wrap around from 1.0 back to 0.0
                scrollRect.horizontalNormalizedPosition = Mathf.Repeat(newPos, 1f);
            }
            else
            {
                // Clamp and stop at the end
                scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(newPos);
            }
        }

        // Pointer interfaces to stop scrolling when the user wants to read a specific panel
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (pauseOnHover) isPaused = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (pauseOnHover) isPaused = false;
        }
    }
}