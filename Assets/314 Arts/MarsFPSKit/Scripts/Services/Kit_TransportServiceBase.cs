using UnityEngine;

namespace MarsFPSKit
{
    namespace Services
    {
        public abstract class Kit_TransportServiceBase : ScriptableObject
        {
            /// <summary>
            /// Initialize
            /// </summary>
            /// <param name="manager"></param>
            public abstract void Initialize(Kit_NetworkManager manager);

            /// <summary>
            /// Host a game
            /// </summary>
            /// <param name="manager"></param>
            public abstract void StartHost(Kit_NetworkManager manager);

            /// <summary>
            /// Start a server only
            /// </summary>
            /// <param name="manager"></param>
            public abstract void StartServer(Kit_NetworkManager manager);

            /// <summary>
            /// Get the currently active connection string
            /// </summary>
            /// <returns></returns>
            public abstract string GetConnectionString(Kit_NetworkManager manager);

            /// <summary>
            /// Connect with the given string
            /// </summary>
            /// <param name="connectionString"></param>
            public abstract void ConnectWithString(Kit_NetworkManager manager, string connectionString);

            /// <summary>
            /// Connect with the given string
            /// </summary>
            /// <param name="connectionString"></param>
            public virtual void ConnectWithStringAndPassword(Kit_NetworkManager manager, string connectionString, string password)
            {
                if (Kit_NetworkAuthenticator.instance)
                    Kit_NetworkAuthenticator.instance.passwordForAuth = password;
                else
                    Debug.LogWarning("No authenticator instance found!");
            }
        }
    }
}