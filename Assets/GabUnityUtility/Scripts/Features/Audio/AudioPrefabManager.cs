using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GabUnity
{
    public class AudioPrefabManager : MonoSingleton<AudioPrefabManager>
    {
        [Header("Pool Settings")]
        [SerializeField] private AudioSource audioSourcePrefab;
        [SerializeField] private int initialPoolSize = 10;

        private Queue<AudioSource> _pool = new Queue<AudioSource>();

        protected override void Awake()
        {
            base.Awake();

            InitializePool();
        }

        private void InitializePool()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                CreateNewSource();
            }
        }

        private AudioSource CreateNewSource()
        {
            AudioSource newSource = Instantiate(audioSourcePrefab, transform);
            newSource.gameObject.SetActive(false);
            _pool.Enqueue(newSource);
            return newSource;
        }

        /// <summary>
        /// Public static function to play a sound by ID. 
        /// Perfect for objects about to be destroyed.
        /// </summary>
        /// <param name="audioId">The string ID from your AudioDictionary.</param>
        /// <param name="position">Optional world position for 3D spatial sound.</param>
        public static void PlayLingeringSound(string audioId, Vector3? position = null)
        {
            if (Instance == null)
            {
                Debug.LogWarning("GlobalSoundManager: No Instance found in scene!");
                return;
            }

            Instance.InternalPlay(audioId, position);
        }

        private void InternalPlay(string audioId, Vector3? position)
        {
            // Get from pool or expand if empty
            AudioSource source = _pool.Count > 0 ? _pool.Dequeue() : Instantiate(audioSourcePrefab, transform);

            // Set position if provided (useful for 3D sounds)
            if (position.HasValue)
            {
                source.transform.position = position.Value;
            }

            source.gameObject.SetActive(true);

            // Use your existing safe play system
            // This ensures it pulls from your AudioDictionary and uses your logic
            AudioClip clip = AudioDictionary.Get(audioId);
            if (clip != null)
            {
                source.PlayOneShotSafe(clip);

                // Start tracking the clip length to return it to the pool
                StartCoroutine(ReturnToPoolAfterFinished(source, clip.length));
            }
            else
            {
                // If clip is missing, return immediately
                ReturnToPool(source);
            }
        }

        private IEnumerator ReturnToPoolAfterFinished(AudioSource source, float duration)
        {
            // Wait for the duration of the clip
            yield return new WaitForSeconds(duration);

            ReturnToPool(source);
        }

        private void ReturnToPool(AudioSource source)
        {
            source.Stop();
            source.gameObject.SetActive(false);
            _pool.Enqueue(source);
        }
    }
}