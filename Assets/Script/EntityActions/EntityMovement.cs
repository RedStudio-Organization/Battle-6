using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EntityMovement : EntityAction
{
    [SerializeField] protected Rigidbody2D _rb;

    public Alterable<Vector3> Direction { get; set; } = new Alterable<Vector3>(Vector3.zero);

    Vector2 NextPosition => _rb.transform.position + Direction.Value();

    protected virtual void FixedUpdate() =>_rb.MovePosition(NextPosition);


}
