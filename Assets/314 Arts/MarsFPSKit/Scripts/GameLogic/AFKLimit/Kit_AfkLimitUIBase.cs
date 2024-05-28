using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// Use this class to implement your own AFK Limit UI.
    /// </summary>
    public abstract class Kit_AfkLimitUIBase : MonoBehaviour
    {
        /// <summary>
        /// Display a warning to the user.
        /// </summary>
        /// <param name="timePlayerWasAfk">Time the player was AFK in seconds</param>
        /// <param name="kickIn"></param>
        /// <param name="warningNumber"></param>
        public abstract void DisplayWarning(float timePlayerWasAfk, float kickIn, int warningNumber);
    }
}
