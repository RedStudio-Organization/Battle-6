using RedStudio.Battle10;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FilterEntityCollectByPlayer : AbstractFilterEntityCollect<Entity>
{
    protected override IEnumerable<Entity> Filter(IEnumerable<Entity> input)
        => input.Where(i => i.Actions?.FirstOrDefault(j => j != null));

}
