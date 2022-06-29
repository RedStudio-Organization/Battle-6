using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhysicEvent2D : MonoBehaviour
{
    [SerializeField, BoxGroup("Collider Ref"), ReadOnly] Collider2D[] _colliders;

    [SerializeField, Foldout("Event - Trigger2D")] UnityEvent<Collider2D> _onTriggerEnter2D;
    [SerializeField, Foldout("Event - Trigger2D")] UnityEvent<Collider2D> _onTriggerStay2D;
    [SerializeField, Foldout("Event - Trigger2D")] UnityEvent<Collider2D> _onTriggerExit2D;

    [SerializeField, Foldout("Event - Collision2D")] UnityEvent<Collision2D> _onCollisionEnter2D;
    [SerializeField, Foldout("Event - Collision2D")] UnityEvent<Collision2D> _onCollisionExit2D;

    public event UnityAction<Collider2D> TriggerEnter2D { add => _onTriggerEnter2D.AddListener(value); remove => _onTriggerEnter2D.RemoveListener(value); }
    public event UnityAction<Collider2D> TriggerStay2D { add => _onTriggerStay2D.AddListener(value); remove => _onTriggerStay2D.RemoveListener(value); }
    public event UnityAction<Collider2D> TriggerExit2D { add => _onTriggerExit2D.AddListener(value); remove => _onTriggerExit2D.RemoveListener(value); }
    public event UnityAction<Collision2D> CollisionEnter2D { add => _onCollisionEnter2D.AddListener(value); remove => _onCollisionEnter2D.RemoveListener(value); }
    public event UnityAction<Collision2D> CollisionExit2D { add => _onCollisionExit2D.AddListener(value); remove => _onCollisionExit2D.RemoveListener(value); }

    Collider2D _cache;
    public Collider2D MainCollider => _cache ?? (_cache = GetComponent<Collider2D>());  // TMP

    private void Reset()
    {
#if UNITY_EDITOR
        EDITOR_UpdateColliderRef();
#endif
    }

    internal static PhysicEvent2D CreateOnRuntime(GameObject target)
    {
        var t = target.AddComponent<PhysicEvent2D>();
        t._colliders = t._colliders ?? target.GetComponents<Collider2D>();
        t._onTriggerEnter2D = t._onTriggerEnter2D ?? new UnityEvent<Collider2D>();
        t._onTriggerStay2D = t._onTriggerStay2D ?? new UnityEvent<Collider2D>();
        t._onTriggerExit2D = t._onTriggerExit2D ?? new UnityEvent<Collider2D>();
        t._onCollisionEnter2D = t._onCollisionEnter2D ?? new UnityEvent<Collision2D>();
        t._onCollisionExit2D = t._onCollisionExit2D ?? new UnityEvent<Collision2D>();
        return t;
    }

    #region InternalEvents
    private void OnTriggerEnter2D(Collider2D collision) => _onTriggerEnter2D?.Invoke(collision);
    private void OnTriggerStay2D(Collider2D collision) => _onTriggerStay2D?.Invoke(collision);
    private void OnTriggerExit2D(Collider2D collision) => _onTriggerExit2D?.Invoke(collision);
    private void OnCollisionEnter2D(Collision2D collision) => _onCollisionEnter2D?.Invoke(collision);
    private void OnCollisionExit2D(Collision2D collision) => _onCollisionEnter2D?.Invoke(collision);
    #endregion

#if UNITY_EDITOR
    [Button("Update Collider Ref")]
    void EDITOR_UpdateColliderRef()
    {
        _colliders = GetComponents<Collider2D>();
    }
#endif

}
