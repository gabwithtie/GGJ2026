using GabUnity;
using UnityEngine;

public class PseudoRamp : MonoBehaviour
{
    [SerializeField] private float forcemult = 1.5f;
    [SerializeField] private Transform top_ref;
    [SerializeField] private Transform bot_ref;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out PlayerController player)){
            var t = Mathf.InverseLerp(bot_ref.position.y, top_ref.position.y, player.transform.position.y);
            t = 1.0f - t;
        }
    }
}
