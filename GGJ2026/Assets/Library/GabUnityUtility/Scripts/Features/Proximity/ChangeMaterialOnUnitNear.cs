using System.Collections.Generic;
using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(UnitDetector))]
    public class ChangeMaterialOnUnitNear : MonoBehaviour
    {
        private UnitDetector detector;

        [SerializeField] private List<MeshRenderer> targets;

        [SerializeField] private Material nearMaterial;
        [SerializeField] private Material farMaterial;

        private void Awake()
        {
            detector = GetComponent<UnitDetector>();
        }

        private void Start()
        {
            detector.RegisterEnterAction(unit =>
            {
                foreach (var target in targets)
                {
                    target.sharedMaterial = nearMaterial;
                }
            });

            detector.RegisterNoneAction(unit =>
            {
                foreach (var target in targets)
                {
                    target.sharedMaterial = farMaterial;
                }
            });
        }
    }
}