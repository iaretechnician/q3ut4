using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// Use this to implement your own ping limit system. As this scriptable object will only ever manage one instance, we can store runtime data in it.
    /// </summary>
    public abstract class Kit_PingLimitBase : ScriptableObject
    {
        /// <summary>
        /// This is always called in Start. It tells the system if it is enabled or not.
        /// </summary>
        /// <param name="main"></param>
        /// <param name="enabled"></param>
        public abstract void StartRelay(bool enabled, ushort pingLimit = 0);

        /// <summary>
        /// This is only called if the system is enabled. It's relayed in  Update from <see cref="Kit_IngameMain"/>
        /// </summary>
        /// <param name="main"></param>
        public abstract void UpdateRelay();
    }
}
