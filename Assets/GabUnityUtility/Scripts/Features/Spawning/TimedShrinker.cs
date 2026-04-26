using System.Collections;
using UnityEngine;

namespace GabUnity
{
    public class TimedShrinker : MonoBehaviour
    {
        [Header("Shrink Settings")]
        [Tooltip("How long to wait before starting to shrink.")]
        [SerializeField] private float delayBeforeShrink = 2.0f;

        [Tooltip("How long the shrinking process takes.")]
        [SerializeField] private float shrinkDuration = 1.0f;

        [Tooltip("If true, the object will be destroyed once it reaches zero scale.")]
        [SerializeField] private bool destroyOnComplete = true;
        [SerializeField] private bool startOnEnable = true;

        private Vector3 _initialScale;

        private void OnEnable()
        {
            if (startOnEnable)
            {
                _initialScale = transform.localScale;
                StartCoroutine(ShrinkRoutine());
            }
        }

        public void StartShrink()
        {
            StopAllCoroutines();

            _initialScale = transform.localScale;
            StartCoroutine(ShrinkRoutine());
        }

        private IEnumerator ShrinkRoutine()
        {
            // 1. Wait for the initial delay (let the debris fall/tumble)
            yield return new WaitForSeconds(delayBeforeShrink);

            float elapsedTime = 0;

            // 2. Interpolate scale down to zero
            while (elapsedTime < shrinkDuration)
            {
                elapsedTime += Time.deltaTime;

                // Use Lerp to smoothly transition from initial scale to Vector3.zero
                float t = elapsedTime / shrinkDuration;

                // Optional: Use t * t for an "ease-in" effect so it starts slow and speeds up
                transform.localScale = Vector3.Lerp(_initialScale, Vector3.zero, t);

                yield return null;
            }

            // Ensure it's exactly zero at the end
            transform.localScale = Vector3.zero;

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