using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace GabUnity
{
    [RequireComponent(typeof(Animation))]
    public class StatusAnimation : MonoBehaviour
    {
        [SerializeField] private StatusHolder statusHolder;
        private Animation _animation;

        [System.Serializable]
        public struct StatusAnimationAssignment
        {
            public StatusInfo statusInfo;
            public AnimationClip anim;
        }

        [SerializeField] private AnimationClip default_anim;
        [SerializeField] private List<StatusAnimationAssignment> statusAnimationAssignments;

        private int currentAnimIndex = -1;

        private void Awake()
        {
            _animation = GetComponent<Animation>();
        }

        void PlayAnim(int index)
        {
            var toplay = default_anim;

            if (index >= 0)
                toplay = statusAnimationAssignments[index].anim;

            currentAnimIndex = index;
            _animation.CrossFade(toplay.name);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            statusHolder.RegisterAddAction((status) =>
            {
                for (int i = 0; i < statusAnimationAssignments.Count; i++)
                {
                    var assignment = statusAnimationAssignments[i];

                    if (assignment.statusInfo != status)
                        continue;

                    if(i <= currentAnimIndex)
                        return;

                    PlayAnim(i);
                    break;
                }
            });

            statusHolder.RegisterRemoveAction((status) =>
            {
                int indextoplay = -1;

                for (int i = 0; i < statusAnimationAssignments.Count; i++)
                {
                    var assignment = statusAnimationAssignments[i];

                    if (statusHolder.HasStatus(assignment.statusInfo))
                        indextoplay = i;
                }

                PlayAnim(indextoplay);
            });

            PlayAnim(-1);
        }
    }
}