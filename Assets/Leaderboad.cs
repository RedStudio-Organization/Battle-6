using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RedStudio.Battle10
{
    public class Leaderboad : MonoBehaviour
    {
        [SerializeField] Transform _slotRoot;
        [SerializeField] LeaderboardSlotUI _playerSlotPrefab;

        GameEntity Game { get; set; }
        List<LeaderboardSlotUI> Slots { get; set; }

        public void Warmup(GameEntity ge)
        {
            // Bind to game
            Game = ge;

            // Create leaderboard instances
            Slots = new List<LeaderboardSlotUI>();
            var tmp = Game.PlayersWithData.ToList();
            foreach (var el in Game.PlayersWithData)
            {
                Slots.Add(Instantiate(_playerSlotPrefab, _slotRoot).Init(el.Item1, el.Item2));
            }

            // Bind to event to update managed slots
            Game.GlobalPlayerData.OnListChanged += UpdateLeadboardSlot;
        }

        void UpdateLeadboardSlot(Unity.Netcode.NetworkListEvent<GameEntity.LocalPlayerData> changeEvent)
        {
            if (changeEvent.Type != Unity.Netcode.NetworkListEvent<GameEntity.LocalPlayerData>.EventType.Value) return;

            // Was alive and now dead =>
            if(changeEvent.PreviousValue.IsAlive==true && changeEvent.Value.IsAlive==false)
            {
                Slots.First(i => i.LocalData.PlayerID == changeEvent.Value.PlayerID).SetEndGamePlayer(changeEvent.Value);
            }

            // Update score
            if(changeEvent.PreviousValue.Score != changeEvent.Value.Score)
            {
                Slots.First(i => i.LocalData.PlayerID == changeEvent.Value.PlayerID).UpdatePlayerScore(changeEvent.Value);
            }

            // Order slots by Score and Alive state
            foreach(var el in Slots.OrderByDescending(i => i.LocalData.Rank).ThenBy(i => i.LocalData.Score))
            {
                el.transform.SetAsFirstSibling();
            }
        }

        void OnDestroy()
        {
            if(Game!=null && Game.GlobalPlayerData!=null)
            {
                Game.GlobalPlayerData.OnListChanged -= UpdateLeadboardSlot;   
            }
        }
    }
}
