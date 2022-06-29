using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RedStudio.Battle10
{
    public class PlayerInteraction : EntityAction
    {
        Interaction FocusedInteraction { get; set; }

        [SerializeField] UnityEvent _onFocus;
        [SerializeField] UnityEvent _onUnFocus;
        [SerializeField] UnityEvent _onInteract;

        public event UnityAction OnFocus { add => _onFocus.AddListener(value); remove => _onFocus.RemoveListener(value); }
        public event UnityAction OnUnFocus { add => _onUnFocus.AddListener(value); remove => _onUnFocus.RemoveListener(value); }
        public event UnityAction OnInteract { add => _onInteract.AddListener(value); remove => _onInteract.RemoveListener(value); }

        public void LaunchInteraction()
        {
            _onInteract?.Invoke();
            FocusedInteraction?.Interact(this);
        }

        public void ManualInteraction(Interaction i)
        {
            FocusedInteraction = i;
            LaunchInteraction();
        }

        internal void GetFocus(Interaction interaction)
        {
            if (FocusedInteraction != null) FocusedInteraction.UnFocus(this);

            FocusedInteraction = interaction;
            FocusedInteraction.Focus(this);
            _onFocus?.Invoke();
        }

        internal void RemoveFocus(Interaction interaction)
        {
            if (FocusedInteraction != interaction) return;

            FocusedInteraction?.UnFocus(this);
            FocusedInteraction = null;
            _onUnFocus?.Invoke();
        }
    }
}
