using NaughtyAttributes;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RedStudio.Battle10
{
    public class Gameplay : MonoBehaviour
    {
        #region InternalTypes
        public enum BuildTarget { Host = 0, Client = 1 };
        public enum GameState { NULL = -1, Lobby = 0, Game = 1 };
        #endregion

        [SerializeField, BoxGroup("Managers")] NetworkManager _network;
        [SerializeField, BoxGroup("Managers")] UnityTransport _transport;

        [SerializeField, BoxGroup("Entities")] LobbyEntity _lobbyEntity;
        [SerializeField, BoxGroup("Entities")] GameEntity _gameEntity;

        [SerializeField, BoxGroup("ServerConf")] bool _EDITOR_allowForceLocalServerInUnity;
        [SerializeField, BoxGroup("ServerConf")] bool _isServer = true;
        [SerializeField, BoxGroup("ServerConf"), ShowIf(nameof(_isServer))] bool _localServer;

        [ShowIf(nameof(IsPlayfabServer))]
        [SerializeField, BoxGroup("PlayfabServerConf")]
        string _portName = "BattleRoyal2D";

        [SerializeField, BoxGroup("PlayfabServerConf"), ShowIf(nameof(IsPlayfabServer))] 
        PlayFabMultiplayerAgentView _heartbeat;

        [SerializeField, BoxGroup("PlayfabServerConf"), ShowIf(nameof(IsPlayfabServer))] 
        List<AzureRegion> _playfabRegions = new List<AzureRegion>() { AzureRegion.NorthEurope };

        [SerializeField, BoxGroup("PlayfabClient"), HideIf(nameof(_isServer))] 
        bool _askForPlayfabServer = false;

        [ShowIf(EConditionOperator.And, nameof(IsClient), nameof(_askForPlayfabServer))]
        [SerializeField, BoxGroup("PlayfabClient")] 
        string _buildTargetOnPlayfab = "";

        [SerializeField, BoxGroup("Client"), ShowIf(nameof(TargetSpecificMachineAsClient))] 
        string _adressToJoin = "128.0.0.0";

        [SerializeField, BoxGroup("Client"), ShowIf(nameof(TargetSpecificMachineAsClient))] 
        int _portToJoin = 8080;

        [SerializeField, Scene] string _mainMenuScene;
        [SerializeField, Scene] string _lobbyScene;
        [SerializeField, Scene] string _gameScene;
        [SerializeField] ObservableSO _closeGame;
        [SerializeField] LocalPlayersRef _playerRef;

        List<PlayFab.MultiplayerAgent.Model.ConnectedPlayer> ConnectedPlayers { get; set; }
        public GameState CurrentState { get; private set; }
        public LobbyEntity CurrentLobby { get; private set; }
        public GameEntity CurrentGame { get; private set; }

        bool IsLocalServerProject() => Application.dataPath.Contains("Remote")==false;
        bool TargetSpecificMachineAsClient() => !_isServer && _askForPlayfabServer == false;
        bool IsPlayfabServer() => _isServer && !_localServer;
        bool IsClient => !_isServer;

        IEnumerator Start()
        {
            DontDestroyOnLoad(gameObject);
            _playerRef.Init();

#if UNITY_EDITOR
            if (IsLocalServerProject() && _EDITOR_allowForceLocalServerInUnity)
            {
                _isServer = true;
                _localServer = true;
            }
#endif
            if (_isServer) // auto server editor
            {
                yield return RunServer( ServerType.BuildConf );
                yield break;
            }
            else
            {
                while(true)
                {
                    yield return SceneManager.LoadSceneAsync(_mainMenuScene, LoadSceneMode.Single);
                    MainMenuUI menuUI = null;
                    while ((menuUI = FindObjectOfType<MainMenuUI>()) == null) yield return null;
                    yield return menuUI.Launch(this, _buildTargetOnPlayfab);
                    yield return WaitEndGame();
                }
            }
        }

        #region Server
        public enum ServerType { BuildConf = -1, Local = 0, Playfab = 1 }
        
        public IEnumerator RunServer(ServerType serverType)
        {
            // Unload unexpected scenes during development
            yield return SceneManager.GetSceneByName(_mainMenuScene).isLoaded ? SceneManager.UnloadSceneAsync(_mainMenuScene) : null;
            yield return SceneManager.GetSceneByName(_lobbyScene).isLoaded ? SceneManager.UnloadSceneAsync(_lobbyScene) : null;
            yield return SceneManager.GetSceneByName(_gameScene).isLoaded ? SceneManager.UnloadSceneAsync(_gameScene) : null;

            // GSDK setup
            if (serverType == ServerType.Playfab || _localServer == false)
            {
                ConnectedPlayers = new List<PlayFab.MultiplayerAgent.Model.ConnectedPlayer>();
                Trigger serverStart = new Trigger();
                void OnAgentError(string e) => Debug.Log(e);
                void OnServerActive() => serverStart.Activate();

                _heartbeat.enabled = true;
                PlayFabMultiplayerAgentAPI.Start();
                PlayFabMultiplayerAgentAPI.OnServerActiveCallback += OnServerActive;
                PlayFabMultiplayerAgentAPI.OnAgentErrorCallback += OnAgentError;
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;

                // Setup port
                _transport.ConnectionData = new UnityTransport.ConnectionAddressData()
                {
                    ServerListenAddress = "0.0.0.0",
                    Port = (ushort)PlayFabMultiplayerAgentAPI.GetGameServerConnectionInfo()
                        .GamePortsConfiguration
                        .First(i => i.Name == _portName)
                        .ServerListeningPort
                };

                // Wait readyforplayer + ServerActive
                yield return new WaitForSeconds(1f);
                PlayFabMultiplayerAgentAPI.ReadyForPlayers();
                yield return new WaitForSeconds(1f);
                while (serverStart.IsActivated() == false) yield return null;
            }
            else // Local server
            {
                _transport.ConnectionData = new UnityTransport.ConnectionAddressData()
                {
                    ServerListenAddress = "0.0.0.0",
                    Port = 8080
                };
            }

            // start Netcode server
            Debug.Log("[Network] Start Server mode");
            NetworkManager.Singleton.ConnectionApprovalCallback += DefaultApproveConnection;
            _network.StartServer();

            // Lobby
            CurrentState = GameState.Lobby;
            (CurrentLobby = Instantiate(_lobbyEntity)).NetworkObject.Spawn(false);
            NetworkManager.Singleton.SceneManager.LoadScene(_lobbyScene, LoadSceneMode.Single);
            NetworkManager.Singleton.ConnectionApprovalCallback -= DefaultApproveConnection;
            yield return CurrentLobby.LaunchLobby();
            NetworkManager.Singleton.ConnectionApprovalCallback += DefaultApproveConnection;
            CurrentLobby.NetworkObject.Despawn(true);

            // Game
            CurrentState = GameState.Game;
            CurrentGame = Instantiate(_gameEntity);
            //yield return null;  // Must wait awake method
            CurrentGame.NetworkObject.Spawn(false);
            NetworkManager.Singleton.SceneManager.LoadScene(_gameScene, LoadSceneMode.Single);
            yield return CurrentGame.LaunchGame();

            // Final Score

            // Quit room
            Application.Quit();
            yield break;
        }
        
        void DefaultApproveConnection(byte[] data, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
        {
            if (CurrentState != GameState.Lobby) callback?.Invoke(false, null, false, null, null);
        }
        void OnClientConnected(ulong obj)
        {
            ConnectedPlayers.Add(new PlayFab.MultiplayerAgent.Model.ConnectedPlayer(obj.ToString()));
            PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers(ConnectedPlayers);
        }
        void OnClientDisconnect(ulong obj)
        {
            PlayFab.MultiplayerAgent.Model.ConnectedPlayer player = ConnectedPlayers.Find(x => x.PlayerId == obj.ToString());
            ConnectedPlayers.Remove(player);
            PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers(ConnectedPlayers);

            if (ConnectedPlayers.Count <= 0)
            {
                Debug.Log("[Server] No more players. Shutdown.");
                NetworkManager.Singleton.Shutdown();
                Application.Quit();
            }
        }
        #endregion

        #region Client
        public static LoginResult PlayerLogin { get; private set; }
        public IEnumerator Login(string playerName)
        {
            Debug.Log("[Network] Start Client mode");
            _network.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(playerName);

            Debug.Log("[Network] Client : Target");
            LoginResult lr = null;

            // Login
            yield return PlayfabAsCoroutine.LoginCustomID((l, e) => lr = l);
            if (lr == null) { } // Error

            PlayerLogin = lr;   // WIP store Login in static field to access it for test

            yield break;
        }

        public void ConnectToServer(GetMatchResult match)
        {
            _network.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(DynamicRename.PlayerName);
            _transport.ConnectionData = new UnityTransport.ConnectionAddressData()
            {
                Address = match.ServerDetails.IPV4Address,
                Port = (ushort)match.ServerDetails.Ports.First().Num,
                ServerListenAddress = "0.0.0.0",
            };
            _network.StartClient();
        }

        public IEnumerator JoinRoom(string playerName, string ip, int port)
        {
            PlayfabAsCoroutine.ConnectToServer(_network, _transport, ip, (ushort)port);
            yield break;
        }

        public IEnumerator WaitEndGame()
        {
            Trigger t = new Trigger();
            _closeGame.OnInvoke += CloseGame;
            void CloseGame() => t.Activate();
            while (t.IsActivated() == false)
            {
                yield return null;
            }
            NetworkManager.Singleton.Shutdown();
            yield break;
        }
        #endregion

        #region Editor
#if UNITY_EDITOR

        [Button("Setup for client to local server")]
        void SetupForLocalServer()
        {
            _isServer = false;
            _localServer = false;
            _adressToJoin = _transport.ConnectionData.Address;
            _portToJoin = _transport.ConnectionData.Port;
        }
#endif
        #endregion

    }
}
