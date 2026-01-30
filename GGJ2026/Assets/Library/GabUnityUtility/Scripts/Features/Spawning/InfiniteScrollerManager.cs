using UnityEngine;

namespace GabUnity
{
    public class InfiniteScrollerManager : MonoSingleton<InfiniteScrollerManager>
    {
        [SerializeField] private float scroll_speed;
        public static float ScrollSpeed { get => Instance.scroll_speed; }
    }
}