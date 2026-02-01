using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(Collider))]
    public class AudioAreaTrigger : MonoBehaviour
    {
        [SerializeField] private string areaid;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out ConditionalAudioPlayer cond))
            {
                cond.SetArea(areaid);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out ConditionalAudioPlayer cond))
            {
                cond.SetArea("");
            }
        }
    }
}
