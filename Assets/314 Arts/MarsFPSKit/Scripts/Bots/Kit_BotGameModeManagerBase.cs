using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFPSKit
{
    public abstract class Kit_BotGameModeManagerBase : ScriptableObject
    {
        /// <summary>
        /// Called in the beginning
        /// </summary>
        /// <param name="manager"></param>
        public abstract void Inizialize(Kit_BotManager manager);
        /// <summary>
        /// A player joined a team
        /// </summary>
        /// <param name="manager"></param>
        public abstract void PlayerJoinedTeam(Kit_BotManager manager);
        /// <summary>
        /// A player left a team
        /// </summary>
        /// <param name="manager"></param>
        public abstract void PlayerLeftTeam(Kit_BotManager manager);
    }
}
