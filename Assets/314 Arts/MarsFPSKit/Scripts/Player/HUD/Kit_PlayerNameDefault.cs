using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// Class used for storing runtime data
    /// </summary>
    public class PlayerNameRuntimeData
    {
        /// <summary>
        /// The ID of our marker
        /// </summary>
        public int myId;
        /// <summary>
        /// When were we spotted for the last time?
        /// </summary>
        public float lastTimeSeen;
    }

    [CreateAssetMenu(menuName = "MarsFPSKit/Name Manager/Default")]
    public class Kit_PlayerNameDefault : Kit_PlayerNameUIBase
    {
        /// <summary>
        /// How much longer is the name going to be displayed?
        /// </summary>
        public float visibleTimeThreshold = 0.4f;

        /// <summary>
        /// If we are  friendly, how far is the name going to be displayed instead of the marker?
        /// </summary>
        public float nameDistance = 15f;

        public override void LocalPlayerGainedControl(Kit_PlayerBehaviour pb)
        {
            //Get Data
            PlayerNameRuntimeData pnrd = (PlayerNameRuntimeData)pb.customNameData;
            if (pnrd != null)
            {
                //Check if we have an id assigned
                if (pnrd.myId >= 0)
                {
                    Kit_IngameMain.instance.hud.ReleasePlayerMarker(pnrd.myId);
                    pnrd.myId = -1;
                }
            }
        }

        public override void OnDestroyRelay(Kit_PlayerBehaviour pb)
        {
            //Get Data
            PlayerNameRuntimeData pnrd = (PlayerNameRuntimeData)pb.customNameData;
            if (pnrd != null)
            {
                //Check if we have an id assigned
                if (pnrd.myId >= 0)
                {
                    Kit_IngameMain.instance.hud.ReleasePlayerMarker(pnrd.myId);
                }
            }
        }

        public override void PlayerSpotted(Kit_PlayerBehaviour pb, float validFor)
        {
            //Get Data
            PlayerNameRuntimeData pnrd = (PlayerNameRuntimeData)pb.customNameData;
            //Set data
            pnrd.lastTimeSeen = Time.time + validFor + visibleTimeThreshold;
        }

        public override void StartRelay(Kit_PlayerBehaviour pb)
        {
            PlayerNameRuntimeData prnd = new PlayerNameRuntimeData();
            //Assign data
            pb.customNameData = prnd;
            //Get id
            prnd.myId = Kit_IngameMain.instance.hud.GetUnusedPlayerMarker();
        }

        public override void UpdateEnemy(Kit_PlayerBehaviour pb)
        {
            //Get Data
            PlayerNameRuntimeData pnrd = (PlayerNameRuntimeData)pb.customNameData;
            if (!pb.isBeingSpectated)
            {
                //Check if we are visible
                if (Time.time <= pnrd.lastTimeSeen)
                {
                    //Display us
                    Kit_IngameMain.instance.hud.UpdatePlayerMarker(pnrd.myId, PlayerNameState.enemy, pb.thirdPersonPlayerModel.enemyNameAboveHeadPos.position, pb.userName);
                }
                else
                {
                    //Do not display us
                    Kit_IngameMain.instance.hud.UpdatePlayerMarker(pnrd.myId, PlayerNameState.none, pb.thirdPersonPlayerModel.enemyNameAboveHeadPos.position, pb.userName);
                }
            }
            else
            {
                //Do not display us
                Kit_IngameMain.instance.hud.UpdatePlayerMarker(pnrd.myId, PlayerNameState.none, pb.thirdPersonPlayerModel.enemyNameAboveHeadPos.position, "");
            }
        }

        public override void UpdateFriendly(Kit_PlayerBehaviour pb)
        {
            //Get Data
            PlayerNameRuntimeData pnrd = (PlayerNameRuntimeData)pb.customNameData;
            if (!pb.isBeingSpectated)
            {
                //Check distance
                float distance = Vector3.Distance(pb.thirdPersonPlayerModel.enemyNameAboveHeadPos.position, Kit_IngameMain.instance.mainCamera.transform.position);
                if (distance < nameDistance)
                {
                    //Display us as name
                    Kit_IngameMain.instance.hud.UpdatePlayerMarker(pnrd.myId, PlayerNameState.friendlyClose, pb.thirdPersonPlayerModel.enemyNameAboveHeadPos.position, pb.userName);
                }
                else
                {
                    //Display us as marker
                    Kit_IngameMain.instance.hud.UpdatePlayerMarker(pnrd.myId, PlayerNameState.friendlyFar, pb.thirdPersonPlayerModel.enemyNameAboveHeadPos.position, pb.userName);
                }
            }
            else
            {
                //Display us as marker
                Kit_IngameMain.instance.hud.UpdatePlayerMarker(pnrd.myId, PlayerNameState.none, pb.thirdPersonPlayerModel.enemyNameAboveHeadPos.position, "");
            }
        }
    }
}