using MarsFPSKit.Networking;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using System.Collections.Generic;
using MarsFPSKit.UI;

namespace MarsFPSKit
{
    namespace Services
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Services/Master Server/UGS Lobby")]
        /// <summary>
        /// Implements a master server using UGS Lobby service.
        /// Make sure to link your project and enable lobby service.
        /// This service is "pay as you go".
        /// </summary>
        public class Kit_MasterServerUgsLobby : Kit_MasterServerBase
        {
            /// <summary>
            /// Currently stored games ready for retrival
            /// </summary>
            private GameInfo[] storedGames = new GameInfo[0];

            public override void Initialize()
            {
                if (Kit_UGS.instance)
                {
                    //Update games instantly
                    FetchTheirSouls();
                }
                else
                {
                    Debug.LogWarning("<color=green>[Master Server UGS]</color> No UGS instance found. Will not work without UGS.");
                }
            }

            public override void GameServerExited(Kit_NetworkGameInformation gameInfo)
            {
                if (activeLobby != null)
                {
                    LobbyService.Instance.DeleteLobbyAsync(activeLobby.Id);
                    activeLobby = null;
                }
            }

            public override void GameServerStarted(Kit_NetworkGameInformation gameInfo)
            {
                CreateAndSaveLobby(gameInfo);
            }

            public override void Heartbeat(Kit_NetworkGameInformation gameInfo)
            {
                if (activeLobby != null)
                {
                    LobbyService.Instance.SendHeartbeatPingAsync(activeLobby.Id);
                }
            }

            public override void GameServerInfoUpdated(Kit_NetworkGameInformation gameInfo)
            {
                if (activeLobby != null)
                {
                    LobbyService.Instance.SendHeartbeatPingAsync(activeLobby.Id);

#if UNITY_SERVER
                    //Update all data
                    string players = "0";
#else
                    //Update all data
                    string players = "1";
#endif

                    if (Kit_NetworkPlayerManager.instance)
                    {
                        players = Kit_NetworkPlayerManager.instance.players.Count.ToString();
                    }

                    UpdateLobbyOptions options = new UpdateLobbyOptions();
                    options.Data = new Dictionary<string, DataObject>();
                    options.Data.Add("map", new DataObject(DataObject.VisibilityOptions.Public, gameInfo.map.ToString(), DataObject.IndexOptions.N1));
                    options.Data.Add("gameMode", new DataObject(DataObject.VisibilityOptions.Public, gameInfo.gameMode.ToString(), DataObject.IndexOptions.N2));
                    options.Data.Add("gameModeType", new DataObject(DataObject.VisibilityOptions.Public, gameInfo.gameModeType.ToString(), DataObject.IndexOptions.N3));
                    options.Data.Add("players", new DataObject(DataObject.VisibilityOptions.Public, players, DataObject.IndexOptions.N4));
                    options.Data.Add("maxPlayers", new DataObject(DataObject.VisibilityOptions.Public, gameInfo.playerLimit.ToString(), DataObject.IndexOptions.N5));
                    options.Data.Add("bots", new DataObject(DataObject.VisibilityOptions.Public, gameInfo.bots.ToString(), DataObject.IndexOptions.S1));
                    options.Data.Add("password", new DataObject(DataObject.VisibilityOptions.Public, (gameInfo.password.Length > 0).ToString(), DataObject.IndexOptions.S2));
                    options.Data.Add("connection", new DataObject(DataObject.VisibilityOptions.Public, gameInfo.connectionString, DataObject.IndexOptions.S3));
                    options.Data.Add("dedicated", new DataObject(DataObject.VisibilityOptions.Public, gameInfo.isDedicatedServer.ToString(), DataObject.IndexOptions.S4));

                    LobbyService.Instance.UpdateLobbyAsync(activeLobby.Id, options);
                }
            }

            public override GameInfo[] GetGames()
            {
                return storedGames;
            }

            private Lobby activeLobby;

