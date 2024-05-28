using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Mirror;
using MarsFPSKit.Networking;
using MarsFPSKit.UI;
using Unity.Services.Lobbies.Models;

#if INTEGRATION_STEAM
using Steamworks;
#endif

namespace MarsFPSKit
{
    //Pause Menu state enum
    public enum PauseMenuState { teamSelection = -1, main = 0 }

    public enum AfterTeamSelection { PauseMenu, AttemptSpawn, Loadout }

    /// <summary>
    /// This class is used to store spawns for a game mode internally
    /// </summary>
    [System.Serializable]
    public class InternalSpawns
    {
        public List<Kit_PlayerSpawn> spawns = new List<Kit_PlayerSpawn>();
    }

    [DefaultExecutionOrder(-1)]
    /// <summary>
    /// The Main script of the ingame logic (This is the heart of the game)
    /// </summary>
    /// //It is a PunBehaviour so we can have all the callbacks that we need
    public class Kit_IngameMain : NetworkBehaviour
    {
        public static Kit_IngameMain instance;

        /// <summary>
        /// The root of all UI
        /// </summary>
        public GameObject ui_root;

        /// <summary>
        /// Main canvas
        /// </summary>
        public Canvas canvas;

        //The current state of the pause menu
        public PauseMenuState pauseMenuState = PauseMenuState.teamSelection;

        //This hols all game information
        #region Game Information
        [Header("Internal Game Information")]
        [Tooltip("This object contains all game information such as Maps, Game Modes and Weapons")]
        public Kit_GameInformation gameInformation;

        public GameObject playerPrefab; //The player prefab that we should use
        #endregion

        #region Menu Managing
        [Header("Menu Manager")]
        /// <summary>
        /// Menu screens
        /// </summary>
        public MenuScreen[] menuScreens;
        /// <summary>
        /// ID for fading INTO the game
        /// </summary>
        public int ingameFadeId = 2;
        /// <summary>
        /// Do we have a screen to fade out?
        /// </summary>
        private bool wasFirstScreenFadedIn;
        /// <summary>
        /// The menu screen that is currently visible (in order to fade it out)
        /// </summary>
        public int currentScreen = -1;
        /// <summary>
        /// True if we are currently switching a screen
        /// </summary>
        private bool isSwitchingScreens;
        /// <summary>
        /// Where we are currently switching screens to
        /// </summary>
        private Coroutine currentlySwitchingScreensTo;
        #endregion

        [Header("Map Settings")]
        /// <summary>
        /// If you are below this position on your y axis, you die
        /// </summary>
        public float mapDeathThreshold = -50f;

        //This contains all the game mode informations
        #region Game Mode Variables
        [Header("Game Mode Variables")]
        [SyncVar]
        /// <summary>
        /// The game mode timer
        /// </summary>
        public float timer = 600f;
        [SyncVar]
        /// <summary>
        /// A universal stage for game modes, since every game mode requires one like this
        /// </summary>
        public int gameModeStage;
        /// <summary>
        /// Used for the game mode stage changed callback (Called for everyone)
        /// </summary>
        private int lastGameModeStage;
        [SyncVar]
        /// <summary>
        /// The game mode type (!) we are currently playing
        /// </summary>
        public int currentGameModeType;
        [SyncVar]
        /// <summary>
        /// The game mode we are currently playing
        /// </summary>
        public int currentGameMode;
        /// <summary>
        /// Here you can store runtime data for the game mode. Just make sure to sync it to everybody
        /// </summary>
        public Kit_GameModeNetworkDataBase currentGameModeRuntimeData;

        [HideInInspector]
        public List<InternalSpawns> internalSpawns = new List<InternalSpawns>();
        #endregion

        #region Team Selection
        /// <summary>
        /// Team selection module
        /// </summary>
        [Header("Team Selection")]
        public UI.Kit_IngameMenuTeamSelection ts;
        /// <summary>
        /// The text which displays that we cannot join that team
        /// </summary>
        public TextMeshProUGUI errorText;
        /// <summary>
        /// How long is the warning going to be displayed?
        /// </summary>
        public float errorTime = 3f;
        /// <summary>
        /// Current alpha of the cant join message
        /// </summary>
        private float errorAlpha = 0f;
        #endregion

        //This contains everything needed for the Pause Menu
        #region Pause Menu
        /// <summary>
        /// Pause Menu module
        /// </summary>
        [Header("Pause Menu, Use 'B' in the editor to open / close it")]
        public Kit_IngameMenuPauseMenu pauseMenu;
        #endregion

        /// <summary>
        /// New, modular options screen!!!
        /// </summary>
        [Header("Options")]
        public Kit_MenuOptions options;

        //This contains the local camera control
        #region Camera Control
        [Header("Camera Control")]
        public Camera mainCamera; //The main camera to use for the whole game
#if INTEGRATION_FPV2
        public Camera weaponCamera; //This is a second camera used by FPV2 for the matrix.
#elif INTEGRATION_FPV3
        public Camera weaponCamera; //This is a second camera used by FPV2 for the matrix.
#endif
        /// <summary>
        /// Camera shake!
        /// </summary>
        public Kit_CameraShake cameraShake;
        //We recycle the same camera for the whole game, for easy setup of image effects
        //Be careful when changing near and far clip
        public Transform activeCameraTransform
        {
            get
            {
                if (mainCamera)
                {
                    return mainCamera.transform.parent;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value)
                {
                    Debug.Log("[Camera] Changing camera parent to " + value.name, value);
                }
                else
                {
                    Debug.Log("[Camera] Changing camera parent to null");
                }

                if (mainCamera)
                {
                    //We use one camera for the complete game
                    //Set parent
                    mainCamera.transform.parent = value;
                    //If the parent is not null, reset position and rotation
                    if (value)
                    {
                        mainCamera.transform.localPosition = Vector3.zero;
                        mainCamera.transform.localRotation = Quaternion.identity;
                    }
                }
            }
        }
        public Transform spawnCameraPosition; //The spawn position for the camera
        #endregion

        [Header("Modules")]
        [Header("HUD")]
        //This contains the HUD reference
        #region HUD
        /// <summary>
        /// Use this to access the Player HUD
        /// </summary>
        public Kit_PlayerHUDBase hud;
        #endregion

        //This contains the Killfeed reference
        #region Killfeed
        [Header("Killfeed")]
        public Kit_KillFeedBase killFeed;
        #endregion

        #region Chat
        [Header("Chat")]
        public Kit_ChatBase chat;
        #endregion

        #region Impact Processor
        [Header("Impact Processor")]
        public Kit_ImpactParticleProcessor impactProcessor;
        #endregion

        #region Scoreboard
        [Header("Scoreboard")]
        public Scoreboard.Kit_ScoreboardBase scoreboard;
        #endregion

        #region PointsUI
        [Header("Points UI")]
        public Kit_PointsUIBase pointsUI;
        #endregion

        #region Victory Screen
        [Header("Victory Screen")]
        public Kit_VictoryScreenUI victoryScreenUI;
        #endregion

        #region MapVoting
        [Header("Map Voting")]
        public Kit_MapVotingUIBase mapVotingUI;
        #endregion

        #region Ping Limit
        [Header("Ping Limit")]
        public Kit_PingLimitBase pingLimitSystem;
        public Kit_PingLimitUIBase pingLimitUI;
        #endregion

        #region AFK Limit
        [Header("AFK Limit")]
        public Kit_AfkLimitBase afkLimitSystem;
        public Kit_AfkLimitUIBase afkLimitUI;
        #endregion

        #region Loadout
        [Header("Loadout")]
        public Kit_LoadoutBase loadoutMenu;
        #endregion

        #region Voting
        [Header("Voting")]
        public Kit_VotingUIBase votingMenu;
        [HideInInspector]
        public Kit_VotingBase currentVoting;
        #endregion

        #region Voice Chat
        [Header("Voice Chat")]
        public Kit_VoiceChatBase voiceChat;
        #endregion

        #region Leveling UI
        /// <summary>
        /// If this is assigned, it will display level ups
        /// </summary>
        [Header("Leveling UI")]
        public Kit_LevelingUIBase levelingUi;
        #endregion

        #region Minimap
        [Header("Minimap")]
        public Kit_MinimapBase minimap;
        #endregion

        #region Auto Spawn System
        /// <summary>
        /// Spawn system to use
        /// </summary>
        [Header("Auto Spawn System")]
        public Kit_AutoSpawnSystemBase autoSpawnSystem;
        #endregion

        #region Object Pooling
        [Header("Object Pooling")]
        /// <summary>
        /// Object Pooling interface
        /// </summary>
        public Optimization.Kit_ObjectPoolingBase objectPooling;
        #endregion

        #region Assists
        /// <summary>
        /// Assist manager module
        /// </summary>
        [Header("Assists")]
        public Kit_AssistManagerBase assistManager;
        /// <summary>
        /// Runtime data for the assist manager
        /// </summary>
        public object assistManagerData;
        #endregion

        #region Spectating
        [Header("Spectator")]
        /// <summary>
        /// Spectator manager
        /// </summary>
        public Spectating.Kit_SpectatorManagerBase spectatorManager;
        /// <summary>
        /// Runtime data for the spectator manager
        /// </summary>
        public object spectatorManagerRuntimeData;
        #endregion

        /// <summary>
        /// A hud that is only visible when the player is alive can be instantiated here
        /// </summary>
        [Header("Plugins")]
        public RectTransform pluginPlayerActiveHudGo;
        /// <summary>
        /// A hud that is always active can be instantiated here
        /// </summary>
        public RectTransform pluginAlwaysActiveHudGo;
        /// <summary>
        /// Where external modules can be instantiated
        /// </summary>
        public Transform pluginModuleGo;

        [Header("Instantiateables")]
        /// <summary>
        /// This contains the prefab for the victory screen. Once its setup it will sync to all other players.
        /// </summary>
        public GameObject victoryScreen;
        [HideInInspector]
        /// <summary>
        /// A reference to the victory screen so it can be destroyed when it's not needed anymore.
        /// </summary>
        public Kit_VictoryScreen currentVictoryScreen;
        /// <summary>
        /// This contains the prefab for the map voting. Once its setup it will sync to all other players
        /// </summary>
        public GameObject mapVoting;
        [HideInInspector]
        /// <summary>
        /// A reference to the map voting. Can be null
        /// </summary>
        public Kit_MapVotingBehaviour currentMapVoting;

