using System.Collections;
using UnityEngine;

namespace GabUnity
{
    public class PositionPopper : MonoBehaviour
    {
        [Header("Pop Settings")]
        [Tooltip("The starting offset (e.g., 0, -1, 0 makes it pop up from below).")]
        [SerializeField] private Vector3 startOffset = new Vector3(0, -1f, 0);
        [SerializeField] private float popDuration = 0.3f;

        [Header("Animation")]
        [Tooltip("Use this to customize the movement feel (e.g., an Overshoot curve for a bouncy landing).")]
        [SerializeField] private AnimationCurve popCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Vector3 _targetLocalPos;
        private Coroutine _popRoutine;

        private bool _initialized = false;

        private void Init()
        {
            if (_initialized) return;

            _initialized = true;
            _targetLocalPos = transform.localPosition;

            transform.localPosition = _targetLocalPos + startOffset;
        }

        private void Awake()
        {
            Init();
        }

        private void OnEnable()
        {
            Init();

            // Reset to the offset position immediately
            transform.localPosition = _targetLocalPos + startOffset;

            if (_popRoutine != null) StopCoroutine(_popRoutine);
            _popRoutine = StartCoroutine(PopRoutine());
        }

        private IEnumerator PopRoutine()
        {
            float elapsedTime = 0;
            Vector3 startPos = _targetLocalPos + startOffset;

            while (elapsedTime < popDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / popDuration;

                // Evaluate the curve to get our interpolation value
                float curveValue = popCurve.Evaluate(t);

                // Move from the offset position back to the target home position
                transform.localPosition = Vector3.Lerp(startPos, _targetLocalPos, curveValue);

                yield return null;
            }

            // Ensure we land exactly at home
            transform.localPosition = _targetLocalPos;
            _popRoutine = null;
        }

        private void OnDisable()
        {
            if (_popRoutine != null) StopCoroutine(_popRoutine);
        }
    }
}