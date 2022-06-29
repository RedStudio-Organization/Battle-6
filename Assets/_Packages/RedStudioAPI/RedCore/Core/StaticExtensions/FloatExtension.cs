using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FloatExtension 
{
    public static bool IsBetweenFloats(this float current, float a, float b)
    {
        if (a > b) return current <= a && current >= b;
        else return current >= a && current <= b;
    }
    public static bool IsBetweenVector2(this float current, Vector2 reference) => IsBetweenFloats(current, reference.x, reference.y);

}
