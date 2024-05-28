using MarsFPSKit.Networking;
using Mirror;
using UnityEngine;

namespace MarsFPSKit
{
    [System.Serializable]
    public class Kit_Player
    {
        /// <summary>
        /// Is this a reference to a bot or a player?
        /// </summary>
        public bool isBot;
        /// <summary>
        /// Either photon ID or bot ID
        /// </summary>
        public uint id;
        /// <summary>
        /// Assigned team of this player
        /// </summary>
        public sbyte team = -1;
        /// <summary>
        /// Name of this player
        /// </summary>
        public string name;
        /// <summary>
        /// How many kills he has
        /// </summary>
        public ushort kills;
        /// <summary>
        /// How many assists he has
        /// </summary>
        public ushort assists;
        /// <summary>
        /// How many deaths
        /// </summary>
        public ushort deaths;
        /// <summary>
        /// Ping
        /// </summary>
        public ushort ping;
        /// <summary>
        /// This is only assigned on the server.
        /// </summary>
        public NetworkConnectionToClient serverToClientConnection;
        /// <summary>
        /// Is this the local player?
        /// </summary>
        public bool isLocal
        {
            get
            {
                if (Kit_NetworkPlayerManager.instance.myId == id && !isBot) return true;
                return false;
            }
        }

        public override string ToString()
        {
            return name + " ID: " + id + " Bot: " + isBot + " Team: " + team + " Kills: " + kills + " Assists: " + assists + " Deaths: " + deaths + " Ping: " + ping;
        }
    }
}