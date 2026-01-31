using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GabUnity;


public class SingletonAssetPreloader : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        IEnumerable<Type> types = assembly.GetTypes();

        bool predicate(Type t)
        {
            Type b = t;

            while (b.BaseType != null)
            {
                b = b.BaseType;

                if (b.IsGenericType == false)
                    continue;

                if (b.GetGenericTypeDefinition() == typeof(SingletonResource<>))
                    return true;
            }

            return false;
        }

        types = types.Where(t => t.IsGenericType == false);
        types = types.Where(predicate);

        var selectedTypes = types.ToArray();
        var requiredTypes = new Type[3];

        foreach (Type asset_type in selectedTypes)
        {
            // 1. Find the "Instance" property on the specific derived type
            PropertyInfo instanceProperty = asset_type.GetProperty(nameof(SingletonResource<ScriptableObject>.Instance),
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            if (instanceProperty != null)
            {
                // 2. Invoke the getter. Since it's a static property, the first argument is null.
                // This triggers the Resources.LoadAll logic inside SingletonResource<T>.
                instanceProperty.GetValue(null);

                Debug.Log($"Preloaded SingletonResource: {asset_type.Name}");
            }
        }
    }
}
