using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace GabUnity
{
    public class SingletonDictionaryAsset<T> : SingletonResource<SingletonDictionaryAsset<T>> where T : class
    {
        [SerializeField] private T fallback;
        [SerializeField] private SerializableDictionary<string, T> main_dictionary = new("");
        [SerializeField] private List<DictionaryAsset<T>> sub_dictionaries;

        public static bool ContainsKey(string id, out T value)
        {
            if (Instance.main_dictionary.ContainsKey(id))
            {
                value = Instance.main_dictionary[id];
                return true;
            }

            foreach (var dict in Instance.sub_dictionaries)
            {
                if (dict.ContainsKey(id, out value))
                    return true;
            }
            value = Instance.fallback;
            return false;
        }

        public static T Get(string id)
        {
            if (Instance.main_dictionary.ContainsKey(id))
            {
                return Instance.main_dictionary[id];
            }

            foreach (var dict in Instance.sub_dictionaries)
            {
                if (dict.ContainsKey(id, out T value))
                    return value;
            }

            return Instance.fallback;
        }
    }
}