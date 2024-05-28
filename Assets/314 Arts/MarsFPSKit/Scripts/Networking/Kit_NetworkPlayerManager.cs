using UnityEngine;
using Mirror;
using System.Linq;
using System;
using UnityEngine.Events;

namespace MarsFPSKit
{
    namespace Networking
    {
        public class Kit_NetworkPlayerManager : NetworkBehaviour
        {
            public static Kit_NetworkPlayerManager instance;

            /// <summary>
            /// List of all active players
            /// </summary>
            public readonly SyncList<Kit_Player> players = new SyncList<Kit_Player>();

            /// <summary>
            /// My local ID
            /// </summary>
            public uint myId;

            //This contains everything needed for the Scoreboard
            #region Scoreboard
            [Header("Scoreboard")]
            public float sb_pingUpdateRate = 1f; //After how many seconds the ping in our Customproperties should be updated
            private float sb_lastPingUpdate; //When our ping was updated for the last time
            #endregion

            public UnityEvent<Kit_Player> onPlayerJoined = new UnityEvent<Kit_Player>();
            public UnityEvent<Kit_Player> onPlayerLeft = new UnityEvent<Kit_Player>();


            /// <summary>
            /// This is used to set an entry in the synclist as dirty
            /// </summary>
            private Kit_Player dirtyPlayer = new Kit_Player();

            private void Awake()
            {
                DontDestroyOnLoad(gameObject);
                instance = this;
            }

            private void Start()
            {
                players.Callback += OnPlayersChanged;
            }

            private void OnPlayersChanged(SyncList<Kit_Player>.Operation op, int itemIndex, Kit_Player oldItem, Kit_Player newItem)
            {
                try
                {
                    switch (op)
                    {
                        case SyncList<Kit_Player>.Operation.OP_ADD:
                            //Update master server to show correct player count
                            if (Kit_NetworkGameInformation.instance && Kit_NetworkManager.instance.game.masterServer && !Kit_NetworkManager.instance.isOfflineMode)
                            {
                                Kit_NetworkManager.instance.game.masterServer.GameServerInfoUpdated(Kit_NetworkGameInformation.instance);
                            }
                            //Generic callback
                            onPlayerJoined.Invoke(newItem);
                            break;
                        case SyncList<Kit_Player>.Operation.OP_REMOVEAT:
                            //Update master server to show correct player count
                            if (Kit_NetworkGameInformation.instance && Kit_NetworkManager.instance.game.masterServer && !Kit_NetworkManager.instance.isOfflineMode)
                            {
                                Kit_NetworkManager.instance.game.masterServer.GameServerInfoUpdated(Kit_NetworkGameInformation.instance);
                            }
                            //Generic callback
                            onPlayerLeft.Invoke(oldItem);
                            break;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning("OnPlayersChanged Exception: " + e.ToString());
                }
            }

            private void Update()
            {
                #region Scoreboard ping update
                //Check if we send a new update
                if (Time.time > sb_lastPingUpdate + sb_pingUpdateRate && NetworkClient.ready)
                {
                    //Set last update
                    sb_lastPingUpdate = Time.time;
                    CmdSetPing(NetworkTime.rtt);
                }
                #endregion
            }

            [Command(requiresAuthority = false)]
            public void CmdSetPing(double p, NetworkConnectionToClient sender = null)
            {
                Kit_Player player = GetPlayerByConnection(sender);
                if (sender != null)
                {
                    //Its RTT, so divide by two
                    p /= 2;
                    p *= 1000;
                    player.ping = (ushort)Mathf.RoundToInt((float)p);
                    ModifyPlayerData(player);
                }
            }

            /// <summary>
            /// Gets a player by his connection
            /// </summary>
            /// <param name="conn"></param>
            /// <returns></returns>
            public Kit_Player GetPlayerByConnection(NetworkConnectionToClient conn)
            {
                return players.Where(x => x.serverToClientConnection == conn).FirstOrDefault();
            }

            /// <summary>
            /// Gets a player by his connection
            /// </summary>
            /// <param name="conn"></param>
            /// <returns></returns>
            public Kit_Player GetPlayerById(uint id)
            {
                return players.Where(x => !x.isBot && x.id == id).FirstOrDefault();
            }

            /// <summary>
            /// Gets a player's active player by his connection
            /// </summary>
            /// <param name="conn"></param>
            /// <returns></returns>
            public Kit_PlayerBehaviour GetPlayerBehaviourById(uint id)
            {
                return Kit_IngameMain.instance.allActivePlayers.Where(x => !x.isBot && x.id == id).FirstOrDefault();
            }

            public Kit_Player GetLocalPlayer()
            {
                if (!NetworkClient.active) return null;

                return players.Where(x => !x.isBot && x.id == myId).FirstOrDefault();
            }

            /// <summary>
            /// Every time a player is modified, he has to be re set, otherwise Mirror will not update his data to the other clients in the list.
            /// </summary>
            /// <param name="player"></param>
            public void ModifyPlayerData(Kit_Player player)
            {
                int index = players.IndexOf(player);
                players[index] = dirtyPlayer;
                players[index] = player;
            }
        }
    }
}