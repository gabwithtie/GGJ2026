using UnityEngine;
using UnityEngine.AdaptivePerformance;

namespace GabUnity
{
    [RequireComponent(typeof(UnitDetector))]
    public class AreaStatusGiver : MonoBehaviour, IStatusGiver
    {
        [SerializeField] private StatusInfo statusInfo;
        [SerializeField] private int stacks = 1;

        private UnitDetector detector;

        int IStatusGiver.Stacks => stacks;

        private void Awake()
        {
            detector = GetComponent<UnitDetector>();
        }

        private void Start()
        {
            detector.RegisterEnterAction(OnEnter);
            detector.RegisterExitAction(OnExit);
        }

        private void OnEnter(UnitIdentifier unit)
        {
            if (!unit.TryGetComponent(out StatusHolder holder))
                return;

            holder.AddStatus(statusInfo, this);
        }

        private void OnExit(UnitIdentifier unit)
        {
            if (!unit.TryGetComponent(out StatusHolder holder))
                return;

            holder.RemoveStatus(statusInfo, this);
        }

        private void OnDisable()
        {
            detector.DoOnAll(OnExit);
        }

        private void OnEnable()
        {
            detector.DoOnAll(OnEnter);
        }
    }
}