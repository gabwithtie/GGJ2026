using System.Linq;
using UnityEngine;

namespace GabUnity
{
    public class DictionaryAsset<T> : ScriptableObject where T : class
    {
        [SerializeField] private T fallback;
        [SerializeField] private SerializableDictionary<string, T> dictionary = new("");
        public int Count => dictionary.Count;

        public bool ContainsKey(string id, out T value)
        {
            value = Get(id);
            return dictionary.ContainsKey(id);
        }

        public T Get(string id)
        {
            if (dictionary.ContainsKey(id))
                return dictionary[id];

            return fallback;
        }

        public T Get(int index)
        {
            if (index < 0)
                return fallback;

            if (index < dictionary.Count)
                return dictionary.ElementAt(index).Value;

            return fallback;
        }
    }
}