using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhysicEvent3D : MonoBehaviour
{
    [SerializeField, BoxGroup("Collider Ref"), ReadOnly] Collider[] _colliders;

    [SerializeField, Foldout("Event - Trigger3D")] UnityEvent<Collider> _onTriggerEnter;
    [SerializeField, Foldout("Event - Trigger3D")] UnityEvent<Collider> _onTriggerStay;
    [SerializeField, Foldout("Event - Trigger3D")] UnityEvent<Collider> _onTriggerExit;

    [SerializeField, Foldout("Event - Collision3D")] UnityEvent<Collision> _onCollisionEnter;
    [SerializeField, Foldout("Event - Collision3D")] UnityEvent<Collision> _onCollisionExit;

    public event UnityAction<Collider> TriggerEnter2D { add => _onTriggerEnter.AddListener(value); remove => _onTriggerEnter.RemoveListener(value); }
    public event UnityAction<Collider> TriggerStay2D { add => _onTriggerStay.AddListener(value); remove => _onTriggerStay.RemoveListener(value); }
    public event UnityAction<Collider> TriggerExit2D { add => _onTriggerExit.AddListener(value); remove => _onTriggerExit.RemoveListener(value); }
    public event UnityAction<Collision> CollisionEnter2D { add => _onCollisionEnter.AddListener(value); remove => _onCollisionEnter.RemoveListener(value); }
    public event UnityAction<Collision> CollisionExit2D { add => _onCollisionExit.AddListener(value); remove => _onCollisionExit.RemoveListener(value); }

    Collider _cache;
    public Collider MainCollider => _cache ?? (_cache = GetComponent<Collider>());  // TMP

    private void Reset()
    {
#if UNITY_EDITOR
        EDITOR_UpdateColliderRef();
#endif
    }

    internal static PhysicEvent3D CreateOnRuntime(GameObject target)
    {
        var t = target.AddComponent<PhysicEvent3D>();
        t._colliders = t._colliders ?? target.GetComponents<Collider>();
        t._onTriggerEnter = t._onTriggerEnter ?? new UnityEvent<Collider>();
        t._onTriggerStay = t._onTriggerStay ?? new UnityEvent<Collider>();
        t._onTriggerExit = t._onTriggerExit ?? new UnityEvent<Collider>();
        t._onCollisionEnter = t._onCollisionEnter ?? new UnityEvent<Collision>();
        t._onCollisionExit = t._onCollisionExit ?? new UnityEvent<Collision>();
        return t;
    }

    #region InternalEvents
    private void OnTriggerEnter(Collider collision) => _onTriggerEnter?.Invoke(collision);
    private void OnTriggerStay(Collider collision) => _onTriggerStay?.Invoke(collision);
    private void OnTriggerExit(Collider collision) => _onTriggerExit?.Invoke(collision);
    private void OnCollisionEnter(Collision collision) => _onCollisionEnter?.Invoke(collision);
    private void OnCollisionExit(Collision collision) => _onCollisionEnter?.Invoke(collision);
    #endregion

#if UNITY_EDITOR
    [Button("Update Collider Ref")]
    void EDITOR_UpdateColliderRef()
    {
        _colliders = GetComponents<Collider>();
    }
#endif

}
