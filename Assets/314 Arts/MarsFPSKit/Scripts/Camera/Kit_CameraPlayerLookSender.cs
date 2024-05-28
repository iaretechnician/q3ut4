using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// This script checks if we are looking at a player and tells that player if we are looking at him. It is used for <see cref="Kit_PlayerNameUIBase"/>
    /// </summary>
    public class Kit_CameraPlayerLookSender : MonoBehaviour
    {
        [Header("Raycast Settings")]
        /// <summary>
        /// How far will the ray check if we saw someone?
        /// </summary>
        public float rayLength = 100f;
        /// <summary>
        /// Which layers will be checked?
        /// </summary>
        public LayerMask rayMask;
        /// <summary>
        /// How many seconds apart will the ray check? 0 means every frame.
        /// </summary>
        public float rayTime = 0f;

        //RUNTIME DATA
        /// <summary>
        /// When was the ray fired the last time?
        /// </summary>
        private float lastCheck;

        public RaycastHit hit;
        //END

        void Update()
        {
            if (rayTime <= 0 || Time.time >= lastCheck + rayTime)
            {
                //Set time
                lastCheck = Time.time;
                //Fire Ray
                if (Physics.Raycast(transform.position, transform.forward, out hit, rayLength, rayMask.value))
                {
                    //Check if we hit something that belongs to a player
                    Kit_PlayerBehaviour pb = hit.transform.root.GetComponent<Kit_PlayerBehaviour>();
                    if (pb && !pb.isFirstPersonActive)
                    {
                        //Check if the player has a name system assigned
                        if (pb.nameManager)
                        {
                            //Tell the system we hit him
                            pb.nameManager.PlayerSpotted(pb, rayTime);
                        }
                    }
                }
            }
        }
    }
}
