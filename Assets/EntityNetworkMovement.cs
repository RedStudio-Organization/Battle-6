using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace RedStudio.Battle10
{
    public class EntityNetworkMovement : EntityMovement
    {
        [SerializeField] NetworkPlayerController _networkPlayerController;

        protected override void FixedUpdate()
        {
            // Empty
            if (_networkPlayerController.IsOwner)
            {
                _networkPlayerController.SendNextPosition_ServerRPC(_rb.transform.position + Direction.Value());
                //_networkPlayerController.NextPosition.Value = _rb.transform.position + Direction.Value();
            }
        }


    }
}
