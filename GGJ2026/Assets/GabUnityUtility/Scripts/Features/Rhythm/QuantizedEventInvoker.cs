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

        private static List<System.Action> invokelist = new List<System.Action>();
        private static List<System.Action> invokelist_next = new List<System.Action>();

        private bool invoked_this_bar = false;

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

        public static float InvokeOnNext(System.Action action)
        {
            if (RemainingTillNextFireTime < Instance.minimum_time)
            {
                invokelist_next.Add(action);
                return RemainingTillNextFireTime + BarLength;
            }
            else
            {
                invokelist.Add(action);
                return RemainingTillNextFireTime;
            }
        }

        private void Update()
        {
            if(RemainingTillNextFireTime < lookahead)
            {
                if (invoked_this_bar)
                    return;

                foreach (System.Action action in invokelist)
                {
                    action.Invoke();
                }

                invoked_this_bar = true;

                invokelist.Clear();
                invokelist.AddRange(invokelist_next);
                invokelist_next.Clear();
            }
            else
            {
                invoked_this_bar = false;
            }
        }
    }
}
