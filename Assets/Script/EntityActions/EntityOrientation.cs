using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityOrientation : EntityAction
{
    [SerializeField] bool _isBasedOnCursor = true;
    [SerializeField, ShowIf(nameof(_isBasedOnCursor))] CursorReference _cursor;

    [SerializeField] bool _applyRotation;
    [SerializeField, ShowIf(nameof(_applyRotation))] Rigidbody2D _rootRotation;
    [SerializeField, ShowIf(nameof(_applyRotation))] float _rotationAngleOffset = -90f;

    [SerializeField] UnityEvent<float> _onOrientationUpdate;

    public Vector2 CharacterOrientation { get; private set; }

    public event UnityAction<float> OnOrientationUpdate;

    public override void StartEvent()
    {
        base.StartEvent();
        StartCoroutine(LateUpdateEvent());
        IEnumerator LateUpdateEvent()
        {
            var waiter = new WaitForEndOfFrame();
            while (true)
            {
                yield return waiter;

                var rot = Mathf.Atan2(CharacterOrientation.y, CharacterOrientation.x) * Mathf.Rad2Deg + _rotationAngleOffset;
                if (_applyRotation)
                {
                    _rootRotation.rotation = rot;
                }
                OnOrientationUpdate?.Invoke(rot);
            }
        }

    }

    public override void UpdateEvent()
    {
        base.UpdateEvent();

        if(_isBasedOnCursor && _cursor.Acquire() != null)
        {
            CharacterOrientation = (_cursor.Acquire().transform.position - Master.transform.position).normalized;
        }
        // else if(_isBasedOnAiming)
        // {
        //     CharacterOrientation = _hand.AimDirection.normalized;
        // }
    }

}

