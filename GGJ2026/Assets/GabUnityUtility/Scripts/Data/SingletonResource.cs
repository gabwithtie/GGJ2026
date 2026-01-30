using System.Linq;
using UnityEngine;

namespace GabUnity
{
    public abstract class SingletonResource<T> : ScriptableObject where T : ScriptableObject
    {
        static T cache_instance = null;

        public static T Instance
        {
            get
            {
                if(cache_instance != null)
                {
                    return cache_instance;
                }

                foreach (var item in Resources.LoadAll("", typeof(T)).Cast<T>())
                {
                    cache_instance = item;
                    return item;
                }

                return null;
            }
        }
    }
}