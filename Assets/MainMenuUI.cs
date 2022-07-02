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
        public enum ButtonPressed { NULL=-1, Server=0, Create=1, Join=2, Leaderboard=3, PlayfabMatchmaking=4 }
        [Serializable] class LangageButton
        {
            public Button Button;
            public Locale Locale;
        }
        [Serializable] class MenuButton
        {
            public Button Button;
            public ButtonPressed Value;
        }
        #endregion
        
        [SerializeField, BoxGroup("UI")] Transform _root;
        [SerializeField, BoxGroup("UI")] GlobalLeaderboadUI _leaderboardUI;

        [SerializeField, BoxGroup("UI - Button")] LangageButton[] _langages;
        [SerializeField, BoxGroup("UI - Button")] MenuButton[] _menuSetup;

        [SerializeField, BoxGroup("UI - Fields")] TMP_InputField _buildID;
        [SerializeField, BoxGroup("UI - Fields")] DynamicRename _username;
        [SerializeField, BoxGroup("UI - Fields")] TMP_InputField _joinIP;
        [SerializeField, BoxGroup("UI - Fields")] TMP_InputField _joinPort;

        Gameplay Master { get; set; }

        string GetUsername => _username.Field.text.Substring(0, Math.Min(_username.Field.text.Length, 20));
        string GetIP => ValidateIPv4(_joinIP.text) ? _joinIP.text : string.Empty;
        int GetPort => int.TryParse(_joinPort.text, out int result) ? result : int.MinValue;

        bool ValidateIPv4(string ipString)
            => String.IsNullOrWhiteSpace(ipString) == false &&
                ipString.Split('.').Length == 4 &&
                ipString.Split('.').All(r => byte.TryParse(r, out byte tempForParsing));

        public IEnumerator Launch(Gameplay gameplay, string defaultBuildId)
        {
            Master = gameplay;

            yield return gameplay.Login(GetUsername);
            yield return _username.Init();

            // Setup ui
            ButtonPressed buttonPressed = ButtonPressed.NULL;
            _menuSetup.ForEach(i => i.Button?.onClick.AddListener(() => buttonPressed = i.Value));
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

                if (buttonPressed == ButtonPressed.NULL ) continue;
                switch (buttonPressed)
                {
                    // WIP Matchmaking
                    case ButtonPressed.PlayfabMatchmaking:
                        _root.gameObject.SetActive(false);
                        yield return Master.PlayfabMatchmaking();
                        break;

                    case ButtonPressed.Leaderboard:
                        yield return _leaderboardUI.LoadUI();
                        break;

#if false   // TMP Remove code
                    case ButtonPressed.Server:
                        _root.gameObject.SetActive(false);
                        yield return Master.RunServer(Gameplay.ServerType.Local);
                        yield break;
                    case ButtonPressed.Create:
                        _root.gameObject.SetActive(false);
                        yield return Master.AskPlayfabRomm(GetUsername, _buildID.text);
                        CurrentState = ButtonPressed.NULL;  // Clear button pressed
                        yield break;
                    case ButtonPressed.Join:
                        if(string.IsNullOrEmpty(GetIP)) { _joinIP.text = "error: not an ip"; break; }
                        if(GetPort == int.MinValue) { _joinPort.text = "error: a port"; break; }
                        _root.gameObject.SetActive(false);
                        yield return Master.JoinRoom(GetUsername, GetIP, GetPort);
                        CurrentState = ButtonPressed.NULL;  // Clear button pressed
                        yield break;
#endif
                    case ButtonPressed.NULL:
                    default:
                        continue;
                }
                buttonPressed = ButtonPressed.NULL;  // Clear button pressed
            }
        }

#region Editor
#if UNITY_EDITOR
        void Reset()
        {
            _menuSetup = new[]
            {
                new MenuButton() { Value= ButtonPressed.PlayfabMatchmaking },
                new MenuButton() { Value= ButtonPressed.Create },
                new MenuButton() { Value= ButtonPressed.Join },
                new MenuButton() { Value= ButtonPressed.Server},
                new MenuButton() { Value= ButtonPressed.Leaderboard},
            };
        }
#endif
#endregion

    }
}
