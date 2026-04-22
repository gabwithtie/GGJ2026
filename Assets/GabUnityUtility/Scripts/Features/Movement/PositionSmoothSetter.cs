using System.Collections.Generic;
using UnityEngine;
using static GabUnity.PositionTweener;
using static UnityEngine.GraphicsBuffer;

namespace GabUnity
{
    public class PositionSmoothSetter : MonoBehaviour
    {
        [SerializeField] private Vector3 pos_a;
        [SerializeField] private Vector3 rot_a;
        [SerializeField] private Vector3 pos_b;
        [SerializeField] private Vector3 rot_b;
        [SerializeField] private float duration;
        [SerializeField] private float target_t = 0;
        [SerializeField] private InterpolationType interpolationType;

        enum InterpolationType
        {
            linear,
            ease
        }

        private float _t = 0;

        [ContextMenu("Set Current As A")]
        private void SetCurrentAsA()
        {
            pos_a = transform.localPosition;
            rot_a = transform.localEulerAngles;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        [ContextMenu("Set Current As B")]
        private void SetCurrentAsB()
        {
            pos_b = transform.localPosition;
            rot_b = transform.localEulerAngles;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        [ContextMenu("Go to A")]
        private void GoToA()
        {
            transform.localPosition = pos_a;
            transform.localEulerAngles = rot_a;
        }

        [ContextMenu("Go to B")]
        private void GoToB()
        {
            transform.localPosition = pos_b;
            transform.localEulerAngles = rot_b;
        }

        public void SetTargetT(float t)
        {
            target_t = t;
        }

        private void Update()
        {
            if(Mathf.Approximately(_t, target_t))
                return;

            var delta = Time.deltaTime / duration;
            if(target_t < _t)
                delta = -delta;

            _t += delta;
            _t = Mathf.Clamp01(_t);

            var final_t = _t;

            switch (interpolationType)
            {
                case InterpolationType.ease:
                    final_t = EaseInOut(_t);
                    break;
                default:
                    break;
            }

            transform.localPosition = Vector3.Lerp(pos_a, pos_b, final_t);
            transform.localRotation = Quaternion.Slerp(Quaternion.Euler(rot_a), Quaternion.Euler(rot_b), final_t);
        }

        public static float EaseInQuad(float t)
        {
            return t * t;
        }

        public static float EaseOutQuad(float t)
        {
            return t * (2f - t);
        }

        public static float EaseInOut(float t)
        {
            if (t < 0.5f)
            {
                return EaseInQuad(t * 2f) * 0.5f;
            }
            else
            {
                return (EaseOutQuad((t - 0.5f) * 2f) * 0.5f) + 0.5f;
            }
        }
    }
}
