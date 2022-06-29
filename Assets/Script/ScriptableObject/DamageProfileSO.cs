using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedStudio.Battle10
{
    [CreateAssetMenu(menuName ="DamageProfile")]
    public class DamageProfileSO : ScriptableObject
    {
        [field:SerializeField] public int RawDamage { get; private set; }

    }
}
