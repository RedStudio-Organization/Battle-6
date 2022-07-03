using PlayFab;
using PlayFab.MultiplayerModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RedStudio.Battle10
{
    public class MatchmakingUI : MonoBehaviour
    {

        [SerializeField] Transform _root;


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

            // Launch Matchmaking
            Trigger cancelFromUser = new Trigger();
            (GetMatchResult matchResult, PlayFabError error, bool cancelled) r = default;
            yield return PlayfabAsCoroutine.Matchmaking(data =>
            {
                r = data;
            }, cancelFromUser);
            // Cancel or Error
            if(r.error != null || r.cancelled)
            {
                CloseMatchmaking();
                yield break;
            }

            // Match : Show match + Connect to server
            Debug.Log($"[Matchmaking] Players matched : {r.matchResult.Members.Select(i=>i.Entity.Id+"|").Aggregate((a,b)=> a+b)}");

            CloseMatchmaking();

            //Gameplay.ConnectToServer(r.matchResult);

            while (true) yield return null;

            yield break;
        }

        void CloseMatchmaking()
        {
            _root.gameObject.SetActive(false);
        }

    }
}
