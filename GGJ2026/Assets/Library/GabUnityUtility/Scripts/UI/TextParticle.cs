using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GabUnity
{
    public class TextParticle : MonoSingleton<TextParticle>
    {
        // Internal struct to manage the state of active particles
        private struct ActiveText
        {
            public TextMeshPro TMP;
            public Vector3 StartPosition;
            public Color StartColor;
            public float ElapsedTime;
            public float MaxLifetime;
            public bool IsActive;
        }

        [Header("Pool Settings")]
        public int poolSize = 20;
        public GameObject textMeshProPrefab;

        [Header("Animation Settings")]
        public float hoverSpeed = 1f;

        private ActiveText[] _particles;
        private int _nextIndex = 0; // Tracks the oldest particle to recycle

        private void Start()
        {
            InitializePool();
        }

        private void InitializePool()
        {
            _particles = new ActiveText[poolSize];

            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(textMeshProPrefab, transform);
                obj.SetActive(false);

                _particles[i] = new ActiveText
                {
                    TMP = obj.GetComponent<TextMeshPro>(),
                    IsActive = false
                };
            }
        }

        static public void SpawnText(string text, Vector3 position, float fontSize, float lifetime, Color color)
        {
            // Select the next particle in the array (Circular Buffer)
            // This naturally picks the oldest one because we increment _nextIndex every time
            ref ActiveText p = ref Instance._particles[Instance._nextIndex];

            // Setup / Reset the particle
            p.TMP.gameObject.SetActive(true);
            p.TMP.text = text;
            p.TMP.fontSize *= fontSize;
            p.TMP.color = color;
            p.TMP.transform.position = position;

            p.StartPosition = position;
            p.StartColor = color;
            p.ElapsedTime = 0f;
            p.MaxLifetime = lifetime;
            p.IsActive = true;

            // Increment index and wrap around (Circular)
            Instance._nextIndex = (Instance._nextIndex + 1) % Instance.poolSize;
        }

        private void Update()
        {
            if (MainCamera.Instance == null) return;

            Quaternion camRotation = MainCamera.Instance.transform.rotation;

            for (int i = 0; i < _particles.Length; i++)
            {
                if (!_particles[i].IsActive) continue;

                // Update Lifetime
                _particles[i].ElapsedTime += Time.deltaTime;

                if (_particles[i].ElapsedTime >= _particles[i].MaxLifetime)
                {
                    _particles[i].IsActive = false;
                    _particles[i].TMP.gameObject.SetActive(false);
                    continue;
                }

                // Calculate Progress (0 to 1)
                float t = _particles[i].ElapsedTime / _particles[i].MaxLifetime;

                // 1. Hovering Up
                _particles[i].TMP.transform.position = _particles[i].StartPosition + Vector3.up * (hoverSpeed * _particles[i].ElapsedTime);

                // 2. Fading Out
                Color c = _particles[i].StartColor;
                c.a = Mathf.Lerp(1f, 0f, t);
                _particles[i].TMP.color = c;

                // 3. Billboarding
                _particles[i].TMP.transform.rotation = camRotation;
            }
        }
    }
}