using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RedStudio.Battle10
{
    public class MainMenuUI : MonoBehaviour
    {
        #region InternalTypes
        public enum ButtonPressed { NULL=-1, Server=0, Create=1, Join=2, Leaderboard=3 }

        [Serializable] class LangageButton
        {
            public Button Button;
            public Locale Locale;
        }

        #endregion
        
        [SerializeField, BoxGroup("UI")] Canvas _root;
        [SerializeField, BoxGroup("UI")] LeaderbordMainMenu _leaderboardUI;

        [SerializeField, BoxGroup("UI - Button")] Button _server;
        [SerializeField, BoxGroup("UI - Button")] Button _createRoom;
        [SerializeField, BoxGroup("UI - Button")] Button _joinRoom;
        [SerializeField, BoxGroup("UI - Button")] LangageButton[] _langages;    // TODO
        [SerializeField, BoxGroup("UI - Button")] Button _leaderboardButton;    

        [SerializeField, BoxGroup("UI - Fields")] TMP_InputField _buildID;
        [SerializeField, BoxGroup("UI - Fields")] DynamicRename _username;
        [SerializeField, BoxGroup("UI - Fields")] TMP_InputField _joinIP;
        [SerializeField, BoxGroup("UI - Fields")] TMP_InputField _joinPort;

        Gameplay Master { get; set; }
        ButtonPressed CurrentState { get; set; }

        void LaunchServer() => CurrentState = ButtonPressed.Server;
        void CreateRoom() => CurrentState = ButtonPressed.Create;
        void JoinRoom() => CurrentState = ButtonPressed.Join;
        void ShowLeaderboard() => CurrentState = ButtonPressed.Leaderboard;

        string GetUsername => _username.Field.text.Substring(0, Math.Min(_username.Field.text.Length, 20));
        string GetIP => ValidateIPv4(_joinIP.text) ? _joinIP.text : string.Empty;
        int GetPort => int.TryParse(_joinPort.text, out int result) ? result : int.MinValue;

        bool ValidateIPv4(string ipString)
        {
            if (String.IsNullOrWhiteSpace(ipString)) return false;

            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4) return false;

            byte tempForParsing;
            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }

        public IEnumerator Launch(Gameplay gameplay, string defaultBuildId)
        {
            Master = gameplay;
            CurrentState = ButtonPressed.NULL;

            yield return gameplay.Login(GetUsername);
            yield return _username.Init();

            // Setup ui
            _server.onClick.AddListener(LaunchServer);
            _createRoom.onClick.AddListener(CreateRoom);
            _joinRoom.onClick.AddListener(JoinRoom);
            _leaderboardButton.onClick.AddListener(ShowLeaderboard);
            _buildID.text = defaultBuildId;

            // Langage support (no cleanup bc it's local to scene and I use closure here)
            foreach(var el in _langages)
            {
                el.Button.onClick.AddListener(() => LocalizationSettings.Instance.SetSelectedLocale(el.Locale));
            }

            while (true)
            {
                yield return null;
                _root.gameObject.SetActive(true);

                switch (CurrentState)
                {
                    case ButtonPressed.Server:
                        _root.gameObject.SetActive(false);
                        yield return Master.RunServer( Gameplay.ServerType.Local );
                        yield break;
                    case ButtonPressed.Create:
                        CurrentState = ButtonPressed.NULL;  // Clear button pressed
                        _root.gameObject.SetActive(false);
                        yield return Master.AskPlayfabRomm(GetUsername, _buildID.text);
                        yield break;
                    case ButtonPressed.Join:
                        CurrentState = ButtonPressed.NULL;  // clear button pressed
                        if(string.IsNullOrEmpty(GetIP)) { _joinIP.text = "error: not an ip"; break; }
                        if(GetPort == int.MinValue) { _joinPort.text = "error: a port"; break; }

                        _root.gameObject.SetActive(false);
                        yield return Master.JoinRoom(GetUsername, GetIP, GetPort);
                        yield break;
                    case ButtonPressed.Leaderboard:
                        CurrentState = ButtonPressed.NULL;
                        yield return _leaderboardUI.LoadUI();
                        break;
                    case ButtonPressed.NULL:
                    default:
                        continue;
                }
            }
        }

        
        
    }
}
