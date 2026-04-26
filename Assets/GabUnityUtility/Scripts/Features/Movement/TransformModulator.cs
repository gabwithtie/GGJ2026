using UnityEngine;

namespace GabUnity
{
    public class TransformModulator : MonoBehaviour
    {
        [Header("General Settings")]
        [SerializeField] private float frequency = 2.0f;
        [SerializeField] private bool randomizeOffset = true;
        [SerializeField] private bool useUnscaledTime = false;

        [Header("Position Modulation")]
        [SerializeField] private bool modulatePosition = false;
        [SerializeField] private Vector3 positionAmplitude = new Vector3(0f, 0.5f, 0f);

        [Header("Rotation Modulation")]
        [SerializeField] private bool modulateRotation = false;
        [SerializeField] private Vector3 rotationAmplitude = new Vector3(0f, 15f, 0f);

        [Header("Scale Modulation")]
        [SerializeField] private bool modulateScale = false;
        [SerializeField] private float scaleBase = 1.0f;
        [SerializeField] private float scaleAmplitude = 0.1f;
        [Tooltip("If true, scales all axes equally using scaleAmplitude. If false, uses scaleVector.")]
        [SerializeField] private bool uniformScale = true;
        [SerializeField] private Vector3 scaleVectorAmplitude = Vector3.zero;

        private Vector3 _initialPosition;
        private Quaternion _initialRotation;
        private Vector3 _initialScale;
        private float _timeOffset;

        private void Start()
        {
            // Store initial states so we modulate AROUND the starting point
            _initialPosition = transform.localPosition;
            _initialRotation = transform.localRotation;
            _initialScale = transform.localScale;

            if (randomizeOffset)
            {
                _timeOffset = Random.Range(0f, 2f * Mathf.PI);
            }
        }

        private void Update()
        {
            float time = useUnscaledTime ? Time.unscaledTime : Time.time;
            float sine = Mathf.Sin((time * frequency) + _timeOffset);

            // 1. Position
            if (modulatePosition)
            {
                transform.localPosition = _initialPosition + (positionAmplitude * sine);
            }

            // 2. Rotation
            if (modulateRotation)
            {
                // We use Euler angles for the amplitude to make it intuitive in the inspector
                transform.localRotation = _initialRotation * Quaternion.Euler(rotationAmplitude * sine);
            }

            // 3. Scale
            if (modulateScale)
            {
                if (uniformScale)
                {
                    float s = scaleBase + (sine * scaleAmplitude);
                    transform.localScale = new Vector3(s, s, s);
                }
                else
                {
                    transform.localScale = _initialScale + (scaleVectorAmplitude * sine);
                }
            }
        }
    }
}