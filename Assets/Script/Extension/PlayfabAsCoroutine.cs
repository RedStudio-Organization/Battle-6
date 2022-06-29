using PlayFab;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using static RedStudio.Battle10.GameEntity;

namespace RedStudio.Battle10
{
    public class PlayfabAsCoroutine
    {
        public const string LeaderboardID = "GlobalRanking";
        static string UserID 
        {
            get
            {
                var t = PlayerPrefs.GetString("ID", string.Empty);
                t = (t == string.Empty ? UserID = Guid.NewGuid().ToString() : t);
#if UNITY_EDITOR
                t += Application.dataPath.Split('/').Reverse().Skip(1).First();
#endif
                return t;
            }
            set => PlayerPrefs.SetString("ID", value);
        }

        public static IEnumerator LoginCustomID(Action<LoginResult,PlayFabError> result, string playerName="NoName")
        {
            // ID generation & request
            if(UserID == string.Empty) UserID= Guid.NewGuid().ToString();
            var request = new LoginWithCustomIDRequest()
            {
                TitleId = PlayFabSettings.TitleId,
                CreateAccount = true,
                CustomId = UserID,
            };

            // Loggin / Create account
            Trigger responseReceived = new Trigger();
            LoginResult loginResult = null;
            Debug.Log($"[Playfab:Login] Request Sent");
            PlayFabClientAPI.LoginWithCustomID(request, Success, Error);
            void Success(LoginResult lr)
            {
                responseReceived.Activate();
                Debug.Log($"[Playfab:Login] {lr}");
                loginResult = lr;
            }
            void Error(PlayFabError e)
            {
                responseReceived.Activate();
                Debug.Log($"[Playfab:Login] {e.GenerateErrorReport()}");
                result?.Invoke(null, e);
            }

            // Wait result
            while (responseReceived.IsActivated()==false)
            {
                yield return null;
            }

            // New player => Rename
            if(loginResult?.NewlyCreated ?? false)
            {
                yield return RenamePlayer(playerName);
            }

            // Finish call
            result?.Invoke(loginResult, null);
            yield break;
        }

        public static IEnumerator RenamePlayer(string newName)
        {
            // Renaming
            Trigger responseReceived = new Trigger();
            UpdateUserTitleDisplayNameResult renameResult = null;
            UpdateUserTitleDisplayNameRequest reqName = new UpdateUserTitleDisplayNameRequest()
            {
                DisplayName = newName
            };
            PlayFabClientAPI.UpdateUserTitleDisplayName(reqName, SuccessRename, Error);
            void SuccessRename(UpdateUserTitleDisplayNameResult r)
            {
                responseReceived.Activate();
                renameResult = r;
            }
            void Error(PlayFabError e)
            {
                responseReceived.Activate();
                Debug.Log($"[Playfab:Rename] {e.GenerateErrorReport()}");
            }
            while (responseReceived.IsActivated() == false) yield return null;
        }

        public static IEnumerator GetLeaderboardValue(Action<int> result)
        {
            Trigger next = new Trigger();
            var req = new GetLeaderboardAroundPlayerRequest { StatisticName = LeaderboardID, MaxResultsCount = 1 };

            PlayFabClientAPI.GetLeaderboardAroundPlayer(req, Success, Error);
            void Success(GetLeaderboardAroundPlayerResult d)
            {
                var v = d.Leaderboard.First(i=>i.DisplayName==DynamicRename.PlayerName).StatValue;
                next.Activate();
                result?.Invoke(v);
            }
            void Error(PlayFabError e)
            {
                next.Activate();
                Debug.Log($"[PlayerName] {e.GenerateErrorReport()}");
            }
            while (next.IsActivated() == false) yield return null;
        }

