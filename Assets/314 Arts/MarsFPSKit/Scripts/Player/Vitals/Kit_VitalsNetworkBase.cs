using Mirror;
using UnityEngine;

namespace MarsFPSKit
{
    public abstract class Kit_VitalsNetworkBase : NetworkBehaviour
    {
        [SyncVar]
        /// <summary>
        /// The network id of the player object that this belongs to
        /// </summary>
        public uint ownerPlayerNetworkId;
        /// <summary>
        /// Our player component
        /// </summary>
        public Kit_PlayerBehaviour pb;

        public override void OnStartServer()
        {
            if (NetworkServer.spawned.ContainsKey(ownerPlayerNetworkId))
            {
                pb = NetworkServer.spawned[ownerPlayerNetworkId].GetComponent<Kit_PlayerBehaviour>();
                pb.vitalsNetworkData = this;
            }
        }

        public override void OnStartClient()
        {
            if (NetworkClient.spawned.ContainsKey(ownerPlayerNetworkId))
            {
                pb = NetworkClient.spawned[ownerPlayerNetworkId].GetComponent<Kit_PlayerBehaviour>();
                pb.vitalsNetworkData = this;
            }
        }
    }
}