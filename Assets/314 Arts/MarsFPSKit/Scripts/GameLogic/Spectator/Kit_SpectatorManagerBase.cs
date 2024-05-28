using UnityEngine;

namespace MarsFPSKit
{
    namespace Spectating
    {
        /// <summary>
        /// Defines whomst the local player can spectate
        /// </summary>
        public enum Spectateable { None = 0, Friendlies = 1, All = 2 }

        /// <summary>
        /// Impements a manager to spectate other players
        /// </summary>
        public abstract class Kit_SpectatorManagerBase : ScriptableObject
        {
            /// <summary>
            /// Setup
            /// </summary>
            /// <param name="main"></param>
            public abstract void Setup();

            /// <summary>
            /// Can we spectate at all (global setting)?
            /// </summary>
            /// <param name="main"></param>
            /// <returns></returns>
            public abstract bool IsSpectatingEnabled();

            /// <summary>
            /// Start spectating
            /// </summary>
            /// <param name="main"></param>
            public abstract void BeginSpectating(bool leaveTeam);

            /// <summary>
            /// End our spectating
            /// </summary>
            /// <param name="main"></param>
            public abstract void EndSpectating();

            /// <summary>
            /// Called when a player was spawned
            /// </summary>
            /// <param name="main"></param>
            /// <param name="pb"></param>
            public abstract void PlayerWasSpawned(Kit_PlayerBehaviour pb);

            /// <summary>
            /// Called when a player was killed
            /// </summary>
            /// <param name="main"></param>
            /// <param name="pb"></param>
            public abstract void PlayerWasKilled(Kit_PlayerBehaviour pb);

            /// <summary>
            /// True if spectator mode is currently active
            /// </summary>
            /// <param name="main"></param>
            /// <returns></returns>
            public abstract bool IsCurrentlySpectating();
        }
    }
}