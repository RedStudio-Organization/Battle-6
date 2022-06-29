using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

interface IObservableFire
{
    void Invoke();
}

[CreateAssetMenu(menuName ="Event/ObservableEvent")]
public class ObservableSO : ScriptableObject, IObservableFire
{
    [SerializeField] UnityEvent _onInvoke;
    public event UnityAction OnInvoke { add => _onInvoke.AddListener(value); remove => _onInvoke.RemoveListener(value); }
    void IObservableFire.Invoke() => _onInvoke?.Invoke();
}
