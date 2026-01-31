using UnityEngine;

namespace GabUnity
{
    public class CurrencyGenerator : MonoBehaviour
    {
        [SerializeField] private float generationInterval = 1;
        [SerializeField] private int generationAmount = 1;
        [SerializeField] private int generationStacks = 1;
        [SerializeField] private CurrencyInfo currencyInfo;

        private float time_last_generation = 0f;

        [SerializeField] private bool use_textparticles = true;
        [SerializeField] private Vector3 textparticles_offset;

        public void SetStacks(int stacks)
        {
            generationStacks = stacks;
        }

        private void Awake()
        {
            if(currencyInfo == null)
            {
                Debug.LogError($"CurrencyGenerator on {gameObject.name} has no CurrencyInfo assigned.", this);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(generationStacks <= 0)
            {
                return;
            }

            time_last_generation += Time.deltaTime;

            if (time_last_generation >= generationInterval)
            {
                var final_generation_amount = generationAmount * generationStacks;

                time_last_generation = 0f;
                CurrencyManager.Add(currencyInfo, final_generation_amount);

                if (use_textparticles)
                    TextParticle.SpawnText(
                        $"+{final_generation_amount} {currencyInfo.CurrencyName}",
                        transform.position + textparticles_offset,
                        1.0f,
                        generationInterval * 1.2f,
                        Color.brown
                    );
            }
        }
    }
}
