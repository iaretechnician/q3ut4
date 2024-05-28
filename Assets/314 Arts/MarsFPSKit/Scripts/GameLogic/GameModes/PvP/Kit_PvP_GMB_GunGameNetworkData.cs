using Mirror;
using UnityEngine;

namespace MarsFPSKit
{
    public class Kit_PvP_GMB_GunGameNetworkData : Kit_GameModeNetworkDataBase
    {
        /// <summary>
        /// When did we check for a winner the last time?
        /// </summary>
        public float lastWinnerCheck;
        [SyncVar]
        /// <summary>
        /// The current weapon order to use
        /// </summary>
        public int currentGunOrder;
        /// <summary>
        /// The current gun of the given pc
        /// </summary>
        public readonly SyncDictionary<uint, int> currentGun = new SyncDictionary<uint, int>();
    }
}