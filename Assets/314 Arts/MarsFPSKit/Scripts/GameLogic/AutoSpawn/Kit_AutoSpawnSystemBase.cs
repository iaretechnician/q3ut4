using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// Base system for the auto spawn system
    /// </summary>
    public abstract class Kit_AutoSpawnSystemBase : MonoBehaviour
    {
        /// <summary>
        /// Called when the local player spawned
        /// </summary>
        public abstract void LocalPlayerSpawned();

        /// <summary>
        /// Called after the local player died
        /// </summary>
        public abstract void LocalPlayerDied();

        /// <summary>
        /// Called when the player interrupts the spawn process (Opens chat, Opens Pause Menu)
        /// </summary>
        public abstract void Interruption();
    }
}
