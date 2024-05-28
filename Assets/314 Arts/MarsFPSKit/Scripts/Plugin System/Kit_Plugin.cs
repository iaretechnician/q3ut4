using MarsFPSKit.UI;
using MarsFPSKit.Weapons;
using UnityEngine;

namespace MarsFPSKit
{
    public class Kit_Plugin : Kit_WeaponInjection
    {
        /// <summary>
        /// Called on Start before anything else is setup
        /// </summary>
        /// <param name="main"></param>
        public virtual void OnPreSetup()
        {

        }

        /// <summary>
        /// Called when everything is done
        /// </summary>
        /// <param name="main"></param>
        public virtual void OnSetupDone()
        {

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
        /// Called in update by Kit_IngameMain
        /// </summary>
        /// <param name="main"></param>
        public virtual void PluginUpdate()
        {

        }

        /// <summary>
        /// Called in LateUpdate Kit_IngameMain
        /// </summary>
        /// <param name="main"></param>
        public virtual void PluginLateUpdate()
        {

        }

        /// <summary>
        /// Called when the local player (not bots) joins a team
        /// </summary>
        /// <param name="main"></param>
        /// <param name="newTeam"></param>
        public virtual void LocalPlayerChangedTeam(int newTeam)
        {

        }

        /// <summary>
        /// Called when the local player spawned
        /// </summary>
        /// <param name="player"></param>
        public virtual void LocalPlayerSpawned(Kit_PlayerBehaviour player)
        {

        }

        /// <summary>
        /// Called when the local player died
        /// </summary>
        /// <param name="player"></param>
        public virtual void LocalPlayerDied(Kit_PlayerBehaviour player)
        {

        }

        /// <summary>
        /// Called when a player spawned
        /// </summary>
        /// <param name="player"></param>
        public virtual void PlayerSpawned(Kit_PlayerBehaviour player)
        {

        }

        /// <summary>
        /// Called when a player died
        /// </summary>
        /// <param name="player"></param>
        public virtual void PlayerDied(Kit_PlayerBehaviour player)
        {

        }

        /// <summary>
        /// Called by our local player in update
        /// </summary>
        /// <param name="player"></param>
        public virtual void LocalPlayerUpdate(Kit_PlayerBehaviour player)
        {

        }

        /// <summary>
        /// Called by other players and bots in update
        /// </summary>
        /// <param name="player"></param>
        public virtual void PlayerUpdate(Kit_PlayerBehaviour player)
        {

        }

        /// <summary>
        /// Called in start by the main menu. You can reset data here
        /// </summary>
        /// <param name=""></param>
        public virtual void Reset(Kit_MenuManager menu)
        {

        }

        public virtual void BotWasKilled(Kit_Bot bot)
        {

        }
        public virtual void BotScoredKill(Kit_Bot bot, bool botKilled, uint killed, int gunId, int thirdPersonPlayerModelId, int ragdollId)
        {

        }

        public virtual void BotScoredKill(Kit_Bot bot, bool botKilled, uint killed, string cause, int thirdPersonPlayerModelId, int ragdollId)
        {

        }

        public virtual void LocalPlayerWasKilled()
        {

        }

        public virtual void LocalPlayerScoredKill(bool botKilled, uint killed, int gunId, int thirdPersonPlayerModelId, int ragdollId)
        {

        }

        public virtual void LocalPlayerScoredKill(bool botKilled, uint killed, string cause, int thirdPersonPlayerModelId, int ragdollId)
        {

        }

        public virtual void BotWasCreated(Kit_BotManager manager, Kit_Bot bot)
        {

        }

        public virtual void PlayerJoinedServer(Kit_Player player)
        {

        }

        public virtual void PlayerLeftServer(Kit_Player player)
        {

        }
    }
}