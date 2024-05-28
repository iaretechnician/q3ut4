using System.Collections;
using TMPro;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;
using System;
using Mirror;
using MarsFPSKit.Networking;
using MarsFPSKit.Services;

namespace MarsFPSKit
{
    namespace UI
    {
        public class Kit_MenuHostScreen : MonoBehaviour
        {
            public Kit_MenuManager menuManager;

            /// <summary>
            /// The Room Name
            /// </summary>
            public TMP_InputField nameField;
            /// <summary>
            /// The room's password
            /// </summary>
            public TMP_InputField passwordField;
            //All Labels
            /// <summary>
            /// Displays the current map
            /// </summary>
            public TextMeshProUGUI curMapLabel;
            /// <summary>
            /// Displays the current game mode
            /// </summary>
            public TextMeshProUGUI curGameModeLabel;
            /// <summary>
            /// Displays the current duration limit
            /// </summary>
            public TextMeshProUGUI curDurationLabel;
            /// <summary>
            /// Displays the current player limit
            /// </summary>
            public TextMeshProUGUI curPlayerLimitLabel;
            /// <summary>
            /// Displays the current players needed
            /// </summary>
            public TextMeshProUGUI curPlayersNeededLabel;
            /// <summary>
            /// Displays the current ping limit
            /// </summary>
            public TextMeshProUGUI curPingLimitLabel;
            /// <summary>
            /// Displays the current afk limit
            /// </summary>
            public TextMeshProUGUI curAfkLimitLabel;
            /// <summary>
            /// Displays the current bot mode
            /// </summary>
            public TextMeshProUGUI curBotModeLabel;
            /// <summary>
            /// Displays the current connectivity mode
            /// </summary>
            public TextMeshProUGUI curOnlineModeLabel;
            //Ints to store the hosting information
            private int currentMap;
            private int currentGameMode;
            private int currentDuration;
            private int currentPlayerLimit;
            private int currentPlayerNeeded = 1;
            private int currentPingLimit;
            private int currentAfkLimit;
            private int currentBotMode;
            private int currentOnlineMode;

#if UNITY_SERVER
            private IEnumerator Start()
            {
                string[] args = System.Environment.GetCommandLineArgs();

                if (Kit_UGS.instance)
                {
                    while (!Kit_UGS.instance.isLoggedIn) yield return null;
                }

                nameField.text = "Dedicated Server";

                if (args.Contains("-name"))
                {
                    int nameIndex = System.Array.IndexOf(args, "-name");
                    if (args.Length > nameIndex + 1)
                    {
                        nameField.text = args[nameIndex + 1];
                        Debug.Log("Using name: " + nameField.text);
                    }
                }

                if (args.Contains("-playerLimit"))
                {
                    int playerLimitIndex = System.Array.IndexOf(args, "-playerLimit");
                    if (args.Length > playerLimitIndex + 1)
                    {
                        currentPlayerLimit = int.Parse(args[playerLimitIndex + 1]);
                        Debug.Log("Using players limit index: " + currentPlayerLimit);
                    }
                }

                if (args.Contains("-playerNeeded"))
                {
                    int playerNeededIndex = System.Array.IndexOf(args, "-playerNeeded");
                    if (args.Length > playerNeededIndex + 1)
                    {
                        currentPlayerNeeded = int.Parse(args[playerNeededIndex + 1]);
                        Debug.Log("Using players needed index: " + currentPlayerNeeded);
                    }
                }

                if (args.Contains("-map"))
                {
                    int mapIndex = System.Array.IndexOf(args, "-map");
                    if (args.Length > mapIndex + 1)
                    {
                        currentMap = int.Parse(args[mapIndex + 1]);
                        Debug.Log("Using map: " + currentMap);
                    }
                }

                if (args.Contains("-gameMode"))
                {
                    int gameModeIndex = System.Array.IndexOf(args, "-gameMode");
                    if (args.Length > gameModeIndex + 1)
                    {
                        currentGameMode = int.Parse(args[gameModeIndex + 1]);
                        Debug.Log("Using game mode index: " + currentGameMode);
                    }
                }

                if (args.Contains("-duration"))
                {
                    int durationIndex = System.Array.IndexOf(args, "-duration");
                    if (args.Length > durationIndex + 1)
                    {
                        currentDuration = int.Parse(args[durationIndex + 1]);
                        Debug.Log("Using duration index: " + durationIndex);
                    }
                }

                if (args.Contains("-pingLimit"))
                {
                    int pingLimitIndex = System.Array.IndexOf(args, "-pingLimit");
                    if (args.Length > pingLimitIndex + 1)
                    {
                        currentPingLimit = int.Parse(args[pingLimitIndex + 1]);
                        Debug.Log("Using ping limit index: " + currentPingLimit);
                    }
                }

                if (args.Contains("-afkLimit"))
                {
                    int afkLimitIndex = System.Array.IndexOf(args, "-afkLimit");
                    if (args.Length > afkLimitIndex + 1)
                    {
                        currentAfkLimit = int.Parse(args[afkLimitIndex + 1]);
                        Debug.Log("Using afk limit index: " + currentAfkLimit);
                    }
                }

                if (args.Contains("-bots"))
                {
                    currentBotMode = 1;
                    Debug.Log("Using bots");
                }

                if (args.Contains("-password"))
                {
                    int passwordIdex = System.Array.IndexOf(args, "-password");
                    if (args.Length > passwordIdex + 1)
                    {
                        passwordField.text = args[passwordIdex + 1];
                        Debug.Log("Using password: " + passwordField.text);
                    }
                }

#if UNITY_EDITOR
                //Enable bots for testing in editor
                currentBotMode = 1;
                currentPlayerLimit = 4;
#endif

                StartSession();
            }
#else


