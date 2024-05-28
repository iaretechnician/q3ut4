using MarsFPSKit.Networking;
using UnityEngine;

namespace MarsFPSKit
{
    namespace Services
    {
        public class GameInfo
        {
            /// <summary>
            /// Name of this server
            /// </summary>
            public string name;
            /// <summary>
            /// Game mode this server is playing
            /// </summary>
            public int gameMode;
            /// <summary>
            /// Game mode type this server is playing
            /// </summary>
            public int gameModeType;
            /// <summary>
            /// Map this server is playing
            /// </summary>
            public int map;
            /// <summary>
            /// How many players currently in there?
            /// </summary>
            public int players;
            /// <summary>
            /// How many players at most?
            /// </summary>
            public int maxPlayers;
            /// <summary>
            /// Ping to this game
            /// </summary>
            public int ping;
            /// <summary>
            /// Bots
            /// </summary>
            public bool bots;
            /// <summary>
            /// Password
            /// </summary>
            public bool password;
            /// <summary>
            /// Password
            /// </summary>
            public bool dedicated;
            /// <summary>
            /// Connection
            /// </summary>
            public string connection;
        }

        /// <summary>
        /// This is the base class for the service that we want to use as a master server to register our game sessions
        /// </summary>
        public abstract class Kit_MasterServerBase : ScriptableObject
        {
            /// <summary>
            /// Called after login
            /// </summary>
            public abstract void Initialize();

            /// <summary>
            /// Local server was started
            /// </summary>
            public abstract void GameServerStarted(Kit_NetworkGameInformation gameInfo);

            /// <summary>
            /// Heartbeat our current entry
            /// </summary>
            /// <param name="gameInfo"></param>
            public abstract void Heartbeat(Kit_NetworkGameInformation gameInfo);

            /// <summary>
            /// Our local server was exited
            /// </summary>
            public abstract void GameServerExited(Kit_NetworkGameInformation gameInfo);

            /// <summary>
            /// Called when something has changed
            /// </summary>
            /// <param name="gameInfo"></param>
            public abstract void GameServerInfoUpdated(Kit_NetworkGameInformation gameInfo);

            /// <summary>
            /// Get all currently available games
            /// </summary>
            /// <returns></returns>
            public abstract GameInfo[] GetGames();

            /// <summary>
            /// Refresh the infos
            /// </summary>
            public abstract void RefreshGames();
        }
    }
}