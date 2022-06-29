using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using static RedStudio.Battle10.LobbyEntity;

namespace RedStudio.Battle10
{
    public class GameEntity : NetworkBehaviour
    {
        public static GameEntity Singleton { get; private set; }    // Bad dirty singleton but hey it's a 10 days game
        #region Types
        [Serializable] public struct LocalPlayerData : IEquatable<LocalPlayerData>, INetworkSerializable
        {
            public ulong PlayerID;
            public ulong Rank;
            public ulong Score;
            public bool IsAlive => Rank == default;

            public LocalPlayerData PlayerAsWinner(ulong newRank, ulong deltaScore) => new LocalPlayerData()
            {
                PlayerID = PlayerID,
                Rank = newRank,
                Score = Score + deltaScore
            };
            public LocalPlayerData UpdateRanking(ulong newRank) => new LocalPlayerData()
            {
                PlayerID = PlayerID,
                Rank = newRank,
                Score = Score
            };
            public LocalPlayerData UpdateScore(ulong delta) => new LocalPlayerData()
            {
                PlayerID = PlayerID,
                Rank = Rank,
                Score = Score+delta
            };

            #region InterfaceImplementation
            bool IEquatable<LocalPlayerData>.Equals(LocalPlayerData other) => this.PlayerID == other.PlayerID;
            void INetworkSerializable.NetworkSerialize<T>(BufferSerializer<T> serializer)
            {
                serializer.SerializeValue(ref PlayerID);
                serializer.SerializeValue(ref Rank);
                serializer.SerializeValue(ref Score);
            }
            #endregion
        }

        #endregion

        [SerializeField, BoxGroup("Data")] LocalPlayersRef _playerRef;
        [SerializeField, BoxGroup("Data")] ObservableSO _endGameEvent;
        [SerializeField, BoxGroup("Prefab")] NetworkPlayerController _playerPrefab;

        NetworkList<LocalPlayerData> _globalPlayerData = new NetworkList<LocalPlayerData>(
            new List<LocalPlayerData>(),
            readPerm: NetworkVariableReadPermission.Everyone,
            writePerm: NetworkVariableWritePermission.Server);

        public MapData MapData { get; private set; }
        public List<NetworkObject> DynamicNetworkObjects { get; private set; }
        public IEnumerable<PlayerNetwork> PlayersInGame => _playerRef.Players;
        public NetworkList<LocalPlayerData> GlobalPlayerData => _globalPlayerData;
        

        public event UnityAction<LocalPlayerData> OnPlayerDied;
        public event UnityAction<LocalPlayerData> OnPlayerOffline;

        public IEnumerable<(PlayerNetwork, LocalPlayerData)> PlayersWithData  
            =>_playerRef.Players.Select(p => (p, _globalPlayerData[_globalPlayerData.FindIndex(i => i.PlayerID == p.OwnerClientId)]));
        public ulong GetNextRank
        {
            get 
            {
                var tmp = _globalPlayerData.ToEnumerable().Where(i => i.Rank != default);
                if(tmp.Count()<=0)  // No player already dead
                {
                    return (ulong)_globalPlayerData.Count;
                }
                else    // or take the best ranking
                {
                    return tmp.Min(i => i.Rank) - 1;
                }
            }
        }
        public bool IsGameCompleted => PlayersWithData.Count(i => i.Item2.IsAlive) <= 1;

        #region Server

        public IEnumerator LaunchGame()
        {
            if (!IsServer) yield break;
            yield return null;

            while ((MapData = FindObjectOfType<MapData>()) == null)
            {
                yield return null;
            }

            // Spawn Players 
            Debug.Log("[Game] Spawn players");
            foreach ((PlayerNetwork player,Transform spawn) pack in MapData.AssignSpawnToPlayer(_playerRef.Players))
            {
                NetworkPlayerController p = Instantiate(_playerPrefab, pack.Item2.position, Quaternion.identity, MapData.PlayerRoot);
                p.NetworkObject.SpawnAsPlayerObject(pack.player.OwnerClientId, true);
                p.Init(this, pack.player);
                p.OnPlayerDeath += RegisterPlayerDeath;
            }

            // Spawn NetworkObjects
            Debug.Log("[Game] First object spawn");
            var tmp = MapData.InitialObjectSpawn().ToList();
            foreach (var el in MapData.InitialObjectSpawn())
            {
                var no = Instantiate(el.Item1, el.Item2, Quaternion.identity, MapData.ObjectsParent);
                no.Spawn();
            }

            // Prepare leaderbord with data
            foreach (var el in _playerRef.Players.Select(i => new LocalPlayerData() { 
                PlayerID = i.OwnerClientId, Rank = default, Score = 10 }))
            {
                _globalPlayerData.Add(el);
            }
            MapData.LeaderboardUI.Warmup(this);
            LeaderboardWarmup_ClientRPC();

            // Main loop to wait the final player
            while (IsGameCompleted == false)
            {
                yield return null;
            }

            // Find the last player
            var idx = _globalPlayerData.FindIndex(i => i.Rank == default);
            _globalPlayerData[idx] = _globalPlayerData[idx].PlayerAsWinner(GetNextRank, 100);
            SendEndGameEvent_ClientRPC();

            // Clean
            foreach (PlayerNetwork el in _playerRef.Players)
            {
                el.PlayerInGame.OnPlayerDeath -= RegisterPlayerDeath;
            }

            yield return new WaitForSeconds(60 * 3);
            Debug.Log("[Game] End of game. Close session");
            yield break;
        }

        void RegisterPlayerDeath((NetworkPlayerController killed, NetworkPlayerController killer) data)
        {
            if (IsServer == false) return;  // Server only

            // Assign Rank to player
            var idx = _globalPlayerData.FindIndex(i => i.PlayerID == data.killed.OwnerClientId);
            if (idx <= -1) return; // ERROR
            _globalPlayerData[idx] = _globalPlayerData[idx].UpdateRanking(GetNextRank);

            // Update killer's score
            idx = _globalPlayerData.FindIndex(i => i.PlayerID == data.killer.OwnerClientId);
            if(idx <= -1) return;   // ERROR
            _globalPlayerData[idx] = _globalPlayerData[idx].UpdateScore(100);

            Debug.Log("Player Global data upated");
        }
        #endregion

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            Singleton = this;       // Allow override
        }

        #region Client only
        [ClientRpc]
        void LeaderboardWarmup_ClientRPC()
        {
            StartCoroutine(WaitMapDataAndInitLeaderboard());
            IEnumerator WaitMapDataAndInitLeaderboard()
            {
                // Client must search MapData during scene loading
                while ((MapData = FindObjectOfType<MapData>()) == null) yield return null;
                // Then launch leaderboard warmup
                MapData.LeaderboardUI.Warmup(this);
                yield break;
            }
        }

        [ClientRpc]
        void SendEndGameEvent_ClientRPC()
        {
            (_endGameEvent as IObservableFire).Invoke();
            if (_playerRef.CurrentPlayer.PlayerInGame.IsAlive == false) return;
            
            _playerRef.CurrentPlayer.PlayerInGame.PlayerWin();
        }

        #endregion

    }
}
