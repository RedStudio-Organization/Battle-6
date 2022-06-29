using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Entity : MonoBehaviour
{
    protected List<EntityAction> _actions;

    protected GameFlowScene Master { get; private set; }

    public IEnumerable<EntityAction> Actions => _actions = _actions ?? new List<EntityAction>();

    internal void RegisterAction(EntityAction entityAction)
    {
        _actions = _actions ?? new List<EntityAction>();
        _actions.Add(entityAction);
    }

    public virtual IEnumerator Init(GameFlowScene master)
    {
        Master = master;
        yield return null;      // Wait all EntityActions register self

        var actionCount = Actions.Count();
        for (var i = 0; i < actionCount; i++)
        {
            yield return _actions[i].Init(this);
        }

        yield break;
    }

    public virtual void StartEvent()
    {
        for(var i=0; i<_actions.Count; i++)
        {
            if (_actions[i].Enabled == false) continue;

            _actions[i].StartEvent();
        }
    }

    public virtual void UpdateEvent()
    {
        for (var i = 0; i < _actions.Count; i++)
        {
            if (_actions[i].Enabled == false) continue;

            _actions[i].UpdateEvent();
        }
    }

}