        public static IEnumerator GetPlayerName(Action<string> result)
        {
            Trigger next = new Trigger();
            GetAccountInfoRequest req = new GetAccountInfoRequest();
            PlayFabClientAPI.GetAccountInfo(req, Success, Error);
            void Success(GetAccountInfoResult d)
            {
                next.Activate();
                result?.Invoke(d.AccountInfo.TitleInfo.DisplayName);
            }
            void Error(PlayFabError e)
            {
                next.Activate();
                Debug.Log($"[PlayerName] {e.GenerateErrorReport()}");
            }
            while(next.IsActivated() == false) yield return null;
        }

        public static IEnumerator AcquireServer(Action<RequestMultiplayerServerResponse, PlayFabError> result, string buildID, List<string> regions)
        {
            RequestMultiplayerServerRequest request = new RequestMultiplayerServerRequest()
            {
                BuildId = buildID,
                SessionId = Guid.NewGuid().ToString(),
                PreferredRegions = regions
            };

            Debug.Log($"[Playfab:Login] Request Sent");
            Trigger responseReceived = new Trigger();
            PlayFabMultiplayerAPI.RequestMultiplayerServer(request, Success, Error);
            void Success(RequestMultiplayerServerResponse rmsr)
            {
                Debug.Log($"[Playfab:RequestServer] {rmsr}");
                responseReceived.Activate();
                result?.Invoke(rmsr, null);
            }
            void Error(PlayFabError e)
            {
                responseReceived.Activate();
                Debug.Log($"[Playfab:RequestServer] {e.GenerateErrorReport()}");
                result?.Invoke(null, e);
            }

            while (responseReceived.IsActivated() == false) yield return null;

            yield break;
        }

        public static void ConnectToServer(NetworkManager manager, UnityTransport transport, RequestMultiplayerServerResponse rmsr)
        {
            if (rmsr == null)
            {
                Debug.Log("[Network] Cannot connect to server. RequestMultiplayerServer is null");
                return;
            }
            ConnectToServer(manager, transport, rmsr.IPV4Address, (ushort)rmsr.Ports.First().Num);
        }

        public static void ConnectToServer(NetworkManager manager, UnityTransport transport, string adress, ushort port)
        {
            manager.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(DynamicRename.PlayerName);
            transport.ConnectionData = new UnityTransport.ConnectionAddressData()
            {
                Address = adress,
                Port = port,
                ServerListenAddress = "0.0.0.0",
            };

            manager.StartClient();
        }

        #region Leaderboard
        public static IEnumerator LeaderboardInsertion(LocalPlayerData data)
        {
            var req = new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate
                    {
                        StatisticName=LeaderboardID,
                        Value=(int)data.Score,
                    }
                }
            };
            Trigger next = new Trigger();
            UpdatePlayerStatisticsResult result = null;
            PlayFabClientAPI.UpdatePlayerStatistics(req, Success, Error);
            void Success(UpdatePlayerStatisticsResult r)
            {
                next.Activate();
                result = r;
            }
            void Error(PlayFabError e)
            {
                next.Activate();
                Debug.Log($"[Leaderboard] {e.GenerateErrorReport()}");
            }
            while (next.IsActivated() == false) yield return null;
            if(result != null)
            {
                Debug.Log($"[Leaderboard] Updated");
            }
            yield break;
        }

        public static IEnumerator LeaderboardAcquisition(Action<GetLeaderboardAroundPlayerResult> output)
        {
            var req = new GetLeaderboardAroundPlayerRequest
            {
                 StatisticName= LeaderboardID,
                 MaxResultsCount=10,
            };
            Trigger t = new Trigger();
            GetLeaderboardAroundPlayerResult result = null;
            PlayFabClientAPI.GetLeaderboardAroundPlayer(req, Success, Error);
            void Success(GetLeaderboardAroundPlayerResult r)
            {
                t.Activate();
                result = r;
            }
            void Error(PlayFabError e)
            {
                t.Activate();
            }
            while (t.IsActivated() == false) yield return null;
            if(result!=null)
            {
                Debug.Log($"[Leaderboard] data received");
                output?.Invoke(result);
            }

            yield break;
        }
        #endregion
    }
}
