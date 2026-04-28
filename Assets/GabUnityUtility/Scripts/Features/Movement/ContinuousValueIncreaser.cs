using UnityEngine;
using UnityEngine.Events;

namespace GabUnity
{
    public class ContinuousValueIncreaser : MonoBehaviour
    {
        [Header("Logic Settings")]
        [SerializeField] private float baseValue = 0f;
        [SerializeField] private float maxValue = 0f;
        [SerializeField] private float increasePerSecond = 10f;

        [Header("State")]
        [SerializeField] private bool isRunning = true;

        [Header("Events")]
        [Tooltip("Fires every frame with the updated value.")]
        [SerializeField] private UnityEvent<float> OnValueChanged;

        private float _currentValue;

        private void OnEnable()
        {
            // Reset to base value when enabled
            _currentValue = baseValue;
            OnValueChanged?.Invoke(_currentValue);
        }

        private void Update()
        {
            if (!isRunning) return;

            // Increment based on time passed since last frame
            _currentValue += increasePerSecond * Time.deltaTime;

            if(_currentValue > maxValue)
                _currentValue = maxValue;

            // Push the value to any listeners
            OnValueChanged?.Invoke(_currentValue);
        }

        /// <summary>
        /// Resets the counter to the base value.
        /// </summary>
        public void ResetValue()
        {
            _currentValue = baseValue;
            OnValueChanged?.Invoke(_currentValue);
        }

        /// <summary>
        /// Public toggle to pause/resume the increase.
        /// </summary>
        public void SetRunning(bool run)
        {
            isRunning = run;
        }

        /// <summary>
        /// Manually adjust the base value if needed at runtime.
        /// </summary>
        public void SetBaseValue(float newValue)
        {
            baseValue = newValue;
        }
    }
}