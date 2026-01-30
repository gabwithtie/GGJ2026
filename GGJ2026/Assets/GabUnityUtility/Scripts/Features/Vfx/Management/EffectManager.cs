using UnityEngine;

namespace GabUnity
{
    public class EffectManager : Manager_Base<EffectManager>
    {
        public EffectController Spawn(string effect_id, Vector3 position, UnitIdentifier spawner = null, Transform parent = null)
        {
            var newobj = Instantiate(EffectDictionary.Get(effect_id).gameObject, parent);
            var controller = newobj.GetComponent<EffectController>();

            controller.Initialize(spawner);

            newobj.transform.position = position;

            return controller;
        }
    }
}
