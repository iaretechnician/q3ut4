using UnityEngine;

namespace MarsFPSKit
{
    public class Kit_PvP_GMB_DeathmatchNetworkData : Kit_GameModeNetworkDataBase
    {
        /// <summary>
        /// When did we check for a winner the last time?
        /// </summary>
        public float lastWinnerCheck;
    }
}