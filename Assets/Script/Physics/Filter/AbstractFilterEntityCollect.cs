using RedStudio.Battle10;
using RedStudio.XP_EBR;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class AbstractFilterEntityCollect<T> : MonoBehaviour
{
    [SerializeField] CollectTrigger<T> _master;
    [SerializeField] int _layerValue = 10;
    [SerializeField] string _layerName = "Filter";
    IAlterationID _alterationID;

    void Reset()
    {
        _master = GetComponent<CollectTrigger<T>>();
    }
    
    protected abstract IEnumerable<T> Filter(IEnumerable<T> input);


    void Start()
    {
        _alterationID = _master.EntityFilters.AddCustomAlteration(Filter, layer: _layerValue, alterationName: _layerName);
    }
    void OnDestroy()
    {
        _master.EntityFilters.RemoveAlteration(ref _alterationID);
    }
}
