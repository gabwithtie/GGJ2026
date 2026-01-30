using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(HealthObject), typeof(AudioSource))]
    public class PlaySoundOnHit : MonoBehaviour
    {
        private HealthObject mHealthObject;
        private AudioSource mAudioSource;

        [SerializeField] private string audio_hit_id;
        [SerializeField] private float volume_scale = 1.0f;

        private void Awake()
        {
            mHealthObject = GetComponent<HealthObject>();
            mAudioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            mHealthObject.SubscribeOnDamage((damage, other) =>
            {
                mAudioSource.PlayOneShotSafe(AudioDictionary.Get(audio_hit_id), volume_scale);
            });
        }
    }
}