        /// <summary>
        /// Prefab for the bot manager
        /// </summary>
        [Header("Bots")]
        public GameObject botManagerPrefab;
        [HideInInspector]
        /// <summary>
        /// If Bots are enabled, this is the bot manager
        /// </summary>
        public Kit_BotManager currentBotManager;
        [HideInInspector]
        /// <summary>
        /// All bot nav points
        /// </summary>
        public Transform[] botNavPoints;

        /// <summary>
        /// This is the input that shall be used if we have a touchscreen
        /// </summary>
        [Header("Touchscreen Input")]
        public GameObject touchScreenPrefab;
        [HideInInspector]
        /// <summary>
        /// If touch screen input is enabled, this is assigned.
        /// </summary>
        public Kit_TouchscreenBase touchScreenCurrent;


        //This section contains internal variables used by the game
        #region Internal Variables
        /// <summary>
        /// Only used in PVP Game Modes. Assigned Team ID
        /// </summary>
        public sbyte assignedTeamID = -1;
        /// <summary>
        /// Our own player, returns null if we have not spawned
        /// </summary>
        public Kit_PlayerBehaviour myPlayer;
        /// <summary>
        /// Is the pause menu currently opened?
        /// </summary>
        [HideInInspector]
        public static bool isPauseMenuOpen;
        [HideInInspector]
        /// <summary>
        /// Active PvE game mode
        /// </summary>
        public Kit_PvE_GameModeBase currentPvEGameModeBehaviour;
        [HideInInspector]
        /// <summary>
        /// Active PvP game mode
        /// </summary>
        public Kit_PvP_GameModeBase currentPvPGameModeBehaviour;
        /// <summary>
        /// Instance of current game mode HUD. Could be null.
        /// </summary>
        [HideInInspector]
        public Kit_GameModeHUDBase currentGameModeHUD;
        [HideInInspector]
        /// <summary>
        /// Is the ping limit system enabled by the user?
        /// </summary>
        public bool isPingLimiterEnabled = false;
        [HideInInspector]
        /// <summary>
        /// Is the afk limit system enabled by the user?
        /// </summary>
        public bool isAfkLimiterEnabled = false;
        [SyncVar]
        /// <summary>
        /// Have we actually begun to play this game mode?
        /// </summary>
        public bool hasGameModeStarted = false;
        [HideInInspector]
        /// <summary>
        /// Is the camera fov overriden?
        /// </summary>
        public bool isCameraFovOverridden;

        public List<object> pluginRuntimeData = new List<object>();
        /// <summary>
        /// This is a list of all active players
        /// </summary>
        public List<Kit_PlayerBehaviour> allActivePlayers = new List<Kit_PlayerBehaviour>();

        public static bool executeDestroyActions
        {
            get
            {
                return Kit_IngameMain.instance && !Kit_IngameMain.instance.isShuttingDown && Kit_SceneSyncer.instance && !Kit_SceneSyncer.instance.isLoading;
            }
        }
        #endregion

        #region Unity Calls
        void Awake()
        {
            //Set instance. Make sure Kit_IngameMain is first in execution order (before any other kit scripts)
            instance = this;
            //Hide HUD initially
            hud.SetVisibility(false);
            //Set pause menu state
            isPauseMenuOpen = false;

            //Disable all the roots
            for (int i = 0; i < menuScreens.Length; i++)
            {
                if (menuScreens[i].root)
                {
                    //Disable
                    menuScreens[i].root.SetActive(false);
                }
                else
                {
                    Debug.LogError("Menu root at index " + i + " is not assigned.", this);
                }
            }
        }

        public void OnEnable()
        {
            //Check if we shall replace camera
            if (gameInformation.mainCameraOverride)
            {
                //Instantiate new
                GameObject newCamera = Instantiate(gameInformation.mainCameraOverride, mainCamera.transform, false);
                //Reparent
                newCamera.transform.parent = spawnCameraPosition;
                //Destroy camera
                Destroy(mainCamera.gameObject);
                //Assign new camera
                mainCamera = newCamera.GetComponent<Camera>();
                //Camera Shake
                cameraShake = newCamera.GetComponentInChildren<Kit_CameraShake>();
#if INTEGRATION_FPV2
                //Get weapon camera
                weaponCamera = newCamera.GetComponentInChildren<FirstPersonView.ShaderMaterialSolution.FPV_SM_FirstPersonCamera>().GetComponent<Camera>();
                if (!weaponCamera) Debug.LogError("FPV2 is enabled but correct prefab is not assigned!", gameInformation.mainCameraOverride);
#elif INTEGRATION_FPV3
                //Get weapon camera
                weaponCamera = newCamera.GetComponentInChildren<FirstPersonView.FPV_Camera_FirstPerson>().GetComponent<Camera>();
                if (!weaponCamera) Debug.LogError("FPV3 is enabled but correct prefab is not assigned!", gameInformation.mainCameraOverride);
#endif
            }
        }

        void Update()
        {
            //If we are in a room
            if (NetworkServer.active || NetworkClient.active)
            {
                if (Time.timeScale != 1)
                {
                    Time.timeScale = 1f;
                    Debug.LogWarning("Timescale must remain 1");
                }

                //Host Logic
                if (NetworkServer.active && hasGameModeStarted)
                {
                    #region Timer
                    //Decrease timer
                    if (timer > 0)
                    {
                        timer -= Time.deltaTime;
                        //Check if the timer has run out
                        if (timer <= 0)
                        {
                            if (currentPvEGameModeBehaviour)
                                //Call the game mode callback
                                currentPvEGameModeBehaviour.TimeRunOut();

                            if (currentPvPGameModeBehaviour)
                                //Call the game mode callback
                                currentPvPGameModeBehaviour.TimeRunOut();
                        }
                    }
                    #endregion
                }

                #region Pause Menu
                //Check if the pause menu is ready to be opened and closed and if nothing is blocking it
                if (pauseMenuState >= 0 && !currentVictoryScreen && !currentMapVoting && (!loadoutMenu || loadoutMenu && currentScreen != loadoutMenu.menuScreenId))
                {
                    if (Input.GetKeyDown(KeyCode.Escape) && Application.platform != RuntimePlatform.WebGLPlayer || Input.GetKeyDown(KeyCode.B) && Application.isEditor || Input.GetKeyDown(KeyCode.M) && Application.platform == RuntimePlatform.WebGLPlayer || Application.isMobilePlatform && UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.GetButtonDown("Pause")) //Escape (for non WebGL), B (For the editor), M (For WebGL)
                    {
                        //Change state
                        isPauseMenuOpen = !isPauseMenuOpen;
                        //Set state
                        if (isPauseMenuOpen)
                        {
                            SwitchMenu(pauseMenu.pauseMenuId, true);
                            //Unlock cursor
                            MarsScreen.lockCursor = false;
                            //Chat callback
                            chat.PauseMenuOpened();
                            //Auto spawn system callack
                            if (autoSpawnSystem && currentPvPGameModeBehaviour)
                            {
                                autoSpawnSystem.Interruption();
                            }
                        }
                        else
                        {
                            SwitchMenu(ingameFadeId, true);
                            pluginOnForceClose.Invoke();
                            //Lock cursor
                            MarsScreen.lockCursor = true;
                            //Chat callback
                            chat.PauseMenuClosed();
                        }
                    }
                }
                #endregion

                #region HUD Update
                if (currentGameModeHUD)
                {
                    //Relay update
                    currentGameModeHUD.HUDUpdate();
                }
                #endregion

                #region Game Mode
                if (NetworkServer.active)
                {
                    if (currentPvPGameModeBehaviour)
                    {
                        currentPvPGameModeBehaviour.GameModeUpdate();
                    }
                    else if (currentPvEGameModeBehaviour)
                    {
                        currentPvEGameModeBehaviour.GameModeUpdate();
                    }
                }
                else
                {
                    if (currentPvPGameModeBehaviour)
                    {
                        currentPvPGameModeBehaviour.GameModeUpdateOthers();
                    }
                    else if (currentPvEGameModeBehaviour)
                    {
                        currentPvEGameModeBehaviour.GameModeUpdateOthers();
                    }
                }

                //Check if the game mode stage has changed
                if (lastGameModeStage != gameModeStage)
                {
                    //Call the callback
                    GameModeStageChanged(lastGameModeStage, gameModeStage);
                    //Set value
                    lastGameModeStage = gameModeStage;
                }
                #endregion

                #region PvP only
                if (currentGameModeType == 2)
                {
                    #region Ping Limiter
                    if (isPingLimiterEnabled && pingLimitSystem && isServer)
                    {
                        pingLimitSystem.UpdateRelay();
                    }
                    #endregion

                    #region AFK Limiter
                    if (isAfkLimiterEnabled && afkLimitSystem)
                    {
                        afkLimitSystem.UpdateRelay();
                    }
                    #endregion

                    #region Waiting for Players
                    //Check if the game mode should begin
                    if (!hasGameModeStarted)
                    {
                        if (NetworkServer.active)
                        {
                            //Check if we now have enough players
                            if (currentPvPGameModeBehaviour.AreEnoughPlayersThere())
                            {
                                hasGameModeStarted = true;
                                currentPvPGameModeBehaviour.GameModeBeginMiddle();
                            }
                        }
                        //Show waiting on the HUD
                        hud.SetWaitingStatus(true);
                    }
                    else
                    {
                        //Hide waiting on the HUD
                        hud.SetWaitingStatus(false);
                    }
                    #endregion
                }
                else //no pvp - no waiting
                {
                    //Hide waiting on the HUD
                    hud.SetWaitingStatus(false);
                }
                #endregion

                #region Cannot Join Team
                if (errorAlpha > 0)
                {
                    //Decrease
                    errorAlpha -= Time.deltaTime;

                    //Set alpha
                    errorText.color = new Color(errorText.color.r, errorText.color.g, errorText.color.b, errorAlpha);

                    //Enable
                    errorText.enabled = true;
                }
                else
                {
                    //Just disable
                    errorText.enabled = false;
                }
                #endregion

                #region FOV
                if (!myPlayer && (!spectatorManager || !spectatorManager.IsCurrentlySpectating()))
                {
                    if (!isCameraFovOverridden)
                        mainCamera.fieldOfView = Kit_GameSettings.baseFov;
                }
#if INTEGRATION_FPV2
                else
                {
                    if (weaponCamera)
                    {
                        //Make sure FOV is the same.
                        weaponCamera.fieldOfView = mainCamera.fieldOfView;
                    }
                }
#elif INTEGRATION_FPV3
                else
                {
                    if (weaponCamera)
                    {
                        //Make sure FOV is the same.
                        weaponCamera.fieldOfView = mainCamera.fieldOfView;
                    }
                }
#endif
                #endregion

                #region Plugin
                //Call Plugin
                for (int i = 0; i < gameInformation.plugins.Length; i++)
                {
                    gameInformation.plugins[i].PluginUpdate();
                }
                #endregion
            }
        }

