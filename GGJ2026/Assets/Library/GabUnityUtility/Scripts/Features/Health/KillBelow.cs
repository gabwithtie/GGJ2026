using GabUnity;
using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(HealthObject))]
    public class KillBelow : MonoBehaviour
    {
        [SerializeField] private float kill_height = -2;

        private HealthObject healthObject;

        private void Awake()
        {
            healthObject = GetComponent<HealthObject>();
        }

        private void Update()
        {
            if(transform.position.y < kill_height)
            {
                healthObject.Kill(null);
            }
        }
    }

}