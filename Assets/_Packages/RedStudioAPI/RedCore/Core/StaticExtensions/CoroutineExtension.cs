using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoroutineExtension
{
    
    public static IEnumerator WaitFrames(this Coroutine @this, int frameToWait=1, Action next=null)
    {
        for (var i = 0; i < frameToWait; i++) yield return new WaitForEndOfFrame();
        next?.Invoke();
        yield break;
    }

    public static IEnumerator WaitSeconds(this Coroutine @this, float waitDuration, Action next=null, bool useUnscaledTime = false)
    {
        if (useUnscaledTime) yield return new WaitForSecondsRealtime(waitDuration);
        else yield return new WaitForSeconds(waitDuration);

        next?.Invoke();
        yield break;
    }

}
