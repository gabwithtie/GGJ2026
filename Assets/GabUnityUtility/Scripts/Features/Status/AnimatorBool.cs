using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorBool : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private string parameterName = "IsActive";

        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Public method to set an animator boolean. 
        /// Hook this up to UnityEvents in the Inspector.
        /// </summary>
        public void SetAnimatorBool(bool value)
        {
            if (_animator == null) return;

            _animator.SetBool(parameterName, value);
        }

        /// <summary>
        /// Simple toggle function if you need it.
        /// </summary>
        public void ToggleAnimatorBool()
        {
            if (_animator == null) return;

            bool current = _animator.GetBool(parameterName);
            _animator.SetBool(parameterName, !current);
        }
    }
}