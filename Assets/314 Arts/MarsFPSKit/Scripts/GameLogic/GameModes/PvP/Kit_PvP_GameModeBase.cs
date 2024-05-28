using UnityEngine;
using System;
using Mirror;
using MarsFPSKit.World;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MarsFPSKit
{
    public abstract class Kit_PvP_GameModeBase : Kit_WeaponInjection
    {
        public GameObject networkData;

        public string gameModeName;

        [Header("Traditional room browser & host")]
        [Tooltip("All Maps that are available")]
        public Kit_MapInformation[] traditionalMaps;
        [Tooltip("Player limit")]
        public byte[] traditionalPlayerLimits = new byte[3] { 2, 5, 10 };
        [Tooltip("Players needed for match start")]
        public int[] traditionalPlayerNeeded = new int[3] { 0, 2, 4 };
        [Tooltip("Game Duration in minutes")]
        public int[] traditionalDurations = new int[3] { 5, 10, 20 };
        [Tooltip("Available Ping limits")]
        public ushort[] traditionalPingLimits = new ushort[5] { 0, 50, 100, 200, 300 };
        [Tooltip("Available AFK limits")]
        public int[] traditionalAfkLimits = new int[6] { 0, 60, 120, 180, 240, 300 };

        [Header("Lobby Matchmaking")]
        [Tooltip("All Maps that are available in lobby mode")]
        public Kit_MapInformation[] lobbyMaps;
        /// <summary>
        /// Players needed to start the game
        /// </summary>
        public int lobbyMinimumPlayersNeeded = 8;
        /// <summary>
        /// Player limit for the game mode
        /// </summary>
        public byte lobbyMaximumPlayers = 12;
        /// <summary>
        /// Duration for the game mode in lobby
        /// </summary>
        public int lobbyGameDuration = 600;
        /// <summary>
        /// Ping limit when using Lobby
        /// </summary>
        public ushort lobbyPingLimit = 200;
        /// <summary>
        /// Afk Limit when using lobby
        /// </summary>
        public int lobbyAfkLimit = 120;
        /// <summary>
        /// Should bots be enabled in this lobby?
        /// </summary>
        public bool lobbyBotsEnabled;
        /// <summary>
        /// From how many maps can we choose?
        /// </summary>
        public int lobbyAmountOfMapsToVoteFor = 2;
        /// <summary>
        /// If the lobby existed for this time after creation without gathering enough players, it will proceed. Setting to 0 or below disables this feature
        /// </summary>
        public float lobbyStartWithBotsAfterSeconds = 60f;

        [Header("Modules")]
        /// <summary>
        /// Which HUD prefab should be used for this game mode? Can be null.
        /// </summary>
        public GameObject hudPrefab;
        /// <summary>
        /// The spawn system that we want to use
        /// </summary>
        public Kit_SpawnSystemBase spawnSystemToUse;
        /// <summary>
        /// The bot manager that this game mode should use
        /// </summary>
        public Kit_BotGameModeManagerBase botManagerToUse;

        /// <summary>
        /// Use this to override bot controls
        /// </summary>
        public Kit_PlayerBotControlBase botControlOverride;

        /// <summary>
        /// If this is enabled, you can send team only chat messages.
        /// </summary>
        public bool isTeamGameMode;

        /// <summary>
        /// If more than 2 teams are configured, this can limit the game mode, so you can make game modes that use a different amount of teams!
        /// </summary>
        public int maximumAmountOfTeams = 2;

        /// <summary>
        /// Called when stats are being reset
        /// </summary>
        /// <param name="table"></param>
        public virtual void ResetStats()
        {

        }

        public virtual void RegisterNetworkPrefabs()
        {

        }

        /// <summary>
        /// Started upon starting playing with this game mode
        /// </summary>
        /// <param name="main"></param>
        public abstract void GamemodeSetupServer();

        /// <summary>
        /// Started upon starting playing with this game mode
        /// </summary>
        /// <param name="main"></param>
        public virtual void GamemodeSetupClient()
        {

        }

        /// <summary>
        /// Called when the game mode starts when enough players are connected. Not called if there are enough players when the game mode initially began.
        /// </summary>
        /// <param name="main"></param>
        public virtual void GameModeBeginMiddle()
        {
            Kit_IngameMain.instance.ResetPlayers();

            //Find all weapon spawners
            Kit_WeaponSpawner[] weaponSpawners = FindObjectsOfType<Kit_WeaponSpawner>();
            //Reset them
            for (int i = 0; i < weaponSpawners.Length; i++)
            {
                weaponSpawners[i].GameModeBeginMiddle();
            }

            //Find all ammo spawners
            Kit_AmmoSpawner[] ammoSpawners = FindObjectsOfType<Kit_AmmoSpawner>();
            //Reset
            for (int i = 0; i < ammoSpawners.Length; i++)
            {
                ammoSpawners[i].GameModeBeginMiddle();
            }

            Kit_DestroyUponGameModeReset[] resets = FindObjectsOfType<Kit_DestroyUponGameModeReset>();
            for (int i = 0; i < resets.Length; i++)
            {
                if (resets[i].GetComponent<NetworkIdentity>())
                {
                    NetworkServer.Destroy(resets[i].gameObject);
                }
                else
                {
                    Destroy(resets[i].gameObject);
                }
            }
        }

        /// <summary>
        /// Called every frame as long as this game mode is active
        /// </summary>
        /// <param name="main"></param>
        public virtual void GameModeUpdate()
        {

        }

        /// <summary>
        /// Called every frame as long as this game mode is active for other players
        /// </summary>
        /// <param name="main"></param>
        public virtual void GameModeUpdateOthers()
        {

        }

        /// <summary>
        /// Called every time a player dies
        /// </summary>
        /// <param name="main"></param>
        public virtual void PlayerDied(bool botKiller, uint killer, bool botKilled, uint killed)
        {

        }

        /// <summary>
        /// Called when a player spawned (others + bots)
        /// </summary>
        /// <param name="pb"></param>
        public virtual void OnPlayerSpawned(Kit_PlayerBehaviour pb)
        {

        }

        /// <summary>
        /// Called when another player was destroyed (bots aswell)
        /// </summary>
        /// <param name="pb"></param>
        public virtual void OnPlayerDestroyed(Kit_PlayerBehaviour pb)
        {

        }

        /// <summary>
        /// Called when we successfully spawned (not bots)
        /// </summary>
        /// <param name="pb"></param>
        public virtual void OnLocalPlayerSpawned(Kit_PlayerBehaviour pb)
        {

        }

        /// <summary>
        /// Called when the local (controlling) player is destroyed
        /// </summary>
        /// <param name="pb"></param>
        public virtual void OnLocalPlayerDestroyed(Kit_PlayerBehaviour pb)
        {

        }

        /// <summary>
        /// Called when our death camera is over
        /// </summary>
        /// <param name="pb"></param>
        public virtual void OnLocalPlayerDeathCameraEnded()
        {

        }

        /// <summary>
        /// Called when the local player has gained a kill
        /// </summary>
        /// <param name="main"></param>
        public virtual void LocalPlayerScoredKill()
        {

        }

        /// <summary>
        /// Called for the master client when a bot has gained a kill
        /// </summary>
        /// <param name="main"></param>
        /// <param name="bot"></param>
        public virtual void MasterClientBotScoredKill(Kit_Bot bot)
        {

        }

        /// <summary>
        /// Called when the timer reaches zero
        /// </summary>
        /// <param name="main"></param>
        public virtual void TimeRunOut()
        {

        }

        /// <summary>
        /// Returns a spawnpoint for the associated player
        /// </summary>
        /// <param name="main"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public abstract Transform GetSpawn(Kit_Player player);

        /// <summary>
        /// Returns a spawnpoint for the associated player
        /// </summary>
        /// <param name="main"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public abstract Transform GetSpawn(Kit_Bot bot);


        /// <summary>
        /// Can we currently spawn?
        /// </summary>
        /// <param name=""></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public virtual bool CanSpawn(Kit_Player player)
        {
            return true;
        }

        /// <summary>
        /// Can we currently spawn?
        /// </summary>
        /// <param name=""></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public virtual bool CanSpawn(Kit_Bot bot)
        {
            return true;
        }

        /// <summary>
        /// Does this game mode have a custom spawn method?
        /// </summary>
        /// <returns></returns>
        public virtual bool UsesCustomSpawn()
        {
            return false;
        }

        public virtual GameObject DoCustomSpawn(Kit_Player player, Loadout selectedLoadout)
        {
            throw new NotImplementedException("Game mode " + this.name + " uses custom spawn, but it has not been implemented [players]!");
        }

        public virtual Loadout DoCustomSpawnBot(Kit_Bot bot)
        {
            throw new NotImplementedException("Game mode " + this.name + " uses custom spawn, but it has not been implemented [bots]!");
        }

        /// <summary>
        /// Can we join this specific team?
        /// </summary>
        /// <param name="main"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public virtual bool CanJoinTeam(NetworkConnectionToClient player, int team)
        {
            return true;
        }

        /// <summary>
        /// Can the player be controlled at this stage of this game mode?
        /// </summary>
        /// <param name="main"></param>
        /// <returns></returns>
        public virtual bool CanControlPlayer()
        {
            return true;
        }

        /// <summary>
        /// Are there enough players currently to play this game mode?
        /// </summary>
        /// <param name="main"></param>
        /// <returns></returns>
        public virtual bool AreEnoughPlayersThere()
        {
            return true;
        }

        /// <summary>
        /// Can weapons be dropped in this game mode?
        /// </summary>
        /// <param name="main"></param>
        /// <returns></returns>
        public virtual bool CanDropWeapons()
        {
            return true;
        }

        /// <summary>
        /// Is our local player enemies with this player?
        /// </summary>
        /// <param name="with"></param>
        /// <returns></returns>
        public virtual bool AreWeEnemies(bool botEnemy, uint enemyId)
        {
            return true;
        }

        /// <summary>
        /// Are these two players enemies?
        /// </summary>
        /// <param name="playerOne"></param>
        /// <param name="playerTwo"></param>
        /// <returns></returns>
        public virtual bool ArePlayersEnemies(Kit_PlayerBehaviour playerOne, Kit_PlayerBehaviour playerTwo)
        {
            return true;
        }

        /// <summary>
        /// Are these two players enemies?
        /// </summary>
        /// <param name="playerOne"></param>
        /// <param name="playerTwo"></param>
        /// <returns></returns>
        public virtual bool ArePlayersEnemies(uint playerOneID, bool playerOneBot, uint playerTwoID, bool playerTwoBot, bool canKillSelf = false)
        {
            return true;
        }

        /// <summary>
        /// Are these two players enemies? (Used by bullets.)
        /// </summary>
        /// <param name="playerOneID"></param>
        /// <param name="playerOneBot"></param>
        /// <param name="playerTwo"></param>
        /// <returns></returns>
        public virtual bool ArePlayersEnemies(uint playerOneID, bool playerOneBot, Kit_PlayerBehaviour playerTwo, bool canKillSelf)
        {
            return true;
        }

        /// <summary>
        /// Can a vote be started currently?
        /// </summary>
        /// <param name="main"></param>
        /// <returns></returns>
        public virtual bool CanStartVote()
        {
            return false;
        }

        /// <summary>
        /// Does this game mode support the loadout menu?
        /// </summary>
        /// <returns></returns>
        public virtual bool LoadoutMenuSupported()
        {
            return true;
        }

        /// <summary>
        /// Does this game mode support auto spawn
        /// </summary>
        /// <returns></returns>
        public virtual bool AutoSpawnSupported()
        {
            return true;
        }

        /// <summary>
        /// These are now always started by the server
        /// </summary>
        /// <param name="eventCode"></param>
        /// <param name="content"></param>
        public virtual void OnGenericEvent(byte eventCode, object content)
        {

        }

        /// <summary>
        /// Returns whomst we can spectate
        /// </summary>
        /// <param name="main"></param>
        /// <returns></returns>
        public virtual Spectating.Spectateable GetSpectateable()
        {
            return Spectating.Spectateable.All;
        }

        /// <summary>
        /// Override if you want to disable spectating. This setting is for the "global" spectating.
        /// </summary>
        /// <param name="main"></param>
        /// <returns></returns>
        public virtual bool SpectatingEnabled()
        {
            return true;
        }

#if UNITY_EDITOR
        /// <summary>
        /// For the scene checker, returns state to display
        /// </summary>
        public abstract string[] GetSceneCheckerMessages();

        /// <summary>
        /// For the scene checker, returns state to display
        /// </summary>
        /// <returns></returns>
        public abstract MessageType[] GetSceneCheckerMessageTypes();
#endif
    }
}
