using Mirror;
using System.Linq;
using UnityEngine;

namespace MarsFPSKit
{
    namespace UI
    {
        public abstract class Kit_MenuPveGameModeBase : NetworkBehaviour
        {
            /// <summary>
            /// The loadout submenus
            /// </summary>
            [Header("Menu")]
            public UI.MenuScreen[] menuScreens;
            /// <summary>
            /// Value at which the concat started
            /// </summary>
            private int concatStart;
            [HideInInspector]
            /// <summary>
            /// Assigned in initialize
            /// </summary>
            public Kit_MenuManager menuManager;

            /// <summary>
            /// 0 = SP, 1 = COOP
            /// </summary>
            protected int myCurrentState;
            [SyncVar]
            /// <summary>
            /// My game mode id
            /// </summary>
            protected int myId;

            /// <summary>
            /// Sets up the menu
            /// </summary>
            /// <param name="mm"></param>
            /// <param name="state">0 = sp, 1 = coop</param>
            public virtual void SetupMenu(Kit_MenuManager mm, int state, int id)
            {
                //Cache
                myCurrentState = state;
                myId = id;
                menuManager = mm;

                if (menuScreens.Length > 0)
                {
                    menuManager = FindObjectOfType<Kit_MenuManager>();

                    if (menuManager)
                    {
                        concatStart = menuManager.menuScreens.Length;
                        //Merge the lists
                        menuManager.menuScreens = menuManager.menuScreens.Concat(menuScreens).ToArray();
                    }
                }

                //Disable all the roots
                for (int i = 0; i < menuScreens.Length; i++)
                {
                    if (menuScreens[i].root)
                    {
                        //Disable
                        menuScreens[i].root.SetActive(false);
                    }
                    else
                    {
                        Debug.LogError("[PvE Menu] Menu root at index " + i + " is not assigned.", this);
                    }
                }
            }

            #region Menu Calls
            /// <summary>
            /// Call for buttons
            /// </summary>
            /// <param name="newMenu"></param>
            public void ChangeMenuButton(int newMenu)
            {
                if (menuManager)
                {
                    menuManager.ChangeMenuButton(concatStart + newMenu);
                }
            }

            public void BackToMainMenu()
            {
                if (myCurrentState == 0)
                {
                    //Go back to sp menu
                    menuManager.ChangeMenuButton(menuManager.singleplayer.singleplayerScreenId);
                }
                else if (myCurrentState == 1)
                {
                    //Go back to coop menu
                    menuManager.ChangeMenuButton(menuManager.coop.coopScreenId);
                }
            }
            #endregion

            /// <summary>
            /// Called by the button in the menu to open this menu
            /// </summary>
            public abstract void OpenMenu();
        }
    }
}