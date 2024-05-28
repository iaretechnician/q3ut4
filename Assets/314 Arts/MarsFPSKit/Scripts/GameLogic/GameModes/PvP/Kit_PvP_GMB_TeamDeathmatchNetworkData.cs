using Mirror;
using UnityEngine;

namespace MarsFPSKit
{
    public class Kit_PvP_GMB_TeamDeathmatchNetworkData : Kit_GameModeNetworkDataBase
    {
        /// <summary>
        /// Points scored by each team
        /// </summary>
        public readonly SyncList<int> teamPoints = new SyncList<int>();
    }
}