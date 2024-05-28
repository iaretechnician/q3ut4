
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace MarsFPSKit
{
    namespace UI
    {
        [System.Serializable]
        public class MenuScreen
        {
            /// <summary>
            /// Root object of this menu
            /// </summary>
            public GameObject root;
            /// <summary>
            /// Animator
            /// </summary>
            public Animator anim;
            /// <summary>
            /// How long does the fade in take?
            /// </summary>
            public float fadeInLength = 0.5f;
            /// <summary>
            /// How long does the fade out take?
            /// </summary>
            public float fadeOutLength = 0.5f;
        }

        /// <summary>
        /// This script manages menu screens
        /// </summary>
        public class Kit_MenuManager : MonoBehaviour
        {
            /// <summary>
            /// Access to the game
            /// </summary>
            public Kit_GameInformation game;

            /// <summary>
            /// Menu screens
            /// </summary>
            public MenuScreen[] menuScreens;

            /// <summary>
            /// Event system we use
            /// </summary>
            public EventSystem eventSystem;

            [Header("Modules")]
            /// <summary>
            /// Login script
            /// </summary>
            public Kit_MenuLoginBase login;

            /// <summary>
            /// Host screen
            /// </summary>
            public Kit_MenuHostScreen hostScreen;

            /// <summary>
            /// Singleplayer screen
            /// </summary>
            public Kit_MenuSingleplayer singleplayer;

            /// <summary>
            /// Coop screen
            /// </summary>
            public Kit_MenuCoop coop;

            /// <summary>
            /// Lobby manager
            /// </summary>
            //public Kit_LobbyManager lobbyManager;

            /// <summary>
            /// Exit screen (question)
            /// </summary>
            public Kit_MenuExitScreen exitScreen;

            /// <summary>
            /// Friends screen
            /// </summary>
            //public Kit_MenuFriendsBase friends;

            /// <summary>
            /// Options
            /// </summary>
            public Kit_MenuOptions options;

            /// <summary>
            /// Player state
            /// </summary>
            public Kit_MenuPlayerStateBase playerState;

            /// <summary>
            /// Loadout
            /// </summary>
            public Kit_LoadoutBase loadout;

            /// <summary>
            /// The main screen ID
            /// </summary>
            [Header("Settings")]
            public int mainScreen = 1;

            /// <summary>
            /// Do we have a screen to fade out?
            /// </summary>
            private bool wasFirstScreenFadedIn;
            [HideInInspector]
            /// <summary>
            /// The menu screen that is currently visible (in order to fade it out)
            /// </summary>
            public int currentScreen;
            /// <summary>
            /// True if we are currently switching a screen
            /// </summary>
            private bool isSwitchingScreens;
            /// <summary>
            /// Where we are currently switching screens to
            /// </summary>
            private Coroutine currentlySwitchingScreensTo;
            /// <summary>
            /// This is called after we log in
            /// </summary>
            public UnityEvent onLogin;
            /// <summary>
            /// Was leveling initialized once?
            /// </summary>
            private static bool wasLevelingInizialized;
            /// <summary>
            /// Did we login yet
            /// </summary>
            private static bool wasLoggedIn;

            private void Awake()
            {
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
                        Debug.LogError("Menu root at index " + i + " is not assigned.", this);
                    }
                }
            }

            private void Start()
            {
#if !UNITY_SERVER
                //Make sure cursor is unlocked
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                //Call to login script
                login.Initialize(this);
#endif
            }

            /// <summary>
            /// Called by LoginBase
            /// </summary>
            /// <param name="username"></param>
            public void LoggedIn(string username)
            {
                if (!wasLoggedIn)
                {
                    wasLoggedIn = true;

                    //Callback
                    if (login)
                    {
                        login.AfterLogin(this);
                    }

                    //MIRROR: Maybe later
                    /*
                    if (friends)
                    {
                        friends.AfterLogin();
                    }
                    */

                    if (game.statistics)
                    {
                        game.statistics.OnStart(this);
                    }

                    if (!wasLevelingInizialized)
                    {
                        if (game.leveling)
                        {
                            game.leveling.Initialize(this);
                        }

                        wasLevelingInizialized = true;
                    }

                    if (playerState)
                    {
                        playerState.Initialize(this);
                    }

                    if (loadout)
                    {
                        loadout.Initialize();
                    }

                    if (game.masterServer)
                    {
                        game.masterServer.Initialize();
                    }

                    //Initialize transport. This has to be assigned!
                    game.transport.Initialize(Kit_NetworkManager.instance);

                    onLogin.Invoke();
                    //MIRROR: Maybe later
                    /*
                    if (!lobbyManager || !lobbyManager.isInLobby)
                    {
                        //Switch to main screen
                        SwitchMenu(mainScreen);
                    }
                    */
                    //Switch to main screen
                    SwitchMenu(mainScreen);
                }
                else
                {
                    wasLoggedIn = true;

                    //Callback
                    if (login)
                    {
                        login.AfterLogin(this);
                    }

                    //MIRROR: Maybe later
                    /*
                    if (friends)
                    {
                        friends.AfterLogin();
                    }
                    */

                    if (game.statistics)
                    {
                        game.statistics.OnStart(this);
                    }

                    if (!wasLevelingInizialized)
                    {
                        if (game.leveling)
                        {
                            game.leveling.Initialize(this);
                        }

                        wasLevelingInizialized = true;
                    }

                    if (playerState)
                    {
                        playerState.Initialize(this);
                    }

                    if (loadout)
                    {
                        loadout.Initialize();
                    }

                    onLogin.Invoke();

                    //MIRROR: Maybe later
                    /*
                    if (!lobbyManager || !lobbyManager.isInLobby)
                    {
                        //Switch to main screen
                        SwitchMenu(mainScreen);
                    }
                    */

                    //Switch to main screen
                    SwitchMenu(mainScreen);
                }
            }

            /// <summary>
            /// Logs us out
            /// </summary>
            public void LoggedOut()
            {
                Kit_GameSettings.userName = "Unassigned";
                wasLoggedIn = false;
            }

            /// <summary>
            /// Call for buttons
            /// </summary>
            /// <param name="newMenu"></param>
            public void ChangeMenuButton(int newMenu)
            {
                if (!isSwitchingScreens)
                {
                    //Start the coroutine
                    StartCoroutine(SwitchRoutine(newMenu));
                }
            }

            /// <summary>
            /// Switch to the given menu
            /// </summary>
            /// <param name="newMenu"></param>
            /// <returns></returns>
            public bool SwitchMenu(int newMenu)
            {
                if (!isSwitchingScreens)
                {
                    //Start the coroutine
                    currentlySwitchingScreensTo = StartCoroutine(SwitchRoutine(newMenu));
                    //We are now switching
                    return true;
                }

                //Not able to switch screens
                return false;
            }

            /// <summary>
            /// Switch to the given menu
            /// </summary>
            /// <param name="newMenu"></param>
            /// <returns></returns>
            public bool SwitchMenu(int newMenu, bool force)
            {
                if (!isSwitchingScreens || force)
                {
                    if (force)
                    {
                        if (currentlySwitchingScreensTo != null)
                        {
                            StopCoroutine(currentlySwitchingScreensTo);
                        }

                        //Make sure all correct ones ARE disabled
                        //Disable all the roots
                        for (int i = 0; i < menuScreens.Length; i++)
                        {
                            if (i != currentScreen)
                            {
                                if (menuScreens[i].root)
                                {
                                    //Disable
                                    menuScreens[i].root.SetActive(false);
                                }
                            }
                        }
                    }

                    //Start the coroutine
                    currentlySwitchingScreensTo = StartCoroutine(SwitchRoutine(newMenu));
                    //We are now switching
                    return true;
                }

                //Not able to switch screens
                return false;
            }

            public void ForceMenuActive(int menu)
            {
                //Disable all the roots
                for (int i = 0; i < menuScreens.Length; i++)
                {
                    if (i != menu)
                    {
                        if (menuScreens[i].root)
                        {
                            //Disable
                            menuScreens[i].root.SetActive(false);
                        }
                    }
                    else
                    {
                        if (menuScreens[i].root)
                        {
                            //Disable
                            menuScreens[i].root.SetActive(true);
                        }
                    }
                }
            }

            private IEnumerator SwitchRoutine(int newMenu)
            {
                //Set bool
                isSwitchingScreens = true;
                if (wasFirstScreenFadedIn)
                {
                    //Fade out screen
                    //Play Animation
                    menuScreens[currentScreen].anim.Play("Fade Out", 0, 0f);
                    //Wait
                    yield return new WaitForSeconds(menuScreens[currentScreen].fadeOutLength);
                    menuScreens[currentScreen].root.SetActive(false);
                }

                //Fade in new screen
                //Set screen
                currentScreen = newMenu;
                //Disable
                menuScreens[currentScreen].root.SetActive(true);
                //Play Animation
                menuScreens[currentScreen].anim.Play("Fade In", 0, 0f);
                //Wait
                yield return new WaitForSeconds(menuScreens[currentScreen].fadeInLength);
                //Set bool
                wasFirstScreenFadedIn = true;
                //Done
                isSwitchingScreens = false;
            }

            private void OnApplicationQuit()
            {
                if (game.leveling)
                {
                    game.leveling.Save();
                }

                if (game.statistics)
                {
                    game.statistics.Save(this);
                }
            }
        }
    }
}