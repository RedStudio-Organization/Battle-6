using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargetSet<T>
{
    IEnumerable<T> Elements { get; }
}
