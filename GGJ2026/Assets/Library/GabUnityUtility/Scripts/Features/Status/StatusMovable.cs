using UnityEngine;

namespace GabUnity
{
    public class StatusMovable : MonoBehaviour
    {
        [SerializeField] private StatusHolder statusHolder;
        [SerializeField] private StatusInfo lookfor;
        [SerializeField] private Vector3 localpos_without;
        [SerializeField] private Vector3 localpos_with;
        [SerializeField] private float lerpspeed = 10;


        private void Awake()
        {
            if (statusHolder == null)
            {
                Debug.LogError("statusHolder is unassigned.");
            }
        }

        // Update is called once per frame
        void Update()
        {
            var target = localpos_without;

            if (statusHolder.HasStatus(lookfor))
            {
                target = localpos_with;
            }

            transform.localPosition = Vector3.Lerp(transform.localPosition, target, Time.deltaTime * lerpspeed);
        }
    }
}