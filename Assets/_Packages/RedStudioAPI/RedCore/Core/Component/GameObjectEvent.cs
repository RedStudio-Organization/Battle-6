using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameObjectEvent : MonoBehaviour
{
    [SerializeField] UnityEvent _onEnabled;
    [SerializeField] UnityEvent _onDisabled;
    [SerializeField] UnityEvent _onStart;

    private void OnEnable()
    {
        _onEnabled?.Invoke();
    }
    private void OnDisable()
    {
        _onDisabled?.Invoke();
    }
    private void Start()
    {
        _onStart?.Invoke();
    }
}
