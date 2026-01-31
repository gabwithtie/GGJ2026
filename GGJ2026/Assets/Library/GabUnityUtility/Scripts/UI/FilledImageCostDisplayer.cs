using UnityEngine;
using UnityEngine.UI;

namespace GabUnity
{
    [RequireComponent(typeof(Image))]
    public class FilledImageCostDisplayer : MonoBehaviour
    {
        private Image base_image;
        [SerializeField]private Image front_image;

        private float cur_cost_display = 0;

        private void Awake()
        {
            base_image = GetComponent<Image>();
        }

        public void SetCurrent(float value)
        {
            base_image.fillAmount = value;
            front_image.fillAmount = value - cur_cost_display;
        }

        public void SetCostDisplay(float cost)
        {
            cur_cost_display = cost;
        }
    }
}
