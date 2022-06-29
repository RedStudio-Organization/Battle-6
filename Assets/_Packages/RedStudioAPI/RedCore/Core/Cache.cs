using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cache<T> where T : class
{
    T _data;
    Func<T> _getter;

    public Cache(Func<T> get)
    {
        _getter = get;
    }

    public T Get() => _data ?? (_data = _getter.Invoke());

}
