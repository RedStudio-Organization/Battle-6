using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedStudio.Battle10
{
    public class TagHolder : EntityAction
    {
        [SerializeField] Tag[] _tags;

        public IEnumerable<Tag> Tags => _tags;
    }
}
