using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace RedStudio.Battle10
{
    public static class NetworkListExtension
    {
        public static int FindIndex<T>(this NetworkList<T> @this, Func<T,bool> predicate) where T: unmanaged, IEquatable<T>
        {
            for(var i = 0; i < @this.Count; i++)
            {
                if (predicate.Invoke(@this[i])) return i;
            }
            return -1;
        }

        public static void Remove<T>(this NetworkList<T> @this, Func<T, bool> predicate) where T : unmanaged, IEquatable<T>
        {
            var idx = @this.FindIndex(predicate);
            if (idx <= -1) return;
            @this.RemoveAt(idx);
        }

        public static IEnumerable<T> ToEnumerable<T>(this NetworkList<T> @this) where T : unmanaged, IEquatable<T>
        {
            for (var i = 0; i < @this.Count; i++)
            {
                yield return @this[i];
            }
        }

    }
}
