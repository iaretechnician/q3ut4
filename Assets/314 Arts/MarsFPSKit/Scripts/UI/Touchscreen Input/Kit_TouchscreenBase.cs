using UnityEngine;

namespace MarsFPSKit
{
    public abstract class Kit_TouchscreenBase : MonoBehaviour
    {
        /// <summary>
        /// Called when the system is first initialized
        /// </summary>
        /// <param name="main"></param>
        public abstract void Setup();

        /// <summary>
        /// Called when our local player spawned
        /// </summary>
        /// <param name="pb"></param>
        public abstract void LocalPlayerSpawned(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called when our local player dies (is destroyed)
        /// </summary>
        /// <param name="pb"></param>
        public abstract void LocalPlayerDied(Kit_PlayerBehaviour pb);
    }
}
