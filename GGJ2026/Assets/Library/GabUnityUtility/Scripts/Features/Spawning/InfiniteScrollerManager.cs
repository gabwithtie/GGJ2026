using UnityEngine;

namespace GabUnity
{
    public class InfiniteScrollerManager : MonoSingleton<InfiniteScrollerManager>
    {
        [SerializeField] private float scroll_speed;
        public static float ScrollSpeed { get => Instance.running ? Instance.scroll_speed : 0; }
        [SerializeField] private bool running;

        public void Stop()
        {
            running = false; 
        }

        public void Play()
        {
            running = true;
        }

    }
}