            private void Start()
            {
                UpdateAllDisplays();
                //Generate random room name
                nameField.text = "Room (" + Random.Range(1, 999) + ")";
            }
#endif

            /// <summary>
            /// Call this whenever you want to make sure that all information displayed is correct
            /// </summary>
            void UpdateAllDisplays()
            {
                #region Host Menu
                //Map
                curMapLabel.text = menuManager.game.allPvpGameModes[currentGameMode].traditionalMaps[currentMap].mapName;

                //Game Mode
                curGameModeLabel.text = menuManager.game.allPvpGameModes[currentGameMode].gameModeName;

                //Duration
                if (menuManager.game.allPvpGameModes[currentGameMode].traditionalDurations[currentDuration] != 60)
                    curDurationLabel.text = (menuManager.game.allPvpGameModes[currentGameMode].traditionalDurations[currentDuration] / 60).ToString() + " minutes";
                else
                    curDurationLabel.text = (menuManager.game.allPvpGameModes[currentGameMode].traditionalDurations[currentDuration] / 60).ToString() + " minute";

                //Player Limit
                if (menuManager.game.allPvpGameModes[currentGameMode].traditionalPlayerLimits[currentPlayerLimit] != 1)
                    curPlayerLimitLabel.text = menuManager.game.allPvpGameModes[currentGameMode].traditionalPlayerLimits[currentPlayerLimit].ToString() + " players";
                else
                    curPlayerLimitLabel.text = menuManager.game.allPvpGameModes[currentGameMode].traditionalPlayerLimits[currentPlayerLimit].ToString() + " player";

                //Player Limit
                if (menuManager.game.allPvpGameModes[currentGameMode].traditionalPlayerNeeded[currentPlayerNeeded] != 1)
                    curPlayersNeededLabel.text = menuManager.game.allPvpGameModes[currentGameMode].traditionalPlayerNeeded[currentPlayerNeeded].ToString() + " players";
                else
                    curPlayersNeededLabel.text = menuManager.game.allPvpGameModes[currentGameMode].traditionalPlayerNeeded[currentPlayerNeeded].ToString() + " player";

                //Ping Limit
                if (menuManager.game.allPvpGameModes[currentGameMode].traditionalPingLimits[currentPingLimit] > 0)
                    curPingLimitLabel.text = menuManager.game.allPvpGameModes[currentGameMode].traditionalPingLimits[currentPingLimit].ToString() + "ms";
                else
                    curPingLimitLabel.text = "Disabled";

                //AFK Limit
                if (menuManager.game.allPvpGameModes[currentGameMode].traditionalAfkLimits[currentAfkLimit] > 0)
                {
                    if (menuManager.game.allPvpGameModes[currentGameMode].traditionalAfkLimits[currentAfkLimit] != 1)
                        curAfkLimitLabel.text = menuManager.game.allPvpGameModes[currentGameMode].traditionalAfkLimits[currentAfkLimit].ToString() + " seconds";
                    else
                        curAfkLimitLabel.text = menuManager.game.allPvpGameModes[currentGameMode].traditionalAfkLimits[currentAfkLimit].ToString() + " second";
                }
                else
                    curAfkLimitLabel.text = "Disabled";

                if (currentOnlineMode == 0)
                {
                    curOnlineModeLabel.text = "Online";
                }
                else
                {
                    curOnlineModeLabel.text = "Offline";
                }

                if (currentBotMode == 0)
                {
                    curBotModeLabel.text = "Disabled";
                }
                else
                {
                    curBotModeLabel.text = "Enabled";
                }
                #endregion
            }

