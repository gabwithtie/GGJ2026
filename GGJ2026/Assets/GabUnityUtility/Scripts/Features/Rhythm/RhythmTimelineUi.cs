using System.Collections.Generic;
using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(RectTransform))]
    public class RhythmTimelineUi : MonoBehaviour
    {
        private struct MarkerData
        {
            public float Time;
            public RectTransform Rect;
            public bool IsMajor;
            public bool IsFeedback; // New flag to distinguish hits from beats

            public MarkerData(float time, RectTransform rect, bool isMajor, bool isFeedback = false)
            {
                Time = time;
                Rect = rect;
                IsMajor = isMajor;
                IsFeedback = isFeedback;
            }
        }

        [SerializeField] private float scale = 100f;
        [SerializeField] private GameObject beatmarker_major_prefab;
        [SerializeField] private GameObject beatmarker_minor_prefab;
        [SerializeField] private GameObject userinput_prefab_bad;
        [SerializeField] private GameObject userinput_prefab_good;

        private RectTransform _recttransform;
        private List<MarkerData> _activeMarkers = new List<MarkerData>();

        // Pools
        private Queue<RectTransform> _majorPool = new Queue<RectTransform>();
        private Queue<RectTransform> _minorPool = new Queue<RectTransform>();
        private Queue<RectTransform> _goodHitPool = new Queue<RectTransform>();
        private Queue<RectTransform> _badHitPool = new Queue<RectTransform>();

        private float CurrentTime => RhythmManager.Time;
        private float Bpm => RhythmManager.Bpm;
        private int BeatsPerBar => RhythmManager.BeatsPerBar;
        private float SecondsPerBeat => 60f / Bpm;
        private float ViewRange => _recttransform.rect.width / scale;

        private void Awake() => _recttransform = GetComponent<RectTransform>();

        public void OnUserHit()
        {
            float hitTime = CurrentTime;
            // Spawn the feedback as a timeline marker
            SpawnFeedbackMarker(hitTime, RhythmManager.IsGood());
        }

        void Update()
        {
            float currentTime = CurrentTime;
            float step = SecondsPerBeat;
            float startTime = currentTime - (ViewRange * 0.5f);
            float endTime = currentTime + (ViewRange * 0.5f);

            // 1. Unified Cleanup
            for (int i = _activeMarkers.Count - 1; i >= 0; i--)
            {
                if (_activeMarkers[i].Time < startTime || _activeMarkers[i].Time > endTime)
                {
                    DespawnMarker(_activeMarkers[i]);
                    _activeMarkers.RemoveAt(i);
                }
            }

            // 2. Beat Spawning
            float firstBeatTime = Mathf.Ceil(startTime / step) * step;
            for (float t = firstBeatTime; t <= endTime; t += step)
            {
                if (IsBeatAlreadySpawned(t)) continue;
                bool isMajor = Mathf.Approximately(t % (step * BeatsPerBar), 0);
                SpawnBeatMarker(t, isMajor);
            }

            // 3. Unified Position Update
            // This now moves both beats and user feedback in sync
            for (int i = 0; i < _activeMarkers.Count; i++)
            {
                float xPos = (_activeMarkers[i].Time - currentTime) * scale;
                _activeMarkers[i].Rect.anchoredPosition = new Vector2(xPos, 0);
            }
        }

        private void SpawnBeatMarker(float time, bool isMajor)
        {
            Queue<RectTransform> pool = isMajor ? _majorPool : _minorPool;
            RectTransform rect = GetFromPool(pool, isMajor ? beatmarker_major_prefab : beatmarker_minor_prefab);

            _activeMarkers.Add(new MarkerData(time, rect, isMajor, false));
        }

        private void SpawnFeedbackMarker(float targetTime, bool isGood)
        {
            Queue<RectTransform> pool = isGood ? _goodHitPool : _badHitPool;
            RectTransform rect = GetFromPool(pool, isGood ? userinput_prefab_good : userinput_prefab_bad);

            // Note: We use isMajor to store "isGood" for hit markers to simplify pooling logic
            _activeMarkers.Add(new MarkerData(targetTime, rect, isGood, true));
        }

        private RectTransform GetFromPool(Queue<RectTransform> pool, GameObject prefab)
        {
            RectTransform rect = (pool.Count > 0) ? pool.Dequeue() : Instantiate(prefab, _recttransform).GetComponent<RectTransform>();
            rect.gameObject.SetActive(true);
            return rect;
        }

        private void DespawnMarker(MarkerData data)
        {
            data.Rect.gameObject.SetActive(false);

            if (data.IsFeedback)
            {
                if (data.IsMajor) _goodHitPool.Enqueue(data.Rect); // Reusing IsMajor as IsGood
                else _badHitPool.Enqueue(data.Rect);
            }
            else
            {
                if (data.IsMajor) _majorPool.Enqueue(data.Rect);
                else _minorPool.Enqueue(data.Rect);
            }
        }

        private bool IsBeatAlreadySpawned(float t)
        {
            // Only check for beats, ignore hit feedback markers
            return _activeMarkers.Exists(m => !m.IsFeedback && Mathf.Abs(m.Time - t) < 0.001f);
        }
    }
}