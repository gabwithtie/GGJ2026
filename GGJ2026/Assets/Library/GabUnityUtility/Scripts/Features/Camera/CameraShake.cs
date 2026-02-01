using Unity.Cinemachine;
using UnityEngine;

namespace GabUnity
{
    public class CameraShake : MonoSingleton<CameraShake>
    {
        public void QuickShake()
        {
            Shake(0.1f, 1);
        }

        [Header("Settings")]
        [SerializeField] private bool useUnscaledTime = false;

        private CinemachineCamera vcam;
        private CinemachineCameraOffset vcamOffset; // The "Cinemachine-friendly" way to move

        private Vector3 initialLocalPos;
        private float shakeTimeRemaining;
        private float shakeMagnitude;
        private float shakeFrequency;
        private float seedX, seedY, seedZ;

        protected override void Awake()
        {
            base.Awake();
        
            initialLocalPos = transform.localPosition;
            vcam = GetComponent<CinemachineCamera>();

            if (vcam != null)
            {
                // Try to get existing offset extension, or add one
                vcamOffset = GetComponent<CinemachineCameraOffset>();
                if (vcamOffset == null)
                    vcamOffset = gameObject.AddComponent<CinemachineCameraOffset>();
            }

            seedX = Random.value * 100f;
            seedY = Random.value * 100f;
            seedZ = Random.value * 100f;
        }

        public void Shake(float duration, float magnitude, float frequency = 1.0f)
        {
            shakeTimeRemaining = duration;
            shakeMagnitude = magnitude;
            shakeFrequency = frequency;
        }

        // Use LateUpdate to ensure we run AFTER Cinemachine's internal positioning
        private void LateUpdate()
        {
            if (shakeTimeRemaining > 0)
            {
                ApplyShake();
                shakeTimeRemaining -= Time.deltaTime;
            }
            else if (shakeTimeRemaining <= 0 && shakeTimeRemaining > -1f)
            {
                ResetCamera();
            }
        }

        private void ApplyShake()
        {
            float time = useUnscaledTime ? Time.unscaledTime : Time.time;

            float x = (Mathf.PerlinNoise(seedX, time * shakeFrequency) * 2f - 1f) * shakeMagnitude;
            float y = (Mathf.PerlinNoise(seedY, time * shakeFrequency) * 2f - 1f) * shakeMagnitude;
            float z = (Mathf.PerlinNoise(seedZ, time * shakeFrequency) * 2f - 1f) * shakeMagnitude;

            Vector3 offset = new Vector3(x, y, z);

            if (vcamOffset != null)
            {
                // Move via Cinemachine Extension (This won't be overwritten)
                vcamOffset.ForceCameraPosition(initialLocalPos + offset, Quaternion.identity);
            }
            else
            {
                // Standard Camera movement
                transform.localPosition = initialLocalPos + offset;
            }
        }

        private void ResetCamera()
        {
            shakeTimeRemaining = -1.1f;
            if (vcamOffset != null)
                vcamOffset.ForceCameraPosition(initialLocalPos, Quaternion.identity);
            else
                transform.localPosition = initialLocalPos;
        }
    }
}