            //This section contains all functions for the hosting menu
            #region HostMenu
            /// <summary>
            /// Starts a new Photon Session (Room)
            /// </summary>
            public void StartSession()
            {
                StartCoroutine(StartSessionRoutine());
            }

            public IEnumerator StartSessionRoutine()
            {
                if (nameField.text.IsNullOrWhiteSpace()) yield break;

                //Online
                if (currentOnlineMode == 0)
                {
                    //Register our host event
                    Kit_NetworkManager.instance.onHostStartedEvent.RemoveAllListeners();
                    Kit_NetworkManager.instance.onHostStartedEvent.AddListener(delegate
                    {
                        //Create game information
                        GameObject gameInfoObject = Instantiate(Kit_NetworkManager.instance.networkGameInformationPrefab);
                        Kit_NetworkGameInformation gameInfo = gameInfoObject.GetComponent<Kit_NetworkGameInformation>();
                        //Set info
                        gameInfo.gameName = nameField.text;
                        Kit_NetworkManager.instance.maxConnections = menuManager.game.allPvpGameModes[currentGameMode].traditionalPlayerLimits[currentPlayerLimit];
                        gameInfo.playerLimit = menuManager.game.allPvpGameModes[currentGameMode].traditionalPlayerLimits[currentPlayerLimit];
                        gameInfo.playersNeeded = currentPlayerNeeded;
                        gameInfo.map = currentMap;
                        gameInfo.gameModeType = 2;
                        gameInfo.gameMode = currentGameMode;
                        gameInfo.duration = currentDuration;
                        gameInfo.ping = currentPingLimit;
                        gameInfo.afk = currentAfkLimit;
                        gameInfo.bots = currentBotMode == 1;
                        gameInfo.password = passwordField.text;
                        gameInfo.connectionString = menuManager.game.transport.GetConnectionString(Kit_NetworkManager.instance);

#if UNITY_SERVER
                        gameInfo.isDedicatedServer = true;
#else
                        gameInfo.isDedicatedServer = false;
#endif

                        //Register
                        NetworkServer.Spawn(gameInfoObject);

                        Debug.Log("Server started. Loading map.");

                        //Load the map
                        Kit_SceneSyncer.instance.LoadScene(menuManager.game.allPvpGameModes[currentGameMode].traditionalMaps[currentMap].sceneName);
                    });

                    //Start
                    Kit_NetworkManager.instance.HostGame(menuManager.game.allPvpGameModes[currentGameMode].traditionalPlayerLimits[currentPlayerLimit]);

                    Debug.Log("Starting online server");
                }
                //Offline
                else
                {
                    //Register our host event
                    Kit_NetworkManager.instance.onHostStartedEvent.RemoveAllListeners();
                    Kit_NetworkManager.instance.onHostStartedEvent.AddListener(delegate
                    {
                        //Create game information
                        GameObject gameInfoObject = Instantiate(Kit_NetworkManager.instance.networkGameInformationPrefab);
                        Kit_NetworkGameInformation gameInfo = gameInfoObject.GetComponent<Kit_NetworkGameInformation>();
                        //Set info
                        gameInfo.gameName = nameField.text;
                        Kit_NetworkManager.instance.maxConnections = menuManager.game.allPvpGameModes[currentGameMode].traditionalPlayerLimits[currentPlayerLimit];
                        gameInfo.playerLimit = menuManager.game.allPvpGameModes[currentGameMode].traditionalPlayerLimits[currentPlayerLimit];
                        gameInfo.playersNeeded = currentPlayerNeeded;
                        gameInfo.map = currentMap;
                        gameInfo.gameModeType = 2;
                        gameInfo.gameMode = currentGameMode;
                        gameInfo.duration = currentDuration;
                        gameInfo.ping = currentPingLimit;
                        gameInfo.afk = currentAfkLimit;
                        gameInfo.bots = currentBotMode == 1;
                        gameInfo.password = passwordField.text;
#if UNITY_SERVER
                        gameInfo.isDedicatedServer = true;
#else
                        gameInfo.isDedicatedServer = false;
#endif
                        gameInfo.connectionString = menuManager.game.transport.GetConnectionString(Kit_NetworkManager.instance);
                        //Register
                        NetworkServer.Spawn(gameInfoObject);

                        Debug.Log("Server started. Loading map.");

                        //Load the map
                        Kit_SceneSyncer.instance.LoadScene(menuManager.game.allPvpGameModes[currentGameMode].traditionalMaps[currentMap].sceneName);
                    });

                    //Start
                    Kit_NetworkManager.instance.HostGame(1, true);

                    Debug.Log("Starting online server");
                }
            }

