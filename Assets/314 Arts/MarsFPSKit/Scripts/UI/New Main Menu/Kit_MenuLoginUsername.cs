using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MarsFPSKit
{
    namespace UI
    {
        /// <summary>
        /// Simple login with 
        /// </summary>
        public class Kit_MenuLoginUsername : Kit_MenuLoginBase
        {
            /// <summary>
            /// Our previous username
            /// </summary>
            public static string previousUsername = string.Empty;
            /// <summary>
            /// Username field
            /// </summary>
            public TMP_InputField usernameInput;
            /// <summary>
            /// Assigned in initialize
            /// </summary>
            private Kit_MenuManager menuManager;

            void Start()
            {
                if (PlayerPrefs.HasKey("previousUsername"))
                {
                    usernameInput.text = PlayerPrefs.GetString("previousUsername");
                }
                else
                {
                    usernameInput.text = "Guest (" + Random.Range(1, 999) + ")";
                }
            }

            public override void Initialize(Kit_MenuManager mm)
            {
                //Store
                menuManager = mm;
                //We are already logged in
                if (previousUsername != string.Empty)
                {
                    mm.LoggedIn(previousUsername);
                }
                else
                {
                    //Switch to our menu
                    mm.SwitchMenu(loginScreen);
                }
            }

            /// <summary>
            /// Called when pressing the button
            /// </summary>
            public void LoginButton()
            {
                //Check if name was entered
                if (usernameInput.text.Trim() != "")
                {
                    //Enter
                    previousUsername = usernameInput.text;
                    //Set name
                    Kit_GameSettings.userName = previousUsername;
                    //Call
                    menuManager.LoggedIn(usernameInput.text);
                    //Set
                    PlayerPrefs.SetString("previousUsername", usernameInput.text);
                }
            }
        }
    }
}