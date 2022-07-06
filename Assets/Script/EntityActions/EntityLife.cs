using NaughtyAttributes;
using RedStudio.Battle10;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class EntityLife : NetworkBehaviour
{
    [SerializeField] Entity Master;
    [SerializeField] int _baseHealth;

    [SerializeField] UnityEvent _onSpawn;
    [SerializeField] UnityEvent _onRespawn;
    [SerializeField] UnityEvent _onDamage;
    [SerializeField] UnityEvent _onDamageMainPlayer;
    [SerializeField] UnityEvent _onDeath;

    public Entity LastHit { get; private set; }

    public event UnityAction OnDamage { add => _onDamage.AddListener(value); remove => _onDamage.RemoveListener(value); }
    public event UnityAction OnDeath { add => _onDeath.AddListener(value); remove => _onDeath.RemoveListener(value); }
    public event UnityAction OnSpawn { add => _onSpawn.AddListener(value); remove => _onSpawn.RemoveListener(value); }
    public event UnityAction OnRespawn { add => _onRespawn.AddListener(value); remove => _onRespawn.RemoveListener(value); }

    [ShowNativeProperty] public bool IsAlive => CurrentHealth.Value > 0;

    public NetworkVariable<int> CurrentHealth { get; private set; } = new NetworkVariable<int>(
        1000,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);
    public int MaxHealth => _baseHealth;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _onSpawn?.Invoke();

        //if (IsOwner)
        if (IsServer)
        {
            CurrentHealth.Value = _baseHealth;
        }
        CurrentHealth.OnValueChanged += ResolveDamage;
    }

    internal void Damage(DamageProfileSO dp, Entity from) => Damage(dp.RawDamage, from);
    internal void Damage(int amount, Entity from)
    {
        //if (IsOwner == false) return;
        if (IsServer == false) return;
        if (!IsAlive) return;   // Already dead

        LastHit = from;
        CurrentHealth.Value = Mathf.Max(CurrentHealth.Value - amount, 0);
    }

    void ResolveDamage(int prev, int current)
    {
        _onDamage?.Invoke();
        if (IsLocalPlayer) _onDamageMainPlayer?.Invoke();
        if (IsAlive == false)
        {
            _onDeath?.Invoke();
            StartCoroutine(Death());
        }
    }

    IEnumerator Death()
    {
        // Dynamic test
        var sprite = GetComponentInChildren<SpriteRenderer>();
        var startColor = sprite.color;
        var endColor = new Color(startColor.r, startColor.g, startColor.b, 0);
        var time = 0f;
        var fadeDuration = 1f;

        while(time < fadeDuration)
        {
            yield return null;
            time += Time.deltaTime;
            sprite.color = Color.Lerp(startColor, endColor, time / fadeDuration);
        }
        sprite.color = endColor;

        yield return new WaitForSeconds(1f);
        yield break;
    }


}
