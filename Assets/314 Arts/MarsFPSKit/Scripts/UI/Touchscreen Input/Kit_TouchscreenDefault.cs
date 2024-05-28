using UnityEngine;

namespace MarsFPSKit
{
    public class Kit_TouchscreenDefault : Kit_TouchscreenBase
    {
        /// <summary>
        /// This is the input that shall only be enabled if our player is active
        /// </summary>
        public GameObject playerInput;

        public override void LocalPlayerDied(Kit_PlayerBehaviour pb)
        {
            //Disable player input
            playerInput.SetActive(false);
        }

        public override void LocalPlayerSpawned(Kit_PlayerBehaviour pb)
        {
            //Enable player input
            playerInput.SetActive(true);
        }

        public override void Setup()
        {
            //Disable player input
            playerInput.SetActive(false);
        }
    }
}