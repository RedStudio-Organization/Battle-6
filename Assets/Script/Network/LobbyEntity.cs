using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace RedStudio.Battle10
{
    public class LobbyEntity : NetworkBehaviour
    {
        #region InternalTypes
        public enum Return { NULL = 0, GO = 1, RETURN = 2 }

        [Serializable]
        public struct PlayerSlot : IEquatable<PlayerSlot>, INetworkSerializable
        {
            public ulong ID;
            public ForceNetworkSerializeByMemcpy<FixedString32Bytes> Name;
            public bool IsReady;

            public string NameS => Name.Value.ToString();
            bool IEquatable<PlayerSlot>.Equals(PlayerSlot other) => this.ID == other.ID;

            void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> serializer)
            {
                serializer.SerializeValue(ref ID);
                serializer.SerializeValue(ref Name);
                serializer.SerializeValue(ref IsReady);
            }
        }
        #endregion

        [SerializeField, BoxGroup("ServerConf")] int _timeout = 10;
        [SerializeField, BoxGroup("ServerConf")] int _minimumPlayerToLaunch = 1;
        [SerializeField, BoxGroup("ServerConf")] int _roomSize = 2;

        public event Action<ulong, string> OnPlayerConnected;
        public event Action<ulong> OnRemovePlayer;

        #region NetworkVariables
        public NetworkVariable<int> Timeout { get; set; } = new NetworkVariable<int>(
            20,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);
        public NetworkList<PlayerSlot> PlayerSlots { get; set; }
        #endregion

        Trigger TimeoutTrigger { get; set; }

        int PlayerCount => PlayerSlots.Count;
        bool IsAllPlayersReady
        {
            get
            {
                if (PlayerSlots == null) return false;
                if (PlayerCount < _minimumPlayerToLaunch) return false;

                foreach (var el in PlayerSlots)
                {
                    if (el.IsReady == false) return false;
                }
                return true;
            }
        }

        void Awake()
        {
            PlayerSlots = new NetworkList<PlayerSlot>(new PlayerSlot[] { },
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Server);
        }

        public IEnumerator LaunchLobby()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += ApproveConnexion;
            OnPlayerConnected += AddNewClient;
            NetworkManager.Singleton.OnClientDisconnectCallback += RemoveClient;

            // Prepare Timeout
            StartCoroutine(TimeoutRoutine());
            IEnumerator TimeoutRoutine()
            {
                TimeoutTrigger = new Trigger();
                void ResetTimeout() => Timeout.Value = _timeout;
                ResetTimeout();
                var waiter = new WaitForSeconds(1f);
                while (Timeout.Value > 0)
                {
                    yield return waiter;
                    if (PlayerSlots.Count <= 1)
                    {
                        ResetTimeout();
                        continue;
                    }
                    Timeout.Value = Timeout.Value - 1; ;
                }
                TimeoutTrigger.Activate();
                yield break;
            }

            var waiter = new WaitForSeconds(1f);
            Debug.Log("[Lobby] Waiting for players ...");
            while (IsAllPlayersReady == false && TimeoutTrigger.IsActivated()==false)
            {
                yield return waiter;
            }

            NetworkManager.Singleton.ConnectionApprovalCallback -= ApproveConnexion;
            OnPlayerConnected -= AddNewClient;
            NetworkManager.Singleton.OnClientDisconnectCallback -= RemoveClient;

            Debug.Log("[Lobby] Launch");
            yield break;
        }

        void ApproveConnexion(byte[] data, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
        {
            if (PlayerSlots.Count >= _roomSize)
            {
                callback?.Invoke(false, null, false, null, null);
                return;
            }

            string playerName = Encoding.ASCII.GetString(data);
            Debug.Log($"new Player ! {playerName} {clientId}");

            OnPlayerConnected?.Invoke(clientId, playerName);
            callback?.Invoke(true, null, true, null, null);
        }

        void AddNewClient(ulong id, string name)
        {
            PlayerSlots.Add(new PlayerSlot() { ID = id, Name = new ForceNetworkSerializeByMemcpy<FixedString32Bytes>(name), IsReady = false });
        }

        void RemoveClient(ulong id)
        {
            PlayerSlots.Remove(i=>i.ID==id);
            OnRemovePlayer?.Invoke(id);
            RemoveOtherPlayer_ClientRPC(id);
        }

        [ClientRpc]
        void RemoveOtherPlayer_ClientRPC(ulong id)
        {
            OnRemovePlayer?.Invoke(id);
        }

        [ServerRpc(RequireOwnership = false)]
        public void MarkAsReady_ServerRPC(ulong id)
        {
            if (IsServer == false) return;
            var idx = PlayerSlots.FindIndex(i => i.ID == id);
            var old = PlayerSlots[idx];
            PlayerSlots[idx] = new PlayerSlot() { ID = old.ID, Name = old.Name, IsReady = true };
        }

    }
}
