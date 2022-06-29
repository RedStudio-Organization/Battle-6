using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RedStudio.Battle10
{

    public class Interaction : EntityAction
    {
        [SerializeField] PhysicEvent2D _physic;

        [SerializeField] Interactable _interactable;

        [SerializeField] UnityEvent _onInteractionFocusOn;
        [SerializeField] UnityEvent _onInteractionFocusOff;
        [SerializeField] UnityEvent<PlayerInteraction> _onInteraction;
        [SerializeField] UnityEvent<PlayerInteraction> _onInteractionRefused;

        public event UnityAction<PlayerInteraction> OnInteraction { 
            add => _onInteraction.AddListener(value); 
            remove => _onInteraction.RemoveListener(value); 
        }
        public event UnityAction<PlayerInteraction> OnInteractionRefused { 
            add => _onInteractionRefused.AddListener(value); 
            remove => _onInteractionRefused.RemoveListener(value); 
        }

        PlayerInteraction CurrentPlayerInteraction { get; set; }

        public override void StartEvent()
        {
            base.StartEvent();
            _physic.TriggerEnter2D += DetectPlayerInteraction;
            _physic.TriggerExit2D += UndetechPlayerInteraction;
        }
        
        public void Interact(PlayerInteraction pi)
        {
            if(_interactable.Interact(pi))
            {
                _onInteraction?.Invoke(pi);
                return;
            }
            else
            {
                _onInteractionRefused?.Invoke(pi);
            }
        }

        void DetectPlayerInteraction(Collider2D arg0)
        {
            if (!enabled) return;
            if (arg0.TryGetComponent(out PlayerInteraction pi))
            {
                pi.GetFocus(this);
            }
        }

        void UndetechPlayerInteraction(Collider2D arg0)
        {
            if (!enabled) return;
            if (arg0.TryGetComponent(out PlayerInteraction pi))
            {
                pi.RemoveFocus(this);
            }
        }

        public void Focus(PlayerInteraction pi)
        {
            CurrentPlayerInteraction = pi;
            _onInteractionFocusOn?.Invoke();
        }
        public void UnFocus(PlayerInteraction pi)
        {
            CurrentPlayerInteraction = null;
            _onInteractionFocusOff?.Invoke();
        }

    }
}
