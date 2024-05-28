using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MarsFPSKit.Networking;
using Mirror;

namespace MarsFPSKit
{
    namespace UI
    {
        public class Kit_PvE_SandboxMenu : Kit_MenuPveGameModeBase
        {
            /// <summary>
            /// Menu that gets opened when we are a singleplayer game mode
            /// </summary>
            public int singleplayerMenuId;
            /// <summary>
            /// Menu that gets opened when we are a coop game mode
            /// </summary>
            public int coopMainMenuId;
            /// <summary>
            /// Hosting / Lobby screen
            /// </summary>
            public int coopHostScreenId = 1;
            /// <summary>
            /// Browsing screen id
            /// </summary>
            public int coopBrowserScreenId = 2;

            /// <summary>
            /// Displays the name of our selected map
            /// </summary>
            [Header("Singleplayer Settings")]
            public TextMeshProUGUI spMapName;
            /// <summary>
            /// Currently selected map
            /// </summary>
            private int spCurMap;

            /// <summary>
            /// Displays the name of our selected map
            /// </summary>
            [Header("Coop Settings")]
            public TextMeshProUGUI coopMapName;
            [SyncVar(hook = "OnCoopMapChanged")]
            /// <summary>
            /// Currently selected map
            /// </summary>
            public int coopCurMap;
            /// <summary>
            /// Start button
            /// </summary>
            public Button coopStartButton;

            /// <summary>
            /// The "Content" object of the Scroll view, where playerEntriesPrefab will be instantiated
            /// </summary>
            [Header("Coop Players")]
            public RectTransform playerEntriesGo;
            /// <summary>
            /// The Player Entry prefab
            /// </summary>
            public GameObject playerEntriesPrefab;
            /// <summary>
            /// Currently active player entries - used for cleanup
            /// </summary>
            private List<GameObject> activePlayerEntries = new List<GameObject>();

            /// <summary>
            /// The "Content" object of the Scroll view, where entriesPrefab will be instantiated
            /// </summary>
            [Header("Coop Browser")]
            public RectTransform entriesGo;
            /// <summary>
            /// The Server Browser Entry prefab
            /// </summary>
            public GameObject entriesPrefab;
            /// <summary>
            /// Currently active server browser entries - used for cleanup
            /// </summary>
            private List<GameObject> activeEntries = new List<GameObject>();
            /// <summary>
            /// Were we in a room? (To fire event for leaving a room)
            /// </summary>
            private bool wasInRoom;

            private void OnEnable()
            {
                Kit_NetworkManager.instance.onHostStartedEvent.AddListener(OnCreatedRoom);
            }

            private void OnDisable()
            {
                if (Kit_NetworkManager.instance)
                {
                    Kit_NetworkManager.instance.onHostStartedEvent.RemoveListener(OnCreatedRoom);
                }
            }

            public override void OnStartClient()
            {
                //As a coop menu, this menu can be spawned in via network. Find menu manager automatically
                menuManager = FindObjectOfType<Kit_MenuManager>();
                SetupMenu(menuManager, 1, myId);
                OnJoinedRoom();
            }

            public override void SetupMenu(Kit_MenuManager mm, int state, int id)
            {
                base.SetupMenu(mm, state, id);

                //Redraw to default values
                if (myCurrentState == 0)
                {
                    RedrawSingleplayerMenu();
                }
                else
                {
                    RedrawCoopMenu();
                }
            }

            public override void OpenMenu()
            {
                if (myCurrentState == 0)
                {
                    ChangeMenuButton(singleplayerMenuId);
                }
                else
                {
                    ChangeMenuButton(coopMainMenuId);
                }
            }

            #region Button Calls


            public void SingleplayerStart()
            {
                //Create a room with this game mode
                if (myCurrentState == 0)
                {
                    //Register our host event
                    Kit_NetworkManager.instance.onHostStartedEvent.RemoveAllListeners();
                    Kit_NetworkManager.instance.onHostStartedEvent.AddListener(delegate
                    {
                        //Create game information
                        GameObject gameInfoObject = Instantiate(Kit_NetworkManager.instance.networkGameInformationPrefab);
                        Kit_NetworkGameInformation gameInfo = gameInfoObject.GetComponent<Kit_NetworkGameInformation>();
                        //Set info
                        gameInfo.gameName = "";
                        gameInfo.playerLimit = 1;
                        Kit_NetworkManager.instance.maxConnections = 1;
                        gameInfo.map = spCurMap;
                        gameInfo.gameModeType = 0;
                        gameInfo.gameMode = myId;
                        gameInfo.duration = 0;
                        gameInfo.ping = 0;
                        gameInfo.afk = 0;
                        gameInfo.bots = false;
                        gameInfo.password = "";
                        gameInfo.connectionString = menuManager.game.transport.GetConnectionString(Kit_NetworkManager.instance);
                        //Register
                        NetworkServer.Spawn(gameInfoObject);
                    });
                    Kit_NetworkManager.instance.onHostStartedEvent.AddListener(OnCreatedRoom);

                    //Start
                    Kit_NetworkManager.instance.HostGame(1, true);
                }
            }

