using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    namespace UI
    {
        public class Kit_MenuMainScreen : MonoBehaviour
        {
            /// <summary>
            /// Menu manager reference
            /// </summary>
            public Kit_MenuManager menuManager;

            /// <summary>
            /// Button to play singleplayer
            /// </summary>
            public Button singleplayerButton;
            /// <summary>
            /// Button to play coop
            /// </summary>
            public Button coopButton;
            /// <summary>
            /// Button to play multiplayer
            /// </summary>
            public Button multiplayerButton;
            /// <summary>
            /// Menu ID for multiplayer
            /// </summary>
            public int multiplayerMenu;

            private void Start()
            {
                //Enable/Disable based on assigned game modes
                singleplayerButton.gameObject.SetActive(menuManager.game.allSingleplayerGameModes.Length > 0 && menuManager.singleplayer);
                coopButton.gameObject.SetActive(menuManager.game.allCoopGameModes.Length > 0 && menuManager.coop);
                multiplayerButton.gameObject.SetActive(menuManager.game.allPvpGameModes.Length > 0);

                //Create Callback
                singleplayerButton.onClick.AddListener(delegate { PlaySingleplayer(); });
                coopButton.onClick.AddListener(delegate { PlayCoop(); });
                multiplayerButton.onClick.AddListener(delegate { PlayMultiplayer(); });
            }

            public void PlaySingleplayer()
            {
                menuManager.SwitchMenu(menuManager.singleplayer.singleplayerScreenId);
            }

            public void PlayCoop()
            {
                menuManager.SwitchMenu(menuManager.coop.coopScreenId);
            }

            public void PlayMultiplayer()
            {
                menuManager.SwitchMenu(multiplayerMenu);
            }

            public void Exit()
            {
                if (menuManager.exitScreen)
                {
                    menuManager.SwitchMenu(menuManager.exitScreen.exitScreenId);
                }
            }

            //MIRROR: Maybe later
            /*
            public void Friends()
            {
                if (menuManager.friends)
                {
                    //Callback
                    menuManager.friends.BeforeOpening();
                    //Switch menu
                    menuManager.SwitchMenu(menuManager.friends.friendsScreenId);
                }
            }
            */

            public void Options()
            {
                if (menuManager.options)
                {
                    //Switch menu
                    menuManager.SwitchMenu(menuManager.options.optionsScreenId);
                }
            }

            public void Loadout()
            {
                if (menuManager.loadout)
                {
                    //Switch menu
                    menuManager.loadout.Open();
                }
            }
        }
    }
}
