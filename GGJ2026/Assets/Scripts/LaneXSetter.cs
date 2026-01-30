using UnityEngine;

public class LaneXSetter : MonoBehaviour
{
    [SerializeField] private int lane;
    [SerializeField] private bool random_x;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (random_x)
            lane = Random.Range(LaneManager.MinLane, LaneManager.MaxLane + 1);

        var pos = this.transform.position;
        pos.x = LaneManager.Get_X(lane);

        transform.position = pos;
    }
}
