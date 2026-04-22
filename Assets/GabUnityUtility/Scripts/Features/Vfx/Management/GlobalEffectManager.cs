using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GabUnity
{
    public class GlobalEffectManager : Manager_Base<GlobalEffectManager>
    {
        private readonly Dictionary<string, GlobalEffectController> globaleffects = new();

        public static void Spawn(string id, Vector3 position, Vector3 direction)
        {

            if (Instance.globaleffects.ContainsKey(id) == false) {
                var neweffect = GlobalEffectDictionary.Get(id);

                if (neweffect == null)
                    return;

                Instance.globaleffects.Add(id, Instantiate(neweffect.gameObject, Instance.transform).GetComponent<GlobalEffectController>());
            }

            Instance.globaleffects[id].Spawn(position, direction);
        }
    }
}
