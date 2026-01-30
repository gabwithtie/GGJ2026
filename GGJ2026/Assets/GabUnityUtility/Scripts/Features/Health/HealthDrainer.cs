using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(HealthObject))]
    public class HealthDrainer : MonoBehaviour
    {
        HealthObject healthObject;

        [SerializeField] private float drainInterval = 1;
        [SerializeField] private int drainAmount = 1;
        [SerializeField] private int drainStacks = 1;

        [SerializeField] private bool use_textparticles = true;
        [SerializeField] private Vector3 textparticles_offset;

        private float time_last_drain = 0f;

        private void Awake()
        {
            healthObject = GetComponent<HealthObject>();
        }

        public void SetStacks(int stacks)
        {
            drainStacks = stacks;
        }

        private void Update()
        {
            if (drainStacks <= 0)
            {
                return;
            }

            time_last_drain += Time.deltaTime;

            if (time_last_drain >= drainInterval)
            {
                var final_generation_amount = drainAmount * drainStacks;

                time_last_drain = 0f;
                
                healthObject.TakeDamage(final_generation_amount, null);

                if (use_textparticles)
                    TextParticle.SpawnText(
                        $"-{final_generation_amount}",
                        transform.position + textparticles_offset,
                        1.0f,
                        drainInterval * 1.2f,
                        Color.red
                    );
            }
        }
    }
}
