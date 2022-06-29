using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace RedStudio.Battle10
{
    public class NetworkInteraction : NetworkBehaviour
    {
        [SerializeField] LocalPlayersRef _players;
        [SerializeField] Interaction _interaction;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _interaction.OnInteraction += SendInteractionEvent;
        }

        void SendInteractionEvent(PlayerInteraction pi)
        {
            if (IsServer) return;
            var playerId = pi.Master.GetComponent<NetworkObject>().OwnerClientId;
            BroadcastInteractionEvent_ServerRPC(playerId);
        }

        [ServerRpc(RequireOwnership = false)]
        void BroadcastInteractionEvent_ServerRPC(ulong playerId)
        {
            ReportInteraction_ClientRPC(playerId);
            RemoteInteract(playerId);
        }

        void RemoteInteract(ulong playerId)
        {
            _players.Players
                .FirstOrDefault(i => i.OwnerClientId == playerId)
                ?.PlayerInGame
                ?.GetComponentInChildren<PlayerInteraction>()
                .ManualInteraction(_interaction);
        }

        [ClientRpc]
        void ReportInteraction_ClientRPC(ulong playerId)
        {
            if (NetworkManager.Singleton.LocalClientId == playerId) return; // Avoid double interaction call from client that interacted
            RemoteInteract(playerId);
        }

#if UNITY_EDITOR
        void Reset()
        {
            //_interactable = GetComponent<Interactable>() ?? GetComponentInChildren<Interactable>() ?? GetComponentInParent<Interactable>();
        }
#endif
    }
}
