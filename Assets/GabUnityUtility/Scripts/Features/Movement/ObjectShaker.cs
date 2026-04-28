using System.Collections;
using UnityEngine;

namespace GabUnity
{
    public class ObjectShaker : MonoBehaviour
    {
        [Header("Shake Settings")]
        [Tooltip("How long the shake lasts.")]
        [SerializeField] private float duration = 0.2f;

        [Tooltip("Maximum distance the object moves from its origin.")]
        [SerializeField] private float strength = 0.1f;

        [Tooltip("Higher values make the shake more 'violent' or jittery.")]
        [SerializeField] private float vibrato = 50f;

        [Header("Options")]
        [Tooltip("If true, the shake strength diminishes over the duration.")]
        [SerializeField] private bool fadeOut = true;

        private Vector3 _initialLocalPos;
        private Coroutine _shakeRoutine;

        private void Awake()
        {
            // Store the 'home' position so we don't drift away after shaking
            _initialLocalPos = transform.localPosition;
        }

        /// <summary>
        /// Public function to trigger the shake. 
        /// Can be called by UnityEvents or other scripts.
        /// </summary>
        public void Shake()
        {
            if (_shakeRoutine != null) StopCoroutine(_shakeRoutine);
            _shakeRoutine = StartCoroutine(DoShake());
        }

        private IEnumerator DoShake()
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                // Calculate the current strength (diminishing if fadeOut is true)
                float currentStrength = fadeOut ? Mathf.Lerp(strength, 0, elapsed / duration) : strength;

                // Generate a random offset using Perlin-like jitter or Random.insideUnitSphere
                // We use vibrato to determine how often we pick a new random point
                Vector3 randomOffset = Random.insideUnitSphere * currentStrength;

                transform.localPosition = _initialLocalPos + randomOffset;

                // Wait for the next 'vibrato' step (or just the next frame)
                if (vibrato > 0)
                {
                    yield return new WaitForSeconds(1f / vibrato);
                }
                else
                {
                    yield return null;
                }
            }

            // Return precisely to the initial position
            transform.localPosition = _initialLocalPos;
            _shakeRoutine = null;
        }

        private void OnDisable()
        {
            // Ensure the object doesn't get stuck in a weird position if disabled mid-shake
            if (_shakeRoutine != null)
            {
                StopCoroutine(_shakeRoutine);
                transform.localPosition = _initialLocalPos;
            }
        }
    }
}