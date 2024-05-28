using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MarsFPSKit
{
    public class Kit_PvP_GMB_DominationNetworkData : Kit_GameModeNetworkDataBase
    {
        /// <summary>
        /// Points scored by each team
        /// </summary>
        public readonly SyncList<int> teamPoints = new SyncList<int>();

        /// <summary>
        /// When did the last tick occur?
        /// </summary>
        public double lastTick;

        /// <summary>
        /// Flags currently used
        /// </summary>
        public List<Kit_Domination_FlagRuntime> flags = new List<Kit_Domination_FlagRuntime>();
    }
}