using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorBehavior : EntityAction
{
    [SerializeField] Camera _camera;
    [SerializeField] InputActionReference _moveCursor;
    [SerializeField] CursorReference _ref;

    public override IEnumerator Init(Entity master)
    {
        yield return base.Init(master);
        (_ref as IReferenceHead<CursorBehavior>).Set(this);
    }

    public override void UpdateEvent()
    {
        base.UpdateEvent();
        transform.position = _camera.ScreenToWorldPoint(_moveCursor.action.ReadValue<Vector2>()).SetZ(0);
    }

}
