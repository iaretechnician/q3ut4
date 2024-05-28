using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using MarsFPSKit.Networking;
using System.Linq;
using static UnityEngine.UIElements.UxmlAttributeDescription;

namespace MarsFPSKit
{
    /// <summary>
    /// All input for the player (e.g. LMB, W,A,S,D, etc) should be stored here, so that bots may use the same scripts.
    /// </summary>
    public class Kit_PlayerInput
    {
        public float hor;
        public float ver;
        public bool crouch;
        public bool sprint;
        public bool jump;
        public bool interact;
        public bool lmb;
        public bool rmb;
        public bool reload;
        public float mouseX;
        public float mouseY;
        public bool leanLeft;
        public bool leanRight;
        public bool thirdPerson;
        public bool flashlight;
        public bool laser;
        public bool[] weaponSlotUses;

        public Vector3 clientCamPos;
        public Vector3 clientCamForward;
    }

    public class Kit_PlayerBehaviour : NetworkBehaviour
    {
        #region Game Information
        [Header("Internal Game Information")]
        [Tooltip("This object contains all game information such as Maps, Game Modes and Weapons")]
        public Kit_GameInformation gameInformation;
        #endregion

        //This section contains everything for the local camera control
        #region Camera Control
        [Header("Camera Control")]
        public Transform playerCameraTransform;
        /// <summary>
        /// Transform that should be used for camera animations from weapons
        /// </summary>
        public Transform playerCameraAnimationTransform;
        /// <summary>
        /// Fall effects should be applied here
        /// </summary>
        public Transform playerCameraFallDownTransform;
        /// <summary>
        /// Hit reactions for camera should be applied here
        /// </summary>
        public Transform playerCameraHitReactionsTransform;
        #endregion

        //This section contains everything for the movement
        #region Movement
        [Header("Movement")]
        public Kit_MovementBase movement; //The system used for movement
        [System.NonSerialized]
        /// <summary>
        /// Unsynced data for movement module.
        /// </summary>
        public Kit_MovementNetworkBase movementNetworkData;

        /// <summary>
        /// Our Character Controller, assign it here
        /// </summary>
        public CharacterController cc;
        /// <summary>
        /// Our footstep audio source
        /// </summary>
        public AudioSource footstepSource;
        /// <summary>
        /// An audio source to play sounds from movement
        /// </summary>
        public AudioSource movementSoundSource;
        #endregion

        //This section contains everything for the Mouse Look
        #region Looking
        [Header("Mouse Look")]
        public Kit_MouseLookBase looking; //The system used for looking
        public Transform mouseLookObject; //The transform used for looking around
        [HideInInspector]
        /// <summary>
        /// This is used by the mouse looking script to apply the recoil and by the weapon script to set the recoil
        /// </summary>
        public Quaternion recoilApplyRotation = Quaternion.identity;
        [HideInInspector]
        public object customMouseLookData; //Used to store custom mouse look data
        #endregion

        //This section contains everything for the weapons
        #region Weapons
        [Header("Weapons")]
        public Weapons.Kit_WeaponManagerBase weaponManager; //The system used for weapon management
        [System.NonSerialized]
        /// <summary>
        /// Synced data for weapon manager
        /// </summary>
        public Kit_WeaponManagerNetworkBase weaponManagerNetworkData;
        public Transform weaponsGo;
        /// <summary>
        /// Hit reactions for weapons should be applied here
        /// </summary>
        public Transform weaponsHitReactions;

        /// <summary>
        /// Layermask for use with weapon Raycasts
        /// </summary>
        [Tooltip("These layers will be hit by Raycasts that weapons use")]
        public LayerMask weaponHitLayers;
        #endregion

        #region Player Vitals
        [Header("Player Vitals")]
        public Kit_VitalsBase vitalsManager;
        [System.NonSerialized]
        /// <summary>
        /// Network Data of vitals module
        /// </summary>
        public Kit_VitalsNetworkBase vitalsNetworkData;
        #endregion

        #region Player Name UI
        [Header("Player Name UI")]
        public Kit_PlayerNameUIBase nameManager;
        public object customNameData;
        #endregion

        #region Spawn Protection
        [Header("Spawn Protection")]
        public Kit_SpawnProtectionBase spawnProtection;
        public object customSpawnProtectionData;
        #endregion

        #region Bots
        [Header("Bot Controls")]
        /// <summary>
        /// This module will control the behaviour of the bot
        /// </summary>
        public Kit_PlayerBotControlBase botControls;
        /// <summary>
        /// Use this to store runtime data for bot control
        /// </summary>
        public object botControlsRuntimeData;
        #endregion

        #region Voice
        [Header("Voice Manager")]
        /// <summary>
        /// If this is assigned, your characters can talk!
        /// </summary>
        public Kit_VoiceManagerBase voiceManager;

        /// <summary>
        /// use this  to store runtime data for the voice manager
        /// </summary>
        public object voiceManagerData;
        #endregion

        #region Input Manager
        [Header("Input Manager")]
        public Kit_InputManagerBase inputManager;
        /// <summary>
        /// Use this to store input manager runtime data
        /// </summary>
        public object inputManagerData;
        #endregion

        //This section contains internal variables
        #region Internal Variables
        [SyncVar]
        //Team
        public int myTeam = -1;

