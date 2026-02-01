using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(AudioSource))]
    public class SafeShotPlayer : MonoBehaviour
    {
        private AudioSource m_AudioSource;

        private void Awake()
        {
            m_AudioSource = GetComponent<AudioSource>();
        }

        public void SafeShot(AudioClip clip)
        {
            m_AudioSource.PlayOneShotSafe(clip);
        }
    }
}
