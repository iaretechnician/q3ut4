using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;

namespace MarsFPSKit
{
    namespace Networking
    {
        /// <summary>
        /// This networked game object holds information about the currently ongoing game.
        /// </summary>
        public class Kit_NetworkGameInformation : NetworkBehaviour
        {
            /// <summary>
            /// Active instance
            /// </summary>
            public static Kit_NetworkGameInformation instance;

            [SyncVar(hook = "OnGameNameChanged")]
            [Tooltip("Visible name of this game")]
            /// <summary>
            /// Visible name of this game
            /// </summary>
            public string gameName;
            [SyncVar(hook = "OnPlayerLimitChanged")]
            [Tooltip("How many players are allowed in this game?")]
            /// <summary>
            /// How many players are allowed in this game?
            /// </summary>
            public int playerLimit;
            [SyncVar(hook = "OnPlayersNeededChanged")]
            [Tooltip("How many players are needed to start the game?")]
            /// <summary>
            /// How many players are needed to start the game?
            /// </summary>
            public int playersNeeded;
            [SyncVar(hook = "OnMapChanged")]
            [Tooltip("Map in the game mode's array")]
            /// <summary>
            /// Map in the game mode's array
            /// </summary>
            public int map;
            [SyncVar(hook = "OnGameModeTypeChanged")]
            [Tooltip("Game Mode Type we are currently playing")]
            /// <summary>
            /// Game Mode Type we are currently playing
            /// </summary>
            public int gameModeType;
            [SyncVar(hook = "OnGameModeChanged")]
            [Tooltip("Game Mode we are currently playing")]
            /// <summary>
            /// Game Mode we are currently playing
            /// </summary>
            public int gameMode;
            [SyncVar(hook = "OnDurationChanged")]
            [Tooltip("Active game mode duration limit")]
            /// <summary>
            /// Active game mode duration limit
            /// </summary>
            public int duration;
            [SyncVar(hook = "OnPingChanged")]
            [Tooltip("Active Ping limit")]
            /// <summary>
            /// Active Ping limit
            /// </summary>
            public int ping;
            [SyncVar(hook = "OnAfkChanged")]
            [Tooltip("Active AFK limit")]
            /// <summary>
            /// Active AFK limit
            /// </summary>
            public int afk;
            [SyncVar(hook = "OnBotsChanged")]
            [Tooltip("Are bots enabled?")]
            /// <summary>
            /// Are bots enabled?
            /// </summary>
            public bool bots;
            [SyncVar(hook = "OnPasswordChanged")]
            [Tooltip("Password of the session")]
            /// <summary>
            /// Password of the session
            /// </summary>
            public string password;
            [SyncVar(hook = "OnConnectionStringChanged")]
            [Tooltip("Connection string of the session")]
            /// <summary>
            /// Connection string
            /// </summary>
            public string connectionString;
            [SyncVar(hook = "OnLobbyChanged")]
            [Tooltip("Is this game mode a lobby or traditional one?")]
            /// <summary>
            /// Is this game mode a lobby or traditional one?
            /// </summary>
            public bool isLobby;
            [SyncVar(hook = "OnDedicatedServerChanged")]
            [Tooltip("Is this game mode a lobby or traditional one?")]
            /// <summary>
            /// Is this host a dedicated (true) or listen server?
            /// </summary>
            public bool isDedicatedServer;

            #region Change Hooks
            public void OnGameNameChanged(string was, string isNow)
            {
                if (Kit_NetworkManager.instance.game.masterServer && !Kit_NetworkManager.instance.isOfflineMode)
                {
                    Kit_NetworkManager.instance.game.masterServer.GameServerInfoUpdated(this);
                }
            }

            public void OnPlayerLimitChanged(int was, int isNow)
            {
                if (Kit_NetworkManager.instance.game.masterServer && !Kit_NetworkManager.instance.isOfflineMode)
                {
                    Kit_NetworkManager.instance.game.masterServer.GameServerInfoUpdated(this);
                }
            }

            public void OnPlayersNeededChanged(int was, int isNow)
            {
                if (Kit_NetworkManager.instance.game.masterServer && !Kit_NetworkManager.instance.isOfflineMode)
                {
                    Kit_NetworkManager.instance.game.masterServer.GameServerInfoUpdated(this);
                }
            }

            public void OnMapChanged(int was, int isNow)
            {
                if (Kit_NetworkManager.instance.game.masterServer && !Kit_NetworkManager.instance.isOfflineMode)
                {
                    Kit_NetworkManager.instance.game.masterServer.GameServerInfoUpdated(this);
                }
            }

