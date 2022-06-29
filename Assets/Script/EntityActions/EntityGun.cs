using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace RedStudio.Battle10
{
    public class EntityGun : EntityAction
    {
        [SerializeField] Weapon _weapon;

        [SerializeField] Bullet _bulletPrefab;
        [SerializeField] Transform _bulletSpawnPoint;
        [SerializeField] float[] _bulletSpawnDirections;
        [SerializeField] float _fireRate = 1f;

        [SerializeField] UnityEvent<Bullet> _onFire;

        public event UnityAction<Bullet> OnFire { add => _onFire.AddListener(value); remove => _onFire.RemoveListener(value); }
        public EntityHand Holder => _weapon.Holder;
        Coroutine FireRoutine { get; set; }
        float LastFireDate { get; set; }

        void Start()
        {
            _weapon.OnUseStart += FireStart;
            _weapon.OnUseStop += FireStop;
        }

        public void SingleFire()
        {
            foreach(var bullet in _bulletSpawnDirections)
            {
                Bullet instantiatedBullet = null;
                instantiatedBullet = Pool.Singleton.Create(_bulletPrefab, _bulletSpawnPoint.position, Quaternion.identity);
                instantiatedBullet.Launch(this, transform.TransformDirection(Vector2.up+(Vector2.right*bullet)));
                _onFire?.Invoke(instantiatedBullet);
            }
        }

        public void FireStart()
        {
            if (FireRoutine != null) return;
            FireRoutine = StartCoroutine(FireProcess());
            IEnumerator FireProcess()
            {
                // wait eventual cooldown
                if(LastFireDate+_fireRate > Time.time)
                {
                    yield return new WaitForSeconds(LastFireDate + _fireRate - Time.time);
                }

                // Base wait
                var waiter = new WaitForSeconds(_fireRate);
                while(true)
                {
                    LastFireDate=Time.time;
                    SingleFire();
                    LastFireDate = Time.time;
                    yield return waiter;
                }
            }
        }

        public void FireStop()
        {
            if (FireRoutine == null) return;
            StopCoroutine(FireRoutine);
            FireRoutine = null;
        }

    }
}
