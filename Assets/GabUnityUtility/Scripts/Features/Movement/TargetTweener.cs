using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class TargetTweener : MonoBehaviour
{
    private enum Curve
    {
        linear, 
        ease_in_2,
        ease_in_3,
        ease_in_4
    }

    [SerializeField] private float duration;
    [SerializeField] private bool reset_on_start;
    [SerializeField] private Vector3 target;
    [SerializeField] private Vector3 reset_pos;
    [SerializeField] private UnityEvent onDone;
    [SerializeField] private UnityEvent<float> onChangeT;
    [SerializeField] private Curve curve;

    private Vector3 _start;
    private float _timeplaying;
    private bool _playing = false;

    public void SetDuration(float dur)
    {
        duration = dur;
    }
    public void SetTarget(Vector3 worldpos)
    {
        target = worldpos;
    }

    public void PlayToTarget(Vector3 worldpos)
    {
        target = worldpos;
        Play();
    }

    public void Play()
    {
        _timeplaying = 0;

        if (reset_on_start)
            _start = reset_pos;
        else
            _start = transform.position;

        _playing = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_playing)
            return;

        _timeplaying += Time.deltaTime;

        var t = _timeplaying / duration;
        var final_t = 0.0f;

        switch (curve)
        {
            case Curve.linear:
                final_t = t;
                break;
            case Curve.ease_in_2:
                final_t = t * t;
                break;
            case Curve.ease_in_3:
                final_t = t * t * t;
                break;
            case Curve.ease_in_4:
                final_t = t * t * t * t;
                break;
            default:
                break;
        }

        this.transform.position = Vector3.Lerp(_start, target, final_t);
        onChangeT.Invoke(final_t);

        if (t >= 1)
        {
            _playing = false;
            onDone.Invoke();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(reset_pos, 0.1f);
    }
}
