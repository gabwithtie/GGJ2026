using UnityEngine;

namespace GabUnity
{
    public class LingeringSoundTrigger : MonoBehaviour
    {
        [Header("Audio Settings")]
        [Tooltip("The string ID from your AudioDictionary.")]
        [SerializeField] private string audioId;

        [Header("Trigger Settings")]
        [Tooltip("If true, sound plays OnDisable. If false, it plays OnDestroy.")]
        [SerializeField] private bool playOnDisable = false;

        [Tooltip("Should the sound play at the object's current position? (For 3D audio)")]
        [SerializeField] private bool playAtPosition = true;

        private void OnDisable()
        {
            // We check _isQuitting to prevent sounds from triggering when the 
            // application is closing or the editor stops playing.
            if (playOnDisable)
            {
                TriggerSound();
            }
        }

        private void OnDestroy()
        {
            if (playOnDisable)
            {
                TriggerSound();
            }
        }

        public void TriggerSound()
        {
            if (string.IsNullOrEmpty(audioId)) return;

            Vector3? position = playAtPosition ? transform.position : (Vector3?)null;
            AudioPrefabManager.PlayLingeringSound(audioId, position);
        }
    }
}