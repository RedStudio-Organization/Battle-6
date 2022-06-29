using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RedStudio.Battle10
{
    public class DynamicRename : MonoBehaviour
    {
        public static string PlayerName { get; private set; }
        public static int PlayerLeaderboardScore { get; private set; }

        [SerializeField] TMPro.TMP_InputField _field;
        public TMP_InputField Field => _field;

        Coroutine Current { get; set; }
        bool Waiting { get; set; }

        public IEnumerator Init()
        {
            // Update routine
            _field.onEndEdit.AddListener(LaunchUpdate);

            // Find playerName to show current Name
            Waiting = true;
            PlayerName = "";
            _field.text = "...";
            yield return PlayfabAsCoroutine.GetPlayerName(n=> PlayerName = n);
            StartCoroutine(PlayfabAsCoroutine.GetLeaderboardValue(r => PlayerLeaderboardScore = r)); // Load data but don't block flow
            _field.text = PlayerName;
            Waiting = false;
            yield break;
        }

        void OnDestroy()
        {
            _field.onEndEdit.RemoveListener(LaunchUpdate);
        }

        void LaunchUpdate(string newName)
        {
            if (Waiting) return;
            if(Current!=null) StopCoroutine(Current);
            Current = StartCoroutine(Update());
            IEnumerator Update()
            {
                yield return PlayfabAsCoroutine.RenamePlayer(newName);
                Current = null;
                PlayerName = newName;
                yield break;
            }
        }
    }
}