            public void SingleplayerNextMap()
            {
                spCurMap++;

                if (spCurMap >= menuManager.game.allSingleplayerGameModes[myId].maps.Length)
                {
                    spCurMap = 0;
                }

                RedrawSingleplayerMenu();
            }

            public void SingleplayerPreviousMap()
            {
                spCurMap--;

                if (spCurMap < 0)
                {
                    spCurMap = menuManager.game.allSingleplayerGameModes[myId].maps.Length - 1;
                }

                RedrawSingleplayerMenu();
            }

            /// <summary>
            /// Creates a coop lobby
            /// </summary>
            public void CoopHostGame()
            {
                //Register our host event
                Kit_NetworkManager.instance.onHostStartedEvent.RemoveAllListeners();
                Kit_NetworkManager.instance.onHostStartedEvent.AddListener(delegate
                {
                    //Create game information
                    GameObject gameInfoObject = Instantiate(Kit_NetworkManager.instance.networkGameInformationPrefab);
                    Kit_NetworkGameInformation gameInfo = gameInfoObject.GetComponent<Kit_NetworkGameInformation>();
                    //Set info
                    gameInfo.gameName = Kit_GameSettings.userName + "'s game";
                    gameInfo.playerLimit = menuManager.game.allCoopGameModes[myId].coopPlayerAmount;
                    Kit_NetworkManager.instance.maxConnections = menuManager.game.allCoopGameModes[myId].coopPlayerAmount;
                    gameInfo.map = coopCurMap;
                    gameInfo.gameModeType = 1;
                    gameInfo.gameMode = myId;
                    gameInfo.duration = 0;
                    gameInfo.ping = 0;
                    gameInfo.afk = 0;
                    gameInfo.bots = false;
                    gameInfo.password = "";
                    gameInfo.connectionString = menuManager.game.transport.GetConnectionString(Kit_NetworkManager.instance);
                    //Register
                    NetworkServer.Spawn(gameInfoObject);
                });
                Kit_NetworkManager.instance.onHostStartedEvent.AddListener(OnCreatedRoom);

                //Start
                Kit_NetworkManager.instance.HostGame(menuManager.game.allCoopGameModes[myId].coopPlayerAmount);
            }

            public void CoopStart()
            {
                if (NetworkServer.active)
                {
                    //Deactivate all input
                    menuManager.eventSystem.enabled = false;
                    //Load the map
                    Kit_SceneSyncer.instance.LoadScene(menuManager.game.allCoopGameModes[myId].maps[coopCurMap].sceneName);
                }
            }

            public void CoopLeaveLobby()
            {
                if (wasInRoom)
                {
                    // stop host if host mode
                    if (NetworkServer.active && NetworkClient.isConnected)
                    {
                        Kit_NetworkManager.instance.StopHost();

                    }
                    // stop client if client-only
                    else if (NetworkClient.isConnected)
                    {
                        Kit_NetworkManager.instance.StopClient();
                    }
                    // stop server if server-only
                    else if (NetworkServer.active)
                    {
                        Kit_NetworkManager.instance.StopServer();
                    }
                }
            }

            public void CoopNextMap()
            {
                if (NetworkServer.active)
                {
                    int map = coopCurMap;

                    map++;

                    if (map >= menuManager.game.allCoopGameModes[myId].maps.Length)
                    {
                        map = 0;
                    }

                    coopCurMap = map;

                    RedrawCoopMenu();
                }
            }

            public void CoopPreviousMap()
            {
                if (NetworkServer.active)
                {
                    int map = coopCurMap;

                    map--;

                    if (map < 0)
                    {
                        map = menuManager.game.allCoopGameModes[myId].maps.Length - 1;
                    }

                    coopCurMap = map;

                    RedrawCoopMenu();
                }
            }
            #endregion

            #region UI
            private void RedrawSingleplayerMenu()
            {
                spMapName.text = menuManager.game.allSingleplayerGameModes[myId].maps[spCurMap].mapName;
            }

