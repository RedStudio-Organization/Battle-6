using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTimer_Update
{
    float _currentValue;

    MonoBehaviour Master { get; set; }
    Coroutine TimerRoutine { get; set; }
    public float CurrentValue => _currentValue;

    public TimeSpan ToTimeSpan() => TimeSpan.FromSeconds(_currentValue);

    public SpecialTimer_Update(MonoBehaviour master)
    {
        _currentValue = 0;
        Master = master;
    }

    public void StartWithValue(float startValue = 0f)
    {
        _currentValue = startValue;
        Start(startValue > 0 ? false : true);
    }

    public SpecialTimer_Update Start(bool reinit = true)
    {
        if (reinit) _currentValue = 0;
        TimerRoutine = Master.StartCoroutine(Timer());
        return this;

        IEnumerator Timer()
        {
            yield return null;
            while(true)
            {
                _currentValue += Time.deltaTime;

                yield return null;
            }
        }
    }

    public void Stop()
    {
        Master.StopCoroutine(TimerRoutine);
        TimerRoutine = null;
    }

}
