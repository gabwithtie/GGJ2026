
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GabUnity
{
    public class ManagerManager : MonoSingleton<ManagerManager>
    {
        protected override void Awake()
        {
            InstantiateAllInheritedClassesOf(typeof(Manager_Base<>));
            InstantiateAllInheritedClassesOf(typeof(ActionRequestHandler<>));
        }

        private void InstantiateAllInheritedClassesOf(Type parentType)
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

                    if (b.GetGenericTypeDefinition() == parentType)
                        return true;
                }

                return false;
            }

            types = types.Where(t => t.IsGenericType == false);
            types = types.Where(predicate);

            var selectedTypes = types.ToArray();
            var requiredTypes = new Type[3];

            foreach (Type manager_type in selectedTypes)
            {
                requiredTypes[0] = manager_type.GetAttributeValue((RequireComponent requirements) => requirements.m_Type0);
                requiredTypes[1] = manager_type.GetAttributeValue((RequireComponent requirements) => requirements.m_Type1);
                requiredTypes[2] = manager_type.GetAttributeValue((RequireComponent requirements) => requirements.m_Type2);

                var new_gameObject = new GameObject(manager_type.Name);
                new_gameObject.transform.parent = transform;

                foreach (var requiredType in requiredTypes)
                {
                    if (requiredType == null)
                        continue;

                    new_gameObject.AddComponent(requiredType);
                }

                new_gameObject.AddComponent(manager_type);
            }
        }
    }
}