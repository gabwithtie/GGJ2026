using System;
using UnityEngine;

namespace GabUnity {
    [RequireComponent(typeof(UnitDetector))]
    public class ClosestUnitSelector : MonoBehaviour
    {
        public UnitIdentifier Closest => _closest;
        private UnitIdentifier _closest = null;

        private UnitDetector _detector;

        [Range(0.1f, 5)]
        [SerializeField] private float check_interval = 0.5f;

        private float time_last_check = 0f;

        private Action<UnitIdentifier> OnChangeClosest;

        public void RegisterOnChangeClosest(Action<UnitIdentifier> action)
        {
            OnChangeClosest += action;
        }

        private void Awake()
        {
            _detector = GetComponent<UnitDetector>();
        }

        private void Update()
        {
            time_last_check += Time.deltaTime;

            if(time_last_check > check_interval)
            {
                check_interval = 0;

                Check();
            }
        }

        private void Check()
        {
            var current_closest = _detector.GetClosest(transform.position);

            if(current_closest != _closest)
            {
                _closest = current_closest;
                OnChangeClosest?.Invoke(_closest);
            }
        }
    }
}