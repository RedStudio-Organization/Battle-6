using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RedStudio.Battle10
{
    public class ObservableBehavior : MonoBehaviour
    {
        [SerializeField] ObservableSO _observable;
        [SerializeField] List<SerializableEvent> _localEvent;

        void Start() => _observable.OnInvoke += Fire;
        void OnDestroy() => _observable.OnInvoke -= Fire;
        void Fire() => _localEvent?.Invoke();
    }
}
