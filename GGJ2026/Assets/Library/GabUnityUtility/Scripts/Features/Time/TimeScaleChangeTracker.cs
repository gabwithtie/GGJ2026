using UnityEngine;

namespace GabUnity
{
    public class TimeScaleChangeTracker : MonoSingleton<TimeScaleChangeTracker>
    {
        [SerializeField] private float gameplay_timescale = 1.0f;
        [SerializeField] private bool paused = false;

        public void SetGameplayTimeScale(float timescale)
        {
            gameplay_timescale = timescale;

            if (!paused)
                Time.timeScale = gameplay_timescale;
        }

        public void SetPause(bool setpause)
        {
            paused = setpause;

            if (paused)
                Time.timeScale = 0;
            else
                Time.timeScale = gameplay_timescale;
        }
    }
}
