using UnityEngine;

namespace GabUnity
{
    public class EffectController : MonoBehaviour
    {
        protected UnitIdentifier _spawner;

        public virtual void Initialize(UnitIdentifier spawner)
        {
            _spawner = spawner;
        }
    }
}
