using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GabUnity
{

    public class PositionTweener : MonoBehaviour
    {
        [System.Serializable]
        public struct PositionTweenTarget
        {
            public enum TweenType { Constant, Linear, EaseIn, EaseOut, EaseInOut }

            public TweenType type;
            public Vector3 pos;
            public bool relative;
            public float duration;

            public UnityEvent onDone;
        }

        [SerializeField] private List<PositionTweenTarget> targets = new List<PositionTweenTarget>();
        [SerializeField] private UnityEvent onComplete;
        [SerializeField] private bool loop = false;

        private int _currentIndex = 0;
        private float _currentTime = 0;
        private Vector3 _startPos;
        private Vector3 _targetPos;
        private bool _isPlaying = false;

        [ContextMenu("Add Current Position as Target")]
        private void AddCurrentPosition()
        {
            if (targets == null) targets = new List<PositionTweenTarget>();

            PositionTweenTarget newTarget = new PositionTweenTarget
            {
                pos = transform.localPosition,
                relative = false, // Usually, you want absolute coordinates when capturing current world state
                duration = 1.0f,  // Default duration
                type = PositionTweenTarget.TweenType.Linear
            };

            targets.Add(newTarget);

            // This marks the object as "dirty" so Unity knows to save the new list item
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif

            Debug.Log($"Added {transform.localPosition} to tween targets.");
        }
        // ----------------------------

        public void Play()
        {
            if (targets == null || targets.Count == 0) return;

            _currentIndex = 0;
            _currentTime = 0;
            _isPlaying = true;
            SetupNextSegment();
        }

        private void SetupNextSegment()
        {
            _startPos = transform.localPosition;
            PositionTweenTarget current = targets[_currentIndex];
            _targetPos = current.relative ? _startPos + current.pos : current.pos;
        }

        void Update()
        {
            if (!_isPlaying || targets.Count == 0) return;

            PositionTweenTarget current = targets[_currentIndex];
            _currentTime += Time.deltaTime;

            float pct = Mathf.Clamp01(_currentTime / current.duration);
            float easedPct = ApplyEasing(pct, current.type);

            transform.localPosition = Vector3.Lerp(_startPos, _targetPos, easedPct);

            if (_currentTime >= current.duration)
            {
                targets[_currentIndex].onDone.Invoke();

                _currentIndex++;
                _currentTime = 0;

                if (_currentIndex < targets.Count)
                {
                    SetupNextSegment();
                }
                else if (loop)
                {
                    _currentIndex = 0;
                    SetupNextSegment();

                    onComplete.Invoke();
                }
                else
                {
                    _isPlaying = false;
                    onComplete.Invoke();
                }
            }
        }

        private float ApplyEasing(float t, PositionTweenTarget.TweenType type)
        {
            switch (type)
            {
                case PositionTweenTarget.TweenType.Constant: return 1;
                case PositionTweenTarget.TweenType.Linear: return t;
                case PositionTweenTarget.TweenType.EaseIn: return t * t * t;
                case PositionTweenTarget.TweenType.EaseOut: return 1 - Mathf.Pow(1 - t, 3);
                case PositionTweenTarget.TweenType.EaseInOut:
                    return t < 0.5f ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;
                default: return t;
            }
        }
    }
}