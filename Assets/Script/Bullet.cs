using RedStudio.Battle10;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static Pool;

public class Bullet : MonoBehaviour, IPlayerOwner
{
    [SerializeField] PhysicEvent2D _physics;
    [SerializeField] EntityWalk _movement;
    [SerializeField] UnityEvent _onCollide;

    [SerializeField] DamageProfileSO _damage;
    [SerializeField] float _angleOffset = -90f;

    public bool IsLaunched { get; private set; }
    public EntityGun From { get; private set; }

    Entity IPlayerOwner.Owner => From.Holder.Master;

    void Start()
    {
        _physics.TriggerEnter2D += Contact;
    }
    void OnDestroy()
    {
        _physics.TriggerEnter2D -= Contact;
    }

    public void Launch(EntityGun from, Vector3 direction)
    {
        if (IsLaunched) return;
        IsLaunched = true;
        From = from;

        _movement.JoystickDirection = direction;
        transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + _angleOffset);
    }

    void Contact(Collider2D collision)
    {
        if (!IsLaunched) return;
        if (collision.TryGetComponent<Bullet>(out _)) return; // Collide on Entity

        if (collision.TryGetComponent(out Entity e))
        {
            if (e == From || e == From?.Holder?.Master) return;  // Ignore self fire

            e.GetComponent<EntityLife>()?.Damage(_damage, this);
            
            SelfDestroy();
        }
        // Collide on object
        else if(collision.isTrigger==false)
        {
            SelfDestroy();
        }
    }

    void SelfDestroy()
    {
        IsLaunched = false;
        _movement.JoystickDirection = Vector2.zero;
        _onCollide?.Invoke();

        StartCoroutine(DelayedDestroy());
        IEnumerator DelayedDestroy()
        {
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
            yield break;
        }
    }


}
