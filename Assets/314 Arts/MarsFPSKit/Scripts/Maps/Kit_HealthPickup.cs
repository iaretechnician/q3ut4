
using Mirror;
using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// Upon entering, this will be picked up
    /// </summary>
    public class Kit_HealthPickup : NetworkBehaviour
    {
        /// <summary>
        /// How much health will be restored? 
        /// </summary>
        public float healthRestored = 30f;
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