        void LateUpdate()
        {
            if (NetworkServer.active || NetworkClient.active)
            {
                //Call Plugin
                for (int i = 0; i < gameInformation.plugins.Length; i++)
                {
                    gameInformation.plugins[i].PluginLateUpdate();
                }
            }
        }

        /// <summary>
        /// Set to true if the application is closing
        /// </summary>
        public bool isShuttingDown = false;

        void OnApplicationQuit()
        {
            isShuttingDown = true;

            if (gameInformation.leveling)
            {
                gameInformation.leveling.Save();
            }

            if (gameInformation.statistics)
            {
                gameInformation.statistics.Save();
            }
        }

        private void OnDestroy()
        {
            if (executeDestroyActions)
            {
                Kit_NetworkPlayerManager.instance.onPlayerJoined.RemoveListener(OnPlayerEnteredRoom);
                Kit_NetworkPlayerManager.instance.onPlayerLeft.RemoveListener(OnPlayerLeftRoom);
            }
        }
        #endregion

        #region Mirror Calls
        public override void OnStartServer()
        {
            //Call Plugin
            for (int i = 0; i < gameInformation.plugins.Length; i++)
            {
                gameInformation.plugins[i].OnPreSetup();
            }

            //Check if mobile input should be used
            if (gameInformation.enableTouchscreenInput)
            {
                //Create input
                GameObject go = Instantiate(touchScreenPrefab);
                touchScreenCurrent = go.GetComponent<Kit_TouchscreenBase>();
                //Setup
                touchScreenCurrent.Setup();
            }

            //Impact Processor
            if (impactProcessor)
            {
                impactProcessor.StartImpactProcessor();
            }

            //Assist Manager
            if (assistManager)
            {
                assistManager.OnStart();
            }

            //Set initial states
            pluginOnForceClose.Invoke();
            ui_root.SetActive(true);
            assignedTeamID = -1;

            //Make sure the main camera is child of the spawn camera position
            activeCameraTransform = spawnCameraPosition;

            if (gameInformation)
            {
                //Get type
                int gameModeType = Kit_NetworkGameInformation.instance.gameModeType;
                //Assign
                currentGameModeType = gameModeType;

                if (gameModeType == 0)
                {
                    //Setup Game Mode based on Custom properties
                    int gameMode = Kit_NetworkGameInformation.instance.gameMode;
                    currentGameMode = gameMode;
                    currentPvEGameModeBehaviour = gameInformation.allSingleplayerGameModes[gameMode];
                    gameInformation.allSingleplayerGameModes[gameMode].GamemodeSetupServer();
                    hasGameModeStarted = true;

                    //Setup HUD
                    if (currentPvEGameModeBehaviour.hudPrefab)
                    {
                        GameObject hudPrefab = Instantiate(currentPvEGameModeBehaviour.hudPrefab, hud.transform, false);
                        //Move to the back
                        hudPrefab.transform.SetAsFirstSibling();
                        //Reset scale
                        hudPrefab.transform.localScale = Vector3.one;
                        //Get script
                        currentGameModeHUD = hudPrefab.GetComponent<Kit_GameModeHUDBase>();
                        //Start
                        currentGameModeHUD.HUDInitialize();
                    }
                }
                else if (gameModeType == 1)
                {
                    //Setup Game Mode based on Custom properties
                    int gameMode = Kit_NetworkGameInformation.instance.gameMode;
                    currentGameMode = gameMode;
                    currentPvEGameModeBehaviour = gameInformation.allCoopGameModes[gameMode];
                    gameInformation.allCoopGameModes[gameMode].GamemodeSetupServer();
                    hasGameModeStarted = true;

                    //Setup HUD
                    if (currentPvEGameModeBehaviour.hudPrefab)
                    {
                        GameObject hudPrefab = Instantiate(currentPvEGameModeBehaviour.hudPrefab, hud.transform, false);
                        //Move to the back
                        hudPrefab.transform.SetAsFirstSibling();
                        //Reset scale
                        hudPrefab.transform.localScale = Vector3.one;
                        //Get script
                        currentGameModeHUD = hudPrefab.GetComponent<Kit_GameModeHUDBase>();
                        //Start
                        currentGameModeHUD.HUDInitialize();
                    }
                }
                else if (gameModeType == 2)
                {
                    //Setup Game Mode based on Custom properties
                    int gameMode = Kit_NetworkGameInformation.instance.gameMode;
                    currentGameMode = gameMode;
                    currentPvPGameModeBehaviour = gameInformation.allPvpGameModes[gameMode];
                    gameInformation.allPvpGameModes[gameMode].GamemodeSetupServer();

                    ts.Setup();

                    //If we already have a game mode hud, destroy it
                    if (currentGameModeHUD)
                    {
                        Destroy(currentGameModeHUD.gameObject);
                    }

                    //Initialize Loadout Menu
                    if (loadoutMenu)
                    {
                        loadoutMenu.Initialize();
                        //Force it to be closed
                        loadoutMenu.ForceClose();
                    }

                    //Setup HUD
                    if (currentPvPGameModeBehaviour.hudPrefab)
                    {
                        GameObject hudPrefab = Instantiate(currentPvPGameModeBehaviour.hudPrefab, hud.transform, false);
                        //Move to the back
                        hudPrefab.transform.SetAsFirstSibling();
                        //Reset scale
                        hudPrefab.transform.localScale = Vector3.one;
                        //Get script
                        currentGameModeHUD = hudPrefab.GetComponent<Kit_GameModeHUDBase>();
                        //Start
                        currentGameModeHUD.HUDInitialize();
                    }

                    //Set timer
                    int duration = Kit_NetworkGameInformation.instance.duration;
                    //Assign global game length
                    Kit_GameSettings.gameLength = duration;

                    if (Kit_GameSettings.currentNetworkingMode == KitNetworkingMode.Traditional)
                    {
                        //Get ping limit
                        int pingLimit = Kit_NetworkGameInformation.instance.ping;

                        if (currentPvPGameModeBehaviour.traditionalPingLimits[pingLimit] > 0)
                        {
                            //Ping limit enabled
                            if (pingLimitSystem)
                            {
                                //Tell the system to start
                                pingLimitSystem.StartRelay(true, currentPvPGameModeBehaviour.traditionalPingLimits[pingLimit]);
                                isPingLimiterEnabled = true;
                            }
                        }
                        else
                        {
                            //Ping limit disablde
                            if (pingLimitSystem)
                            {
                                //Tell the system to not start
                                pingLimitSystem.StartRelay(false);
                                isPingLimiterEnabled = false;
                            }
                        }

                        //Get AFK limit
                        int afkLimit = Kit_NetworkGameInformation.instance.afk;

                        if (currentPvPGameModeBehaviour.traditionalAfkLimits[afkLimit] > 0)
                        {
                            //AFK limit enabled
                            if (afkLimitSystem)
                            {
                                //Relay to the system
                                afkLimitSystem.StartRelay(true, currentPvPGameModeBehaviour.traditionalAfkLimits[afkLimit]);
                                isAfkLimiterEnabled = true;
                            }
                        }
                        else
                        {
                            //AFK limit disabled
                            if (afkLimitSystem)
                            {
                                afkLimitSystem.StartRelay(false);
                                isAfkLimiterEnabled = false;
                            }
                        }
                    }
                    else if (Kit_GameSettings.currentNetworkingMode == KitNetworkingMode.Lobby)
                    {
                        if (currentPvPGameModeBehaviour.lobbyPingLimit > 0)
                        {
                            //Ping limit enabled
                            if (pingLimitSystem)
                            {
                                //Tell the system to start
                                pingLimitSystem.StartRelay(true, currentPvPGameModeBehaviour.lobbyPingLimit);
                                isPingLimiterEnabled = true;
                            }
                        }
                        else
                        {
                            //Ping limit disablde
                            if (pingLimitSystem)
                            {
                                //Tell the system to not start
                                pingLimitSystem.StartRelay(false);
                                isPingLimiterEnabled = false;
                            }
                        }

                        if (currentPvPGameModeBehaviour.lobbyAfkLimit > 0)
                        {
                            //AFK limit enabled
                            if (afkLimitSystem)
                            {
                                //Relay to the system
                                afkLimitSystem.StartRelay(true, currentPvPGameModeBehaviour.lobbyAfkLimit);
                                isAfkLimiterEnabled = true;
                            }
                        }
                        else
                        {
                            //AFK limit disabled
                            if (afkLimitSystem)
                            {
                                afkLimitSystem.StartRelay(false);
                                isAfkLimiterEnabled = false;
                            }
                        }
                    }

                    //Setup Bots
                    if (Kit_NetworkGameInformation.instance.bots)
                    {
                        //Setup Nav Points
                        Kit_BotNavPoint[] navPoints = FindObjectsOfType<Kit_BotNavPoint>();

                        if (navPoints.Length == 0) throw new System.Exception("[Bots] No Nav Points have been found for this scene! You need to add some.");
                        List<Transform> tempNavPoints = new List<Transform>();
                        for (int i = 0; i < navPoints.Length; i++)
                        {
                            if (navPoints[i].gameModes.Contains(currentPvPGameModeBehaviour))
                            {
                                if (navPoints[i].navPointGroupID == 0)
                                {
                                    tempNavPoints.Add(navPoints[i].transform);
                                }
                            }
                        }
                        botNavPoints = tempNavPoints.ToArray();

                        if (NetworkServer.active)
                        {
                            if (!currentBotManager)
                            {
                                //Create locally
                                GameObject go = Instantiate(botManagerPrefab);
                                currentBotManager = go.GetComponent<Kit_BotManager>();
                                if (currentPvPGameModeBehaviour.botManagerToUse)
                                {
                                    currentPvPGameModeBehaviour.botManagerToUse.Inizialize(currentBotManager);
                                }
                                //Attach to network
                                NetworkServer.Spawn(go);
                            }
                        }
                        else
                        {
                            if (!currentBotManager)
                            {
                                currentBotManager = FindObjectOfType<Kit_BotManager>();
                            }
                        }
                    }

                    //Check if we already have enough players to start playing
                    if (currentPvPGameModeBehaviour.AreEnoughPlayersThere())
                    {
                        hasGameModeStarted = true;
                    }
                }

                if (voiceChat)
                {
                    //Setup Voice Chat
                    voiceChat.Setup();
                }

                if (minimap)
                {
                    //Tell Minimap to set itself up
                    minimap.Setup();
                }

                if (spectatorManager)
                {
                    //Setup spectator manager
                    spectatorManager.Setup();
                }

#if INTEGRATION_STEAM
                    //Set Steam Rich Presence
                    //Set connect
                    //Region@Room Name
                    SteamFriends.SetRichPresence("connect", PhotonNetwork.CloudRegion.ToString() + ":" + PhotonNetwork.CurrentRoom.Name);
                    //Set Status
                    SteamFriends.SetRichPresence("status", "Playing " + gameInformation.allPvpGameModes[gameMode].gameModeName + " on " + gameInformation.GetMapInformationFromSceneName(SceneManager.GetActiveScene().name).mapName);
#endif

                //Call Plugin
                for (int i = 0; i < gameInformation.plugins.Length; i++)
                {
                    gameInformation.plugins[i].OnSetupDone();
                }

                Kit_NetworkPlayerManager.instance.onPlayerJoined.AddListener(OnPlayerEnteredRoom);
                Kit_NetworkPlayerManager.instance.onPlayerLeft.AddListener(OnPlayerLeftRoom);
            }
            else
            {
                Debug.LogError("No Game Information assigned. Game will not work.");
            }
        }

