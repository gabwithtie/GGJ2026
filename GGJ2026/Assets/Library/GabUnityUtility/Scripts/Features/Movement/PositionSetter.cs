using UnityEngine;

public class PositionSetter : MonoBehaviour
{
    [SerializeField] private Vector3 pos_0;
    [SerializeField] private Vector3 pos_1;

    public void SetT(float t)
    {
        transform.localPosition = Vector3.Lerp(pos_0, pos_1, t);
    }
}
