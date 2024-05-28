using UnityEngine;

namespace MarsFPSKit
{
    public enum PlayerNameState { none, friendlyClose, friendlyFar, enemy }

    /// <summary>
    /// Use this to implement your own player name ui
    /// </summary>
    public abstract class Kit_PlayerNameUIBase : ScriptableObject
    {
        /// <summary>
        /// Called from start
        /// </summary>
        /// <param name="pb"></param>
        public abstract void StartRelay(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called when the local player gained the control
        /// </summary>
        /// <param name="pb"></param>
        public abstract void LocalPlayerGainedControl(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called from update if this player is friendly
        /// </summary>
        /// <param name="pb"></param>
        public abstract void UpdateFriendly(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called from update if this player is an enemy
        /// </summary>
        /// <param name="pb"></param>
        public abstract void UpdateEnemy(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called from OnDestroy so we can release our marker
        /// </summary>
        /// <param name="pb"></param>
        public abstract void OnDestroyRelay(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called when our local player sees this player
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="validFor">If the ray is not fired every frame, this is the time this ray is definitely valid for</param>
        public abstract void PlayerSpotted(Kit_PlayerBehaviour pb, float validFor);
    }
}