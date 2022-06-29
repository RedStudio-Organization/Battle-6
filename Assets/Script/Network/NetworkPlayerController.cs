using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace RedStudio.Battle10
{
    [DefaultExecutionOrder(0)] // before client component
    public class NetworkPlayerController : NetworkBehaviour
    {
        [SerializeField, BoxGroup("External Ref")] LocalPlayersRef _players;

        [SerializeField, BoxGroup("Internal Ref")] Entity _entity;
        [SerializeField, BoxGroup("Internal Ref")] EntityLife _life;
        [SerializeField, BoxGroup("Internal Ref")] InputToActions _input;
        [SerializeField, BoxGroup("Internal Ref")] Rigidbody2D _rb;
        [SerializeField, BoxGroup("Internal Ref")] EntityOrientation _orientation;

        [SerializeField, BoxGroup("Events")] ObservableSO _onGameOverEvent;
        [SerializeField, BoxGroup("Events")] ObservableSO _onWinEvent;
        [SerializeField, BoxGroup("Events")] UnityEvent _onLocalPlayer;
        [SerializeField, BoxGroup("Events")] UnityEvent _onRemotePlayer;

        Vector2 _nextPosition;
        float _nextAxisRotation;

        /// <summary>
        /// Item1 is killed by Item2
        /// </summary>
        public event UnityAction<(NetworkPlayerController, NetworkPlayerController)> OnPlayerDeath;

        public GameEntity Master { get; private set; }
        public PlayerNetwork PlayerNetwork { get; private set; }

        public Entity Entity => _entity;
        IObservableFire GameOverFire => _onGameOverEvent;
        IObservableFire WinFire => _onWinEvent;
        public bool IsMainPlayer => !IsServer && IsOwner;

        public bool IsAlive => _life.IsAlive;

        internal NetworkPlayerController Init(GameEntity gameEntity, PlayerNetwork player)
        {
            Master = gameEntity;
            PlayerNetwork = player;
            return this;
        }

        [ServerRpc]
        public void SendNextPosition_ServerRPC(Vector2 n)
        {
            _nextPosition = n;
        }

        [ServerRpc]
        public void SendNextRotation_ServerRPC(float axisValue)
        {
            _nextAxisRotation = axisValue;
        }

        void FixedUpdate()
        {
            if (IsServer == false) return;

            transform.position = _nextPosition;
            transform.rotation = Quaternion.Euler(0,0,_nextAxisRotation);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            name = $"{_players.Players.First(i => this.OwnerClientId == i.OwnerClientId)}_PlayerInstance";
            _input.gameObject.SetActive(IsOwner);
            _players.Players.First(i => i.OwnerClientId == this.OwnerClientId).InjectEntity(this);

            // Manage events
            _life.OnDeath += PlayerDeath;

            if (IsMainPlayer)
            {
                _input.ManualBind();
                _orientation.OnOrientationUpdate += SendNextRotation_ServerRPC;
                _onLocalPlayer?.Invoke();
            }
            else
            {
                _onRemotePlayer?.Invoke();
            }
        }

        void PlayerDeath()
        {
            if (IsMainPlayer)
            {
                _input.Unbind();
                GameOverFire.Invoke();
            }
                OnPlayerDeath?.Invoke((this,
                    IsServer?_life.LastHit.Owner.GetComponent<NetworkPlayerController>():null));

        }
        public void PlayerWin()
        {
            if(IsMainPlayer)
            {
                _input.Unbind();
                WinFire.Invoke();
            }
        }

        public override void OnDestroy()
        {
            _life.OnDeath -= PlayerDeath;
            if (IsMainPlayer)
            {
                _orientation.OnOrientationUpdate -= SendNextRotation_ServerRPC;
            }
        }

    }
}
