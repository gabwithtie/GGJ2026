using UnityEngine;

namespace GabUnity
{
    public class Panel : MonoBehaviour
    {
        [SerializeField] protected bool isOpen = false;

        public void Toggle()
        {
            SetOpen(!isOpen);
        }

        public virtual void SetOpen(bool open)
        {
            isOpen = open;
        }
    }

}