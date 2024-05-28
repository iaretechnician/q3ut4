using Mirror;
using UnityEngine;

namespace MarsFPSKit
{
    public class Kit_SimpleVitalsNetworkData : Kit_VitalsNetworkBase
    {
        [SyncVar(hook = "OnHitPointsChangedCallback")]
        public float hitPoints;

        /// <summary>
        /// For displaying the bloody screen
        /// </summary>
        public float hitAlpha;

        public void OnHitPointsChangedCallback(float was, float isNow)
        {
            if (pb)
            {
                if (pb.isFirstPersonActive)
                {
                    Kit_IngameMain.instance.hud.DisplayHealth(isNow);
                }
            }
        }
    }
}