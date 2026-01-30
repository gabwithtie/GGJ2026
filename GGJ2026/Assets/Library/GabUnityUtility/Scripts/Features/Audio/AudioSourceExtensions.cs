using UnityEngine;

namespace GabUnity
{
    public static class AudioSourceExtensions
    {
        public static void PlayOneShotSafe(this AudioSource audioSource, AudioClip clip, float volumeScale = 1.0f)
        {
            if (AudioManager.CheckPlayedRecently(clip))
            {
                return;
            }

            AudioManager.RegisterPlayedAudio(clip);
            audioSource.PlayOneShot(clip, volumeScale);
        }
    }
}