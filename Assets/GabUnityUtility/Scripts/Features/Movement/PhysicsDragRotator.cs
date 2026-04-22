using UnityEngine;
using UnityEngine.EventSystems;

namespace GabUnity
{
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicsDragRotator : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Header("Settings")]
        [SerializeField] private float sensitivity = 0.5f;
        [SerializeField] private float dragAngularDrag = 0.06f;
        [SerializeField] private float releaseAngularDrag = 0.01f;

        private Rigidbody _rb;

        void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            _rb.angularDamping = dragAngularDrag;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            var inputdelta = -eventData.delta;

            var rotationSpeed = (inputdelta * sensitivity) / Time.fixedDeltaTime;

            var newvel = new Vector3(0, rotationSpeed.x, 0) * Mathf.Deg2Rad;
            newvel += -rotationSpeed.y * MainCamera.Instance.transform.right * Mathf.Deg2Rad;

            // Apply rotation specifically to the Y axis (up/down axis)
            _rb.angularVelocity = newvel;
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            _rb.angularDamping = releaseAngularDrag;
        }
    }
}
