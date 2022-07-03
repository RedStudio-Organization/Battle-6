using PlayFab;
using PlayFab.MultiplayerModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace RedStudio.Battle10
{
    public class MatchmakingUI : MonoBehaviour
    {
        [SerializeField] Transform _root;
        [SerializeField] Button _cancelButton;

        public event Action OnMatchmakingStart;
        public event Action OnMatchmakingFound;
        public event Action OnMatchmakingCanceled;
        public event Action OnMatchmakingLaunchGame;

        Gameplay Gameplay { get; set; }

        public IEnumerator LaunchMatchmaking(Gameplay g)
        {
            Gameplay = g;
            _root.gameObject.SetActive(true);
            OnMatchmakingStart?.Invoke();

            // Prepare UI
            Trigger cancelFromUser = new Trigger();
            void ActivateCancelToken() => cancelFromUser.Activate();
            _cancelButton.onClick.AddListener(ActivateCancelToken);

            // Launch Matchmaking
            (GetMatchResult matchResult, PlayFabError error, bool cancelled) result = default;
            yield return PlayfabAsCoroutine.Matchmaking(this, data => result = data, cancelFromUser);

            // Clean
            _cancelButton.onClick.RemoveListener(ActivateCancelToken);

            // Cancel or Error behavior
            if (result.error != null || result.cancelled)
            {
                OnMatchmakingCanceled?.Invoke();
                CloseMatchmaking();
                yield break;
            }

            OnMatchmakingFound?.Invoke();
            // Match : Show match + Connect to server
            Debug.Log($"[Matchmaking] Players matched : {result.matchResult.Members.Select(i=>i.Entity.Id+"|").Aggregate((a,b)=> a+b)}");

            CloseMatchmaking();
            Gameplay.ConnectToServer(result.matchResult);

            OnMatchmakingLaunchGame?.Invoke();
            yield return Gameplay.WaitEndGame();
            yield break;
        }

        void CloseMatchmaking()
        {
            _root.gameObject.SetActive(false);
        }

    }
}
