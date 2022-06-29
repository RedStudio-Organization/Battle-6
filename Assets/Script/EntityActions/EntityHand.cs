using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedStudio.Battle10
{
    public class EntityHand : EntityAction
    {
        [SerializeField] bool _allowChangeParenting;

        public Weapon CurrentWeapon { get; private set; }
        public Vector2 AimDirection { get; set; }

        internal void Equip(Weapon weapon)
        {
            if(CurrentWeapon != null)
            {
                CurrentWeapon.Unequip();
                CurrentWeapon = null;
            }

            CurrentWeapon = weapon;
            if(_allowChangeParenting)
            {
                CurrentWeapon.Root.SetParent(transform);
                CurrentWeapon.Root.localPosition = Vector3.zero;
                CurrentWeapon.Root.localRotation = Quaternion.identity;
            }
        }

        internal void Unequip(Weapon weapon)
        {
            if(_allowChangeParenting)
            {
                CurrentWeapon.Root.SetParent(null);
            }
            CurrentWeapon = null;
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + (Vector3)AimDirection, 0.1f);
        }

    }
}