        public override void OnStartClient()
        {
            if (!NetworkServer.active)
            {
                //Call Plugin
                for (int i = 0; i < gameInformation.plugins.Length; i++)
                {
                    gameInformation.plugins[i].OnPreSetup();
                }

                //Check if mobile input should be used
                if (gameInformation.enableTouchscreenInput)
                {
                    //Create input
                    GameObject go = Instantiate(touchScreenPrefab);
                    touchScreenCurrent = go.GetComponent<Kit_TouchscreenBase>();
                    //Setup
                    touchScreenCurrent.Setup();
                }

                //Impact Processor
                if (impactProcessor)
                {
                    impactProcessor.StartImpactProcessor();
                }

                //Assist Manager
                if (assistManager)
                {
                    assistManager.OnStart();
                }

                //Set initial states
                pluginOnForceClose.Invoke();
                ui_root.SetActive(true);
                assignedTeamID = -1;

                //Make sure the main camera is child of the spawn camera position
                activeCameraTransform = spawnCameraPosition;

                if (!gameInformation)
                {
                    Debug.LogError("No Game Information assigned. Game will not work.");
                }

                Kit_NetworkPlayerManager.instance.onPlayerJoined.AddListener(OnPlayerEnteredRoom);
                Kit_NetworkPlayerManager.instance.onPlayerLeft.AddListener(OnPlayerLeftRoom);
            }

            //Unlock the cursor
            MarsScreen.lockCursor = false;
        }
        #endregion

        #region Custom Mirror related calls
        public void OnClientDataReadyToSetup()
        {
            //Also called on host, host does not need to do this
            if (!NetworkServer.active)
            {
                //Get type
                int gameModeType = Kit_NetworkGameInformation.instance.gameModeType;
                //Assign
                currentGameModeType = gameModeType;

                if (gameModeType == 0)
                {
                    //Setup Game Mode based on Custom properties
                    int gameMode = Kit_NetworkGameInformation.instance.gameMode;
                    currentGameMode = gameMode;
                    currentPvEGameModeBehaviour = gameInformation.allSingleplayerGameModes[gameMode];
                    gameInformation.allSingleplayerGameModes[gameMode].GamemodeSetupClient();
                    hasGameModeStarted = true;

                    //Setup HUD
                    if (currentPvEGameModeBehaviour.hudPrefab)
                    {
                        GameObject hudPrefab = Instantiate(currentPvEGameModeBehaviour.hudPrefab, hud.transform, false);
                        //Move to the back
                        hudPrefab.transform.SetAsFirstSibling();
                        //Reset scale
                        hudPrefab.transform.localScale = Vector3.one;
                        //Get script
                        currentGameModeHUD = hudPrefab.GetComponent<Kit_GameModeHUDBase>();
                        //Start
                        currentGameModeHUD.HUDInitialize();
                    }
                }
                else if (gameModeType == 1)
                {
                    //Setup Game Mode based on Custom properties
                    int gameMode = Kit_NetworkGameInformation.instance.gameMode;
                    currentGameMode = gameMode;
                    currentPvEGameModeBehaviour = gameInformation.allCoopGameModes[gameMode];
                    gameInformation.allCoopGameModes[gameMode].GamemodeSetupClient();
                    hasGameModeStarted = true;

                    //Setup HUD
                    if (currentPvEGameModeBehaviour.hudPrefab)
                    {
                        GameObject hudPrefab = Instantiate(currentPvEGameModeBehaviour.hudPrefab, hud.transform, false);
                        //Move to the back
                        hudPrefab.transform.SetAsFirstSibling();
                        //Reset scale
                        hudPrefab.transform.localScale = Vector3.one;
                        //Get script
                        currentGameModeHUD = hudPrefab.GetComponent<Kit_GameModeHUDBase>();
                        //Start
                        currentGameModeHUD.HUDInitialize();
                    }
                }
                else if (gameModeType == 2)
                {
                    //Setup Game Mode based on Custom properties
                    int gameMode = Kit_NetworkGameInformation.instance.gameMode;
                    currentGameMode = gameMode;
                    currentPvPGameModeBehaviour = gameInformation.allPvpGameModes[gameMode];
                    currentPvPGameModeBehaviour.GamemodeSetupClient();

                    ts.Setup();

                    //If we already have a game mode hud, destroy it
                    if (currentGameModeHUD)
                    {
                        Destroy(currentGameModeHUD.gameObject);
                    }

                    //Initialize Loadout Menu
                    if (loadoutMenu)
                    {
                        loadoutMenu.Initialize();
                        //Force it to be closed
                        loadoutMenu.ForceClose();
                    }

                    //Setup HUD
                    if (currentPvPGameModeBehaviour.hudPrefab)
                    {
                        GameObject hudPrefab = Instantiate(currentPvPGameModeBehaviour.hudPrefab, hud.transform, false);
                        //Move to the back
                        hudPrefab.transform.SetAsFirstSibling();
                        //Reset scale
                        hudPrefab.transform.localScale = Vector3.one;
                        //Get script
                        currentGameModeHUD = hudPrefab.GetComponent<Kit_GameModeHUDBase>();
                        //Start
                        currentGameModeHUD.HUDInitialize();
                    }

                    //Set timer
                    int duration = Kit_NetworkGameInformation.instance.duration;
                    //Assign global game length
                    Kit_GameSettings.gameLength = duration;

                    if (Kit_GameSettings.currentNetworkingMode == KitNetworkingMode.Traditional)
                    {
                        //Get AFK limit
                        int afkLimit = Kit_NetworkGameInformation.instance.afk;

                        if (currentPvPGameModeBehaviour.traditionalAfkLimits[afkLimit] > 0)
                        {
                            //AFK limit enabled
                            if (afkLimitSystem)
                            {
                                //Relay to the system
                                afkLimitSystem.StartRelay(true, currentPvPGameModeBehaviour.traditionalAfkLimits[afkLimit]);
                                isAfkLimiterEnabled = true;
                            }
                        }
                        else
                        {
                            //AFK limit disabled
                            if (afkLimitSystem)
                            {
                                afkLimitSystem.StartRelay(false);
                                isAfkLimiterEnabled = false;
                            }
                        }
                    }
                    else if (Kit_GameSettings.currentNetworkingMode == KitNetworkingMode.Lobby)
                    {
                        if (currentPvPGameModeBehaviour.lobbyAfkLimit > 0)
                        {
                            //AFK limit enabled
                            if (afkLimitSystem)
                            {
                                //Relay to the system
                                afkLimitSystem.StartRelay(true, currentPvPGameModeBehaviour.lobbyAfkLimit);
                                isAfkLimiterEnabled = true;
                            }
                        }
                        else
                        {
                            //AFK limit disabled
                            if (afkLimitSystem)
                            {
                                afkLimitSystem.StartRelay(false);
                                isAfkLimiterEnabled = false;
                            }
                        }
                    }

                    //Setup Bots
                    if (Kit_NetworkGameInformation.instance.bots)
                    {
                        //Setup Nav Points
                        Kit_BotNavPoint[] navPoints = FindObjectsOfType<Kit_BotNavPoint>();

                        if (navPoints.Length == 0) throw new System.Exception("[Bots] No Nav Points have been found for this scene! You need to add some.");
                        List<Transform> tempNavPoints = new List<Transform>();
                        for (int i = 0; i < navPoints.Length; i++)
                        {
                            if (navPoints[i].gameModes.Contains(currentPvPGameModeBehaviour))
                            {
                                if (navPoints[i].navPointGroupID == 0)
                                {
                                    tempNavPoints.Add(navPoints[i].transform);
                                }
                            }
                        }
                        botNavPoints = tempNavPoints.ToArray();

                        if (NetworkServer.active)
                        {
                            if (!currentBotManager)
                            {
                                //Create locally
                                GameObject go = Instantiate(botManagerPrefab);
                                currentBotManager = go.GetComponent<Kit_BotManager>();
                                if (currentPvPGameModeBehaviour.botManagerToUse)
                                {
                                    currentPvPGameModeBehaviour.botManagerToUse.Inizialize(currentBotManager);
                                }
                                //Attach to network
                                NetworkServer.Spawn(go);
                            }
                        }
                        else
                        {
                            if (!currentBotManager)
                            {
                                currentBotManager = FindObjectOfType<Kit_BotManager>();
                            }
                        }
                    }

                    //Check if we already have enough players to start playing
                    if (currentPvPGameModeBehaviour.AreEnoughPlayersThere())
                    {
                        hasGameModeStarted = true;
                    }
                }

                if (voiceChat)
                {
                    //Setup Voice Chat
                    voiceChat.Setup();
                }

                if (minimap)
                {
                    //Tell Minimap to set itself up
                    minimap.Setup();
                }

                if (spectatorManager)
                {
                    //Setup spectator manager
                    spectatorManager.Setup();
                }

#if INTEGRATION_STEAM
                    //Set Steam Rich Presence
                    //Set connect
                    //Region@Room Name
                    SteamFriends.SetRichPresence("connect", PhotonNetwork.CloudRegion.ToString() + ":" + PhotonNetwork.CurrentRoom.Name);
                    //Set Status
                    SteamFriends.SetRichPresence("status", "Playing " + gameInformation.allPvpGameModes[gameMode].gameModeName + " on " + gameInformation.GetMapInformationFromSceneName(SceneManager.GetActiveScene().name).mapName);
#endif

                //Unlock the cursor
                MarsScreen.lockCursor = false;

                //Call Plugin
                for (int i = 0; i < gameInformation.plugins.Length; i++)
                {
                    gameInformation.plugins[i].OnSetupDone();
                }
            }

            switch (currentGameModeType)
            {
                case 0:
                    currentPvEGameModeBehaviour.GameModeProceed();
                    break;

                case 1:
                    currentPvEGameModeBehaviour.GameModeProceed();
                    break;

                case 2:
                    if (!currentMapVoting && !currentVictoryScreen)
                    {
                        SwitchMenu(ts.teamSelectionId);
                        //Set Pause Menu state
                        pauseMenuState = PauseMenuState.teamSelection;
                    }

                    break;
            }
        }

