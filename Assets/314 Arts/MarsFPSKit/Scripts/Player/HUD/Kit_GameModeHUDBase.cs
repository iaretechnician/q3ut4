using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// Used for game mode specific HUD elements
    /// </summary>
    public abstract class Kit_GameModeHUDBase : MonoBehaviour
    {
        /// <summary>
        /// Called after the game mode was initialized
        /// </summary>
        /// <param name="main"></param>
        public virtual void HUDInitialize()
        {

        }

        /// <summary>
        /// Calculate the hud
        /// </summary>
        /// <param name="main"></param>
        public abstract void HUDUpdate();
    }
}
