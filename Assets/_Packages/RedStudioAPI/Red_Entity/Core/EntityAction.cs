using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityAction : MonoBehaviour
{
    public Entity Master { get; private set; }
    public bool Enabled => enabled;

    void Awake()
    {
        (Master = GetComponentInParent<Entity>())?.RegisterAction(this);
    }

    public virtual IEnumerator Init(Entity master)
    {
        yield break;
    }

    public virtual void StartEvent() { }
    public virtual void UpdateEvent() { }
}
