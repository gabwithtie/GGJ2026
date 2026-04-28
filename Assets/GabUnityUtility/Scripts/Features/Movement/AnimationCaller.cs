using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(Animation))]
    public class AnimationCaller : MonoBehaviour
    {
        private Animation _anim;
        private string clipName;

        private void Awake()
        {
            _anim = GetComponent<Animation>();

            // If no clip name is provided, try to use the default clip
            if (string.IsNullOrEmpty(clipName) && _anim.clip != null)
            {
                clipName = _anim.clip.name;
            }
        }

        /// <summary>
        /// Plays the animation forward normally.
        /// </summary>
        public void PlayForward()
        {
            AnimationState state = _anim[clipName];
            state.speed = 1;
            state.time = 0;
            _anim.Play(clipName);
        }

        /// <summary>
        /// Plays the animation backwards. 
        /// Perfect for "Fading Out" using a "Fade In" clip.
        /// </summary>
        public void PlayReverse()
        {
            AnimationState state = _anim[clipName];

            // Set speed to negative to play backwards
            state.speed = -1;

            // Set the starting point to the very end of the clip
            state.time = state.length;

            _anim.Play(clipName);
        }
    }
}