            private void RedrawCoopMenu()
            {
                if (!menuManager)
                {
                    //As a coop menu, this menu can be spawned in via network. Find menu manager automatically if necessary
                    menuManager = FindObjectOfType<Kit_MenuManager>();
                }

                coopMapName.text = menuManager.game.allCoopGameModes[myId].maps[coopCurMap].mapName;

                if (NetworkClient.active)
                {
                    //Redraw players
                    //Clean Up
                    for (int i = 0; i < activePlayerEntries.Count; i++)
                    {
                        //Destroy
                        Destroy(activePlayerEntries[i]);
                    }
                    //Reset list
                    activePlayerEntries = new List<GameObject>();

                    for (int i = 0; i < Kit_NetworkPlayerManager.instance.players.Count; i++)
                    {
                        //Instantiate entry
                        GameObject go = Instantiate(playerEntriesPrefab, playerEntriesGo) as GameObject;
                        //Set it up
                        TextMeshProUGUI txt = go.GetComponentInChildren<TextMeshProUGUI>();
                        //Display that mf's name
                        txt.text = Kit_NetworkPlayerManager.instance.players[i].name;
                        //Add it to our active list so it will get cleaned up next time
                        activePlayerEntries.Add(go);
                    }
                }
            }

            public void CoopRedrawBrowser()
            {
                menuManager.game.masterServer.RefreshGames();
                var games = menuManager.game.masterServer.GetGames();

                //Clean Up
                for (int i = 0; i < activeEntries.Count; i++)
                {
                    //Destroy
                    Destroy(activeEntries[i]);
                }
                //Reset list
                activeEntries = new List<GameObject>();

                //Instantiate new List
                foreach (var info in games)
                {
                    //1 = Coop
                    if (info.gameModeType == 1)
                    {
                        //Check if game mode matches
                        if (info.gameMode == myId)
                        {
                            //Instantiate entry
                            GameObject go = Instantiate(entriesPrefab, entriesGo) as GameObject;
                            //Set it up
                            Kit_CoopBrowserEntry entry = go.GetComponent<Kit_CoopBrowserEntry>();
                            entry.Setup(this, info);
                            //This sets up the join function
                            entry.joinButton.onClick.AddListener(delegate { JoinRoom(info); });
                            //Add it to our active list so it will get cleaned up next time
                            activeEntries.Add(go);
                        }
                    }
                }
            }

            public void JoinRoom(Services.GameInfo room)
            {
                //Just join room
                Kit_NetworkManager.instance.JoinGame(room);
            }
            #endregion


            #region Photon Calls
            public void OnCoopMapChanged(int was, int isNow)
            {
                RedrawCoopMenu();
            }

            //We just created a room
            public void OnCreatedRoom()
            {
                //Our room is created and ready
                //Lets load the appropriate map
                if (Kit_NetworkGameInformation.instance.gameModeType == 0) //Singleplayer - load map - coop does it on button click
                {
                    if (Kit_NetworkGameInformation.instance.gameMode == myId)
                    {
                        //Deactivate all input
                        menuManager.eventSystem.enabled = false;
                        //Load the map
                        Kit_SceneSyncer.instance.LoadScene(menuManager.game.allSingleplayerGameModes[myId].maps[Kit_NetworkGameInformation.instance.map].sceneName);
                    }
                }

                if (Kit_NetworkGameInformation.instance.gameModeType == 1) //COOP
                {
                    if (Kit_NetworkGameInformation.instance.gameMode == myId)
                    {
                        //Enable button for host
                        coopStartButton.enabled = true;

                        wasInRoom = true;

                        //Attach to network
                        NetworkServer.Spawn(gameObject);
                    }
                }
            }

            public void OnJoinedRoom()
            {
                if (Kit_NetworkGameInformation.instance.gameModeType == 1) //COOP
                {
                    if (Kit_NetworkGameInformation.instance.gameMode == myId)
                    {
                        //Go to host screen
                        ChangeMenuButton(coopHostScreenId);

                        //Redraw that mf
                        RedrawCoopMenu();

                        //Disable the button
                        coopStartButton.enabled = NetworkServer.active;

                        wasInRoom = true;
                    }
                }
            }

            public void OnPlayerEnteredRoom(Kit_Player newPlayer)
            {
                if (wasInRoom)
                {
                    RedrawCoopMenu();
                }
            }

            public void OnPlayerLeftRoom(Kit_Player otherPlayer)
            {
                if (wasInRoom)
                {
                    RedrawCoopMenu();
                }
            }
            #endregion
        }
    }
}