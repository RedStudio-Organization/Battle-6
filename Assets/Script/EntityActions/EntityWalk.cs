using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedStudio.Battle10
{
    public class EntityWalk : EntityAction
    {
        [SerializeField] EntityMovement _movement;
        [SerializeField] float _defaultSpeed = 1f;
        [SerializeField] bool _includeFixedDeltaTime=true;
        public Vector2 JoystickDirection { get; set; }

        IAlterationID _defaultWalkAlterable;

        public override void StartEvent()
        {
            base.StartEvent();
            _defaultWalkAlterable = _movement.Direction.AddCustomAlteration(v => 
                JoystickDirection.normalized * 
                _defaultSpeed * 
                (_includeFixedDeltaTime?Time.fixedDeltaTime:1), layer: 1);
        }

        private void OnDestroy()
        {
            _movement.Direction.RemoveAlteration(ref _defaultWalkAlterable);
        }

    }
}
