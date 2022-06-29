using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTimer
{
    float _currentValue;
    float _lastSyncDate;

    public SpecialTimer()
    {
        _currentValue = 0;
        _lastSyncDate =-1;
    }

    public SpecialTimer StartWithValue(float startValue = 0f)
    {
        _currentValue = startValue;
        return Start(startValue > 0 ? false : true);
    }

    public SpecialTimer Start(bool reinit = true)
    {
        if (reinit) _currentValue = 0;
        UpdateCount(justLastSync: true);
        return this;
    }

    public SpecialTimer Stop()
    {
        UpdateCount(true);
        _lastSyncDate = -1;
        return this;
    }

    public float CurrentValue
    {
        get
        {
            if (_lastSyncDate > 0) UpdateCount();
            return _currentValue;
        }
    }

    void UpdateCount(bool justLastSync=false)
    {
        if(!justLastSync) _currentValue += Time.realtimeSinceStartup - _lastSyncDate;
        _lastSyncDate = Time.realtimeSinceStartup;
    }

    public TimeSpan ToTimeSpan() => TimeSpan.FromSeconds(_currentValue);

}
