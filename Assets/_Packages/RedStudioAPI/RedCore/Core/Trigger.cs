using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger {

    bool _value;

    public Trigger() => _value = false;
    public void Activate() => _value = true;

    public bool IsActivated()
    {
        if (!_value) return false;

        _value = false;
        return true;
    }

}

public class Trigger<T>
{
    bool _value;
    T _data;

    public Trigger() => _value = false;
    public void Activate(T data)
    {
        _data = data;
        _value = true;
    }

    public bool SoftCheckIsActivated() => _value;

    public bool IsActivated(out T data)
    {
        // Default situation, not activated
        data = default(T);
        if (!_value) return false;

        // Send data
        data = _data;

        // Reset structure
        _value = false;

        // Validate the activation
        return true;
    }

    public T GetDataIfActivated()
    {
        if (!_value) return default(T);
        var data = _data;
        _value = false;
        _data = default(T);
        return data;
    }

}

