using Mirror;
using UnityEngine;

namespace MarsFPSKit
{
    public abstract class Kit_WeaponManagerNetworkBase : NetworkBehaviour
    {
        [SyncVar(hook = "OnOwnerPlayerNetworkIdChanged")]
        /// <summary>
        /// The network id of the player object that this belongs to
        /// </summary>
        public uint ownerPlayerNetworkId;

        public Kit_PlayerBehaviour ownerPlayer;

        public override void OnStartServer()
        {
            if (ownerPlayerNetworkId != 0)
            {
                if (NetworkServer.spawned.ContainsKey(ownerPlayerNetworkId))
                {
                    Kit_PlayerBehaviour pb = NetworkServer.spawned[ownerPlayerNetworkId].GetComponent<Kit_PlayerBehaviour>();
                    pb.weaponManagerNetworkData = this;
                    ownerPlayer = pb;
                }
            }
        }

        public override void OnStartClient()
        {
            if (NetworkClient.spawned.ContainsKey(ownerPlayerNetworkId))
            {
                Kit_PlayerBehaviour pb = NetworkClient.spawned[ownerPlayerNetworkId].GetComponent<Kit_PlayerBehaviour>();
                pb.weaponManagerNetworkData = this;
                ownerPlayer = pb;
            }
        }

        public void OnOwnerPlayerNetworkIdChanged(uint was, uint isNow)
        {
            if (NetworkServer.active)
            {
                if (NetworkServer.spawned.ContainsKey(ownerPlayerNetworkId))
                {
                    Kit_PlayerBehaviour pb = NetworkServer.spawned[ownerPlayerNetworkId].GetComponent<Kit_PlayerBehaviour>();
                    pb.weaponManagerNetworkData = this;
                    ownerPlayer = pb;
                }
            }
            else
            {
                if (NetworkClient.spawned.ContainsKey(ownerPlayerNetworkId))
                {
                    Kit_PlayerBehaviour pb = NetworkClient.spawned[ownerPlayerNetworkId].GetComponent<Kit_PlayerBehaviour>();
                    pb.weaponManagerNetworkData = this;
                    ownerPlayer = pb;
                }
            }
        }
    }
}