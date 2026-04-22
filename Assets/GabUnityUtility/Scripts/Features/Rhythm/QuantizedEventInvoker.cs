using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GabUnity
{
    public class QuantizedEventInvoker : MonoSingleton<QuantizedEventInvoker>
    {
        [SerializeField] private int beat_offset = 1;
        [SerializeField] private int interval = 2;
        [SerializeField] private float lookahead = 0.07f;
        [SerializeField] private float minimum_time = 0.15f;
        public static float BarLength => RhythmManager.SecondsPerBeat * Instance.interval;
        public static float RemainingTillNextFireTime => BarLength - ((RhythmManager.Time + (RhythmManager.SecondsPerBeat * Instance.beat_offset)) % BarLength);

        public static float GetNextInvokationFromNow()
        {
            if (RemainingTillNextFireTime < Instance.minimum_time)
            {
                return RemainingTillNextFireTime + BarLength;
            }
            else
            {
                return RemainingTillNextFireTime;
            }
        }
    }
}
