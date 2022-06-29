using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TransformExtension
{
    public static IEnumerable<Transform> ToLinq(this Transform @this)
    {
        return @this.Cast<Transform>();
    }

    public static void DestroyAllChildren(this Transform @this)
    {
        for (int t = @this.childCount - 1; t >= 0; t--)
        {
            @this.GetChild(t).gameObject.SelfSafeDestroy();
        }
    }

}
