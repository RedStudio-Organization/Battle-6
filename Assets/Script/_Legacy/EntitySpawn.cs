using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntitySpawn : EntityAction
{
    [SerializeField] ObservableSO _reloadLevelEvent;

    [SerializeField] bool _respawnPlayer;

    [SerializeField] UnityEvent _onSpawn;
    [SerializeField] UnityEvent _onRespawn;

    public event UnityAction OnSpawn { add => _onSpawn.AddListener(value); remove => _onSpawn.RemoveListener(value); }
    public event UnityAction OnRespawn { add => _onRespawn.AddListener(value); remove => _onRespawn.RemoveListener(value); }

    public override IEnumerator Init(Entity master)
    {
        yield return base.Init(master);
        _reloadLevelEvent.OnInvoke += ResetState;
    }

    public override void StartEvent()
    {
        base.StartEvent();
        _onSpawn?.Invoke();
    }

    public void ResetState()
    {
        _onRespawn?.Invoke();
    }
}
