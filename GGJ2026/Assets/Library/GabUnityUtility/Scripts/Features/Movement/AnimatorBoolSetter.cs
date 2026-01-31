using UnityEngine;

namespace GabUnity
{

    [RequireComponent(typeof(Animator))]
    public class AnimatorBoolSetter : MonoBehaviour
    {
        [SerializeField] private string label;
        [SerializeField] private bool trigger_mode;

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void SetBool(bool towhat)
        {
            if (trigger_mode)
            {
                if (towhat)
                    animator.SetTrigger(label);
                else
                    animator.ResetTrigger(label);

                return;
            }
            animator.SetBool(label, towhat);
        }
    }

}