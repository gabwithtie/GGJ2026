using UnityEngine;

namespace GabUnity
{
    public abstract class ConsistentDataHolder<TData> : MonoBehaviour where TData : class
    {
        public static TData Value { get; set; }
    }
}