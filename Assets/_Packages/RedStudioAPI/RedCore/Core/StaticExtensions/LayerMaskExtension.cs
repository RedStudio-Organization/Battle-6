using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerMaskExtension
{
    public static IEnumerable<(int, string)> GetAllLayerMask()
    {
        string result = "";
        int index = 0;

        while (true)
        {
            result = LayerMask.LayerToName(index);
            if (string.IsNullOrEmpty(result)) break;

            yield return (index, result);
            index++;
        }
        yield break;
    }

    public static bool ContainsLayer(this LayerMask @this, int layer) => ((1 << layer) & @this) != 0;



}
