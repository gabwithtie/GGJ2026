using System.Collections.Generic;
using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioOneShotter : MonoBehaviour
    {
        private AudioSource audioSource;
        [SerializeField] private List<AudioClip> clips;
        [SerializeField] private bool auto_increment;
        [SerializeField] private bool loop;
        [SerializeField] private bool playOnAwake;
        [SerializeField] private float loop_interval;

        private bool _playing = false;
        private float _timeLastPlayed = 0;

        private int _counter;


        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();

            if (playOnAwake)
                _playing = true;
        }

        public void SetIndex(int index)
        {
            _counter = index;
        }

        private void Update()
        {
            if (loop && _playing)
            {
                _timeLastPlayed += Time.deltaTime;
                if (_timeLastPlayed > loop_interval)
                {
                    PlayOne();
                    _timeLastPlayed = 0;
                }
            }
        }

        private void PlayOne()
        {
            _counter %= clips.Count;

            audioSource.PlayOneShotSafe(clips[_counter]);

            if (auto_increment)
                _counter++;
        }

        public void Play()
        {
            if (loop)
                _playing = true;
            else
                PlayOne();
        }

        public void Stop()
        {
            _playing = false;
        }
    }
}
