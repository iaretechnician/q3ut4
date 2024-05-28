using Mirror;
using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// Upon entering, this will be picked up
    /// </summary>
    public class Kit_AmmoPickup : NetworkBehaviour
    {
        /// <summary>
        /// How many clips will be picked up
        /// </summary>
        public int amountOfClipsToPickup = 2;
        /// <summary>
        /// Rigidbody
        /// </summary>
        public Rigidbody body;
        /// <summary>
        /// Root object of renderer to hide before destroyed
        /// </summary>
        public GameObject renderRoot;
        
        public void PickedUp()
        {
            if (isServer)
            {
                NetworkServer.Destroy(gameObject);
            }
        }
    }
}