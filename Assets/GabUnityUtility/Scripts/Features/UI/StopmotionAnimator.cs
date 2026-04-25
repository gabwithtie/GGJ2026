using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GabUnity
{
    public class StopmotionAnimator : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private List<Sprite> frames;
        [SerializeField] private float fps = 12f;
        [SerializeField] private bool loop = true;
        [SerializeField] private bool playOnAwake = true;

        private Image imageComponent;
        private int currentFrame;
        private float timer;
        private bool isPlaying;

        private void Awake()
        {
            imageComponent = GetComponent<Image>();
            if (playOnAwake) isPlaying = true;
        }

        private void Update()
        {
            if (!isPlaying || frames == null || frames.Count == 0) return;

            // Increment timer by real time (ignores game pause if needed)
            timer += Time.deltaTime;

            // Calculate how long each frame should last
            float frameDuration = 1f / fps;

            if (timer >= frameDuration)
            {
                timer -= frameDuration;
                AdvanceFrame();
            }
        }

        private void AdvanceFrame()
        {
            currentFrame++;

            if (currentFrame >= frames.Count)
            {
                if (loop)
                {
                    currentFrame = 0;
                }
                else
                {
                    currentFrame = frames.Count - 1;
                    isPlaying = false;
                    return;
                }
            }

            imageComponent.sprite = frames[currentFrame];
        }

        // --- API Methods ---

        public void Play() => isPlaying = true;
        public void Pause() => isPlaying = false;
        public void Stop()
        {
            isPlaying = false;
            currentFrame = 0;
            if (frames.Count > 0) imageComponent.sprite = frames[0];
        }

        public void SetFPS(float newFPS) => fps = newFPS;
    }
}