        /// <summary>
        /// True if first person view is active on this player. Does not mean that this is our local player. To determine actual perspective call looking.GetPerspective()
        /// </summary>
        public bool isFirstPersonActive
        {
            get
            {
                if (isLocalPlayer || isBeingSpectated)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Is input enabled?
        /// </summary>
        public bool enableInput
        {
            get
            {
                if (isOwned || isServer && isBot)
                {
                    if (isBot)
                    {
                        return canControlPlayer;
                    }
                    else
                    {
                        return MarsScreen.lockCursor && canControlPlayer;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Runtime data from the game mode can be stored here
        /// </summary>
        public object gameModeCustomRuntimeData;

        /// <summary>
        /// Input for the player, only assigned if we are controlling this player or we are the master client
        /// </summary>
        public Kit_PlayerInput input;
        [SyncVar]
        /// <summary>
        /// Is this player being controlled by AI?
        /// </summary>
        public bool isBot;
        [SyncVar]
        /// <summary>
        /// This is either the player or bot id, depending on <see cref="isBot"/>
        /// </summary>
        public uint id;
        /// <summary>
        /// User name fetched in sync setup
        /// </summary>
        public string userName;

        [HideInInspector]
        //Position and rotation are synced by photon transform view
        public bool syncSetup;

        //We cache this value to avoid to calculate it many times
        [HideInInspector]
        public bool canControlPlayer = true;

        /// <summary>
        /// Currently active third person model
        /// </summary>
        public Kit_ThirdPersonPlayerModel thirdPersonPlayerModel;
        [SyncVar]
        /// <summary>
        /// Id of the player model we are currently using! Only assigned in PvP game modes.
        /// </summary>
        public int thirdPersonPlayerModelID;
        /// <summary>
        /// Active customizations
        /// </summary>
        public readonly SyncList<int> thirdPersonPlayerModelCustomizations = new SyncList<int>();
        [SyncVar]
        /// <summary>
        /// Last forward vector from where we were shot
        /// </summary>
        public Vector3 ragdollForward;
        [SyncVar]
        /// <summary>
        /// Last force which we were shot with
        /// </summary>
        public float ragdollForce;
        [SyncVar]
        /// <summary>
        /// Last point from where we were shot
        /// </summary>
        public Vector3 ragdollPoint;
        [SyncVar]
        /// <summary>
        /// Which collider should the force be applied to?
        /// </summary>
        public int ragdollId;
        [HideInInspector]
        /// <summary>
        /// The category to play
        /// </summary>
        public int deathSoundCategory;
        [HideInInspector]
        /// <summary>
        /// The specific sound to play
        /// </summary>
        public int deathSoundID;
        [HideInInspector]
        /// <summary>
        /// Who damaged us? For the assist manager.
        /// </summary>
        public List<AssistedKillData> damagedBy = new List<AssistedKillData>();
        [HideInInspector]
        /// <summary>
        /// Are we  currently being spectated?
        /// </summary>
        public bool isBeingSpectated;
        /// <summary>
        /// Cached mirror variable for use after destroy
        /// </summary>
        private bool wasLocalPlayer;
        #endregion

        public IEnumerator LocalPlayerRoutine()
        {
            while (!thirdPersonPlayerModel) yield return null;

            Kit_IngameMain.instance.hud.PlayerStart(this);
            //Disable our own name hitbox
            thirdPersonPlayerModel.enemyNameAboveHeadTrigger.enabled = false;
            // Setup voice
            if (voiceManager)
            {
                voiceManager.SetupOwner(this);
            }

            //Setup marker
            if (nameManager)
            {
                nameManager.LocalPlayerGainedControl(this);
            }

            //Assign ourselves
            Kit_IngameMain.instance.myPlayer = this;

            //Lock the cursor
            MarsScreen.lockCursor = true;
            //Close pause menu
            Kit_IngameMain.isPauseMenuOpen = false;
            Kit_IngameMain.instance.SwitchMenu(Kit_IngameMain.instance.ingameFadeId, true);
            yield return new WaitForSeconds(Kit_IngameMain.instance.menuScreens[Kit_IngameMain.instance.currentScreen].fadeOutLength);
            //Move camera to the right position
            Kit_IngameMain.instance.activeCameraTransform = playerCameraTransform;
            //Setup third person model
            thirdPersonPlayerModel.FirstPerson();
            //Tell Minimap we spawned
            if (Kit_IngameMain.instance.minimap)
            {
                Kit_IngameMain.instance.minimap.LocalPlayerSpawned(this);
            }
            //Tell touchscreen
            if (Kit_IngameMain.instance.touchScreenCurrent)
            {
                Kit_IngameMain.instance.touchScreenCurrent.LocalPlayerSpawned(this);
            }
            //Auto spawn system callack
            if (Kit_IngameMain.instance.autoSpawnSystem && Kit_IngameMain.instance.currentPvPGameModeBehaviour)
            {
                Kit_IngameMain.instance.autoSpawnSystem.LocalPlayerSpawned();
            }
            if (Kit_IngameMain.instance.currentPvPGameModeBehaviour)
            {
                //Tell Game Mode
                Kit_IngameMain.instance.currentPvPGameModeBehaviour.OnLocalPlayerSpawned(this);
            }
            else if (Kit_IngameMain.instance.currentPvEGameModeBehaviour)
            {
                //Tell Game Mode
                Kit_IngameMain.instance.currentPvEGameModeBehaviour.OnLocalPlayerSpawned(this);
            }
            //Show HUD
            Kit_IngameMain.instance.hud.SetVisibility(true);
            Kit_IngameMain.instance.pluginOnForceClose.Invoke();
            //Call Plugin
            for (int i = 0; i < gameInformation.plugins.Length; i++)
            {
                gameInformation.plugins[i].LocalPlayerSpawned(this);
            }

            //Call loadout
            Kit_IngameMain.instance.loadoutMenu.LocalPlayerSpawned(this);

            //Take control in modules
            looking.TakeControl(this);
            weaponManager.TakeControl(this);

            double waitCur = 0f;
            double waitTarget = Time.fixedDeltaTime;

            while (true)
            {
                //Fetch input
                inputManager.WriteToPlayerInput(this);
                waitCur += Time.deltaTime;

                if (waitCur >= waitTarget)
                {
                    if (NetworkClient.ready)
                        CmdInput(input, NetworkTime.rtt * 0.75f);

                    waitCur = 0f;
                }

                PredictionInput(input);

                //Send CMD every frame to server with all inputs.
                yield return null;
            }
        }

        IEnumerator ServerRoutine()
        {
            var wait = new WaitForFixedUpdate();

            while (true)
            {
                if (isBot)
                {
                    botControls.WriteToPlayerInput(this);
                }

                if (isServer && !isLocalPlayer && input != null)
                {
                    //We are only executing in this coroutine so that if we receive more commands than expected, it doesn't matter. Execution rate will stay the same.
                    try
                    {
                        movement.AuthorativeInput(this, input, Time.fixedDeltaTime, lastRevertTime);
                        looking.AuthorativeInput(this, input, Time.fixedDeltaTime, lastRevertTime);
                        weaponManager.AuthorativeInput(this, input, Time.fixedDeltaTime, lastRevertTime);
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log("Input execution exception: " + e.ToString());
                    }
                }
                yield return wait;
            }
        }

        #region Mirror Calls
        private bool localOnlyCallsDone;
        void LocalOnlyCalls()
        {
            if (!localOnlyCallsDone)
            {
                if (Kit_IngameMain.instance.currentGameModeType == 2)
                {
                    //Start Spawn Protection
                    if (spawnProtection)
                    {
                        spawnProtection.CustomStart(this);
                    }
                }
                else
                {
                    //Spawn protection not used in SP / Coop
                    spawnProtection = null;
                }

                //Setup marker
                if (nameManager)
                {
                    nameManager.StartRelay(this);
                }

                // Setup voice
                if (voiceManager)
                {
                    voiceManager.SetupOthers(this);
                }

                //Tell Minimap we spawned
                if (Kit_IngameMain.instance.minimap)
                {
                    Kit_IngameMain.instance.minimap.PlayerSpawned(this);
                }

                //Call Plugin
                for (int i = 0; i < gameInformation.plugins.Length; i++)
                {
                    gameInformation.plugins[i].PlayerSpawned(this);
                }

                if (Kit_IngameMain.instance.currentPvPGameModeBehaviour)
                {
                    //Tell Game Mode
                    Kit_IngameMain.instance.currentPvPGameModeBehaviour.OnPlayerSpawned(this);
                }
                else if (Kit_IngameMain.instance.currentPvEGameModeBehaviour)
                {
                    //Tell Game Mode
                    Kit_IngameMain.instance.currentPvEGameModeBehaviour.OnPlayerSpawned(this);
                }

                //Call event system
                Kit_Events.onPlayerSpawned.Invoke(this);

                //Spectator call. Do this last so that this player is completely setup.
                if (Kit_IngameMain.instance.spectatorManager)
                {
                    Kit_IngameMain.instance.spectatorManager.PlayerWasSpawned(this);
                }

                localOnlyCallsDone = true;
            }
        }

        public override void OnStartServer()
        {
            if (isBot)
            {
                //Check for game mode override
                if (Kit_IngameMain.instance.currentPvEGameModeBehaviour && Kit_IngameMain.instance.currentPvEGameModeBehaviour.botControlOverride)
                {
                    botControls = Kit_IngameMain.instance.currentPvEGameModeBehaviour.botControlOverride;
                }
                if (Kit_IngameMain.instance.currentPvPGameModeBehaviour && Kit_IngameMain.instance.currentPvPGameModeBehaviour.botControlOverride)
                {
                    botControls = Kit_IngameMain.instance.currentPvPGameModeBehaviour.botControlOverride;
                }
                input = new Kit_PlayerInput();
                Kit_BotManager manager = FindObjectOfType<Kit_BotManager>();
                manager.AddActiveBot(this);
                //Initialize bot input
                botControls.InitializeControls(this);
            }

            if (!thirdPersonPlayerModel) CreatePlayerModel();

            //Add us to player list
            if (!Kit_IngameMain.instance.allActivePlayers.Contains(this)) Kit_IngameMain.instance.allActivePlayers.Add(this);

            //Initialize input module
            input = new Kit_PlayerInput();
            inputManager.InitializeServer(this);
            //Initialize looking
            looking.InitializeServer(this);
            //Initialize movement
            movement.InitializeServer(this);
            //Setup Vitals
            vitalsManager.InitializeServer(this);
            //Setup weapon manager
            weaponManager.InitializeServer(this);

            LocalOnlyCalls();

            syncSetup = true;

            StartCoroutine(ServerRoutine());
        }

        public override void OnStartClient()
        {
            StartCoroutine(OnStartClientWaitForModules());
        }

        IEnumerator OnStartClientWaitForModules()
        {
            //Wait for these modules to arrive before we try to setup our client. The server spawns them simultaneously(-ish) with the player, they may arrive out of order or one frame later
            while (!movementNetworkData || !vitalsNetworkData || !weaponManagerNetworkData)
            {
                if (!movementNetworkData) movementNetworkData = FindObjectsOfType<Kit_MovementNetworkBase>().Where(x => x.ownerPlayerNetworkId == netId).FirstOrDefault();
                if (!vitalsNetworkData) vitalsNetworkData = FindObjectsOfType<Kit_VitalsNetworkBase>().Where(x => x.ownerPlayerNetworkId == netId).FirstOrDefault();
                if (!weaponManagerNetworkData)
                {
                    weaponManagerNetworkData = FindObjectsOfType<Kit_WeaponManagerNetworkBase>().Where(x => x.ownerPlayerNetworkId == netId).FirstOrDefault();

                    if (weaponManagerNetworkData)
                    {
                        weaponManagerNetworkData.ownerPlayer = this;
                    }
                }

                yield return null;
            }

            if (isBot)
            {
                //Fetch bot's username
                userName = Kit_IngameMain.instance.currentBotManager.GetBotWithID(id).name;
            }
            else
            {
                //Fetch player's username
                userName = Kit_NetworkPlayerManager.instance.GetPlayerById(id).name;
            }

            //Set name
            gameObject.name = userName;

            if (!thirdPersonPlayerModel) CreatePlayerModel();

            //Add us to player list
            if (!Kit_IngameMain.instance.allActivePlayers.Contains(this)) Kit_IngameMain.instance.allActivePlayers.Add(this);

            //Initialize input module
            input = new Kit_PlayerInput();
            inputManager.InitializeClient(this);
            //Initialize looking
            looking.InitializeClient(this);
            //Movement
            movement.InitializeClient(this);
            //Setup Vitals
            vitalsManager.InitializeClient(this);
            //Setup weapon manager
            weaponManager.InitializeClient(this);

            LocalOnlyCalls();

            syncSetup = true;
        }

        public override void OnStartLocalPlayer()
        {
            StartCoroutine(LocalPlayerRoutine());

            wasLocalPlayer = true;
        }

        public override void OnStopLocalPlayer()
        {
            base.OnStopLocalPlayer();
        }
        #endregion

        #region Authorative Gameplay
        public void PredictionInput(Kit_PlayerInput input)
        {
            movement.PredictionInput(this, input, Time.deltaTime);
            looking.PredictionInput(this, input, Time.deltaTime);
            weaponManager.PredictionInput(this, input, Time.deltaTime);
        }

        private double lastRevertTime;

        [Command]
        public void CmdInput(Kit_PlayerInput input, double revertTime, NetworkConnectionToClient sender = null)
        {
            /*
            var revertTime = connectionToClient.remoteTimeline - connectionToClient.remoteTimeStamp;

            if (!isOwned)
                Debug.Log(revertTime);

            if (revertTime < 0) revertTime = 0;
            */

            this.input = input;
            lastRevertTime = revertTime;
        }
        #endregion

        #region Unity Calls
        void Start()
        {

        }

        void OnDestroy()
        {
            if (Kit_IngameMain.executeDestroyActions)
            {
                //Clean up our additional data
                if (isServer)
                {
                    if (movementNetworkData) NetworkServer.Destroy(movementNetworkData.gameObject);
                    if (weaponManagerNetworkData) NetworkServer.Destroy(weaponManagerNetworkData.gameObject);
                    if (vitalsNetworkData) NetworkServer.Destroy(vitalsNetworkData.gameObject);
                }

                //Hide HUD if we were killed
                if (isLocalPlayer)
                {
                    Kit_IngameMain.instance.hud.SetVisibility(false);
                }

                if (!isFirstPersonActive)
                {
                    //Release marker
                    if (nameManager)
                    {
                        nameManager.OnDestroyRelay(this);
                    }
                }

                if (wasLocalPlayer)
                {
                    //Tell minimap
                    if (Kit_IngameMain.instance.minimap)
                    {
                        Kit_IngameMain.instance.minimap.LocalPlayerDied(this);
                    }
                    //Tell touchscreen
                    if (Kit_IngameMain.instance.touchScreenCurrent)
                    {
                        Kit_IngameMain.instance.touchScreenCurrent.LocalPlayerDied(this);
                    }
                    //Auto spawn system callack
                    if (Kit_IngameMain.instance.autoSpawnSystem && Kit_IngameMain.instance.currentPvPGameModeBehaviour)
                    {
                        Kit_IngameMain.instance.autoSpawnSystem.LocalPlayerDied();
                    }
                    if (Kit_IngameMain.instance.currentPvPGameModeBehaviour)
                    {
                        //Tell Game Mode
                        Kit_IngameMain.instance.currentPvPGameModeBehaviour.OnLocalPlayerDestroyed(this);
                    }
                    else if (Kit_IngameMain.instance.currentPvEGameModeBehaviour)
                    {
                        //Tell Game Mode
                        Kit_IngameMain.instance.currentPvEGameModeBehaviour.OnLocalPlayerDestroyed(this);
                    }
                    //Tell HUD
                    Kit_IngameMain.instance.hud.PlayerEnd(this);
                    //Call Plugin
                    for (int i = 0; i < gameInformation.plugins.Length; i++)
                    {
                        gameInformation.plugins[i].LocalPlayerDied(this);
                    }
                }
                else
                {
                    //Tell minimap
                    if (Kit_IngameMain.instance.minimap)
                    {
                        Kit_IngameMain.instance.minimap.PlayerDied(this);
                    }
                    if (Kit_IngameMain.instance.currentPvPGameModeBehaviour)
                    {
                        //Tell Game Mode
                        Kit_IngameMain.instance.currentPvPGameModeBehaviour.OnPlayerDestroyed(this);
                    }
                    else if (Kit_IngameMain.instance.currentPvEGameModeBehaviour)
                    {
                        //Tell Game Mode
                        Kit_IngameMain.instance.currentPvEGameModeBehaviour.OnPlayerDestroyed(this);
                    }
                    //Call Plugin
                    for (int i = 0; i < gameInformation.plugins.Length; i++)
                    {
                        gameInformation.plugins[i].PlayerDied(this);
                    }
                }

                //Spectator call
                if (Kit_IngameMain.instance.spectatorManager)
                {
                    Kit_IngameMain.instance.spectatorManager.PlayerWasKilled(this);
                }

                //Make sure the camera never gets destroyed
                if (Kit_IngameMain.instance.activeCameraTransform == playerCameraTransform)
                {
                    Kit_IngameMain.instance.activeCameraTransform = Kit_IngameMain.instance.spawnCameraPosition;
                    //Set Fov
                    Kit_IngameMain.instance.mainCamera.fieldOfView = Kit_GameSettings.baseFov;
                }

                if (canControlPlayer)
                {
                    if (thirdPersonPlayerModel)
                    {
                        //Unparent sounds
                        thirdPersonPlayerModel.soundFire.transform.parent = null;
                        if (thirdPersonPlayerModel.soundFire.clip)
                        {
                            Destroy(thirdPersonPlayerModel.soundFire.gameObject, thirdPersonPlayerModel.soundFire.clip.length);
                        }
                        else
                        {
                            Destroy(thirdPersonPlayerModel.soundFire.gameObject, 1f);
                        }

                        //Setup ragdoll
                        thirdPersonPlayerModel.CreateRagdoll();
                    }
                }

                //Call event system
                Kit_Events.onPlayerDied.Invoke(this);

                //Remove us from list
                Kit_IngameMain.instance.allActivePlayers.Remove(this);
            }
        }

        void Update()
        {
            if (syncSetup && thirdPersonPlayerModel)
            {
                //Everyone update dcalls
                //Weapon manager update
                weaponManager.CustomUpdate(this, lastRevertTime);
                //looking.CalculateLookUpdate(this);
                //movement.CalculateMovementUpdate(this);
                vitalsManager.CustomUpdate(this);


                //Not controller update calls
                if (!isServer && !isLocalPlayer)
                {
                    movement.NotControllerUpdate(this);
                    looking.NotControllerUpdate(this);
                }
                else
                {

                }

                movement.CalculateFootstepsUpdate(this);

                if (isLocalPlayer)
                {
                    //Call Plugin
                    for (int i = 0; i < gameInformation.plugins.Length; i++)
                    {
                        gameInformation.plugins[i].LocalPlayerUpdate(this);
                    }

                    Kit_IngameMain.instance.hud.PlayerUpdate(this);
                }

                //Call Plugin
                for (int i = 0; i < gameInformation.plugins.Length; i++)
                {
                    gameInformation.plugins[i].PlayerUpdate(this);
                }

                if (isFirstPersonActive)
                {
                    movement.Visuals(this);
                    looking.Visuals(this);
                }

                if (isOwned || isServer)
                {
                    if (Kit_IngameMain.instance.currentPvPGameModeBehaviour)
                    {
                        //Update control value
                        canControlPlayer = Kit_IngameMain.instance.currentPvPGameModeBehaviour.CanControlPlayer();
                    }
                    else if (Kit_IngameMain.instance.currentPvEGameModeBehaviour)
                    {
                        //Update control value
                        canControlPlayer = Kit_IngameMain.instance.currentPvEGameModeBehaviour.CanControlPlayer();
                    }
                }

                if (isServer || isFirstPersonActive)
                {
                    //Update spawn protection
                    if (spawnProtection)
                    {
                        spawnProtection.CustomUpdate(this);
                    }
                }

                if (isServer)
                {
                    //Update voice scanner
                    if (voiceManager)
                    {
                        voiceManager.ScanForEnemies(this);
                    }
                }
            }
        }

        void LateUpdate()
        {
            if (syncSetup)
            {
                if (!isFirstPersonActive)
                {
                    if (Kit_IngameMain.instance.currentGameModeType == 2 && Kit_IngameMain.instance.currentPvPGameModeBehaviour.AreWeEnemies(isBot, id))
                    {
                        if (nameManager)
                        {
                            nameManager.UpdateEnemy(this);
                        }
                    }
                    else
                    {
                        if (nameManager)
                        {
                            nameManager.UpdateFriendly(this);
                        }
                        if (Kit_IngameMain.instance.minimap)
                        {
                            Kit_IngameMain.instance.minimap.PlayerFriendlyUpdate(this);
                        }
                    }
                }
            }

            //If we are the controller, update everything
            if (isServer || isLocalPlayer)
            {
                looking.CalculateLookLateUpdate(this);
            }
        }

        private void FixedUpdate()
        {
            if (isLocalPlayer && !isServer)
            {
                movement.CreateSnapshot(this);
            }
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            //Relay to movement script
            movement.OnControllerColliderHitRelay(this, hit);
            //Relay to mouse look script
            looking.OnControllerColliderHitRelay(this, hit);
            //Relay to weapon manager
            weaponManager.OnControllerColliderHitRelay(this, hit);
        }

        void OnTriggerEnter(Collider col)
        {
            if (isServer)
            {
                //Check for amo
                if (col.transform.root.GetComponent<Kit_AmmoPickup>())
                {
                    Kit_AmmoPickup pickup = col.transform.root.GetComponent<Kit_AmmoPickup>();
                    //Relay to weapon manager
                    weaponManager.OnAmmoPickup(this, pickup);
                    //Destroy
                    pickup.PickedUp();
                }
                else if (col.transform.root.GetComponent<Kit_HealthPickup>())
                {
                    Kit_HealthPickup pickup = col.transform.root.GetComponent<Kit_HealthPickup>();
                    //Relay to health
                    vitalsManager.ApplyHeal(this, pickup.healthRestored);
                    //Destroy
                    pickup.PickedUp();
                }
            }

            //Relay to weapon manager
            weaponManager.OnTriggerEnterRelay(this, col);

            //Relay to movement
            movement.OnTriggerEnterRelay(this, col);
        }

        void OnTriggerExit(Collider col)
        {
            //Relay to weapon manager
            weaponManager.OnTriggerExitRelay(this, col);

            //Relay to movement
            movement.OnTriggerExitRelay(this, col);
        }
        #endregion

        #region Custom Calls
        private void CreatePlayerModel()
        {
            if (!thirdPersonPlayerModel)
            {
                int[] playerModelCustomizations = new int[0];
                GameObject go = null;

                switch (Kit_IngameMain.instance.currentGameModeType)
                {
                    case 0:
                        //Get player model
                        PlayerModelConfig pmcSp = Kit_IngameMain.instance.currentPvEGameModeBehaviour.GetPlayerModel(this);
                        playerModelCustomizations = pmcSp.customization;

                        //Set up player model
                        //Instantiate one random player model for chosen team
                        go = Instantiate(pmcSp.information.prefab, transform, false);
                        //Assign
                        thirdPersonPlayerModel = go.GetComponent<Kit_ThirdPersonPlayerModel>();
                        //Set information
                        thirdPersonPlayerModel.information = pmcSp.information;
                        break;
                    case 1:
                        //Get player model
                        PlayerModelConfig pmcCoop = Kit_IngameMain.instance.currentPvEGameModeBehaviour.GetPlayerModel(this);
                        playerModelCustomizations = pmcCoop.customization;

                        //Set up player model
                        //Instantiate one random player model for chosen team
                        go = Instantiate(pmcCoop.information.prefab, transform, false);
                        //Assign
                        thirdPersonPlayerModel = go.GetComponent<Kit_ThirdPersonPlayerModel>();
                        //Set information
                        thirdPersonPlayerModel.information = pmcCoop.information;
                        break;
                    case 2:
                        //Set up player model
                        //Instantiate one random player model for chosen team
                        go = Instantiate(Kit_IngameMain.instance.gameInformation.allPvpTeams[myTeam].playerModels[thirdPersonPlayerModelID].prefab, transform, false);
                        //Assign
                        thirdPersonPlayerModel = go.GetComponent<Kit_ThirdPersonPlayerModel>();
                        //And cache information
                        thirdPersonPlayerModel.information = Kit_IngameMain.instance.gameInformation.allPvpTeams[myTeam].playerModels[thirdPersonPlayerModelID];
                        break;
                }

                //Reset scale
                go.transform.localScale = Vector3.one;
                //Setup
                thirdPersonPlayerModel.SetupModel(this);
                //Setup Customization
                thirdPersonPlayerModel.SetCustomizations(playerModelCustomizations, this);
                //Make it third person initially
                thirdPersonPlayerModel.ThirdPerson();

                if (thirdPersonPlayerModel.firstPersonArmsPrefab.Count == 0)
                {
                    Debug.LogWarning("WARNING: Player Model does not have ANY first person arms prefabs assigned! Game might be broken! To fix, assign first person arms prefabs to this player model with your key (Default Key: Kit)", thirdPersonPlayerModel.gameObject);
                }
            }
        }

        public void ServerDamage(float dmg, int gunID, Vector3 shotPos, Vector3 forward, float force, Vector3 hitPos, int id, bool botShot, uint idWhoShot)
        {
            //Damage is server authorative now
            if (isServer)
            {
                if (Kit_IngameMain.instance.assistManager)
                {
                    Kit_IngameMain.instance.assistManager.PlayerDamaged(botShot, idWhoShot, this, dmg);
                }

                ragdollForce = force;
                ragdollForward = forward;
                ragdollPoint = hitPos;
                ragdollId = id;
                deathSoundCategory = gameInformation.allWeapons[gunID].deathSoundCategory;
                if (voiceManager)
                {
                    deathSoundID = voiceManager.GetDeathSoundID(this, deathSoundCategory);
                }

                //Relay to the assigned manager
                vitalsManager.ApplyDamage(this, dmg, botShot, idWhoShot, gunID, shotPos);

                RpcDamageVisuals(dmg, shotPos);
            }
        }

        public void ServerDamage(float dmg, string deathCause, Vector3 shotPos, Vector3 forward, float force, Vector3 hitPos, int id, bool botShot, uint idWhoShot)
        {
            if (isServer)
            {
                if (Kit_IngameMain.instance.assistManager)
                {
                    Kit_IngameMain.instance.assistManager.PlayerDamaged(botShot, idWhoShot, this, dmg);
                }

                //Apply damage
                vitalsManager.ApplyDamage(this, dmg, botShot, idWhoShot, deathCause, shotPos);

                ragdollForce = force;
                ragdollForward = forward;
                ragdollId = id;
                ragdollPoint = hitPos;
                deathSoundCategory = gameInformation.allWeapons[0].deathSoundCategory;

                RpcDamageVisuals(dmg, shotPos);
            }
        }

        public void ServerBlind(float time, int gunID, Vector3 shotPos, bool botShot, uint idWhoShot)
        {
            if (isServer)
            {
                RpcBlind(time, gunID, shotPos, botShot, idWhoShot);
            }
        }

        public void ApplyFallDamage(float dmg)
        {
            if (isServer)
            {
                vitalsManager.ApplyFallDamage(this, dmg);
            }
        }

        [Command]
        public void CmdSuicide()
        {
            if (isServer)
            {
                vitalsManager.Suicide(this);
            }
        }

        /// <summary>
        /// Kill the player by cause.
        /// </summary>
        /// <param name="cause"></param>
        public void Die(int cause)
        {
            if (isServer || isLocalPlayer)
            {
                //Tell weapon manager
                weaponManager.PlayerDead(this);
            }

            if (isServer)
            {
                //Tell clients for the killfeed
                Kit_IngameMain.instance.ServerDeathInfo(isBot, id, isBot, id, cause, thirdPersonPlayerModelID, ragdollId);
                Kit_IngameMain.instance.RpcDeathInfo(isBot, id, isBot, id, cause, thirdPersonPlayerModelID, ragdollId);

                if (Kit_IngameMain.instance.assistManager)
                {
                    Kit_IngameMain.instance.assistManager.PlayerKilled(isBot, id, this);
                }

                NetworkServer.Destroy(gameObject);
            }
        }

        public void Die(bool botShot, uint killer, int gunID)
        {
            if (isServer || isLocalPlayer)
            {
                //Tell weapon manager
                weaponManager.PlayerDead(this);
            }

            if (isServer)
            {
                //Tell clients for the killfeed
                Kit_IngameMain.instance.ServerDeathInfo(botShot, killer, isBot, id, gunID, thirdPersonPlayerModelID, ragdollId);
                Kit_IngameMain.instance.RpcDeathInfo(botShot, killer, isBot, id, gunID, thirdPersonPlayerModelID, ragdollId);

                if (Kit_IngameMain.instance.assistManager)
                {
                    Kit_IngameMain.instance.assistManager.PlayerKilled(botShot, killer, this);
                }

                NetworkServer.Destroy(gameObject);
            }
        }

        public void Die(bool botShot, uint killer, string cause)
        {
            if (isServer || isLocalPlayer)
            {
                //Tell weapon manager
                weaponManager.PlayerDead(this);
            }

            if (isServer)
            {
                //Tell clients for the killfeed
                Kit_IngameMain.instance.ServerDeathInfo(botShot, killer, isBot, id, cause, thirdPersonPlayerModelID, ragdollId);
                Kit_IngameMain.instance.RpcDeathInfo(botShot, killer, isBot, id, cause, thirdPersonPlayerModelID, ragdollId);

                if (Kit_IngameMain.instance.assistManager)
                {
                    Kit_IngameMain.instance.assistManager.PlayerKilled(botShot, killer, this);
                }

                NetworkServer.Destroy(gameObject);
            }
        }
        #endregion

        #region Server Calls
        /// <summary>
        /// Server tells us that we hit something.
        /// 0 = Normal, 1 = Spawn protection
        /// Chose byte so you can add more types.
        /// </summary>
        /// <param name="type"></param>
        [TargetRpc]
        public void TargetHitMarker(byte type)
        {
            if (isFirstPersonActive)
            {
                switch (type)
                {
                    case 0:
                        Kit_IngameMain.instance.hud.DisplayHitmarker();
                        break;
                    case 1:
                        Kit_IngameMain.instance.hud.DisplayHitmarkerSpawnProtected();
                        break;
                }
            }
        }

        [ClientRpc(includeOwner = true)]
        public void ClientImpactProcess(Vector3 pos, Vector3 normal, string material)
        {
            //Relay to impact processor
            Kit_IngameMain.instance.impactProcessor.ProcessImpact(pos, normal, material);
        }
        #endregion

        #region RPCs
        [ClientRpc] //Client RPC for spectating purposes
        public void RpcBlind(float time, int gunID, Vector3 shotPos, bool botShot, uint idWhoShot)
        {
            if (isFirstPersonActive)
            {
                Kit_IngameMain.instance.hud.DisplayBlind(time);
                //Tell HUD
                Kit_IngameMain.instance.hud.DisplayShot(shotPos);
            }
        }

        [ClientRpc] //Client RPC for spectating purposes
        public void RpcDamageVisuals(float dmg, Vector3 shotPos)
        {
            if (isFirstPersonActive)
            {
                //Tell HUD
                Kit_IngameMain.instance.hud.DisplayShot(shotPos);
            }
        }

        //If we fire using a semi auto weapon, this is called

        [ClientRpc]
        public void RpcWeaponSemiFireNetwork()
        {
            //Relay to weapon manager
            weaponManager.NetworkSemiRPCReceived(this);
        }

        [ClientRpc]
        //If we fire using a bolt action weapon, this is called
        public void RpcWeaponBoltActionFireNetwork(int state)
        {
            //Relay to weapon manager
            weaponManager.NetworkBoltActionRPCReceived(this, state);
        }

        [ClientRpc]
        public void RpcWeaponBurstFireNetwork(int burstLength)
        {
            //Relay to weapon manager
            weaponManager.NetworkBurstRPCReceived(this, burstLength);
        }

        [ClientRpc]
        public void RpcWeaponFirePhysicalBulletOthers(Vector3 pos, Vector3 dir)
        {
            //Relay to weapon manager
            weaponManager.NetworkPhysicalBulletFired(this, pos, dir);
        }

        //When we reload, this is called
        [ClientRpc]
        public void RpcWeaponReloadNetwork(bool empty)
        {
            //Reload to weapon manager
            weaponManager.NetworkReloadRPCReceived(this, empty);
        }

        //When a procedural reload occurs, this will be called with the correct stage
        [ClientRpc]
        public void RpcWeaponProceduralReloadNetwork(int stage)
        {
            //Relay to weapon manager
            weaponManager.NetworkProceduralReloadRPCReceived(this, stage);
        }

        [ClientRpc]
        public void RpcMeleeStabNetwork(int state, int slot)
        {
            //Send to player model
            thirdPersonPlayerModel.PlayMeleeAnimation(0, state);
            //Weapon Manager
            weaponManager.NetworkMeleeStabRPCReceived(this, state, slot);
        }

        [ClientRpc]
        public void RpcMeleeChargeNetwork(int id, int slot)
        {
            //Send to player model
            thirdPersonPlayerModel.PlayMeleeAnimation(1, id);
            //Weapon Manager
            weaponManager.NetworkMeleeChargeRPCReceived(this, id, slot);
        }

        [ClientRpc]
        public void RpcMeleeHealNetwork(int id)
        {
            //Send to playyer model
            thirdPersonPlayerModel.PlayMeleeAnimation(2, id);
            //Weapon Manager
            weaponManager.NetworkMeleeHealRPCReceived(this, id);
        }

        [ClientRpc]
        public void RpcGrenadePullPinNetwork()
        {
            //Relay
            weaponManager.NetworkGrenadePullPinRPCReceived(this);
        }

        [ClientRpc]
        public void RpcGrenadeThrowNetwork()
        {
            //Relay
            weaponManager.NetworkGrenadeThrowRPCReceived(this);
        }

        public void WeaponRestockAll(bool allWeapons)
        {
            if (isServer)
            {
                //Relay
                weaponManager.RestockAmmo(this, allWeapons);
            }
        }


        public void ReplaceWeapon(int slot, int weapon, int bulletsLeft, int bulletsLeftToReload, int[] attachments)
        {
            if (isServer)
            {
                //Relay to weapon manager
                weaponManager.NetworkReplaceWeapon(this, slot, weapon, bulletsLeft, bulletsLeftToReload, attachments);
            }
            else
            {
                Debug.LogError("Server only function called on non-server!");
            }
        }


        [ClientRpc]
        public void RpcPlayVoiceLine(int catId, int id)
        {
            if (voiceManager)
            {
                voiceManager.PlayVoiceRpcReceived(this, catId, id);
            }
        }


        [ClientRpc]
        public void RpcPlayVoiceLine(int catId, int id, int idTwo)
        {
            if (voiceManager)
            {
                voiceManager.PlayVoiceRpcReceived(this, catId, id, idTwo);
            }
        }

        [ClientRpc]
        public void RpcMovementPlaySound(int id, int id2, int arrayID)
        {
            //Relay to movement
            movement.PlaySound(this, id, id2, arrayID);
        }

        [ClientRpc]
        public void RpcMovementPlayAnimation(int id, int id2)
        {
            //Relay to movement
            movement.PlayAnimation(this, id, id2);
        }
        #endregion

        #region Spectating
        /// <summary>
        /// Begin spectating this player
        /// </summary>
        public void OnSpectatingStart()
        {
            //Move camera to the right position
            Kit_IngameMain.instance.activeCameraTransform = playerCameraTransform;
            //Set bool
            isBeingSpectated = true;

            if (looking.GetPerspective(this) == Kit_GameInformation.Perspective.FirstPerson)
            {
                //Hide third person object
                thirdPersonPlayerModel.FirstPerson();
            }
            else
            {

            }

            //Call Weapon Manager
            weaponManager.BeginSpectating(this);

            //HUD
            Kit_IngameMain.instance.hud.SetVisibility(true);

            //Minimap
            if (Kit_IngameMain.instance.minimap)
            {
                Kit_IngameMain.instance.minimap.LocalPlayerSpawned(this);
            }
        }

        /// <summary>
        /// End spectating this player
        /// </summary>
        public void OnSpectatingEnd()
        {
            //Set bool
            isBeingSpectated = false;
            //Third Person
            thirdPersonPlayerModel.ThirdPerson();

            //Weapon Manager
            weaponManager.EndSpectating(this);

            //HUD
            Kit_IngameMain.instance.hud.SetVisibility(false);

            //Minimap
            if (Kit_IngameMain.instance.minimap)
            {
                Kit_IngameMain.instance.minimap.LocalPlayerDied(this);
            }
        }
        #endregion

        #region Lag Compensation
        public void SetLagCompensationTo(float revertBy)
        {
            thirdPersonPlayerModel.SetLagCompensationTo(revertBy);
        }
        #endregion
    }
}
