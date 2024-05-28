using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// Implements assist management
    /// </summary>
    public abstract class Kit_AssistManagerBase : ScriptableObject
    {
        /// <summary>
        /// Called on start to set up the manager
        /// </summary>
        /// <param name="main"></param>
        public abstract void OnStart();

        /// <summary>
        /// Called when a player is damaged
        /// </summary>
        /// <param name="who"></param>
        /// <param name="damaged"></param>
        public abstract void PlayerDamaged(bool botShot, uint shotId, Kit_PlayerBehaviour damagedPlayer, float dmg);

        /// <summary>
        /// Called when a player is killed
        /// </summary>
        /// <param name="botKilled"></param>
        /// <param name="idKilled"></param>
        /// <param name="botKiller"></param>
        /// <param name="idKiller"></param>
        public abstract void PlayerKilled(bool botKiller, uint idKiller, Kit_PlayerBehaviour killedPlayer);

        /// <summary>
        /// These are now always started by the server
        /// </summary>
        /// <param name="eventCode"></param>
        /// <param name="content"></param>
        public abstract void OnGenericEvent(byte eventCode, int content);
    }
}