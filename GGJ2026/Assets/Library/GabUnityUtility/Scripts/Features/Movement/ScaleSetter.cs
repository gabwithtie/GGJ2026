using UnityEngine;

namespace GabUnity
{
    public class ScaleSetter : MonoBehaviour
    {
        [SerializeField] private Vector3 scale_a;
        [SerializeField] private Vector3 scale_b;

        public void SetTargetT(float t)
        {
            transform.localScale = Vector3.Lerp(scale_a, scale_b, t);
        }
    }
}
