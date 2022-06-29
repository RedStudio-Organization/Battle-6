using NaughtyAttributes;
using RedStudio.Battle10;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace RedStudio.Battle10
{
    public class Weapon : Interactable
    {
        [SerializeField] Transform _root;

        [SerializeField, BoxGroup("Event")] UnityEvent<EntityHand> _onEquip;
        [SerializeField, BoxGroup("Event")] UnityEvent<EntityHand> _onUnequip;
        [SerializeField, BoxGroup("Event")] UnityEvent _onUseStart;
        [SerializeField, BoxGroup("Event")] UnityEvent _onUseStop;

        public event UnityAction<EntityHand> OnEquip { add => _onEquip.AddListener(value); remove => _onEquip.RemoveListener(value); }
        public event UnityAction<EntityHand> OnUnequip { add => _onUnequip.AddListener(value); remove => _onUnequip.RemoveListener(value); }
        public event UnityAction OnUseStart { add => _onUseStart.AddListener(value); remove => _onUseStart.RemoveListener(value); }
        public event UnityAction OnUseStop { add => _onUseStop.AddListener(value); remove => _onUseStop.RemoveListener(value); }

        public EntityHand Holder { get; private set; }
        public Transform Root => _root;

        public override bool FollowTarget => true;

        public override bool Interact(PlayerInteraction pi)
        {
            if (!base.Interact(pi)) return false;

            Equip(pi);
            return true;
        }

        public void Equip(PlayerInteraction pi) => Equip(pi.Master.Actions.FirstOrDefault(i => i is EntityHand) as EntityHand);
        public void Equip(EntityHand h)
        {
            h.Equip(this);
            Holder = h;
            Owner = h;
            _onEquip?.Invoke(h);
        }
        public void Unequip()
        {
            Holder?.Unequip(this);
            _onUnequip?.Invoke(Holder);
            Holder = null;
            Owner = null;

            Root.SetParent(null);
        }

        public void UseStart()
        {
            _onUseStart?.Invoke();
        }
        public void UseStop()
        {
            _onUseStop?.Invoke();
        }

        #region Editor
        void Reset()
        {
            _root = transform;
        }
        #endregion

    }
}
