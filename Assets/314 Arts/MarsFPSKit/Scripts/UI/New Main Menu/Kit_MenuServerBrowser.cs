using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Services.Authentication;
using Unity.Services.Core;
using MarsFPSKit.Services;

namespace MarsFPSKit
{
    namespace UI
    {
        /// <summary>
        /// This implements a server browser that is based on UGS (Unity Game Services) Lobbies service.
        /// </summary>
        public class Kit_MenuServerBrowser : MonoBehaviour
        {
            /// <summary>
            /// Menu Manager
            /// </summary>
            public Kit_MenuManager menuManager;

            /// <summary>
            /// The "Content" object of the Scroll view, where entriesPrefab will be instantiated
            /// </summary>
            public RectTransform entriesGo;
            /// <summary>
            /// The Server Browser Entry prefab
            /// </summary>
            public GameObject entriesPrefab;
            /// <summary>
            /// Currently active server browser entries - used for cleanup
            /// </summary>
            private List<Kit_ServerBrowserEntry> activeEntries = new List<Kit_ServerBrowserEntry>();

            #region Password
            [Header("Password")]
            /// <summary>
            /// The room we are currently trying to join
            /// </summary>
            public GameInfo passwordRoom;
            /// <summary>
            /// Root for password
            /// </summary>
            public GameObject passwordUi;
            /// <summary>
            /// The password to compare
            /// </summary>
            public TMP_InputField passwordInput;
            /// <summary>
            /// Are we currently entering a password?
            /// </summary>
            private bool isPasswordActive;
            #endregion

            //This section includes everything needed for the error message window
            #region Error Message
            [Header("Error Message")]
            /// <summary>
            /// The root object of the error message.
            /// </summary>
            public GameObject em_root;
            /// <summary>
            /// The text object that will hold the error details
            /// </summary>
            public TextMeshProUGUI em_text;
            /// <summary>
            /// The "ok" button of the Error Mesesage.
            /// </summary>
            public Button em_button;
            #endregion

            /// <summary>
            /// Called by button
            /// </summary>
            public void RequestRefreshGames()
            {
                if (menuManager.game.masterServer)
                {
                    menuManager.game.masterServer.RefreshGames();
                }
            }

            /// <summary>
            /// Redraw the list
            /// </summary>
            public void RedrawGames()
            {
                if (menuManager.game.masterServer)
                {
                    GameInfo[] infos = menuManager.game.masterServer.GetGames();

                    int currentIndex = 0;

                    for (int i = 0; i < infos.Length; i++)
                    {
                        if (infos[i].gameModeType == 2)
                        {
                            int id = i;

                            if (currentIndex + 1 > activeEntries.Count)
                            {
                                GameObject go = Instantiate(entriesPrefab, entriesGo, false);
                                Kit_ServerBrowserEntry entry = go.GetComponent<Kit_ServerBrowserEntry>();
                                activeEntries.Add(entry);
                            }

                            activeEntries[currentIndex].gameObject.SetActiveOptimized(true);
                            activeEntries[currentIndex].Setup(this, infos[id]);
                            currentIndex++;
                        }
                    }

                    for (int i = currentIndex + 1; i < activeEntries.Count; i++)
                    {
                        activeEntries[i].gameObject.SetActiveOptimized(false);
                    }
                }
            }

            
            /// <summary>
            /// Attempts to join a room
            /// <para>See also: <seealso cref="Kit_ServerBrowserEntry"/></para>
            /// </summary>
            /// <param name="room"></param>
            public void JoinRoom(GameInfo room)
            {
                //Join directly when there is no password
                if (!room.password)
                {
                    Kit_NetworkManager.instance.JoinGame(room);
                }
                else
                {
                    //Ask for password.
                    //Set room
                    passwordRoom = room;
                    //Reset input
                    passwordInput.text = "";
                    //Open
                    passwordUi.SetActive(true);
                }
            }

            public void PasswordJoin()
            {
                Kit_NetworkManager.instance.JoinGame(passwordRoom, passwordInput.text);
            }

            public void PasswordAbort()
            {
                //Close
                passwordUi.SetActive(false);
            }

            public void DisplayErrorMessage(string content)
            {
                //Set text
                em_text.text = content;
                //Show
                em_root.SetActive(true);
                //Select button
                em_button.Select();
            }
        }
    }
}