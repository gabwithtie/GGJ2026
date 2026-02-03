using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(AudioSource))]
    public class ConditionalAudioPlayer : MonoBehaviour
    {
        private AudioSource audioSource;

        [SerializeField] private string area_id;

        [SerializeField] private AudioClip defaultclip;
        [SerializeField] SerializableDictionary<string, AudioClip> audioclips;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void SetArea(string area)
        {
            this.area_id = area;
        }

        public void PlayShot()
        {
            if(!this.enabled)
                return;

            if (audioclips.ContainsKey(area_id))
                audioSource.PlayOneShotSafe(audioclips[area_id]);
            else
                audioSource.PlayOneShotSafe(defaultclip);
        }
    }
}