        public void OnPlayerLeftRoom(Kit_Player player)
        {
            Debug.Log("Player: " + player.name + " left the server");

            if (currentBotManager && currentPvPGameModeBehaviour.botManagerToUse && NetworkServer.active)
            {
                currentPvPGameModeBehaviour.botManagerToUse.PlayerLeftTeam(currentBotManager);
            }

            //Inform chat
            chat.PlayerLeft(player);

            //Call Plugin
            for (int i = 0; i < gameInformation.plugins.Length; i++)
            {
                gameInformation.plugins[i].PlayerLeftServer(player);
            }
        }

        public void OnPlayerEnteredRoom(Kit_Player newPlayer)
        {
            Debug.Log("Player: " + newPlayer.name + " joined the server");

            //Inform chat
            chat.PlayerJoined(newPlayer);

            //Call Plugin
            for (int i = 0; i < gameInformation.plugins.Length; i++)
            {
                gameInformation.plugins[i].PlayerJoinedServer(newPlayer);
            }
        }

        [TargetRpc]
        public void TargetHitmarker(NetworkConnection target, byte type)
        {
            switch (type)
            {
                case 0:
                    hud.DisplayHitmarker();
                    break;
                case 1:
                    hud.DisplayHitmarkerSpawnProtected();
                    break;
            }
        }

        [Command(requiresAuthority = false)]
        public void CmdStartVote(byte type, uint argument, NetworkConnectionToClient who = null)
        {
            if (!currentVoting && currentPvPGameModeBehaviour && currentPvPGameModeBehaviour.CanStartVote())
            {
                votingMenu.StartVote(type, argument);
            }
        }

        [ClientRpc(includeOwner = true)]
        public void ClientImpactProcess(Vector3 pos, Vector3 normal, string material)
        {
            //Relay to impact processor
            impactProcessor.ProcessImpact(pos, normal, material);
        }

        [ClientRpc]
        public void RpcDeathInfo(bool botShot, uint killer, bool botKilled, uint killed, int gunId, int thirdPersonPlayerModelId, int ragdollId)
        {
            //Update death stat
            if (botKilled)
            {

            }
            else
            {
                if (killed == Kit_NetworkPlayerManager.instance.myId)
                {
                    for (int i = 0; i < gameInformation.plugins.Length; i++)
                    {
                        gameInformation.plugins[i].LocalPlayerWasKilled();
                    }

                    if (gameInformation.statistics)
                    {
                        gameInformation.statistics.OnDeath(gunId);
                    }
                }
            }

            if (botShot)
            {
                //Check if bot killed himself
                if (!botKilled || botKilled && killer != killed)
                {
                    if (isServer && currentBotManager)
                    {
                        Kit_Bot killerBot = currentBotManager.GetBotWithID(killer);
                        killerBot.kills++;

                        //Call on game mode
                        currentPvPGameModeBehaviour.MasterClientBotScoredKill(killerBot);

                        for (int i = 0; i < gameInformation.plugins.Length; i++)
                        {
                            gameInformation.plugins[i].BotScoredKill(killerBot, botKilled, killed, gunId, thirdPersonPlayerModelId, ragdollId);
                        }
                    }
                }
            }
            else
            {
                if (killer == Kit_NetworkPlayerManager.instance.myId && (botKilled || killed != Kit_NetworkPlayerManager.instance.myId))
                {
                    //Display points
                    pointsUI.DisplayPoints(gameInformation.pointsPerKill, PointType.Kill);
                    //Add XP
                    if (gameInformation.leveling)
                    {
                        gameInformation.leveling.AddXp(gameInformation.pointsPerKill);
                    }
                    //Call on game mode
                    currentPvPGameModeBehaviour.LocalPlayerScoredKill();

                    //Call Plugins
                    for (int i = 0; i < gameInformation.plugins.Length; i++)
                    {
                        gameInformation.plugins[i].LocalPlayerScoredKill(botKilled, killed, gunId, thirdPersonPlayerModelId, ragdollId);
                    }

                    if (gameInformation.statistics)
                    {
                        //Check which statistics to call
                        gameInformation.statistics.OnKill(gunId);
                    }
                }
            }

            if (currentPvPGameModeBehaviour)
            {
                killFeed.Append(botShot, killer, botKilled, killed, gunId, thirdPersonPlayerModelId, ragdollId);
            }
        }

        [ClientRpc]
        public void RpcDeathInfo(bool botShot, uint killer, bool botKilled, uint killed, string cause, int thirdPersonPlayerModelId, int ragdollId)
        {
            //Update death stat
            if (botKilled)
            {

            }
            else
            {
                if (killed == Kit_NetworkPlayerManager.instance.myId)
                {
                    for (int i = 0; i < gameInformation.plugins.Length; i++)
                    {
                        gameInformation.plugins[i].LocalPlayerWasKilled();
                    }

                    if (gameInformation.statistics)
                    {
                        gameInformation.statistics.OnDeath(cause);
                    }
                }
            }

            if (botShot)
            {

            }
            else
            {
                if (killer == Kit_NetworkPlayerManager.instance.myId && (botKilled || killed != Kit_NetworkPlayerManager.instance.myId))
                {
                    //Display points
                    pointsUI.DisplayPoints(gameInformation.pointsPerKill, PointType.Kill);
                    //Add XP
                    if (gameInformation.leveling)
                    {
                        gameInformation.leveling.AddXp(gameInformation.pointsPerKill);
                    }
                    //Call on game mode
                    currentPvPGameModeBehaviour.LocalPlayerScoredKill();

                    //Call Plugins
                    for (int i = 0; i < gameInformation.plugins.Length; i++)
                    {
                        gameInformation.plugins[i].LocalPlayerScoredKill(botKilled, killed, cause, thirdPersonPlayerModelId, ragdollId);
                    }

                    if (gameInformation.statistics)
                    {
                        gameInformation.statistics.OnKill(cause);
                    }
                }
            }

            if (currentPvPGameModeBehaviour)
            {
                killFeed.Append(botShot, killer, botKilled, killed, cause, thirdPersonPlayerModelId, ragdollId);
            }
        }
        #endregion

        #region Chat Calls
        [Command(requiresAuthority = false)]
        public void CmdChatMessage(string msg, byte type, NetworkConnectionToClient who = null)
        {
            Kit_Player sender = Kit_NetworkPlayerManager.instance.GetPlayerByConnection(who);
            if (sender != null)
            {
                //Relay to all clients
                RpcChatMessage(false, sender.id, msg, type);
                //Debug the message
                Debug.Log("Chat message from: " + sender.name + ": " + msg);
            }
        }

        [ClientRpc]
        public void RpcChatMessage(bool bot, uint who, string msg, byte type)
        {
            if (bot)
            {
                if (currentBotManager)
                {
                    Kit_Bot sender = currentBotManager.GetBotWithID(who);

                    if (sender != null)
                    {
                        chat.DisplayChatMessage(sender, msg, type);
                    }
                }
            }
            else
            {
                Kit_Player sender = Kit_NetworkPlayerManager.instance.GetPlayerById(who);
                if (sender != null)
                {
                    chat.DisplayChatMessage(sender, msg, type);
                }
            }
        }
        #endregion

