using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(HealthObject))]
    public class DamageTextParticle : MonoBehaviour
    {
        private HealthObject mHealthObject;

        [SerializeField] private float lifetime = 1.0f;
        [SerializeField] private float size_mult = 1.0f;
        [SerializeField] private Color text_color= Color.red;

        private void Awake()
        {
            mHealthObject = GetComponent<HealthObject>();
        }

        private void OnDamage(float damage, UnitIdentifier id)
        {
            TextParticle.SpawnText(damage.ToString(), transform.position, size_mult, lifetime, text_color);
        }

        private void Start()
        {
            mHealthObject.SubscribeOnDamage(OnDamage);
        }
    }
}
