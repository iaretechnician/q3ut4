using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// Use this class to implement your own killfeed
    /// </summary>
    public abstract class Kit_KillFeedBase : MonoBehaviour
    {
        /// <summary>
        /// Add an entry to the killfeed
        /// </summary>
        /// <param name="killer">Who shot</param>
        /// <param name="killed">Who was killed</param>
        /// <param name="gun">Which weapon was used to kill</param>
        public abstract void Append(bool botKiller, uint killer, bool botKilled, uint killed, int gun, int playerModel, int ragdollId);

        /// <summary>
        /// Add an entry to the killfeed
        /// </summary>
        /// <param name="killer">Who shot</param>
        /// <param name="killed">Who was killed</param>
        /// <param name="gun">Which weapon was used to kill</param>
        public abstract void Append(bool botKiller, uint killer, bool botKilled, uint killed, string cause, int playerModel, int ragdollId);
    }
}
