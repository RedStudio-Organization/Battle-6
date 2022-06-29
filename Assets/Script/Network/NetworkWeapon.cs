using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace RedStudio.Battle10
{
    public class NetworkWeapon : NetworkBehaviour
    {
        [SerializeField] Weapon _weapon;
        NetworkPlayerController OwnerNetwork => _weapon.Owner?.Master.GetComponent<NetworkPlayerController>();
        bool IsUsedByLocalPlayer => (OwnerNetwork?.OwnerClientId ?? ulong.MaxValue) == NetworkManager.Singleton.LocalClientId;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _weapon.OnUseStart += _weapon_OnUseStart;
            _weapon.OnUseStop += _weapon_OnUseStop;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            _weapon.OnUseStart -= _weapon_OnUseStart;
            _weapon.OnUseStop -= _weapon_OnUseStop;
        }

        void _weapon_OnUseStart()
        {
            if (!IsUsedByLocalPlayer) return;   // remote call, avoid broadcast
            BroadcastUseEvent_ServerRPC(NetworkManager.Singleton.LocalClientId, true);
        }
        void _weapon_OnUseStop()
        {
            if (!IsUsedByLocalPlayer) return; // remote call, avoid broadcast
            BroadcastUseEvent_ServerRPC(NetworkManager.Singleton.LocalClientId, false);
        }

        [ServerRpc(RequireOwnership = false)]
        void BroadcastUseEvent_ServerRPC(ulong playerId, bool isStart)
        {
            ReceiveUseStartEvent_ClientRPC(playerId, isStart);

            // Server side use
            if (isStart)
                _weapon.UseStart();
            else
                _weapon.UseStop();
        }

        [ClientRpc]
        void ReceiveUseStartEvent_ClientRPC(ulong playerId, bool isStart)
        {
            if (playerId == NetworkManager.Singleton.LocalClientId) return; // Avoid double event
            if (isStart)
                _weapon.UseStart();
            else
                _weapon.UseStop();
        }

        #region Editor
        void Reset()
        {
            _weapon = _weapon ?? GetComponent<Weapon>() ?? GetComponentInParent<Weapon>();
        }
        #endregion
    }
}
