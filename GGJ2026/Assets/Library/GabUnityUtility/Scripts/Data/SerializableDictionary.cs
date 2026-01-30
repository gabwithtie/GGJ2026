using System;
using System.Collections.Generic;
using UnityEngine;

namespace GabUnity
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [Serializable]
        private class KeyValueClass
        {
            public TKey Key;
            public TValue Value;
        }

        [SerializeField]
        private List<KeyValueClass> structs = new();
        private readonly TKey default_key;
        private readonly Func<TValue> default_creater;

        public TValue GetOrCreate(TKey key)
        {
            if (this.ContainsKey(key) == false)
            {
                if (this.default_creater != null)
                    this.Add(key, default_creater.Invoke());
                else
                    this.Add(key, default);
            }
            return this[key];
        }


        public SerializableDictionary(TKey default_key, Func<TValue> default_creater = null)
        {
            this.default_key = default_key;
            this.default_creater = default_creater;
        }

        public void OnBeforeSerialize()
        {
            if (structs == null)
                return;

            structs.Clear();

            foreach (var kvp in this)
            {
                structs.Add(new KeyValueClass() { Key = kvp.Key, Value = kvp.Value });
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();

            foreach (var item in structs)
            {
                if (ContainsKey(item.Key))
                {
                    Add(default_key, item.Value);
                }
                else
                {
                    Add(item.Key, item.Value);
                }
            }
        }
    }
}