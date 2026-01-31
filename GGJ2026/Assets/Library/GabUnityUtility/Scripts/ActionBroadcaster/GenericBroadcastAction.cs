
using UnityEngine;

namespace GabUnity
{
    [CreateAssetMenu(menuName = "GabUnity/ActionRequest/Generic Broadcast")]
    public class GenericBroadcastAction : ActionRequest
    {
        [SerializeField] private string broadcast;
        public string Broadcast => broadcast;
    }
}
