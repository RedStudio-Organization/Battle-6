using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace RedStudio.Battle10
{
    public class DynamicSetCamera : MonoBehaviour
    {
        [SerializeField] CinemachineVirtualCamera _camera;
        [SerializeField] EntityReference _playerRef;

        [SerializeField] UnityEvent _onServerCam;

        void Start()
        {
            if(NetworkManager.Singleton.IsServer)
            {
                _onServerCam?.Invoke();
                return;
            }

            UpdateTarget(_playerRef.Acquire());
            _playerRef.OnValueChanged += UpdateTarget;
        }

        void OnDestroy()
        {
            _playerRef.OnValueChanged -= UpdateTarget;
        }

        void UpdateTarget(Entity obj)
        {
            if (obj == null) 
            {
                _camera.Follow = null; 
                return;
            }
            _camera.Follow = obj?.transform;
        }
    }
}
