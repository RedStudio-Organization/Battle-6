using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReferenceHead<T>
{
    void Set(T v);
}

public class Reference<T> : ScriptableObject, IReferenceHead<T>
{
    private T _value { get; set; }

    public event Action<T> OnValueChanged;


    public T Acquire() => _value;

    void Start()
    {
        _value = default(T);
    }

    void IReferenceHead<T>.Set(T v)
    {
        _value = v;
        OnValueChanged?.Invoke(_value);
    }

}
