using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RedStudio.Battle10
{
    public class LeaderboardLineUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _rank;
        [SerializeField] TextMeshProUGUI _displayName;
        [SerializeField] TextMeshProUGUI _score;
        [SerializeField] Color _otherPlayerColor;
        [SerializeField] Color _localPlayerColor;

        bool IsLocalPlayer { get; set; }

        IEnumerable<TextMeshProUGUI> AllText()
        {
            yield return _rank;
            yield return _displayName;
            yield return _score;
        }

        public LeaderboardLineUI Init(int rank, string displayName, int score, bool isLocalPlayer)
        {
            _rank.text = rank.ToString();
            _displayName.text = displayName;
            _score.text = score.ToString();
            IsLocalPlayer = isLocalPlayer;

            var selectedColor = IsLocalPlayer ? _localPlayerColor : _otherPlayerColor;
            foreach(var el in AllText()) el.color = selectedColor;

            return this;
        }

    }
}
