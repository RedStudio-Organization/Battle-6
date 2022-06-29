using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedStudio.Battle10
{
    /// <summary>
    /// Represent an entity that has a PlayerOwner
    /// </summary>
    public interface IPlayerOwner
    {
        Entity Owner { get; }
    }
}
