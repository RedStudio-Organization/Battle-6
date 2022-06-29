using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace RedStudio.Battle10
{
    [CreateAssetMenu(menuName ="Ref/PlayerPool")]
    public class LocalPlayersRef : ScriptableObject
    {
        List<PlayerNetwork> _players;

        public PlayerNetwork CurrentPlayer { get; private set; }
        public IEnumerable<PlayerNetwork> Players => _players;

        public void Init()
        {
            _players = new List<PlayerNetwork>();
        }

        public void AddPlayer(PlayerNetwork pn)
        {
            _players.Add(pn);
            if (pn.IsOwner && CurrentPlayer == null)
            {
                CurrentPlayer = pn;
            }
        }
        public void RemovePlayer(ulong id)
        {
            _players.Remove(Players.First(i => i.OwnerClientId == id));
        }

    }
}
