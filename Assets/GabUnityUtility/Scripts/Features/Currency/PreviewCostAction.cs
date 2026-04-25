using UnityEngine;

namespace GabUnity
{
    [CreateAssetMenu(menuName = "GabUnity/ActionRequest/Preview Cost")]
    public class PreviewCostAction : ActionRequest
    {
        [SerializeField] private CurrencyChangeInfo amount;
        public CurrencyChangeInfo Amount => amount;
    }
}
