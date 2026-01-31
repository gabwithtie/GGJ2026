using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class ComboCounter : MonoBehaviour
{
    [SerializeField] private int _counter = 0;
    [SerializeField] private float max_time_between = 1.0f;

    private float _time_since = 0.0f;

    public int Combo => _counter;

    [Serializable]
    struct ComboInvoker
    {
        public int counter_divisor;
        public int invoke_counter_offset;
        public UnityEvent<int> OnChangeCounter;
    }
    [SerializeField] private List<ComboInvoker> invokelist;

    public void ResetCombo()
    {
        _counter = 0;
    }

    public void RegisterCombo()
    {
        _time_since = 0;
        _counter++;

        foreach (var item in invokelist)
        {
            var final_i = _counter + item.invoke_counter_offset;
            final_i /= item.counter_divisor;

            item.OnChangeCounter.Invoke(final_i);
        }
    }

    private void Update()
    {
        if (_time_since > max_time_between)
        {
            foreach (var item in invokelist)
            {
                item.OnChangeCounter.Invoke(0);
            }
            _counter = 0;
        }

        _time_since += Time.deltaTime;
    }
}
