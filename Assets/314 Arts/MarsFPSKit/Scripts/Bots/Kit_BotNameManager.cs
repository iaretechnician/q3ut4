using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// This manages how bot names are given
    /// </summary>
    public abstract class Kit_BotNameManager : ScriptableObject
    {
        /// <summary>
        /// Returns a random name for a bot
        /// </summary>
        /// <returns></returns>
        public abstract string GetRandomName(Kit_BotManager bm);
    }
}
