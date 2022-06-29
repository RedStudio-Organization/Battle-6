using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RedStudio.Battle10
{
    public class FilterByTag : AbstractFilterEntityCollect<Entity>
    {
        [SerializeField] Tag[] _admissibleTag;

        HashSet<Tag> _optimizedTag;

        void Awake()
        {
            _optimizedTag = new HashSet<Tag>(_admissibleTag);
        }

        protected override IEnumerable<Entity> Filter(IEnumerable<Entity> input)
            => input.Select(i => (i, i.Actions.Where(j => j is TagHolder).Cast<TagHolder>().SelectMany(k => k.Tags)))
                .Where(i => i.Item2.FirstOrDefault(j => _optimizedTag.Contains(j)) != null)
                .Select(i => i.i);
    }
}
