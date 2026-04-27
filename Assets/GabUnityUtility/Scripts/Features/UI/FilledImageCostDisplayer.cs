using UnityEngine;
using UnityEngine.UI;

namespace GabUnity
{
    [RequireComponent(typeof(Image))]
    public class FilledImageCostDisplayer : MonoBehaviour
    {
        private Image base_image;
        [SerializeField] private Image front_image;
        [SerializeField] private float interp_speed = 10;

        [Header("Scale Animation Settings")]
        [Tooltip("Maximum scale multiplier when the bar is moving fast.")]
        [SerializeField] private float maxScaleMultiplier = 1.2f;
        [Tooltip("How long the scale stays at its peak before returning.")]
        [SerializeField] private float stayDelay = 0.5f;
        [Tooltip("How fast the object returns to its base scale after the delay.")]
        [SerializeField] private float scaleReturnSpeed = 5f;
        [Tooltip("Sensitivity to resource changes. Higher values make it pop more with smaller changes.")]
        [SerializeField] private float changeSensitivity = 1.0f;

        private float cur_cost_display = 0;
        private float base_fill = 0;
        private float front_fill = 0;

        private Vector3 _originalScale;
        private float _lastBaseFill;
        private float _stayTimer;

        private void Awake()
        {
            base_image = GetComponent<Image>();
            _originalScale = transform.localScale;
            _lastBaseFill = base_fill;
        }

        public void SetCurrent(float value)
        {
            base_fill = value;
            front_fill = value - cur_cost_display;
        }

        public void SetCostDisplay(float cost)
        {
            cur_cost_display = cost;
        }

        private void Update()
        {
            // Existing Fill Interpolation Logic
            base_image.fillAmount = Mathf.Lerp(base_image.fillAmount, base_fill, Time.deltaTime * interp_speed);
            front_image.fillAmount = Mathf.Lerp(front_image.fillAmount, front_fill, Time.deltaTime * interp_speed);

            HandleScaleModulation();
        }

        public void ForceEnlarge()
        {
            HandleScaleModulation(100);
        }

        private void HandleScaleModulation(float overridedelta = 0)
        {
            float delta = Mathf.Abs(base_fill - _lastBaseFill) + overridedelta;
            float changeSpeed = delta / Time.deltaTime;
            float scaleIntensity = Mathf.Clamp01(changeSpeed * changeSensitivity * 0.1f);

            Vector3 targetScale = Vector3.Lerp(_originalScale, _originalScale * maxScaleMultiplier, scaleIntensity);

            if (scaleIntensity > 0.01f)
            {
                // If the change is significant, pop to the target scale and reset the stay timer
                transform.localScale = Vector3.Max(transform.localScale, targetScale);
                _stayTimer = stayDelay;
            }
            else if (_stayTimer > 0)
            {
                // Count down the stay delay while maintaining current scale
                _stayTimer -= Time.deltaTime;
            }
            else
            {
                // Smoothly return to original scale only after the timer hits zero
                transform.localScale = Vector3.Lerp(transform.localScale, _originalScale, Time.deltaTime * scaleReturnSpeed);
            }

            _lastBaseFill = base_fill;
        }
    }
}