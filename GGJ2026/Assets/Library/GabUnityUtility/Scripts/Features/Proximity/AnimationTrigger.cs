using GabUnity;
using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    [SerializeField] private Animator toplay;
    [SerializeField] private string triggername;
    [SerializeField] private int unit_mask = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null) return;
        if (!other.attachedRigidbody.TryGetComponent(out UnitIdentifier ui)) return;

        if (ui.TeamId == unit_mask)
            toplay.SetTrigger(triggername);
    }
}
