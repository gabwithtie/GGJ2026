using UnityEngine;

namespace GabUnity
{
    public class GlobalEffectSpawner : MonoBehaviour
    {
        [SerializeField] private string effect_id;
        [SerializeField] private Vector3 default_direction;


        public void Spawn()
        {
            GlobalEffectManager.Spawn(effect_id, this.transform.position, default_direction);
        }

        public void SpawnDir(Vector3 dir)
        {
            GlobalEffectManager.Spawn(effect_id, this.transform.position, dir);
        }
    }
}
