using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GabUnity
{
    [RequireComponent(typeof(Animation))]
    public class AnimatedButton : Selectable, IPointerClickHandler
    {
        [SerializeField] private AnimationClip idle_anim;
        [SerializeField] private AnimationClip hover_anim;

        [SerializeField] private UnityEvent onClick;

        private Animation _animation;

        protected override void Awake()
        {
            _animation = GetComponent<Animation>();
        }

        void PlayAnim(bool hovered)
        {
            var toplay = idle_anim;

            if(hovered)
                toplay = hover_anim;

            _animation.CrossFade(toplay.name);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            PlayAnim(true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            PlayAnim(false);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            onClick.Invoke();
        }
    }
}
