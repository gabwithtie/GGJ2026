using UnityEngine;

namespace GabUnity{
    public class GenericBroadcaster : MonoBehaviour
    {
        [SerializeField] private ActionRequest action;
        [SerializeField] private ActionRequest alt_off_action;

        public void Fire()
        {
            ActionRequestManager.Request(action);
        }

        public void FireAlt()
        {
            ActionRequestManager.Request(alt_off_action);
        }
    }
}
