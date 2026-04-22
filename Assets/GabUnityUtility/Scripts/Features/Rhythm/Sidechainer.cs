using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GabUnity
{
    public class Sidechainer : MonoBehaviour
    {
        [Serializable]
        public struct SidechainEvent
        {
            [Tooltip("Label for organization in the inspector")]
            public string label;

            [Header("Output Mapping")]
            public UnityEvent<float> onValueChanged;
            public float min;
            public float max;
            public float normalized_offset;
            public int beat_interval;

            [Header("Timing (Normalized 0-1)")]
            [Range(0, 1)] public float easeInDuration;
            [Range(0, 1)] public float easeOutDuration;

            // Internal calculation
            public void Evaluate(float t)
            {
                float normalizedValue = 0;
                t += normalized_offset;
                t %= 1f;

                // 1. Ease In Phase (Rising edge)
                if (t < easeInDuration && easeInDuration > 0)
                {
                    float localT = t / easeInDuration;
                    normalizedValue = localT * localT; // Quadratic Ease In
                }
                // 2. Ease Out Phase (Falling edge)
                else if (t < easeInDuration + easeOutDuration)
                {
                    float localT = (t - easeInDuration) / easeOutDuration;
                    normalizedValue = 1f - (localT * localT); // Quadratic Ease Out
                }
                // 3. Resting Phase
                else
                {
                    normalizedValue = 0;
                }

                // Map 0-1 to Min-Max and invoke
                float finalValue = Mathf.Lerp(min, max, normalizedValue);
                onValueChanged?.Invoke(finalValue);
            }
        }

        [SerializeField] private List<SidechainEvent> sidechainEvents;

        private void Update()
        {
            for (int i = 0; i < sidechainEvents.Count; i++)
            {
                float t = Mathf.Repeat(RhythmManager.TimeInBeatNormalized(sidechainEvents[i].beat_interval), 1f);
                sidechainEvents[i].Evaluate(t);
            }
        }
    }
}