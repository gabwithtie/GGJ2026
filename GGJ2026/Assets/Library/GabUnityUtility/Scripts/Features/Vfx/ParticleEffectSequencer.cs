using System;
using UnityEngine;


namespace GabUnity
{
    public class ParticleEffectSequencer : MonoBehaviour
    {
        [Serializable]
        struct EffectEntry
        {
            public ParticleSystem effect;
            public float delay;
        }

        [SerializeField] private EffectEntry[] effectsSequence;

        private bool _isplaying = false;
        private float _timecurrent = 0f;
        private int _index = 0;

        public void Play()
        {
            _isplaying = true;
            _index = 0;
        }

        public void Stop()
        {
            _isplaying = false;
        }

        private void Update()
        {
            if(!_isplaying)
                return;

            var cureffect = effectsSequence[_index];
            _timecurrent += Time.deltaTime;

            if (_timecurrent >= cureffect.delay)
            {
                cureffect.effect.Play();
                _timecurrent = 0f;
                _index++;
                if (_index >= effectsSequence.Length)
                {
                    _isplaying = false;
                }
            }
        }
    }
}
