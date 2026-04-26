using System.Collections;
using UnityEngine;

namespace GabUnity
{
    public class TimedPopper : MonoBehaviour
    {
        [Header("Pop Settings")]
        [SerializeField] private bool startDisabled = true;
        [SerializeField] private float popDuration = 0.2f;
        [Tooltip("The scale it hits at the peak of the pop before settling (1.2 = 120%)")]
        [SerializeField] private float overshootAmount = 1.2f;

        [Header("Ease Settings")]
        [SerializeField] private AnimationCurve popCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Vector3 _targetScale;
        private Coroutine _popRoutine;

        private void Awake()
        {
            // Store the scale you set in the Inspector as the "final" goal
            _targetScale = transform.localScale;
        }

        private void Start()
        {
            if (startDisabled)
                this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            // Reset scale to zero immediately
            transform.localScale = Vector3.zero;

            if (_popRoutine != null) StopCoroutine(_popRoutine);
            _popRoutine = StartCoroutine(PopIn());
        }

        private IEnumerator PopIn()
        {
            float elapsedTime = 0;

            while (elapsedTime < popDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / popDuration;

                // Evaluate the curve to get a 0-1 value
                float curveValue = popCurve.Evaluate(t);

                // If you want a slight overshoot (bouncy feel), 
                // we lerp towards the targetScale multiplied by our overshoot
                float currentModifier = Mathf.Lerp(0, overshootAmount, curveValue);

                // Once we pass the halfway point, start settling back to 1.0
                if (t > 0.5f)
                {
                    float settleT = (t - 0.5f) * 2f;
                    currentModifier = Mathf.Lerp(overshootAmount, 1f, settleT);
                }

                transform.localScale = _targetScale * currentModifier;

                yield return null;
            }

            transform.localScale = _targetScale;
        }

        private void OnDisable()
        {
            if (_popRoutine != null) StopCoroutine(_popRoutine);
        }
    }
}