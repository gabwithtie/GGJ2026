using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(FilledImageCostDisplayer))]
    public class PreviewCostActionHandler : ActionRequestHandler<PreviewCostAction>
    {
        private FilledImageCostDisplayer costDisplayer;
        [SerializeField] private CurrencyInfo target_currency;

        [SerializeField] private bool auto_reset = false;
        [SerializeField] private float auto_resettime = 0.3f;
        private float time_since_last_request = 0f;

        protected override bool ProcessRequest(PreviewCostAction somerequest)
        {
            var costinfo = somerequest.Amount.Costs[0];

            costDisplayer.SetCostDisplay(Mathf.Abs((float)costinfo.amount) / target_currency.Max);

            time_since_last_request = 0;
            return true;
        }

        protected override void Awake()
        {
            base.Awake();

            costDisplayer = GetComponent<FilledImageCostDisplayer>();
        }

        // Update is called once per frame
        void Update()
        {
            costDisplayer.SetCurrent((float)CurrencyManager.GetAmount(target_currency) / target_currency.Max);

            time_since_last_request += Time.deltaTime;

            if (auto_reset && time_since_last_request >= auto_resettime)
            {
                costDisplayer.SetCostDisplay(0);
            }
        }
    }
}