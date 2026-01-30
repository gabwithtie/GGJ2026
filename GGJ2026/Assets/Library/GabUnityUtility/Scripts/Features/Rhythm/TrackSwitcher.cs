using GabUnity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TrackSwitcher : MonoBehaviour
{
    [SerializeField] private List<AudioSource> tracks;
    [SerializeField] private int cur_track = 0;
    [SerializeField] private float fade_time = 0.1f;

    [SerializeField] private UnityEvent<AudioSource> onSwitch;

    private int _from_track = 0;
    private int _target_track = 0;
    
    private float _timefading = 0;

    public void SwitchTo(int index)
    {
        index = Mathf.Clamp(index, 0, tracks.Count - 1);
        _target_track = index;
        _from_track = cur_track;
    }

    private void Update()
    {
        _timefading += Time.deltaTime;

        var t = _timefading / fade_time;
        t = Mathf.Clamp(t, 0, 1);

        tracks[cur_track].volume = Mathf.Lerp(0, 1, t);

        if(_from_track != cur_track)
            tracks[_from_track].volume = Mathf.Lerp(1, 0, t);

        if (_timefading > fade_time)
            for (int i = 0; i < tracks.Count; i++)
                if (i != cur_track)
                    tracks[i].volume = 0;

        if (cur_track == _target_track)
            return;

        if (tracks[cur_track].time > tracks[cur_track].clip.length - fade_time)
        {
            _timefading = 0;

            tracks[cur_track].volume = 0;
            tracks[_target_track].volume = 1;
            tracks[_target_track].time = tracks[cur_track].time;

            cur_track = _target_track;

            onSwitch.Invoke(tracks[_target_track]);
        }
    }
}
