using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// Use this to implement your own minimap
    /// </summary>
    public abstract class Kit_MinimapBase : MonoBehaviour
    {
        /// <summary>
        /// Called at the beginning of the match
        /// </summary>
        /// <param name="main"></param>
        public abstract void Setup();

        /// <summary>
        /// This is called when our local player switches teams and should be used to redraw sprites for other players
        /// </summary>
        /// <param name="main"></param>
        public abstract void LocalPlayerSwitchedTeams();

        /// <summary>
        /// Called when our local player spawned
        /// </summary>
        /// <param name="pb"></param>
        public abstract void LocalPlayerSpawned(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called when our local player dies
        /// </summary>
        /// <param name="pb"></param>
        public abstract void LocalPlayerDied(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called by our local player in update
        /// </summary>
        /// <param name="pb"></param>
        public abstract void LocalPlayerUpdate(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called when a different player spawned
        /// </summary>
        /// <param name="pb"></param>
        public abstract void PlayerSpawned(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called when a different player dies
        /// </summary>
        /// <param name="pb"></param>
        public abstract void PlayerDied(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called when a player shoots
        /// </summary>
        /// <param name="pb"></param>
        public abstract void PlayerShoots(Kit_PlayerBehaviour pb, bool silenced);

        /// <summary>
        /// Called by a friendly player (team based game mode) in Update
        /// </summary>
        /// <param name="pb"></param>
        public abstract void PlayerFriendlyUpdate(Kit_PlayerBehaviour pb);
    }
}