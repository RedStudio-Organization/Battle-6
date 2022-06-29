using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class EntityUpdater : MonoBehaviour
{
    Entity _e;

    private void Awake()
    {
        _e = GetComponent<Entity>();
    }

    private IEnumerator Start()
    {
        yield return _e.Init(null);
        yield return null;
        _e.StartEvent();

        while(true)
        {
            yield return null;
            _e.UpdateEvent();
        }
    }

}
