using UnityEngine;

namespace GabUnity
{
    public interface ISingleton<T> where T : MonoBehaviour
    {
        public static T Instance { get; protected set; }

        protected virtual void Awake()
        {
            Instance = this as T;
        }
    }

    public class MonoSingleton<T> : MonoBehaviour, ISingleton<T> where T : MonoBehaviour
    {
        public static T Instance { get {
                var inst = ISingleton<T>.Instance;

                if (inst == null)
                {
                    ISingleton<T>.Instance = FindAnyObjectByType<T>();
                }

                return ISingleton<T>.Instance;
            }
            private set => ISingleton<T>.Instance = value; }

        protected virtual void Awake()
        {
            Instance = this as T;
        }
    }
}