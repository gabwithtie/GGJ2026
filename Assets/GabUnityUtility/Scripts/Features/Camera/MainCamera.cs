using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(Camera))]
    public class MainCamera : MonoSingleton<MainCamera>
    {
        static public Camera Cam { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            Cam = GetComponent<Camera>();
        }
    }
}