        #region Game Logic calls
        public void ServerDeathInfo(bool botShot, uint killer, bool botKilled, uint killed, int gunId, int thirdPersonPlayerModelId, int ragdollId)
        {
            //Update death stat
            if (botKilled)
            {
                if (isServer && currentBotManager)
                {
                    Kit_Bot killedBot = currentBotManager.GetBotWithID(killed);
                    killedBot.deaths++;

                    for (int i = 0; i < gameInformation.plugins.Length; i++)
                    {
                        gameInformation.plugins[i].BotWasKilled(killedBot);
                    }

                    Kit_IngameMain.instance.currentBotManager.ModifyBotData(killedBot);
                }
            }
            else
            {
                Kit_Player player = Kit_NetworkPlayerManager.instance.GetPlayerById(killed);
                player.deaths++;
                Kit_NetworkPlayerManager.instance.ModifyPlayerData(player);
            }

            if (botShot)
            {
                //Check if bot killed himself
                if (!botKilled || botKilled && killer != killed)
                {
                    if (isServer && currentBotManager)
                    {
                        Kit_Bot killerBot = currentBotManager.GetBotWithID(killer);
                        killerBot.kills++;

                        //Call on game mode
                        currentPvPGameModeBehaviour.MasterClientBotScoredKill(killerBot);

                        for (int i = 0; i < gameInformation.plugins.Length; i++)
                        {
                            gameInformation.plugins[i].BotScoredKill(killerBot, botKilled, killed, gunId, thirdPersonPlayerModelId, ragdollId);
                        }

                        Kit_IngameMain.instance.currentBotManager.ModifyBotData(killerBot);
                    }
                }
            }
            else
            {
                Kit_Player player = Kit_NetworkPlayerManager.instance.GetPlayerById(killer);
                player.kills++;
                Kit_NetworkPlayerManager.instance.ModifyPlayerData(player);
            }

            if (isServer)
            {
                if (currentPvPGameModeBehaviour)
                {
                    //Game Mode callback
                    currentPvPGameModeBehaviour.PlayerDied(botShot, killer, botKilled, killed);
                }

                if (currentPvEGameModeBehaviour)
                {
                    //Game Mode callback
                    currentPvEGameModeBehaviour.PlayerDied(botShot, killer, botKilled, killed);
                }
            }
        }

        public void ServerDeathInfo(bool botShot, uint killer, bool botKilled, uint killed, string cause, int thirdPersonPlayerModelId, int ragdollId)
        {
            //Update death stat
            if (botKilled)
            {
                if (currentBotManager)
                {
                    Kit_Bot killedBot = currentBotManager.GetBotWithID(killed);
                    killedBot.deaths++;

                    for (int i = 0; i < gameInformation.plugins.Length; i++)
                    {
                        gameInformation.plugins[i].BotWasKilled(killedBot);
                    }

                    Kit_IngameMain.instance.currentBotManager.ModifyBotData(killedBot);
                }
            }
            else
            {
                Kit_Player player = Kit_NetworkPlayerManager.instance.GetPlayerById(killed);
                player.deaths++;
                Kit_NetworkPlayerManager.instance.ModifyPlayerData(player);
            }

            if (botShot)
            {
                //Check if bot killed himself
                if (!botKilled || botKilled && killer != killed)
                {
                    if (isServer && currentBotManager)
                    {
                        Kit_Bot killerBot = currentBotManager.GetBotWithID(killer);
                        killerBot.kills++;

                        //Call on game mode
                        currentPvPGameModeBehaviour.MasterClientBotScoredKill(killerBot);

                        for (int i = 0; i < gameInformation.plugins.Length; i++)
                        {
                            gameInformation.plugins[i].BotScoredKill(killerBot, botKilled, killed, cause, thirdPersonPlayerModelId, ragdollId);
                        }

                        Kit_IngameMain.instance.currentBotManager.ModifyBotData(killerBot);
                    }
                }
            }
            else
            {
                Kit_Player player = Kit_NetworkPlayerManager.instance.GetPlayerById(killer);
                player.kills++;
                Kit_NetworkPlayerManager.instance.ModifyPlayerData(player);
            }

            if (isServer)
            {
                if (currentPvPGameModeBehaviour)
                {
                    //Game Mode callback
                    currentPvPGameModeBehaviour.PlayerDied(botShot, killer, botKilled, killed);
                }

                if (currentPvEGameModeBehaviour)
                {
                    //Game Mode callback
                    currentPvEGameModeBehaviour.PlayerDied(botShot, killer, botKilled, killed);
                }
            }
        }

        /// <summary>
        /// Tries to spawn a player
        /// <para>See also: <seealso cref="Kit_GameModeBase.CanSpawn(Kit_IngameMain, Photon.Realtime.Player)"/></para>
        /// </summary>
        public void Spawn(bool sendAnyway = false)
        {
            if (!myPlayer || sendAnyway)
            {
                CmdRequestSpawn(loadoutMenu.GetCurrentLoadout());
            }
        }