            /// <summary>
            /// Selects the next map
            /// </summary>
            public void NextMap()
            {
                //Increase number
                currentMap++;
                //Check if we have that many
                if (currentMap >= menuManager.game.allPvpGameModes[currentGameMode].traditionalMaps.Length) currentMap = 0; //If not, reset

                //Update display
                UpdateAllDisplays();
            }

            /// <summary>
            /// Selects the previous Map
            /// </summary>
            public void PreviousMap()
            {
                //Decrease number
                currentMap--;
                //Check if we are below zero
                if (currentMap < 0) currentMap = menuManager.game.allPvpGameModes[currentGameMode].traditionalMaps.Length - 1; //If so, set to end of the array

                //Update display
                UpdateAllDisplays();
            }

            /// <summary>
            /// Selects the next Game Mode
            /// </summary>
            public void NextGameMode()
            {
                int previousGameMode = currentGameMode;

                //Increase number
                currentGameMode++;
                //Check if we have that many
                if (currentGameMode >= menuManager.game.allPvpGameModes.Length) currentGameMode = 0; //If not, reset

                //Reset settings
                currentAfkLimit = 0;
                currentDuration = 0;
                currentPingLimit = 0;
                currentPlayerLimit = 0;
                currentPlayerNeeded = 0;

                if (menuManager.game.allPvpGameModes[currentGameMode].traditionalMaps.Contains(menuManager.game.allPvpGameModes[previousGameMode].traditionalMaps[currentMap]))
                {
                    currentMap = Array.IndexOf(menuManager.game.allPvpGameModes[currentGameMode].traditionalMaps, menuManager.game.allPvpGameModes[previousGameMode].traditionalMaps[currentMap]);

                    if (currentMap < 0)
                    {
                        currentMap = 0;
                    }
                }
                else
                {
                    currentMap = 0;
                }

                //Update display
                UpdateAllDisplays();
            }

