using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GabUnity
{
    public class TimedEventTweener : MonoBehaviour
    {
        [Header("Tween Settings")]
        [SerializeField] private float startValue = 1.0f;
        [SerializeField] private float endValue = 0.0f;

        [Tooltip("How long to wait before starting to transition.")]
        [SerializeField] private float delay = 2.0f;

        [Tooltip("How long the transition process takes.")]
        [SerializeField] private float duration = 1.0f;

        [Header("Lifecycle")]
        [Tooltip("If true, the object will be destroyed once the process completes.")]
        [SerializeField] private bool destroyOnComplete = true;
        [SerializeField] private bool startOnEnable = true;

        [Header("Events")]
        [Tooltip("Fires every frame with the current interpolated value.")]
        public UnityEvent<float> OnValueChanged;

        private void OnEnable()
        {
            if (startOnEnable)
            {
                StartTween();
            }
        }

        /// <summary>
        /// Manually triggers the transition process.
        /// </summary>
        public void StartTween()
        {
            StopAllCoroutines();
            StartCoroutine(TweenRoutine());
        }

        private IEnumerator TweenRoutine()
        {
            // Initial value broadcast
            OnValueChanged?.Invoke(startValue);

            // 1. Wait for the initial delay (similar to TimedShrinker)
            if (delay > 0)
            {
                yield return new WaitForSeconds(delay);
            }

            float elapsedTime = 0;

            // 2. Interpolate value from start to end
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;

                // Calculate the interpolation factor (0 to 1)
                float t = Mathf.Clamp01(elapsedTime / duration);

                // Calculate current value and invoke the event
                float currentValue = Mathf.Lerp(startValue, endValue, t);
                OnValueChanged?.Invoke(currentValue);

                yield return null;
            }

            // Ensure the final value is exactly reached
            OnValueChanged?.Invoke(endValue);

            // 3. Cleanup
            if (destroyOnComplete)
            {
                Destroy(gameObject);
            }
        }

        private void OnDisable()
        {
            // Stop the routine if the object is disabled to prevent errors
            StopAllCoroutines();
        }
    }
}