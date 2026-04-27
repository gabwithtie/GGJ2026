using UnityEngine;
using UnityEngine.UIElements;

namespace GabUnity
{
    class RandomTransformationAtStart : MonoBehaviour
    {
        [SerializeField] private Vector3 position_randomness;
        [SerializeField] private Vector3 scale_randomness;
        [SerializeField] private bool uniform_scale_randomness = true;
        [SerializeField] private Vector3 rotation_randomness;

        private void Awake()
        {
            transform.localPosition += new Vector3(
                Random.Range(-position_randomness.x, position_randomness.x),
                Random.Range(-position_randomness.y, position_randomness.y),
                Random.Range(-position_randomness.z, position_randomness.z));

            if (!uniform_scale_randomness)
                transform.localScale += new Vector3(
                    Random.Range(-scale_randomness.x, scale_randomness.x),
                    Random.Range(-scale_randomness.y, scale_randomness.y),
                    Random.Range(-scale_randomness.z, scale_randomness.z));
            else
                transform.localScale += Vector3.one * Random.Range(-scale_randomness.x, scale_randomness.x);

            transform.localEulerAngles += new Vector3(
                Random.Range(-rotation_randomness.x, rotation_randomness.x),
                Random.Range(-rotation_randomness.y, rotation_randomness.y),
                Random.Range(-rotation_randomness.z, rotation_randomness.z));
        }
    }
}
