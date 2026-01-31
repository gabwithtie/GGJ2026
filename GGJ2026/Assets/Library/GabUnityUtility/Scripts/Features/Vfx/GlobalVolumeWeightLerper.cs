using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Volume))]
public class GlobalVolumeWeightLerper : MonoBehaviour
{
    [SerializeField] private float t_speed = 1.0f;
     private Volume volume;

    [SerializeField] private bool _on;
    [SerializeField] private float _t;

    private void Awake()
    {
        volume = GetComponent<Volume>();
    }

    public void SetOn(bool val)
    {
        _on = val;
    }

    private void Update()
    {
        if (_on)
            _t += Time.unscaledDeltaTime * t_speed;
        else
            _t -= Time.unscaledDeltaTime * t_speed;

        _t = Mathf.Clamp01(_t);

        volume.weight = _t;
    }
}
