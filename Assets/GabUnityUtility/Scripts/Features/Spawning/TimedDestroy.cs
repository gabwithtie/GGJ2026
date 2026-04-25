using UnityEngine;

namespace GabUnity
{
    public class TimedDestroy : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Seconds to wait before destroying this object.")]
        [SerializeField] private float lifetime = 3.0f;

        private void OnEnable()
        {
            // Destroy the gameObject after the specified delay
            Destroy(gameObject, lifetime);
        }

        // Optional: If you want to cancel the destruction if the object is disabled manually
        private void OnDisable()
        {
            CancelInvoke();
        }
    }
}