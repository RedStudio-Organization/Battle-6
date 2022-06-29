using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace RedStudio.Battle10
{
    public class KeepNearestEntity : EntityAction
    {
        [SerializeField] CollectEntityTrigger _nearPlayers;
        [SerializeField] float _tickInSeconds=0.5f;

        public Entity CurrentNearestPlayer { get; private set; }

        public event Action<Entity> OnNearestPlayerChanged;

        Entity CalculateNearestPlayer() => _nearPlayers.Elements
                    .Select(i => (i, Vector3.SqrMagnitude(Master.transform.position - i.transform.position)))
                    .Aggregate((a, b) => a.Item2 < b.Item2 ? a : b).i;

        public override void StartEvent()
        {
            base.StartEvent();
            return;
            // StartCoroutine(NearestPlayer());
            // IEnumerator NearestPlayer()
            // {
            //     var waiter = new WaitForSeconds(_tickInSeconds);
            //     while(true)
            //     {
            //         yield return waiter;
            //         var tmp = CalculateNearestPlayer();
            //         if(CurrentNearestPlayer != tmp)
            //         {
            //             CurrentNearestPlayer = tmp;
            //             OnNearestPlayerChanged?.Invoke(CurrentNearestPlayer);
            //         }
            //     }
            // }
        }
    }
}
