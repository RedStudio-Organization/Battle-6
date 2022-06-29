using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RedStudio.Battle10
{
    public class Entity_SimpleEnemy_AI : MonoBehaviour
    {
        [SerializeField, BoxGroup("Internal Refs")] Entity _root;
        [SerializeField, BoxGroup("Internal Refs")] EntityMovement _move;
        [SerializeField, BoxGroup("Internal Refs")] EntityLife _life;
        [SerializeField, BoxGroup("Internal Refs")] EntityHand _hand;

        [Header("Conf")]
        [SerializeField] float _playerDetectionRadius = 3f;

        [Header("Editor")]
        [SerializeField] bool _drawGizmos;

        Cache<EntityGun> Gun { get; set; }

        IEnumerator Start()
        {
            yield break;
            /*
            _gun = new Cache<EntityGun>(()=>_hand.CurrentWeapon.Master.Actions.First(i => i is EntityGun) as EntityGun);
            while (true)
            {
                yield return null;
                if (_life.IsAlive == false) continue;

                // Look target
                var hits = Physics2D.CircleCastAll(_root.transform.position, _playerDetectionRadius, Vector2.up);
                var playerFound = hits.Select(i => i.collider.GetComponent<Entity>())
                    .Where(i => i != null)
                    .Select(i => i.GetComponent<Player>())
                    .FirstOrDefault();

                if (playerFound == null) continue;

                _hand.AimDirection = playerFound.transform.position;
                _gun.Get().SingleFire();

                yield return new WaitForSeconds(0.5f);
            }
            */
        }

        #region EDITOR
        void OnDrawGizmos()
        {
            if (_root == null) return;
            if (_drawGizmos == false) return;

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_root.transform.position, _playerDetectionRadius);
        
        }
        #endregion
    }
}
