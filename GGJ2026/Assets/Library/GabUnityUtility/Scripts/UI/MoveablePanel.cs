using UnityEngine;

namespace GabUnity
{
    [ExecuteInEditMode]
    public class MoveablePanel : Panel
    {
        [SerializeField] private Vector2 anchoredpos_open;
        [SerializeField] private Vector2 anchoredpos_closed;
        [SerializeField] private float move_duration = 0.2f;
        [SerializeField] private int t_ease = 2;

        private RectTransform rectTransform;
        private float cur_t = 0;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        [ContextMenu("Teleport To Closed State")]
        public void TeleportToClosedState()
        {
            rectTransform.anchoredPosition = anchoredpos_closed;
        }

        [ContextMenu("Teleport To Open State")]
        public void TeleportToOpenState()
        {
            rectTransform.anchoredPosition = anchoredpos_open;
        }

        // Update is called once per frame
        void Update()
        {
            float delta_t = Time.unscaledDeltaTime / move_duration;

            if (isOpen)
                cur_t += delta_t;
            else
                cur_t -= delta_t;
            
            cur_t = Mathf.Clamp01(cur_t);

            float t_final = 1.0f - Mathf.Pow(1.0f - cur_t, t_ease);

            rectTransform.anchoredPosition = Vector2.Lerp(anchoredpos_closed, anchoredpos_open, t_final);
        }
    }
}