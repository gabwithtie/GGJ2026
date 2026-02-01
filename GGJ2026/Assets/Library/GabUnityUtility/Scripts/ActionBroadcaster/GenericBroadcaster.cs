using UnityEngine;

namespace GabUnity{
    public class GenericBroadcaster : MonoBehaviour
    {
        [SerializeField] private GenericBroadcastAction action;

        public void Fire()
        {
            ActionRequestManager.Request(action);
        }
    }
}
