using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace RedStudio.Battle10
{
    public class PlayerNetwork : NetworkBehaviour
    {
        [SerializeField, Foldout("Extern")] LocalPlayersRef _allPlayers;
        [SerializeField, Foldout("Extern")] EntityReference _currentPlayer;
        [SerializeField, Foldout("Extern")] ObservableSO _winEvent;

        public NetworkPlayerController PlayerInGame { get; private set; }
        public bool IsMainPlayer => _allPlayers.CurrentPlayer == this;

        public NetworkVariable<ForceNetworkSerializeByMemcpy<FixedString64Bytes>> PlayerName;
        public NetworkVariable<int> InternalLeaderboard;

        void UpdateName(ForceNetworkSerializeByMemcpy<FixedString64Bytes> p, ForceNetworkSerializeByMemcpy<FixedString64Bytes> c)
            => UpdateName(c.Value.ToString());
        void UpdateName(string c)
            => gameObject.name = c;

        void Awake()
        {
            PlayerName = new NetworkVariable<ForceNetworkSerializeByMemcpy<FixedString64Bytes>>(
                    readPerm: NetworkVariableReadPermission.Everyone,
                    writePerm: NetworkVariableWritePermission.Server);

            InternalLeaderboard = new NetworkVariable<int>(
                readPerm: NetworkVariableReadPermission.Everyone,
                writePerm: NetworkVariableWritePermission.Owner);
        }

        public void InjectEntity(NetworkPlayerController e)
        {
            PlayerInGame = e;
            if(NetworkManager.Singleton.LocalClientId == OwnerClientId)
            {
                (_currentPlayer as IReferenceHead<Entity>).Set(e.Entity);
                PlayerInGame.OnPlayerDeath += SendFinalStat;
                _winEvent.OnInvoke += WinSendStat;
            }
        }

        void WinSendStat() => SendFinalStat();
        void SendFinalStat((NetworkPlayerController, NetworkPlayerController) arg0) => SendFinalStat();
        void SendFinalStat()
        {
            if (PlayerInGame.OwnerClientId != NetworkManager.Singleton.LocalClientId) return;// Error

            StartCoroutine(LeaderboardInsertion());
            IEnumerator LeaderboardInsertion()
            {
                yield return PlayfabAsCoroutine.LeaderboardInsertion(
                    GameEntity.Singleton.PlayersWithData.First(i => i.Item1 == this).Item2);
                yield break;
            }
        }

        public override void OnNetworkSpawn()
        {
            _allPlayers.AddPlayer(this);

            if (IsOwner)
            {
                UpdatePlayerName_ServerRPC($"{DynamicRename.PlayerName}");
                InternalLeaderboard.Value = DynamicRename.PlayerLeaderboardScore;
            }

            UpdateName(PlayerName.Value.ToString());
            PlayerName.OnValueChanged += UpdateName;
        }
        public override void OnNetworkDespawn()
        {
            _allPlayers.RemovePlayer(OwnerClientId);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            PlayerName.OnValueChanged -= UpdateName;
            if (NetworkManager.Singleton?.LocalClientId == OwnerClientId)
            {
                if(PlayerInGame!=null) PlayerInGame.OnPlayerDeath -= SendFinalStat;
                if (_winEvent != null) _winEvent.OnInvoke -= WinSendStat;
            }
        }

        #region Server
        [ServerRpc]
        void UpdatePlayerName_ServerRPC(string n)
        {
            // TODO : Moderation

            //
            PlayerName.Value = new ForceNetworkSerializeByMemcpy<FixedString64Bytes>(n);
        }
        #endregion

    }
}
