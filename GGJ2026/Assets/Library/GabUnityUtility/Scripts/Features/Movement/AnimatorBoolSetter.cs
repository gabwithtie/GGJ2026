using UnityEngine;

namespace GabUnity
{

    [RequireComponent(typeof(Animator))]
    public class AnimatorBoolSetter : MonoBehaviour
    {
        [SerializeField] private string label;

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void SetBool(bool towhat)
        {
            animator.SetBool(label, towhat);
        }
    }

}