using UnityEngine;

namespace MarsFPSKit
{
    namespace UI
    {
        /// <summary>
        /// Handles login
        /// </summary>
        public abstract class Kit_MenuLoginBase : MonoBehaviour
        {
            /// <summary>
            /// Login screen in <see cref="Kit_MenuManager.menuScreens"/>
            /// </summary>
            public int loginScreen;

            /// <summary>
            /// Initialize the system and if already logged in, proceed
            /// </summary>
            /// <param name="mm"></param>
            public abstract void Initialize(Kit_MenuManager mm);

            /// <summary>
            /// Do something after our login was registered by the kit
            /// </summary>
            /// <param name="mm"></param>
            public virtual void AfterLogin(Kit_MenuManager mm)
            {

            }

            public virtual void OnConnectedToMaster(Kit_MenuManager mm)
            {

            }
        }
    }
}