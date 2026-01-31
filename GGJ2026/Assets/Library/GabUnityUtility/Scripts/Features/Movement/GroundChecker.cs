using UnityEngine;
using UnityEngine.Events;

namespace GabUnity
{
    [RequireComponent(typeof(Rigidbody))]
    public class GroundChecker : MonoBehaviour
    {
        [SerializeField] private bool m_IsGrounded;
        public bool Grounded => m_IsGrounded;

        [SerializeField] private UnityEvent OnGround;
        [SerializeField] private UnityEvent OnGroundOff;

        private void OnCollisionStay(Collision collision)
        {
            if (collision.GetContact(0).normal.y < 0.6f)
                return;

            if (!m_IsGrounded)
                OnGround.Invoke();

            m_IsGrounded = true;
        }

        private void OnCollisionExit(Collision collision)
        {
            OnGroundOff.Invoke();

            m_IsGrounded = false;
        }
    }
}
