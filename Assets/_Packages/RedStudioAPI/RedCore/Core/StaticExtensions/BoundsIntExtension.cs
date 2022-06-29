using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Slider;

public static class BoundsIntExtension
{

    /// <summary>
    /// Used for tilemap grid
    /// </summary>
    public struct BoundsIntIterable : IEnumerable<Vector3Int>
    {
        BoundsInt _bi;

        public BoundsIntIterable(BoundsInt bi)
        {
            _bi = bi;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<Vector3Int> GetEnumerator()
        {
            var enumerator = _bi.allPositionsWithin;
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            };
            yield break;
        }
    }

    public static IEnumerable<Vector3Int> ToEnumerable(this BoundsInt @this) => new BoundsIntIterable(@this);

}
