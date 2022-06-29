using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NaughtyAttributes;
using UnityEngine.Events;
using static RedStudio.Battle10.GameEntity;

namespace RedStudio.Battle10
{
    public class LeaderboardSlotUI : MonoBehaviour
    {
        [SerializeField, BoxGroup("Common UI")] TextMeshProUGUI _playerName;

        [SerializeField, BoxGroup("AliveUI")] TextMeshProUGUI _currentScore;

        [SerializeField, BoxGroup("DeadUI")] TextMeshProUGUI _rank;
        [SerializeField, BoxGroup("DeadUI")] TextMeshProUGUI _appliedScore;
        [SerializeField, BoxGroup("DeadUI")] TextMeshProUGUI _leaderboardScore;

        [SerializeField, Foldout("Event")] UnityEvent _onAlive;
        [SerializeField, Foldout("Event")] UnityEvent _onDead;
        [SerializeField, Foldout("Event")] UnityEvent _onLosePlayer;
        [SerializeField, Foldout("Event")] UnityEvent _onWinPlayer;

        public bool IsAliveSlot { get; private set; }
        public PlayerNetwork PlayerNetwork { get; private set; }
        public LocalPlayerData LocalData { get; private set; }

        public LeaderboardSlotUI Init(PlayerNetwork pn, GameEntity.LocalPlayerData data)
        {
            PlayerNetwork = pn;
            LocalData = data;

            IsAliveSlot = data.IsAlive;
            AliveLayout();

            // Fill data for alive state
            _playerName.text = pn.PlayerName.Value.Value.ToString();
            _currentScore.text = "+0";
            _leaderboardScore.text = pn.Leaderboard.Value.ToString();

            _onAlive?.Invoke();
            return this;
        }

        public void UpdatePlayerScore(LocalPlayerData localData)
        {
            LocalData = localData;

            if (IsAliveSlot) AliveLayout();
            else EndGameLayout();

            _currentScore.text = $"+ {localData.Score}";
        }

        public void SetEndGamePlayer(LocalPlayerData localData)
        {
            if (localData.IsAlive == true) return; // ERROR

            _onDead?.Invoke();

            // fire right event
            (localData.Rank == 1 ? _onWinPlayer : _onLosePlayer).Invoke();

            // Setup ui
            LocalData = localData;
            EndGameLayout();
            IsAliveSlot = false;
            _appliedScore.text = $"+ {localData.Score}";
            _rank.text = localData.Rank.ToString();
        }


        [Button("Alive Layout")]
        void AliveLayout()
        {
            // Common
            _playerName.gameObject.SetActive(true);

            // Alive slot activation
            _currentScore.gameObject.SetActive(true);

            // Alive slot so desactivate this
            _rank.gameObject.SetActive(false);
            _appliedScore.gameObject.SetActive(false);
            _leaderboardScore.gameObject.SetActive(false);
        }

        [Button("Dead Layout")]
        void EndGameLayout()
        {
            // Common
            _playerName.gameObject.SetActive(true);
            // Alive
            _currentScore.gameObject.SetActive(false);
            // Dead
            _appliedScore.gameObject.SetActive(true);
            _leaderboardScore.gameObject.SetActive(true);
        }

    }
}
