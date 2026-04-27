using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace GabUnity
{
    public class SpriteColorTweener : MonoBehaviour
    {
        [Header("Color Settings")]
        [SerializeField] private Color colorA = Color.white;
        [SerializeField] private Color colorB = Color.red;
        [SerializeField] private float transitionDuration = 0.25f;

        [Header("Targeting")]
        [Tooltip("If true, it affects this object and all children. If false, only this object.")]
        [SerializeField] private bool includeChildren = true;
        [SerializeField] private bool manual_assignment = false;
        [SerializeField] private UnityEvent OnOn;
        [SerializeField] private UnityEvent OnOff;

        [SerializeField] private List<SpriteRenderer> _renderers = new List<SpriteRenderer>();
        private Coroutine _tweenRoutine;
        private bool _isColorB = false;

        private void Awake()
        {
            RefreshRenderers();
        }

        /// <summary>
        /// Call this to find new SpriteRenderers if you add children at runtime.
        /// </summary>
        public void RefreshRenderers()
        {
            if (manual_assignment)
                return;

            _renderers.Clear();
            if (includeChildren)
            {
                _renderers.AddRange(GetComponentsInChildren<SpriteRenderer>(true));
            }
            else if (TryGetComponent(out SpriteRenderer sr))
            {
                _renderers.Add(sr);
            }
        }

        /// <summary>
        /// Toggles between Color A and Color B.
        /// </summary>
        public void ToggleColor()
        {
            SetColorState(!_isColorB);
        }

        /// <summary>
        /// Explicitly sets the state to A (false) or B (true).
        /// </summary>
        public void SetColorState(bool toColorB)
        {
            _isColorB = toColorB;
            Color target = _isColorB ? colorB : colorA;

            if (_tweenRoutine != null) StopCoroutine(_tweenRoutine);
            _tweenRoutine = StartCoroutine(TweenColorRoutine(target));

            if(_isColorB)
                OnOn.Invoke();
            else
                OnOff.Invoke();
        }

        private IEnumerator TweenColorRoutine(Color targetColor)
        {
            float elapsed = 0;

            // We capture the starting colors for each renderer to ensure smooth 
            // interpolation even if interrupted mid-way.
            List<Color> startColors = new List<Color>();
            foreach (var sr in _renderers)
            {
                startColors.Add(sr != null ? sr.color : Color.white);
            }

            while (elapsed < transitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / transitionDuration;

                for (int i = 0; i < _renderers.Count; i++)
                {
                    if (_renderers[i] == null) continue;
                    _renderers[i].color = Color.Lerp(startColors[i], targetColor, t);
                }

                yield return null;
            }

            // Ensure final color is set exactly
            foreach (var sr in _renderers)
            {
                if (sr != null) sr.color = targetColor;
            }
        }
    }
}