            public void OnGameModeTypeChanged(int was, int isNow)
            {
                if (Kit_NetworkManager.instance.game.masterServer && !Kit_NetworkManager.instance.isOfflineMode)
                {
                    Kit_NetworkManager.instance.game.masterServer.GameServerInfoUpdated(this);
                }
            }

            public void OnGameModeChanged(int was, int isNow)
            {
                if (Kit_NetworkManager.instance.game.masterServer && !Kit_NetworkManager.instance.isOfflineMode)
                {
                    Kit_NetworkManager.instance.game.masterServer.GameServerInfoUpdated(this);
                }
            }

            public void OnDurationChanged(int was, int isNow)
            {
                if (Kit_NetworkManager.instance.game.masterServer && !Kit_NetworkManager.instance.isOfflineMode)
                {
                    Kit_NetworkManager.instance.game.masterServer.GameServerInfoUpdated(this);
                }
            }

            //These hooks just update the info on the master server, nothing else.
            public void OnPingChanged(int was, int isNow)
            {
                if (Kit_NetworkManager.instance.game.masterServer && !Kit_NetworkManager.instance.isOfflineMode)
                {
                    Kit_NetworkManager.instance.game.masterServer.GameServerInfoUpdated(this);
                }
            }

            public void OnAfkChanged(int was, int isNow)
            {
                if (Kit_NetworkManager.instance.game.masterServer && !Kit_NetworkManager.instance.isOfflineMode)
                {
                    Kit_NetworkManager.instance.game.masterServer.GameServerInfoUpdated(this);
                }
            }

            public void OnBotsChanged(bool was, bool isNow)
            {
                if (Kit_NetworkManager.instance.game.masterServer && !Kit_NetworkManager.instance.isOfflineMode)
                {
                    Kit_NetworkManager.instance.game.masterServer.GameServerInfoUpdated(this);
                }
            }

            public void OnPasswordChanged(string was, string isNow)
            {
                if (Kit_NetworkManager.instance.game.masterServer && !Kit_NetworkManager.instance.isOfflineMode)
                {
                    Kit_NetworkManager.instance.game.masterServer.GameServerInfoUpdated(this);
                }
            }

            public void OnConnectionStringChanged(string was, string isNow)
            {
                if (Kit_NetworkManager.instance.game.masterServer && !Kit_NetworkManager.instance.isOfflineMode)
                {
                    Kit_NetworkManager.instance.game.masterServer.GameServerInfoUpdated(this);
                }
            }

            public void OnLobbyChanged(bool was, bool isNow)
            {
                if (Kit_NetworkManager.instance.game.masterServer && !Kit_NetworkManager.instance.isOfflineMode)
                {
                    Kit_NetworkManager.instance.game.masterServer.GameServerInfoUpdated(this);
                }
            }

            public void OnDedicatedServerChanged(bool was, bool isNow)
            {
                if (Kit_NetworkManager.instance.game.masterServer && !Kit_NetworkManager.instance.isOfflineMode)
                {
                    Kit_NetworkManager.instance.game.masterServer.GameServerInfoUpdated(this);
                }
            }
            #endregion

            #region Unity Calls
            private void OnEnable()
            {
                DontDestroyOnLoad(gameObject);
                instance = this;
            }

            private void OnDestroy()
            {
                if (Kit_NetworkManager.instance && Kit_NetworkManager.instance.game.masterServer && !Kit_NetworkManager.instance.isOfflineMode)
                {
                    Kit_NetworkManager.instance.game.masterServer.GameServerExited(this);
                }
            }
            #endregion

            #region Mirror Calls
            public override void OnStartServer()
            {
                if (Kit_NetworkManager.instance.game.masterServer && !Kit_NetworkManager.instance.isOfflineMode)
                {
                    Kit_NetworkManager.instance.game.masterServer.GameServerStarted(this);
                    StartCoroutine(Heartbeat());
                }
            }

            public override void OnStartClient()
            {
                //At this point, everything should be ready.
                Kit_NetworkManager.instance.onConnectedToServer.Invoke();
            }
            #endregion

            #region Heartbeat
            IEnumerator Heartbeat()
            {
                while (true)
                {
                    Kit_NetworkManager.instance.game.masterServer.Heartbeat(this);
                    yield return new WaitForSeconds(5f);
                }
            }
            #endregion
        }
    }
}