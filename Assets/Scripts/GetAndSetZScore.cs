using UnityEngine;
using UnityEngine.Events;

public class GetAndSetZScore : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private UnityEvent<float> zScoreEvents;

    private void Update()
    {
        zScoreEvents.Invoke(Mathf.Round(targetTransform.position.z));
    }
}
