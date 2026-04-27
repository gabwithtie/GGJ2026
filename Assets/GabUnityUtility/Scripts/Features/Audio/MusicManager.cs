using UnityEngine;
using System.Collections;

namespace GabUnity
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance { get; private set; }

        [Header("Startup Settings")]
        [Tooltip("The ID of the track to play as soon as the game starts.")]
        [SerializeField] private string defaultTrackId;
        [SerializeField] private bool playDefaultOnAwake = true;

        [Header("Fade Settings")]
        [SerializeField] private float fadeDuration = 1.5f;
        [SerializeField] private float targetVolume = 0.5f;

        private AudioSource _audioSource;
        private Coroutine _fadeRoutine;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                _audioSource = GetComponent<AudioSource>();
                _audioSource.loop = true;
                _audioSource.volume = targetVolume;
                _audioSource.playOnAwake = false;
            }
            else
            {
                // Destroy the duplicate immediately so it doesn't run Start()
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            // Only the persistent Instance will reach this point
            if (playDefaultOnAwake && !string.IsNullOrEmpty(defaultTrackId))
            {
                PlayTrack(defaultTrackId, false); // No fade on the very first start
            }
        }

        /// <summary>
        /// Public function to switch tracks by ID.
        /// </summary>
        public void PlayTrack(string trackId, bool fade = true)
        {
            AudioClip nextTrack = AudioDictionary.Get(trackId);

            if (nextTrack == null)
            {
                Debug.LogWarning($"MusicManager: Track '{trackId}' not found in dictionary.");
                return;
            }

            if (_audioSource.clip == nextTrack && _audioSource.isPlaying) return;

            if (fade && _audioSource.isPlaying)
            {
                if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
                _fadeRoutine = StartCoroutine(FadeToTrack(nextTrack));
            }
            else
            {
                _audioSource.clip = nextTrack;
                _audioSource.volume = targetVolume;
                _audioSource.Play();
            }
        }

        private IEnumerator FadeToTrack(AudioClip newClip)
        {
            float startVolume = _audioSource.volume;

            // Fade Out
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                _audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
                yield return null;
            }

            _audioSource.volume = 0;
            _audioSource.clip = newClip;
            _audioSource.Play();

            // Fade In
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                _audioSource.volume = Mathf.Lerp(0, targetVolume, t / fadeDuration);
                yield return null;
            }

            _audioSource.volume = targetVolume;
            _fadeRoutine = null;
        }

        public void StopMusic(bool fade = true)
        {
            if (fade)
            {
                if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
                _fadeRoutine = StartCoroutine(FadeOutAndStop());
            }
            else
            {
                _audioSource.Stop();
            }
        }

        private IEnumerator FadeOutAndStop()
        {
            float startVolume = _audioSource.volume;
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                _audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
                yield return null;
            }
            _audioSource.Stop();
            _fadeRoutine = null;
        }
    }
}