        [Command(requiresAuthority = false)]
        public void CmdRequestSpawn(Loadout requestedLoadout, NetworkConnectionToClient sender = null)
        {
            Kit_Player player = Kit_NetworkPlayerManager.instance.GetPlayerByConnection(sender);
            Debug.Log(player);
            Kit_PlayerBehaviour pb = GetPlayerOf(player);

            if (!pb)
            {
                switch (currentGameModeType)
                {
                    case 0:
                        //We can only spawn if we do not have a player currently
                        if (!pb)
                        {
                            //Check if we can currently spawn
                            if (!currentPvEGameModeBehaviour.UsesCustomSpawn())
                            {
                                if (gameInformation.allSingleplayerGameModes[currentGameMode].CanSpawn(player))
                                {
                                    //Get a spawn
                                    Transform spawnLocation = gameInformation.allSingleplayerGameModes[currentGameMode].GetSpawn(player);
                                    Loadout loadout = gameInformation.allSingleplayerGameModes[currentGameMode].GetSpawnLoadout();
                                    if (spawnLocation)
                                    {
                                        GameObject go = Instantiate(playerPrefab, spawnLocation.position, spawnLocation.rotation);
                                        pb = go.GetComponent<Kit_PlayerBehaviour>();
                                        pb.myTeam = player.team;
                                        pb.isBot = false;
                                        pb.id = player.id;
                                        //If you're wondering where the player model is being setup, its fetched from the game mode at creation time
                                        pb.weaponManager.SetupSpawnData(pb, loadout, sender);
                                        NetworkServer.ReplacePlayerForConnection(sender, go);
                                    }
                                }
                            }
                            else
                            {
                                GameObject playerGameObject = currentPvEGameModeBehaviour.DoCustomSpawn();
                                if (playerGameObject)
                                {
                                    pb = playerGameObject.GetComponent<Kit_PlayerBehaviour>();
                                    pb.myTeam = player.team;
                                    pb.isBot = false;
                                    pb.id = player.id;
                                    NetworkServer.ReplacePlayerForConnection(sender, playerGameObject);
                                }
                            }
                        }
                        break;
                    case 1:
                        //We can only spawn if we do not have a player currently
                        if (!pb)
                        {
                            //Check if we can currently spawn
                            if (!currentPvEGameModeBehaviour.UsesCustomSpawn())
                            {
                                if (gameInformation.allCoopGameModes[currentGameMode].CanSpawn(player))
                                {
                                    //Get a spawn
                                    Transform spawnLocation = gameInformation.allCoopGameModes[currentGameMode].GetSpawn(player);
                                    Loadout loadout = gameInformation.allCoopGameModes[currentGameMode].GetSpawnLoadout();
                                    if (spawnLocation)
                                    {
                                        GameObject go = Instantiate(playerPrefab, spawnLocation.position, spawnLocation.rotation);
                                        pb = go.GetComponent<Kit_PlayerBehaviour>();
                                        pb.myTeam = player.team;
                                        pb.isBot = false;
                                        pb.id = player.id;
                                        //If you're wondering where the player model is being setup, its fetched from the game mode at creation time
                                        pb.weaponManager.SetupSpawnData(pb, loadout, sender);
                                        NetworkServer.ReplacePlayerForConnection(sender, go);
                                    }
                                }
                            }
                            else
                            {
                                GameObject playerGameObject = currentPvEGameModeBehaviour.DoCustomSpawn();
                                if (playerGameObject)
                                {
                                    pb = playerGameObject.GetComponent<Kit_PlayerBehaviour>();
                                    pb.myTeam = player.team;
                                    pb.isBot = false;
                                    pb.id = player.id;
                                    NetworkServer.ReplacePlayerForConnection(sender, playerGameObject);
                                }
                            }
                        }

                        break;

                    case 2:
                        //We can only spawn if we do not have a player currently and picked a team
                        if (!pb && player.team >= 0)
                        {
                            //Check if we can currently spawn
                            if (!currentPvPGameModeBehaviour.UsesCustomSpawn())
                            {
                                if (gameInformation.allPvpGameModes[currentGameMode].CanSpawn(player))
                                {
                                    //Get a spawn
                                    Transform spawnLocation = gameInformation.allPvpGameModes[currentGameMode].GetSpawn(player);
                                    if (spawnLocation)
                                    {
                                        //Ideally, we should verify here that the loadout is valid first
                                        //IMPROVE:
                                        Loadout loadout = requestedLoadout;
                                        GameObject go = Instantiate(playerPrefab, spawnLocation.position, spawnLocation.rotation);
                                        pb = go.GetComponent<Kit_PlayerBehaviour>();
                                        pb.myTeam = player.team;
                                        pb.isBot = false;
                                        pb.id = player.id;
                                        pb.thirdPersonPlayerModelID = loadout.teamLoadout[player.team].playerModelID;
                                        pb.thirdPersonPlayerModelCustomizations.AddRange(loadout.teamLoadout[player.team].playerModelCustomizations);
                                        pb.weaponManager.SetupSpawnData(pb, loadout, sender);
                                        NetworkServer.ReplacePlayerForConnection(sender, go);
                                    }
                                }
                            }
                            else
                            {
                                //Get player from game mode
                                GameObject playerGameObject = currentPvPGameModeBehaviour.DoCustomSpawn(player, requestedLoadout);
                                if (playerGameObject)
                                {
                                    //End the spawn sequence
                                    pb = playerGameObject.GetComponent<Kit_PlayerBehaviour>();
                                    pb.myTeam = player.team;
                                    pb.isBot = false;
                                    pb.id = player.id;
                                    NetworkServer.ReplacePlayerForConnection(sender, playerGameObject);
                                }
                            }
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Get active player behaviour of the given player
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public Kit_PlayerBehaviour GetPlayerOf(Kit_Player player)
        {
            return allActivePlayers.Where(x => x.isBot == player.isBot && x.id == player.id).FirstOrDefault();
        }

        /// <summary>
        /// Ask the server if we can join this team
        /// </summary>
        /// <param name="teamID"></param>
        /// <param name="sender"></param>
        [Command(requiresAuthority = false)]
        public void CmdRequestJoinTeam(sbyte teamID, NetworkConnectionToClient sender = null)
        {
            //We can just do this if we are in a room
            if (NetworkServer.active)
            {
                //Update player
                Kit_Player player = Kit_NetworkPlayerManager.instance.GetPlayerByConnection(sender);
                //We only allow to change teams if we have not spawned
                if (!GetPlayerOf(player))
                {
                    //Clamp the team id to the available teams
                    teamID = (sbyte)Mathf.Clamp(teamID, 0, Mathf.Clamp(gameInformation.allPvpTeams.Length, 0, currentPvPGameModeBehaviour.maximumAmountOfTeams) - 1);
                    //Check if we can join this team OR if we are already in that team
                    if (gameInformation.allPvpGameModes[currentGameMode].CanJoinTeam(sender, teamID))
                    {
                        //Join the team
                        TeamJoinGranted(sender, teamID);
                        player.team = teamID;
                        Kit_NetworkPlayerManager.instance.ModifyPlayerData(player);

                        //Rebalance teams with bot
                        if (currentPvPGameModeBehaviour && currentBotManager)
                        {
                            currentPvPGameModeBehaviour.botManagerToUse.PlayerJoinedTeam(currentBotManager);
                        }
                    }
                    else
                    {
                        TargetTeamJoinNotGranted(sender);
                    }
                }
            }
        }

        [TargetRpc]
        public void TargetTeamJoinNotGranted(NetworkConnection target)
        {
            //Display message
            DisplayMessage("Could not join team");
        }

        [TargetRpc]
        public void TeamJoinGranted(NetworkConnection target, sbyte teamID)
        {
            //Don't display no error.
            errorAlpha = 0f;

            //Assign it to ourselves too
            assignedTeamID = teamID;

            //Loadout callback
            if (loadoutMenu)
            {
                loadoutMenu.TeamChanged(assignedTeamID);
            }
            //Voice Chat Callback
            try
            {
                if (voiceChat)
                {
                    voiceChat.JoinedTeam(teamID);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Voice Chat Error: " + e);
            }
            //Minimap callback
            if (minimap)
            {
                minimap.LocalPlayerSwitchedTeams();
            }

            //Should we attempt to spawn?
            if (ts.afterSelection == AfterTeamSelection.AttemptSpawn)
            {
                pluginOnForceClose.Invoke();
                pauseMenuState = PauseMenuState.main;
                //Activate scoreboard
                scoreboard.Enable();
                //Try to spawn
                Spawn();
                if (!myPlayer)
                {
                    SwitchMenu(pauseMenu.pauseMenuId, true);
                }
            }
            else if (ts.afterSelection == AfterTeamSelection.Loadout)
            {
                pauseMenuState = PauseMenuState.main;
                isPauseMenuOpen = true;
                //Activate scoreboard
                scoreboard.Enable();

                //Then go to loadout
                OpenLoadoutMenu();
            }
            else
            {
                SwitchMenu(pauseMenu.pauseMenuId, true);
                pauseMenuState = PauseMenuState.main;
                isPauseMenuOpen = true;
                //Activate scoreboard
                scoreboard.Enable();
            }

            //Call Plugin
            for (int i = 0; i < gameInformation.plugins.Length; i++)
            {
                gameInformation.plugins[i].LocalPlayerChangedTeam(teamID);
            }
        }

        [TargetRpc]
        public void TargetDisplayPingWarning(NetworkConnection target, ushort ping, ushort warning)
        {
            //Display warning
            if (pingLimitUI)
            {
                pingLimitUI.DisplayWarning(ping, warning);
            }
        }

        [ClientRpc]
        public void RpcAssistEvent(byte eventCode, int content)
        {
            //Relay
            if (assistManager)
            {
                assistManager.OnGenericEvent(eventCode, content);
            }
        }

        [ClientRpc]
        public void RpcGenericEvent(byte eventCode, object content)
        {
            if (currentPvPGameModeBehaviour)
            {
                //Relay
                currentPvPGameModeBehaviour.OnGenericEvent(eventCode, content);
            }

            if (currentPvEGameModeBehaviour)
            {
                //Relay
                currentPvEGameModeBehaviour.OnGenericEvent(eventCode, content);
            }
            for (int i = 0; i < gameInformation.plugins.Length; i++)
            {
                gameInformation.plugins[i].OnGenericEvent(eventCode, content);
            }
        }

        [ClientRpc]
        public void RpcServerRespawnRequest()
        {
            Spawn(true);
        }

        /// <summary>
        /// Ends the game with the supplied Photon.Realtime.Player as winner
        /// </summary>
        /// <param name="winner">The Winner</param>
        public void EndGame(Kit_Player winner)
        {
            if (isServer)
            {
                GameObject go = Instantiate(victoryScreen, Vector3.zero, Quaternion.identity);
                Kit_VictoryScreen data = go.GetComponent<Kit_VictoryScreen>();
                data.winnerType = 0;
                data.winnerBot = winner.isBot;
                data.winnerId = winner.id;
                NetworkServer.Spawn(go);

                //Call Event System
                Kit_Events.onEndGamePlayerWin.Invoke(winner);
            }
        }

        /// <summary>
        /// Ends the game with the supplied team (or 2 for draw) as winner
        /// </summary>
        /// <param name="winner">The winning team. 2 means draw.</param>
        public void EndGame(uint winner)
        {
            if (isServer)
            {
                GameObject go = Instantiate(victoryScreen, Vector3.zero, Quaternion.identity);
                Kit_VictoryScreen data = go.GetComponent<Kit_VictoryScreen>();
                data.winnerType = 1;
                data.winnerId = winner;
                NetworkServer.Spawn(go);

                //Call Event System
                Kit_Events.onEndGameTeamWin.Invoke(winner);
            }
        }

        /// <summary>
        /// Ends the game and displays scores for two team
        /// </summary>
        /// <param name="winner"></param>
        /// <param name="scoreTeamOne"></param>
        /// <param name="scoreTeamTwo"></param>
        public void EndGame(uint winner, int[] scores)
        {
            if (isServer)
            {
                GameObject go = Instantiate(victoryScreen, Vector3.zero, Quaternion.identity);
                Kit_VictoryScreen data = go.GetComponent<Kit_VictoryScreen>();
                data.winnerType = 1;
                data.winnerId = winner;

                for (int i = 0; i < scores.Length; i++)
                {
                    data.winnerScores.Add(scores[i]);
                }

                NetworkServer.Spawn(go);

                //Call Event System
                Kit_Events.onEndGameTeamWinWithScore.Invoke(winner, scores);
            }
        }

        /// <summary>
        /// Opens the voting menu if we are the master client
        /// </summary>
        public void OpenVotingMenu()
        {
            if (isServer)
            {
                List<MapGameModeCombo> usedCombos = new List<MapGameModeCombo>();

                //Get combos
                while (usedCombos.Count < mapVotingUI.amountOfAvailableVotes)
                {
                    //Get a new combo
                    usedCombos.Add(Kit_MapVotingBehaviour.GetMapGameModeCombo(gameInformation, usedCombos));
                }

                GameObject go = Instantiate(mapVoting, Vector3.zero, Quaternion.identity);
                Kit_MapVotingBehaviour data = go.GetComponent<Kit_MapVotingBehaviour>();
                data.combos.AddRange(usedCombos);
                //Attach to network
                NetworkServer.Spawn(go);
            }
        }

        /// <summary>
        /// Destroys all Players if we are the master client
        /// </summary>
        public void DeleteAllPlayers()
        {
            if (isServer)
            {
                //Reverse iterate through player list and destroy all of them
                for (int i = allActivePlayers.Count - 1; i >= 0; i--)
                {
                    NetworkServer.Destroy(allActivePlayers[i].gameObject);
                }
            }
        }

        /// <summary>
        /// Called when the victory screen opened
        /// </summary>
        public void VictoryScreenOpened()
        {
            //Reset alpha
            errorAlpha = 0f;
            //Force close loadout menu
            if (loadoutMenu)
            {
                loadoutMenu.ForceClose();
            }
        }

        /// <summary>
        /// Called when the map voting screen opened
        /// </summary>
        public void MapVotingOpened()
        {
            errorAlpha = 0f;
            //Force close loadout menu
            if (loadoutMenu)
            {
                loadoutMenu.ForceClose();
            }
        }

        /// <summary>
        /// Switches the map to
        /// </summary>
        /// <param name="to"></param>
        public void SwitchMap(int to)
        {
            if (isServer && currentGameModeType == 2)
            {
                //Update settings
                if (Kit_NetworkGameInformation.instance)
                {
                    Kit_NetworkGameInformation.instance.map = to;
                }

                if (Kit_GameSettings.currentNetworkingMode == KitNetworkingMode.Traditional)
                {
                    //Load the map
                    Kit_SceneSyncer.instance.LoadScene(currentPvPGameModeBehaviour.traditionalMaps[to].sceneName);
                }
                else if (Kit_GameSettings.currentNetworkingMode == KitNetworkingMode.Lobby)
                {
                    //Load the map
                    Kit_SceneSyncer.instance.LoadScene(currentPvPGameModeBehaviour.lobbyMaps[to].sceneName);
                }
            }
        }

        /// <summary>
        /// Switches the game mode to
        /// </summary>
        /// <param name="to"></param>
        public void SwitchGameMode(int to)
        {
            if (isServer && currentGameModeType == 2)
            {
                //Update settings
                if (Kit_NetworkGameInformation.instance)
                {
                    Kit_NetworkGameInformation.instance.gameMode = to;
                }

                if (Kit_GameSettings.currentNetworkingMode == KitNetworkingMode.Traditional)
                {
                    //Load the map
                    Kit_SceneSyncer.instance.LoadScene(currentPvPGameModeBehaviour.traditionalMaps[to].sceneName);
                }
                else if (Kit_GameSettings.currentNetworkingMode == KitNetworkingMode.Lobby)
                {
                    //Load the map
                    Kit_SceneSyncer.instance.LoadScene(currentPvPGameModeBehaviour.lobbyMaps[to].sceneName);
                }
            }
        }
        #endregion

        public void DisplayMessage(string msg)
        {
            //Display message
            errorText.text = msg;
            //Set alpha
            errorAlpha = errorTime;
        }

        #region ButtonCalls
        /// <summary>
        /// Attempt to join the team with teamID
        /// </summary>
        /// <param name="teamID"></param>
        public void JoinTeam(sbyte teamID)
        {
            //Relay to server
            CmdRequestJoinTeam(teamID);
        }

        public void NoTeam()
        {
            //Ask server to join no team
            CmdRequestJoinTeam(-1);
        }

        public void ChangeTeam()
        {
            //We only allow to change teams if we have not spawned
            if (!myPlayer)
            {
                SwitchMenu(ts.teamSelectionId);
                pauseMenuState = PauseMenuState.teamSelection;
            }
            else
            {
                //Commit suicide
                myPlayer.CmdSuicide();
            }
        }

        /// <summary>
        /// Disconnect from the current room
        /// </summary>
        public void Disconnect()
        {
            isShuttingDown = true;

            //Save Leveling
            if (gameInformation.leveling)
            {
                gameInformation.leveling.Save();
            }
            if (gameInformation.statistics)
            {
                gameInformation.statistics.Save();
            }

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

        /// <summary>
        /// Press the resume button. Either locks cursor or tries to spawn
        /// </summary>
        public void ResumeButton()
        {
            //Check if we have spawned
            if (myPlayer)
            {
                //We have, just lock cursor
                //Close pause menu
                isPauseMenuOpen = false;
                SwitchMenu(ingameFadeId, true);
                pluginOnForceClose.Invoke();
                //Lock Cursor
                MarsScreen.lockCursor = true;
            }
            else if (currentPvPGameModeBehaviour && currentPvPGameModeBehaviour.CanSpawn(Kit_NetworkPlayerManager.instance.GetLocalPlayer()))
            {
                //We haven't, try to spawn
                Spawn();
            }
            else if (currentPvEGameModeBehaviour && currentPvEGameModeBehaviour.CanSpawn(Kit_NetworkPlayerManager.instance.GetLocalPlayer()))
            {
                //We haven't, try to spawn
                Spawn();
            }
            else if (spectatorManager && spectatorManager.IsCurrentlySpectating())
            {
                //Close pause menu
                isPauseMenuOpen = false;
                SwitchMenu(ingameFadeId, true);
                pluginOnForceClose.Invoke();
                //Lock Cursor
                MarsScreen.lockCursor = false;
            }
            else
            {
                //Close pause menu
                isPauseMenuOpen = false;
                SwitchMenu(ingameFadeId, true);
                pluginOnForceClose.Invoke();
                //Lock Cursor
                MarsScreen.lockCursor = false;
            }
        }

        /// <summary>
        /// Opens the loadout menu
        /// </summary>
        public void OpenLoadoutMenu()
        {
            //Check if something is blocking that
            if (!currentVictoryScreen && !currentMapVoting)
            {
                if (loadoutMenu)
                {
                    loadoutMenu.Open();
                }
            }
        }

        /// <summary>
        /// Opens the vote menu if no vote is in progress
        /// </summary>
        public void StartVote()
        {
            if (votingMenu)
            {
                votingMenu.OpenVotingMenu();
            }
        }

        public void OptionsButton()
        {
            if (options)
            {
                SwitchMenu(options.optionsScreenId);
            }
        }
        #endregion

        #region Plugin Calls
        /// <summary>
        /// Called when the menu is forcefully closed
        /// </summary>
        public UnityEvent pluginOnForceClose = new UnityEvent();

        public Button InjectButtonIntoPauseMenu(string txt)
        {
            GameObject go = Instantiate(pauseMenu.pluginButtonPrefab, pauseMenu.pluginButtonGo, false);
            go.transform.SetSiblingIndex(3);
            go.GetComponentInChildren<TextMeshProUGUI>().text = txt;
            return go.GetComponent<Button>();
        }
        #endregion

        #region Other Calls
        /// <summary>
        /// Opens or closes the pause menu
        /// </summary>
        /// <param name="open"></param>
        public void SetPauseMenuState(bool open, bool canLockCursor = true)
        {
            if (isPauseMenuOpen != open)
            {
                isPauseMenuOpen = open;
                //Set state
                if (isPauseMenuOpen)
                {
                    SwitchMenu(pauseMenu.pauseMenuId, true);
                    //Unlock cursor
                    MarsScreen.lockCursor = false;
                    //Chat callback
                    chat.PauseMenuOpened();
                    //Auto spawn system callack
                    if (autoSpawnSystem && currentPvPGameModeBehaviour)
                    {
                        autoSpawnSystem.Interruption();
                    }
                }
                else
                {
                    SwitchMenu(ingameFadeId, true);
                    pluginOnForceClose.Invoke();
                    if (canLockCursor)
                    {
                        //Lock cursor
                        MarsScreen.lockCursor = true;
                        //Chat callback
                        chat.PauseMenuClosed();
                    }
                }
            }
        }

        /// <summary>
        /// When the server tells us to reset ourselves!
        /// </summary>
        public UnityEvent pluginOnResetStats;

        public void ResetPlayers()
        {
            for (int i = allActivePlayers.Count - 1; i >= 0; i--)
            {
                NetworkServer.Destroy(allActivePlayers[i].gameObject);
            }
        }

        public void ResetAllStatsEndOfRound()
        {
            if (isServer)
            {
                for (int i = 0; i < Kit_NetworkPlayerManager.instance.players.Count; i++)
                {
                    Kit_NetworkPlayerManager.instance.players[i].team = -1;
                    Kit_NetworkPlayerManager.instance.players[i].kills = 0;
                    Kit_NetworkPlayerManager.instance.players[i].assists = 0;
                    Kit_NetworkPlayerManager.instance.players[i].deaths = 0;
                }

                //Callbacks for game modes
                if (currentPvPGameModeBehaviour)
                {
                    currentPvPGameModeBehaviour.ResetStats();
                }

                if (currentPvEGameModeBehaviour)
                {
                    currentPvEGameModeBehaviour.ResetStats();
                }
            }
        }

        /// <summary>
        /// Called when the game mode stage changes
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        void GameModeStageChanged(int from, int to)
        {

        }
        #endregion

        #region Lag Compensation Calls
        public void SetLagCompensationTo(float revertBy)
        {
            for (int i = 0; i < allActivePlayers.Count; i++)
            {
                allActivePlayers[i].SetLagCompensationTo(revertBy);
            }
        }
        #endregion

        #region Menu Manager
        /// <summary>
        /// Call for buttons
        /// </summary>
        /// <param name="newMenu"></param>
        public void ChangeMenuButton(int newMenu)
        {
            if (!isSwitchingScreens)
            {
                //Start the coroutine
                StartCoroutine(SwitchRoutine(newMenu));
            }
        }

        public void ForceMenuActive(int menu)
        {
            //Disable all the roots
            for (int i = 0; i < menuScreens.Length; i++)
            {
                if (i != menu)
                {
                    if (menuScreens[i].root)
                    {
                        //Disable
                        menuScreens[i].root.SetActive(false);
                    }
                }
                else
                {
                    if (menuScreens[i].root)
                    {
                        //Disable
                        menuScreens[i].root.SetActive(true);
                    }
                }
            }
        }

        /// <summary>
        /// Switch to the given menu
        /// </summary>
        /// <param name="newMenu"></param>
        /// <returns></returns>
        public bool SwitchMenu(int newMenu)
        {
            Debug.Log("Requested switch from " + currentScreen + " to " + newMenu);

            if (currentScreen == newMenu) return true;

            if (!isSwitchingScreens)
            {
                //Start the coroutine
                currentlySwitchingScreensTo = StartCoroutine(SwitchRoutine(newMenu));
                //We are now switching
                return true;
            }

            //Not able to switch screens
            return false;
        }

        /// <summary>
        /// Switch to the given menu
        /// </summary>
        /// <param name="newMenu"></param>
        /// <returns></returns>
        public bool SwitchMenu(int newMenu, bool force)
        {
            Debug.Log("Requested switch from " + currentScreen + " to " + newMenu + ". Force? " + force);

            if (!isSwitchingScreens || force)
            {
                if (force)
                {
                    if (currentlySwitchingScreensTo != null)
                    {
                        StopCoroutine(currentlySwitchingScreensTo);
                    }

                    //Make sure all correct ones ARE disabled
                    //Disable all the roots
                    for (int i = 0; i < menuScreens.Length; i++)
                    {
                        if (i != currentScreen)
                        {
                            if (menuScreens[i].root)
                            {
                                //Disable
                                menuScreens[i].root.SetActive(false);
                            }
                        }
                    }
                }

                //Start the coroutine
                currentlySwitchingScreensTo = StartCoroutine(SwitchRoutine(newMenu));
                //We are now switching
                return true;
            }

            //Not able to switch screens
            return false;
        }

        private IEnumerator SwitchRoutine(int newMenu)
        {
            //Set bool
            isSwitchingScreens = true;
            if (wasFirstScreenFadedIn && currentScreen >= 0)
            {
                //Fade out screen
                //Play Animation
                menuScreens[currentScreen].anim.Play("Fade Out", 0, 0f);
                //Wait
                yield return new WaitForSeconds(menuScreens[currentScreen].fadeOutLength);
                menuScreens[currentScreen].root.SetActive(false);
            }

            //Fade in new screen
            //Set screen
            currentScreen = newMenu;
            if (currentScreen >= 0)
            {
                //Disable
                menuScreens[currentScreen].root.SetActive(true);
                //Play Animation
                menuScreens[currentScreen].anim.Play("Fade In", 0, 0f);
                //Wait
                yield return new WaitForSeconds(menuScreens[currentScreen].fadeInLength);
                //Set bool
                wasFirstScreenFadedIn = true;
            }
            //Done
            isSwitchingScreens = false;
        }
        #endregion
    }
}
