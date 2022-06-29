using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace RedStudio.XP_EBR
{
    public abstract class CollectTrigger<T> : MonoBehaviour, ITargetSet<T>
    {
        [SerializeField] protected PhysicEvent2D _physic;
        [AllowNesting, ShowNonSerializedField] List<T> _elements;
        [AllowNesting, ShowNonSerializedField] Alterable<IEnumerable<T>> _elementFilters;

        public Alterable<IEnumerable<T>> EntityFilters => _elementFilters;
        public IEnumerable<T> Elements => _elementFilters.Value();

        public event UnityAction<T> OnElementEnter;
        public event UnityAction<T> OnElementExit;

        #region EDITOR
        private void Reset()
        {
            _physic = GetComponent<PhysicEvent2D>() ?? 
                GetComponentInChildren<PhysicEvent2D>() ?? 
                GetComponentInParent<PhysicEvent2D>();
        }
        #endregion

        void Awake()
        {
            _elements = new List<T>();
            _elementFilters = new Alterable<IEnumerable<T>>(_elements);
        }

        void Start()
        {
            _physic.TriggerEnter2D += PhysicEvent_TriggerEnter2D;
            _physic.TriggerExit2D += PhysicEvent_TriggerExit2D;
        }

        void OnDestroy()
        {
            _physic.TriggerEnter2D -= PhysicEvent_TriggerEnter2D;
            _physic.TriggerExit2D -= PhysicEvent_TriggerExit2D;
        }

        void PhysicEvent_TriggerEnter2D(Collider2D arg0)
        {
            if (arg0 == null) return;
            if (arg0.TryGetComponent(out T ee))
            {
                _elements.Add(ee);
                OnElementEnter?.Invoke(ee);
            }
            else if (arg0.attachedRigidbody != null && arg0.attachedRigidbody.TryGetComponent(out T e))
            {
                _elements.Add(e);
                OnElementEnter?.Invoke(e);
            }
        }

        void PhysicEvent_TriggerExit2D(Collider2D arg0)
        {
            if (arg0 == null) return;
            if (arg0.TryGetComponent(out T ee))
            {
                _elements.Remove(ee);
                OnElementExit?.Invoke(ee);
            }
            else if (arg0.attachedRigidbody != null && arg0.attachedRigidbody.TryGetComponent(out T e))
            {
                _elements.Remove(e);
                OnElementExit?.Invoke(e);
            }
        }

    }
}
