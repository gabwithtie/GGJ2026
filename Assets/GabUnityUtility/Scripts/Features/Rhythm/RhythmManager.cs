using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GabUnity
{
    public class RhythmManager : MonoSingleton<RhythmManager>
    {
        [SerializeField] private float bpm = 120;
        [SerializeField] private float max_error = 0.2f;
        [SerializeField] private int beatsperbar = 4;
        [SerializeField] private float lookahead = 0.05f;
        [SerializeField] private UnityEvent<int> onChangeBar;
        [SerializeField] private int prev_bar = -1;
        [SerializeField] private int prev_beat = -1;

        [Serializable]
        struct MeasureChangeEvent
        {
            public bool add_cur_bar;
            public bool local_to_bar;
            public int beat_divisor;
            public int bar_divisor;
            public UnityEvent<int> onChangeBeat;
        }
        [SerializeField] private List<MeasureChangeEvent> beatChangeEvents;

        public static float Bpm => Instance.bpm;
        public static float MaxError => Instance.max_error;
        public static int BeatsPerBar => Instance.beatsperbar;
        public static int CurrentBar => Mathf.FloorToInt(Time_ahead / (BeatsPerBar * SecondsPerBeat));
        public static int CurrentBeat => Mathf.FloorToInt(Time_ahead / SecondsPerBeat);
        public static float TimeInBar => Time % (BeatsPerBar * SecondsPerBeat);
        public static float BarDuration => BeatsPerBar * SecondsPerBeat;
        public static float TimeInBeat(int interval = 1, bool inversed = false)
        {
            if (inversed)
                return (SecondsPerBeat * interval) - (Time % (SecondsPerBeat * interval));
            else
                return Time % (SecondsPerBeat * interval);
        }
        public static float TimeInBeatNormalized(int interval = 1, bool inversed = false)
        {
            var time = 0.0f;

            if (inversed)
                time = (SecondsPerBeat * interval) - (Time % (SecondsPerBeat * interval));
            else
                time = Time % (SecondsPerBeat * interval);

            return time / (SecondsPerBeat * interval);
        }
        public static float SecondsPerBeat => 60f / Bpm;

        [SerializeField] private AudioSource basis;
        public void SetBasis(AudioSource _basis) => basis = _basis;


        public static float Time_ahead => Instance.basis.time + Instance.lookahead;
        public static float Time => Instance.basis.time;

        public static bool IsGood(out int result)
        {
            float targetTime = Mathf.Round(Time / SecondsPerBeat) * SecondsPerBeat;
            float error = Mathf.Abs(Time - targetTime);
            bool isGood = error <= RhythmManager.MaxError;

            if (isGood)
                result = 0;
            else
                result = MathF.Sign(Time - targetTime);


            return isGood;
        }

        public static bool IsGood()
        {
            return IsGood(out _);
        }

        public void SetBpm(float _bpm)
        {
            this.bpm = _bpm;
        }

        private void Update()
        {
            if (prev_bar != CurrentBar)
            {
                onChangeBar.Invoke(CurrentBar);
                prev_bar = CurrentBar;
            }

            if(prev_beat != CurrentBeat)
            {
                prev_beat = CurrentBeat;

                foreach (var item in beatChangeEvents)
                {
                    var final_i = CurrentBeat / item.beat_divisor;

                    if(item.local_to_bar)
                        final_i %= beatsperbar;

                    if (item.add_cur_bar)
                        final_i += (CurrentBar / item.bar_divisor) * beatsperbar;

                    item.onChangeBeat.Invoke(final_i);
                }
            }
        }
    }
}
