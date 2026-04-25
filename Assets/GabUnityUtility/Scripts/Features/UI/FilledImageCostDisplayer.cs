using UnityEngine;
using UnityEngine.UI;

namespace GabUnity
{
    [RequireComponent(typeof(Image))]
    public class FilledImageCostDisplayer : MonoBehaviour
    {
        private Image base_image;
        [SerializeField]private Image front_image;
        [SerializeField]private float interp_speed = 10;

        private float cur_cost_display = 0;

        float base_fill = 0;
        float front_fill = 0;

        private void Awake()
        {
            base_image = GetComponent<Image>();
        }

        public void SetCurrent(float value)
        {
            base_fill = value;
            front_fill = value - cur_cost_display;
        }

        public void SetCostDisplay(float cost)
        {
            cur_cost_display = cost;
        }

        private void Update()
        {
            base_image.fillAmount = Mathf.Lerp(base_image.fillAmount, base_fill, Time.deltaTime * interp_speed);
            front_image.fillAmount = Mathf.Lerp(front_image.fillAmount, front_fill, Time.deltaTime * interp_speed);
        }
    }
}