            /// <summary>
            /// Creates the lobby and saves it for later update use.
            /// </summary>
            /// <param name="gameInfo"></param>
            async void CreateAndSaveLobby(Kit_NetworkGameInformation gameInfo)
            {
                string lobbyName = gameInfo.gameName;
                CreateLobbyOptions options = new CreateLobbyOptions();
                options.IsPrivate = false;
                options.Data = new Dictionary<string, DataObject>();
                options.Data.Add("map", new DataObject(DataObject.VisibilityOptions.Public, gameInfo.map.ToString(), DataObject.IndexOptions.N1));
                options.Data.Add("gameMode", new DataObject(DataObject.VisibilityOptions.Public, gameInfo.gameMode.ToString(), DataObject.IndexOptions.N2));
                options.Data.Add("gameModeType", new DataObject(DataObject.VisibilityOptions.Public, gameInfo.gameModeType.ToString(), DataObject.IndexOptions.N3));
#if UNITY_SERVER
                options.Data.Add("players", new DataObject(DataObject.VisibilityOptions.Public, "0", DataObject.IndexOptions.N4));
#else
                options.Data.Add("players", new DataObject(DataObject.VisibilityOptions.Public, "1", DataObject.IndexOptions.N4));
#endif
                options.Data.Add("maxPlayers", new DataObject(DataObject.VisibilityOptions.Public, gameInfo.playerLimit.ToString(), DataObject.IndexOptions.N5));
                options.Data.Add("bots", new DataObject(DataObject.VisibilityOptions.Public, gameInfo.bots.ToString(), DataObject.IndexOptions.S1));
                options.Data.Add("password", new DataObject(DataObject.VisibilityOptions.Public, (gameInfo.password.Length > 0).ToString(), DataObject.IndexOptions.S2));
                options.Data.Add("connection", new DataObject(DataObject.VisibilityOptions.Public, gameInfo.connectionString, DataObject.IndexOptions.S3));
                options.Data.Add("dedicated", new DataObject(DataObject.VisibilityOptions.Public, gameInfo.isDedicatedServer.ToString(), DataObject.IndexOptions.S4));

                //1 Player limit because other players do not join the lobby, its just to store data.
                Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 2, options);
                activeLobby = lobby;
            }

            public override void RefreshGames()
            {
                FetchTheirSouls();
            }

            /// <summary>
            /// Queries the lobby system and stores the data
            /// </summary>
            async void FetchTheirSouls()
            {
                try
                {
                    QueryLobbiesOptions options = new QueryLobbiesOptions();
                    options.Count = 100;
                    // Filter for open lobbies only
                    options.Filters = new List<QueryFilter>() { };

                    // Order by newest lobbies first
                    options.Order = new List<QueryOrder>() { new QueryOrder(asc: false, field: QueryOrder.FieldOptions.Created) };

                    QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);

                    //Store the games
                    storedGames = new GameInfo[lobbies.Results.Count];

                    for (int i = 0; i < storedGames.Length; i++)
                    {
                        storedGames[i] = new GameInfo();
                        storedGames[i].name = lobbies.Results[i].Name;
                        storedGames[i].map = int.Parse(lobbies.Results[i].Data["map"].Value);
                        storedGames[i].gameMode = int.Parse(lobbies.Results[i].Data["gameMode"].Value);
                        storedGames[i].gameModeType = int.Parse(lobbies.Results[i].Data["gameModeType"].Value);
                        storedGames[i].players = int.Parse(lobbies.Results[i].Data["players"].Value);
                        storedGames[i].maxPlayers = int.Parse(lobbies.Results[i].Data["maxPlayers"].Value);
                        storedGames[i].password = bool.Parse(lobbies.Results[i].Data["password"].Value);
                        storedGames[i].bots = bool.Parse(lobbies.Results[i].Data["bots"].Value);
                        storedGames[i].dedicated = bool.Parse(lobbies.Results[i].Data["dedicated"].Value);
                        storedGames[i].connection = lobbies.Results[i].Data["connection"].Value;
                    }

                    //Redraw games
                    Kit_MenuServerBrowser serverBrowser = FindObjectOfType<Kit_MenuServerBrowser>();
                    if (serverBrowser)
                    {
                        serverBrowser.RedrawGames();
                    }
                }
                catch (LobbyServiceException e)
                {
                    Debug.Log("<color=green>[Master Server UGS]</color> error: " + e.ToString());
                }
            }
        }
    }
}