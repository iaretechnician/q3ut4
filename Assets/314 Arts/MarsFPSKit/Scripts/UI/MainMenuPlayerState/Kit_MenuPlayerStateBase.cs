using UnityEngine;

namespace MarsFPSKit
{
    namespace UI
    {
        /// <summary>
        /// Use this to implement your own menu player state
        /// </summary>
        public abstract class Kit_MenuPlayerStateBase : MonoBehaviour
        {
            /// <summary>
            /// Called once the player is logged in
            /// </summary>
            /// <param name="main"></param>
            public abstract void Initialize(Kit_MenuManager main);
        }
    }
}