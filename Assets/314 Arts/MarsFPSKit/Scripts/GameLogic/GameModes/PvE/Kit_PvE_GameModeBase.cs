using UnityEngine;
using System.Collections;
using System;
using Mirror;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MarsFPSKit
{
    [System.Serializable]
    public class PlayerModelConfig
    {
        /// <summary>
        /// Player model to spawn with
        /// </summary>
        public Kit_PlayerModelInformation information;
        /// <summary>
        /// Selected customizations
        /// </summary>
        public int[] customization;
    }

    public abstract class Kit_PvE_GameModeBase : Kit_WeaponInjection
    {
        public GameObject networkData;

        /// <summary>
        /// Name of this game mode
        /// </summary>
        public string gameModeName = "Sandbox";

        /// <summary>
        /// Maps
        /// </summary>
        [Tooltip("Maps for this game mode")]
        public Kit_MapInformation[] maps;

        /// <summary>
        /// How many players in coop?
        /// </summary>
        public byte coopPlayerAmount = 4;

        /// <summary>
        /// Menu Prefab in the menu
        /// </summary>
        [Header("Modules")]
        public GameObject menuPrefab;
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
        /// Gets player model for this player
        /// </summary>
        /// <param name="pb"></param>
        /// <returns></returns>
        public abstract PlayerModelConfig GetPlayerModel(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Returns the spawn loadout
        /// </summary>
        /// <returns></returns>
        public abstract Loadout GetSpawnLoadout();

        /// <summary>
        /// Called when stats are being reset
        /// </summary>
        /// <param name="table"></param>
        public virtual void ResetStats()
        {

        }

        /// <summary>
        /// Started upon starting playing with this game mode
        /// </summary>
        /// <param name="main"></param>
        public virtual void GamemodeSetupServer()
        {
            if (networkData)
            {
                //Create network data
                GameObject nData = Instantiate(networkData, Vector3.zero, Quaternion.identity);
                Kit_IngameMain.instance.currentGameModeRuntimeData = nData.GetComponent<Kit_GameModeNetworkDataBase>();
                NetworkServer.Spawn(nData);
            }
        }

        /// <summary>
        /// Started upon starting playing with this game mode
        /// </summary>
        /// <param name="main"></param>
        public virtual void GamemodeSetupClient()
        {

        }

        /// <summary>
        /// Called after player setup
        /// </summary>
        /// <param name="main"></param>
        public abstract void GameModeProceed();


        /// <summary>
        /// Called every frame as long as this game mode is active
        /// </summary>
        /// <param name="main"></param>
        public abstract void GameModeUpdate();

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
        public abstract void PlayerDied(bool botKiller, uint killer, bool botKilled, uint killed);

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
        public abstract void TimeRunOut();

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
        public abstract bool CanSpawn(Kit_Player player);

        /// <summary>
        /// Does this game mode have a custom spawn method?
        /// </summary>
        /// <returns></returns>
        public virtual bool UsesCustomSpawn()
        {
            return false;
        }

        public virtual GameObject DoCustomSpawn()
        {
            throw new NotImplementedException("Game mode " + this.name + " uses custom spawn, but it has not been implemented [players]!");
        }

        public virtual Loadout DoCustomSpawnBot(Kit_Bot bot)
        {
            throw new NotImplementedException("Game mode " + this.name + " uses custom spawn, but it has not been implemented [bots]!");
        }

        /// <summary>
        /// Can the player be controlled at this stage of this game mode?
        /// </summary>
        /// <param name="main"></param>
        /// <returns></returns>
        public abstract bool CanControlPlayer();

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
        /// Can a vote be started currently?
        /// </summary>
        /// <param name="main"></param>
        /// <returns></returns>
        public abstract bool CanStartVote();

        /// <summary>
        /// These are now always started by the server
        /// </summary>
        /// <param name="eventCode"></param>
        /// <param name="content"></param>
        public virtual void OnGenericEvent(byte eventCode, object content)
        {

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
