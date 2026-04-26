using UnityEngine;

namespace GabUnity
{
    class CopyOnStart : MonoBehaviour
    {
        [SerializeField] private Vector3 copy_pos;
        [SerializeField] private Vector3 copy_rot;
        private void Start()
        {
            Destroy(this); //Destroy this component so it doesn't run again if the copy also has it
            var copy = Instantiate(gameObject, transform.parent);
            copy.transform.localPosition = copy_pos;
            copy.transform.localEulerAngles = copy_rot;
        }
    }

}
