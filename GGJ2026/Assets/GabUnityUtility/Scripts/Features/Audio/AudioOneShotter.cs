using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

namespace GabUnity
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioOneShotter : MonoBehaviour
    {
        private AudioSource audioSource;
        [SerializeField] private List<AudioClip> clips;
        [SerializeField] private bool auto_increment;

        private int _counter;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void SetIndex(int index)
        {
            _counter = index;
        }

        public void Play()
        {
            _counter %= clips.Count;

            audioSource.PlayOneShotSafe(clips[_counter]);

            if (auto_increment)
                _counter++;
        }
    }
}
