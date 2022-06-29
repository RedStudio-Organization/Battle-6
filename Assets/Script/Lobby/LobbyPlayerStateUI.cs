using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static RedStudio.Battle10.LobbyEntity;

namespace RedStudio.Battle10
{
    public class LobbyPlayerStateUI : MonoBehaviour
    {
        [SerializeField] TMP_Text _playerName;

        [SerializeField] UnityEvent _onReady;
        [SerializeField] UnityEvent _onNotReady;

        public ulong OwnerID { get; private set; }

        internal LobbyPlayerStateUI Init(PlayerSlot slot)
        {
            OwnerID = slot.ID;
            _playerName.text = slot.NameS;
            (slot.IsReady ? _onReady : _onNotReady).Invoke();
            return this;
        }

        internal void MarkAsReady()
        {
            _onReady?.Invoke();
        }
        
    }
}
