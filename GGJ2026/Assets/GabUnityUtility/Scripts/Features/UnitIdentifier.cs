using UnityEngine;

namespace GabUnity
{
    public class UnitIdentifier : MonoBehaviour
    {
        [SerializeField] private int _teamId = 0;
        public int TeamId => _teamId;

        public Collider Collider => m_collider;

        private Collider m_collider;

        private void Awake()
        {
            m_collider = GetComponent<Collider>();

            if(m_collider == null)
            {
                foreach (var col in GetComponentsInChildren<Collider>())
                {
                    if (col.isTrigger == false)
                    {
                        m_collider = col;
                        break;
                    }
                }
            }
        }
    }
}