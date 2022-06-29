using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2Extension
{

    public static bool Approximately(Vector2 a, Vector2 b) =>
            Mathf.Approximately(a.x, b.x) &&
            Mathf.Approximately(a.y, b.y);

    public static Vector3 SetX(this Vector2 @this, float newValue) => new Vector2(newValue, @this.y);
    public static Vector3 SetY(this Vector2 @this, float newValue) => new Vector2(@this.x, newValue);

    public enum Direction { NULL = -1, Right = 0, Left = 1, Up = 2, Down = 3 }
    public static Direction AxisToDirection(this Vector2 @this)
    {
        bool right = Mathf.Abs(@this.x) > Mathf.Abs(@this.y);
        bool inverse = right ? @this.x < 0 : @this.y < 0;

        if (right)
            return inverse ? Direction.Left : Direction.Right;
        else
            return inverse ? Direction.Down : Direction.Up;
    }

}
