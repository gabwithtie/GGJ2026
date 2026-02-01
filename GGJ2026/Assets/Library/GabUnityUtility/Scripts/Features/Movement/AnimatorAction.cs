using UnityEngine;
using UnityEngine.Events;

public class AnimatorAction : MonoBehaviour
{
    [SerializeField] private bool animator_bool;

    [SerializeField] private UnityEvent onTrue;

    private bool prev_known;

    private void LateUpdate()
    {
        if (prev_known != animator_bool)
        {
            if(animator_bool)
                onTrue.Invoke();

            prev_known = animator_bool;
        }
    }
}
