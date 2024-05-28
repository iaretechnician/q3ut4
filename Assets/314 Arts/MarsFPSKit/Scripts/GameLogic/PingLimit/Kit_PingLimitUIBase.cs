using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// Use this class to implement your own Ping Limit UI.
    /// </summary>
    public abstract class Kit_PingLimitUIBase : MonoBehaviour
    {
        /// <summary>
        /// Display a warning to the user.
        /// </summary>
        /// <param name="currentPing"></param>
        /// <param name="warningNumber"></param>
        public abstract void DisplayWarning(ushort currentPing, ushort warningNumber);
    }
}