            /// <summary>
            /// Selects the previous Game Mode
            /// </summary>
            public void PreviousGameMode()
            {
                int previousGameMode = currentGameMode;

                //Decrease number
                currentGameMode--;
                //Check if we are below zero
                if (currentGameMode < 0) currentGameMode = menuManager.game.allPvpGameModes.Length - 1; //If so, set to end of the array

                //Reset settings
                currentAfkLimit = 0;
                currentDuration = 0;
                currentPingLimit = 0;
                currentPlayerLimit = 0;
                currentPlayerNeeded = 0;

                if (menuManager.game.allPvpGameModes[currentGameMode].traditionalMaps.Contains(menuManager.game.allPvpGameModes[previousGameMode].traditionalMaps[currentMap]))
                {
                    currentMap = Array.IndexOf(menuManager.game.allPvpGameModes[currentGameMode].traditionalMaps, menuManager.game.allPvpGameModes[previousGameMode].traditionalMaps[currentMap]);

                    if (currentMap < 0)
                    {
                        currentMap = 0;
                    }
                }
                else
                {
                    currentMap = 0;
                }

                //Update display
                UpdateAllDisplays();
            }

            /// <summary>
            /// Selects the next Duration
            /// </summary>
            public void NextDuration()
            {
                //Increase number
                currentDuration++;
                //Check if we have that many
                if (currentDuration >= menuManager.game.allPvpGameModes[currentGameMode].traditionalDurations.Length) currentDuration = 0; //If not, reset

                //Update display
                UpdateAllDisplays();
            }

            /// <summary>
            /// Selects the previous Duration
            /// </summary>
            public void PreviousDuration()
            {
                //Decrease number
                currentDuration--;
                //Check if we are below zero
                if (currentDuration < 0) currentDuration = menuManager.game.allPvpGameModes[currentGameMode].traditionalDurations.Length - 1; //If so, set to end of the array

                //Update display
                UpdateAllDisplays();
            }

            /// <summary>
            /// Selects the next Player Limit
            /// </summary>
            public void NextPlayerLimit()
            {
                //Increase number
                currentPlayerLimit++;
                //Check if we have that many
                if (currentPlayerLimit >= menuManager.game.allPvpGameModes[currentGameMode].traditionalPlayerLimits.Length) currentPlayerLimit = 0; //If not, reset

                //Check if we have more players needed than max players
                while (menuManager.game.allPvpGameModes[currentGameMode].traditionalPlayerNeeded[currentPlayerNeeded] > menuManager.game.allPvpGameModes[currentGameMode].traditionalPlayerLimits[currentPlayerLimit] && currentPlayerNeeded > 0)
                {
                    currentPlayerNeeded--;
                }

                //Update display
                UpdateAllDisplays();
            }

            /// <summary>
            /// Selects the previous Player Limit
            /// </summary>
            public void PreviousPlayerLimit()
            {
                //Decrease number
                currentPlayerLimit--;
                //Check if we are below zero
                if (currentPlayerLimit < 0) currentPlayerLimit = menuManager.game.allPvpGameModes[currentGameMode].traditionalPlayerLimits.Length - 1; //If so, set to end of the array

                //Check if we have more players needed than max players
                while (menuManager.game.allPvpGameModes[currentGameMode].traditionalPlayerNeeded[currentPlayerNeeded] > menuManager.game.allPvpGameModes[currentGameMode].traditionalPlayerLimits[currentPlayerLimit] && currentPlayerNeeded > 0)
                {
                    currentPlayerNeeded--;
                }

                //Update display
                UpdateAllDisplays();
            }

            /// <summary>
            /// Selects the next Player Needed
            /// </summary>
            public void NextPlayerNeeded()
            {
                //Increase number
                currentPlayerNeeded++;
                //Check if we have that many
                if (currentPlayerNeeded >= menuManager.game.allPvpGameModes[currentGameMode].traditionalPlayerNeeded.Length) currentPlayerNeeded = 0; //If not, reset
                                                                                                                                                      //Check if we have more players needed than max players
                if (menuManager.game.allPvpGameModes[currentGameMode].traditionalPlayerNeeded[currentPlayerNeeded] > menuManager.game.allPvpGameModes[currentGameMode].traditionalPlayerLimits[currentPlayerLimit]) currentPlayerNeeded = 0;

                //Update display
                UpdateAllDisplays();
            }

