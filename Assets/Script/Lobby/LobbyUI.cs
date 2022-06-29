using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RedStudio.Battle10
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField, BoxGroup("UI")] Button _readyButton;
        [SerializeField, BoxGroup("UI")] TMP_Text _roomInfoText;
        [SerializeField, BoxGroup("UI")] TMP_Text _timeoutText;
        [SerializeField, BoxGroup("UI")] Transform _playerStateRoot;
        [SerializeField, BoxGroup("UI")] LobbyPlayerStateUI _playerLobbySlotUI;

        [SerializeField, BoxGroup("Event")] UnityEvent _onReadyClicked;

        LobbyEntity Lobby { get; set; }
        List<LobbyPlayerStateUI> _slots;

        void UpdateTimeout(int p, int c) => _timeoutText.text = c.ToString();

        void Awake()
        {
            _slots = new List<LobbyPlayerStateUI>();
        }

        IEnumerator Start()
        {
            while (Lobby == null)
            {
                yield return null;
                Lobby = NetworkManager.Singleton.SpawnManager.SpawnedObjectsList
                .Select(i => i.GetComponent<LobbyEntity>())
                .FirstOrDefault(i => i != null);
            }

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData;
            _roomInfoText.text = $"IP {transport.Address} / Port {transport.Port}";

            // UI prepare
            _readyButton.onClick.AddListener(MarkAsReady);
            UpdateTimeout(Lobby.Timeout.Value, Lobby.Timeout.Value);
            foreach(var el in Lobby.PlayerSlots)
            {
                UpdateSlots(new NetworkListEvent<LobbyEntity.PlayerSlot>()
                {
                    Type = NetworkListEvent<LobbyEntity.PlayerSlot>.EventType.Add,
                    Value = el
                });
            }

            // UI Update
            Lobby.Timeout.OnValueChanged += UpdateTimeout;
            Lobby.PlayerSlots.OnListChanged += UpdateSlots;
            Lobby.OnRemovePlayer += Lobby_OnRemovePlayer;
        }

        void OnDestroy()
        {
            if (Lobby == null) return;
            // UI Update
            Lobby.Timeout.OnValueChanged -= UpdateTimeout;
            Lobby.PlayerSlots.OnListChanged -= UpdateSlots;
            Lobby.OnRemovePlayer -= Lobby_OnRemovePlayer;
        }

        void Lobby_OnRemovePlayer(ulong obj)
        {
            var idx = _slots.FindIndex(i => i.OwnerID == obj);
            if (idx <= -1) return;       // ERROR

            Destroy(_slots[idx].gameObject);
            _slots.RemoveAt(idx);
        }

        void UpdateSlots(NetworkListEvent<LobbyEntity.PlayerSlot> changeEvent)
        {
            switch(changeEvent.Type)
            {
                case NetworkListEvent<LobbyEntity.PlayerSlot>.EventType.Add:
                    _slots.Add(Instantiate(_playerLobbySlotUI, _playerStateRoot).Init(changeEvent.Value));
                    break;
                case NetworkListEvent<LobbyEntity.PlayerSlot>.EventType.Value:
                    if(changeEvent.PreviousValue.IsReady == false &&
                        changeEvent.Value.IsReady==true)
                    {
                        var idxx = _slots.FindIndex(i => i.OwnerID == changeEvent.Value.ID);
                        _slots[idxx].MarkAsReady();
                    }
                    break;
                default: break;
            }
        }

        void MarkAsReady()
        {
            Lobby.MarkAsReady_ServerRPC(NetworkManager.Singleton.LocalClientId);
        }

    }
}
