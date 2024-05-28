using Mirror;
using UnityEngine;

namespace MarsFPSKit
{
    public class Kit_BloodyScreenVitalsNetworkData : Kit_VitalsNetworkBase
    {
        [SyncVar]
        public float hitPoints;
        public float lastHit;
        /// <summary>
        /// For displaying the bloody screen
        /// </summary>
        public float hitAlpha;
    }
}