using UnityEngine;

namespace GabUnity
{
    [CreateAssetMenu(menuName = "GabUnity/Status/Status Info")]
    public class StatusInfo : ScriptableObject
    {
        [SerializeField] private string statusName;
        [SerializeField] private Sprite statusIcon;

        public string StatusName => statusName;
        public Sprite StatusIcon => statusIcon;
    }
}