            /// <summary>
            /// Selects the previous Player Needed
            /// </summary>
            public void PreviousPlayerNeeded()
            {
                //Decrease number
                currentPlayerNeeded--;
                //Check if we are below zero
                if (currentPlayerNeeded < 0) currentPlayerNeeded = menuManager.game.allPvpGameModes[currentGameMode].traditionalPlayerNeeded.Length - 1; //If so, set to end of the array

                //Check if we have more players needed than max players
                while (menuManager.game.allPvpGameModes[currentGameMode].traditionalPlayerNeeded[currentPlayerNeeded] > menuManager.game.allPvpGameModes[currentGameMode].traditionalPlayerLimits[currentPlayerLimit] && currentPlayerNeeded > 0)
                {
                    currentPlayerNeeded--;
                }

                //Update display
                UpdateAllDisplays();
            }

            /// <summary>
            /// Selects the next Ping Limit
            /// </summary>
            public void NextPingLimit()
            {
                //Increase number
                currentPingLimit++;
                //Check if we have that many
                if (currentPingLimit >= menuManager.game.allPvpGameModes[currentGameMode].traditionalPingLimits.Length) currentPingLimit = 0; //If not, reset

                //Update display
                UpdateAllDisplays();
            }

            /// <summary>
            /// Selects the previous Ping Limit
            /// </summary>
            public void PreviousPingLimit()
            {
                //Decrease number
                currentPingLimit--;
                //Check if we are below zero
                if (currentPingLimit < 0) currentPingLimit = menuManager.game.allPvpGameModes[currentGameMode].traditionalPingLimits.Length - 1; //If so, set to end of the array

                //Update display
                UpdateAllDisplays();
            }

            /// <summary>
            /// Selects the next Afk Limit
            /// </summary>
            public void NextAFKLimit()
            {
                //Increase number
                currentAfkLimit++;
                //Check if we have that many
                if (currentAfkLimit >= menuManager.game.allPvpGameModes[currentGameMode].traditionalAfkLimits.Length) currentAfkLimit = 0; //If not, reset

                //Update display
                UpdateAllDisplays();
            }

            /// <summary>
            /// Selects the previous Afk Limit
            /// </summary>
            public void PreviousAFKLimit()
            {
                //Decrease number
                currentAfkLimit--;
                //Check if we are below zero
                if (currentAfkLimit < 0) currentAfkLimit = menuManager.game.allPvpGameModes[currentGameMode].traditionalAfkLimits.Length - 1; //If so, set to end of the array

                //Update display
                UpdateAllDisplays();
            }

            /// <summary>
            /// Selects the next online mode
            /// </summary>
            public void NextOnlineMode()
            {
                //Increase number
                currentOnlineMode++;
                //Check if we have that many
                if (currentOnlineMode >= 2) currentOnlineMode = 0; //If not, reset

                //Update display
                UpdateAllDisplays();
            }

            /// <summary>
            /// Selects the previous online mode
            /// </summary>
            public void PreviousOnlineMode()
            {
                //Decrease number
                currentOnlineMode--;
                //Check if we are below zero
                if (currentOnlineMode < 0) currentOnlineMode = 1; //If so, set to end of the array

                //Update display
                UpdateAllDisplays();
            }

            /// <summary>
            /// Selects the next bot mode
            /// </summary>
            public void NextBotMode()
            {
                //Increase number
                currentBotMode++;
                //Check if we have that many
                if (currentBotMode >= 2) currentBotMode = 0; //If not, reset

                //Update display
                UpdateAllDisplays();
            }

            /// <summary>
            /// Selects the previous bot mode
            /// </summary>
            public void PreviousBotMode()
            {
                //Decrease number
                currentBotMode--;
                //Check if we are below zero
                if (currentBotMode < 0) currentBotMode = 1; //If so, set to end of the array

                //Update display
                UpdateAllDisplays();
            }
            #endregion
        }
    }
}