using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class EnumerableExtension
{

    readonly static Func<(int min, int max), int> _classicUnityRandom = (amplitude) => UnityEngine.Random.Range(amplitude.min, amplitude.max);
    readonly static Func<System.Random, (int min, int max), int> _systemRandom = (random, amplitude) => random.Next(amplitude.min, amplitude.max);
    public static T PickRandom<T>(this List<T> @this) => PickRandom(@this, _classicUnityRandom);
    public static T PickRandom<T>(this List<T> @this, System.Random random) => PickRandom(@this, (amplitude) => _systemRandom.Invoke(random, amplitude));
    public static T PickRandom<T>(this List<T> @this, Func<(int min, int max), int> strategy) => @this[strategy.Invoke((0, @this.Count))];
    public static T PickRandom<T>(this T[] @this) => @this[_classicUnityRandom((0, @this.Length))];
    public static void Shuffle<T>(this List<T> @this, Func<(int min, int max), int> randomProvider = null)
    {
        if (randomProvider == null) randomProvider = _classicUnityRandom;
        int n = @this.Count;
        while (n > 1)
        {
            n--;
            int k = randomProvider((0, n + 1));
            T value = @this[k];
            @this[k] = @this[n];
            @this[n] = value;
        }
    }

    public static IEnumerable<T> InfiniteLoopingList<T>(this IEnumerable<T> @this)
    {
        var enumerator = @this.GetEnumerator();
        while (true)
        {
            if (!enumerator.MoveNext())
            {
                enumerator = @this.GetEnumerator();
                enumerator.MoveNext();
            }
            yield return enumerator.Current;
        }
    }
    public static IEnumerable<T> InfiniteLoopingList<T>(this T[] @this)
    {
        var index = 0;
        var length = @this.Length;
        while (index < @this.Length)
        {
            yield return @this[index];
            index++;
            if (index >= length) index = 0;
        }
    }
    
    public static string LogCollection<T>(this IEnumerable<T> @this, bool lineReturn = false)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var el in @this)
        {
            sb.Append(el.ToString());
            sb.Append(" | ");
            if (lineReturn) sb.Append("\n");
        }
        return sb.ToString();
    }

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> @this, System.Action<T> callback)
    {
        foreach (var el in @this) callback.Invoke(el);
        return @this;
    }
    
    public static T Last<T>(this List<T> @this) => @this[@this.Count - 1];
    public static T Last<T>(this T[] @this) => @this[@this.Length - 1];

}
