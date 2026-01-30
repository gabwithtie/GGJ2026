using UnityEngine;

namespace GabUnity
{
    public class RotationSetter : MonoBehaviour
    {
        [SerializeField] private Vector3 rot_a;
        [SerializeField] private Vector3 rot_b;

        public void SetTargetT(float t)
        {
            transform.localRotation = Quaternion.Euler(Vector3.Lerp(rot_a, rot_b, t));
        }
    }
}