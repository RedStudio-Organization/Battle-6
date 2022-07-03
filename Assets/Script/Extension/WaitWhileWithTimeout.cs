using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedStudio.Battle10
{
    public class CoroutineExtension
    {
        public static IEnumerator WaitWhileWithTimeout(Func<bool> predicate, float timeoutInSeconds)
        {
            var startTime = Time.time;
            while ((Time.time < startTime + timeoutInSeconds) && predicate.Invoke())
            {
                yield return null;
            }
            yield break;
        }

    }
}
