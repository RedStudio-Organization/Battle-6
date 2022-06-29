using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RedStudio.Battle10
{
    public abstract class Interactable : EntityAction       
    {
        EntityHand _owner;
        Coroutine _followTargetRoutine;
        public EntityHand Owner
        {
            get => _owner;
            set
            {
                if(_owner != null && _followTargetRoutine!=null)
                {
                    StopCoroutine(_followTargetRoutine);
                    _followTargetRoutine = null;
                }
                var old = _owner;
                _owner = value;
                if(_owner != null)
                {
                    _followTargetRoutine = StartCoroutine(FollowTargetRoutine());
                    OnOwnerChanged?.Invoke(old, _owner);
                }
            }
        }

        /// <summary>
        /// Input : Old owner, current owner
        /// </summary>
        public event UnityAction<EntityHand, EntityHand> OnOwnerChanged;

        public abstract bool FollowTarget { get; }
        public virtual bool Interact(PlayerInteraction pi) => true;

        IEnumerator FollowTargetRoutine()
        {
            var waiter = new WaitForEndOfFrame();
            while(true)
            {
                yield return waiter;
                if(Master!=null&&Owner!=null)
                {
                    Master.transform.position = Owner.transform.position;
                    Master.transform.rotation = Owner.transform.rotation;
                }
                else
                {
                    Owner = null;
                }
            }
        }

    }
}
