using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using MarsFPSKit.Networking;

namespace MarsFPSKit
{
    public class Kit_NetworkAuthenticator : NetworkAuthenticator
    {
        /// <summary>
        /// The last player id that was used
        /// </summary>
        private uint nextPlayerId;
        [HideInInspector]
        /// <summary>
        /// Set password that is sent at spawn
        /// </summary>
        public string passwordForAuth;

        public static Kit_NetworkAuthenticator instance;

        private void Start()
        {
            instance = this;
        }

        #region Messages

        /// <summary>
        /// This default authenticator is basically just used to transmit our username
        /// </summary>
        public struct KitAuthRequestMessage : NetworkMessage
        {
            public string userName;
            public string password;
        }

        public struct KitAuthResponseMessage : NetworkMessage
        {

        }

        #endregion

        #region Server

        /// <summary>
        /// Called on server from StartServer to initialize the Authenticator
        /// <para>Server message handlers should be registered in this method.</para>
        /// </summary>
        public override void OnStartServer()
        {
            // register a handler for the authentication request we expect from client
            NetworkServer.RegisterHandler<KitAuthRequestMessage>(OnAuthRequestMessage, false);
        }

        /// <summary>
        /// Called on server from StopServer to reset the Authenticator
        /// <para>Server message handlers should be registered in this method.</para>
        /// </summary>
        public override void OnStopServer()
        {
            // unregister the handler for the authentication request
            NetworkServer.UnregisterHandler<KitAuthRequestMessage>();
        }

        /// <summary>
        /// Called on server from OnServerConnectInternal when a client needs to authenticate
        /// </summary>
        /// <param name="conn">Connection to client.</param>
        public override void OnServerAuthenticate(NetworkConnectionToClient conn)
        {
            // do nothing...wait for AuthRequestMessage from client
        }

        /// <summary>
        /// Called on server when the client's AuthRequestMessage arrives
        /// </summary>
        /// <param name="conn">Connection to client.</param>
        /// <param name="msg">The message payload</param>
        public void OnAuthRequestMessage(NetworkConnectionToClient conn, KitAuthRequestMessage msg)
        {
            if (Kit_NetworkGameInformation.instance && Kit_NetworkGameInformation.instance.password != "")
            {
                if (msg.password != Kit_NetworkGameInformation.instance.password)
                {
                    ServerReject(conn);
                    return;
                }
            }

            Kit_Player joinedPlayer = new Kit_Player();
            joinedPlayer.team = -1;
            joinedPlayer.id = nextPlayerId;
            joinedPlayer.serverToClientConnection = conn;
            joinedPlayer.name = msg.userName;
            Kit_NetworkPlayerManager.instance.players.Add(joinedPlayer);

            //Increase player id for the next player
            nextPlayerId++;

            ServerAccept(conn);

            // create and send msg to client so it knows to proceed
            KitAuthResponseMessage authResponseMessage = new KitAuthResponseMessage
            {

            };

            conn.Send(authResponseMessage);
        }

        #endregion

        #region Client

        /// <summary>
        /// Called on client from StartClient to initialize the Authenticator
        /// <para>Client message handlers should be registered in this method.</para>
        /// </summary>
        public override void OnStartClient()
        {
            // register a handler for the authentication response we expect from server
            NetworkClient.RegisterHandler<KitAuthResponseMessage>(OnAuthResponseMessage, false);
        }

        /// <summary>
        /// Called on client from StopClient to reset the Authenticator
        /// <para>Client message handlers should be unregistered in this method.</para>
        /// </summary>
        public override void OnStopClient()
        {
            // unregister the handler for the authentication response
            NetworkClient.UnregisterHandler<KitAuthResponseMessage>();
        }

        /// <summary>
        /// Called on client from OnClientConnectInternal when a client needs to authenticate
        /// </summary>
        public override void OnClientAuthenticate()
        {
            KitAuthRequestMessage authRequestMessage = new KitAuthRequestMessage
            {
                userName = Kit_GameSettings.userName,
                password = passwordForAuth
            };

            NetworkClient.connection.Send(authRequestMessage);
        }

        /// <summary>
        /// Called on client when the server's AuthResponseMessage arrives
        /// </summary>
        /// <param name="msg">The message payload</param>
        public void OnAuthResponseMessage(KitAuthResponseMessage msg)
        {
            ClientAccept();
            if (!NetworkClient.ready)
                NetworkClient.Ready();
            if (!NetworkClient.localPlayer)
                NetworkClient.AddPlayer();
        }

        #endregion
    }
}
