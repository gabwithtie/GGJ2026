using UnityEngine;
using UnityEngine.Events;

namespace GabUnity
{
    public class InfiniteScrollerManager : MonoSingleton<InfiniteScrollerManager>
    {
        [SerializeField] private float scroll_speed;
        [SerializeField] private float _distance;
        [SerializeField] private UnityEvent<float> onChangeDistance;
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

        private void Update()
        {
            _distance += ScrollSpeed * Time.deltaTime;

            onChangeDistance.Invoke(_distance);
        }

    }
}