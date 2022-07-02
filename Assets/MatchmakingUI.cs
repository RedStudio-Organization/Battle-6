using System;
using System.Collections;
using System.Collections.Generic;
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

            yield return Gameplay.PlayfabMatchmaking();
            



            _root.gameObject.SetActive(false);
            yield break;
        }



    }
}
