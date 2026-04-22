using GabUnity;
using UnityEngine;

public class LaneManager : MonoSingleton<LaneManager>
{
    [SerializeField] private float lane_width = 3;
    public static float LaneWidth => Instance.lane_width;

    [SerializeField] private int min_lane = -1;
    [SerializeField] private int max_lane = 1;
    public static int MinLane => Instance.min_lane;
    public static int MaxLane => Instance.max_lane;

    public static float Get_X(int lane)
    {
        return lane * LaneWidth;
    }
}
