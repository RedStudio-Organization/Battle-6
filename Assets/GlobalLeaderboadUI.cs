using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RedStudio.Battle10
{
    public class GlobalLeaderboadUI : MonoBehaviour
    {
        [SerializeField] Transform _lineRoot;
        [SerializeField] LeaderboardLineUI _leaderboardLineUI;

        [SerializeField] Button _returnButton;
        [SerializeField] UnityEvent _onLaunch;
        [SerializeField] UnityEvent _onReturn;

        public IEnumerator LoadUI()
        {
            _onLaunch?.Invoke();

            // Prepare close
            Trigger returnTrigger = new Trigger();
            void ReturnButton() => returnTrigger.Activate();
            _returnButton.onClick.AddListener(ReturnButton);

            // Clean leaderboard old content
            _lineRoot.ToLinq().ForEach(t => Destroy(t.gameObject));

            // Make request
            GetLeaderboardAroundPlayerResult leaderboardContent = null;
            yield return PlayfabAsCoroutine.LeaderboardAcquisition(r => leaderboardContent = r);

            // Create new lines
            if (leaderboardContent != null)
            {
                foreach (var el in leaderboardContent.Leaderboard)
                {
                    var ui = Instantiate(_leaderboardLineUI, _lineRoot).Init(el.Position, el.DisplayName, el.StatValue, false);
                }
            }
            else { } // Error

            // Wait return
            while (returnTrigger.IsActivated() == false) yield return null;
            _onReturn?.Invoke();

            // Clean up
            _returnButton.onClick.RemoveListener(ReturnButton);
            yield break;
        }

    }
}
