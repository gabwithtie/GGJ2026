using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(ParticleSystem))]
    public class GlobalEffectController : MonoBehaviour
    {
        private ParticleSystem mParticleSystem;

        private void Awake()
        {
            mParticleSystem = GetComponent<ParticleSystem>();
        }

        public void Spawn(Vector3 position, Vector3 direction)
        {
            mParticleSystem.Emit(new ParticleSystem.EmitParams()
            {
                position = position
            }, 1);
        }
    }
}