using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IAlterationID { }

public class Alterable<T>
{
    class AlterationID : IAlterationID { }
    T _defaultValue;
    List<(IAlterationID, Func<T, T>alteration, int layer, string name)> _effects;

    public event Action<IAlterationID> OnNewAlteration;
    public event Action<IAlterationID> OnBeforeRemovedAlteration;
    public event Action<IAlterationID> OnRemovedAlteration;
    public event Action OnRemoveAllAlteration;

    public IEnumerable<string> DebugString => _effects.Select(i => $"{i.name} Layer: {i.layer}\n");

    public Alterable(T defaultValue)
    {
        _defaultValue = defaultValue;
        _effects = new List<(IAlterationID, Func<T, T>, int, string)>();
    }

    public T Value() => Value(_defaultValue);
    public T Value(T referenceValue)
    {
        T tmpValue = referenceValue;
        for (var i = 0; i < _effects.Count; i++)
        {
            tmpValue = _effects[i].Item2.Invoke(tmpValue);
        }
        return tmpValue;
    }

    public IEnumerable<T> AllAlterations() => AllAlterations(_defaultValue);
    public IEnumerable<T> AllAlterations(T referenceValue)
    {
        yield return _defaultValue;
        T tmpValue = referenceValue;
        for (var i = 0; i < _effects.Count; i++)
        {
            tmpValue = _effects[i].Item2.Invoke(tmpValue);
            yield return tmpValue;
        }
    }

    public IAlterationID AddCustomAlteration(Func<T,T> newTransformator, int layer=1, string alterationName="")
    {
        if (newTransformator == null) throw new NullReferenceException();

        IAlterationID id = new AlterationID();

        // Insert alteration in list
        if(_effects.Count > 0 && _effects[_effects.Count-1].Item3 > layer)
        {
            var idx = 0;
            for(var i = 0; i<_effects.Count; i++)
            {
                idx = i;
                if (layer < _effects[i].Item3) break;
            }
            _effects.Insert(idx, (id, newTransformator, layer, alterationName));
        }
        else
        {
            _effects.Add((id, newTransformator, layer, alterationName));
        }

        OnNewAlteration?.Invoke(id);
        return id;
    }

    public IAlterationID AddReplaceAlteration(T newValue) => AddCustomAlteration((old) => newValue);

    public bool RemoveAlteration(ref IAlterationID alteration)
    {
        OnBeforeRemovedAlteration?.Invoke(alteration);
        for (var i=0; i<_effects.Count; i++)
        {
            if(_effects[i].Item1 == alteration)
            {
                _effects.Remove(_effects[i]);
                alteration = null;
                OnRemovedAlteration?.Invoke(alteration);
                return true;
            }
        }
        return false;
    }

    internal void RemoveAllAlteration()
    {
        OnRemoveAllAlteration?.Invoke();
        _effects.Clear();
    }

}

