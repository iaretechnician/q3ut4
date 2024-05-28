using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace MarsFPSKit
{
    /// <summary>
    /// This is used for <see cref="Kit_PvP_GMB_TeamDeathmatch"/>
    /// </summary>
    public class Kit_TeamDeathmatchHUD : Kit_GameModeHUDBase
    {
        public TextMeshProUGUI timer;

        /// <summary>
        /// Where the multi team points go
        /// </summary>
        public RectTransform pointsGo;
        /// <summary>
        /// Prefab for multi team points
        /// </summary>
        public GameObject pointsPrefab;
        /// <summary>
        /// Active points
        /// </summary>
        public List<TextMeshProUGUI> pointsActive = new List<TextMeshProUGUI>();

        private int roundedRestSeconds;
        private int displaySeconds;
        private int displayMinutes;

        public override void HUDUpdate()
        {
            if (Kit_IngameMain.instance.currentPvPGameModeBehaviour.AreEnoughPlayersThere() || Kit_IngameMain.instance.hasGameModeStarted)
            {
                roundedRestSeconds = Mathf.CeilToInt(Kit_IngameMain.instance.timer);
                displaySeconds = roundedRestSeconds % 60; //Get seconds
                displayMinutes = roundedRestSeconds / 60; //Get minutes
                                                          //Update text
                timer.text = string.Format("{0:00} : {1:00}", displayMinutes, displaySeconds);
                timer.enabled = true;

                //Ensure we are using the correct runtime data
                if (Kit_IngameMain.instance.currentGameModeRuntimeData != null && Kit_IngameMain.instance.currentGameModeRuntimeData.GetType() == typeof(Kit_PvP_GMB_TeamDeathmatchNetworkData))
                {
                    Kit_PvP_GMB_TeamDeathmatchNetworkData drd = Kit_IngameMain.instance.currentGameModeRuntimeData as Kit_PvP_GMB_TeamDeathmatchNetworkData;
                    //Setup points
                    if (pointsActive.Count == 0)
                    {
                        for (int i = 0; i < Mathf.Clamp(Kit_IngameMain.instance.gameInformation.allPvpTeams.Length, 0, Kit_IngameMain.instance.currentPvPGameModeBehaviour.maximumAmountOfTeams); i++)
                        {
                            GameObject go = Instantiate(pointsPrefab, pointsGo, false);
                            //Get
                            pointsActive.Add(go.GetComponentInChildren<TextMeshProUGUI>());
                            //Color
                            pointsActive[i].color = Kit_IngameMain.instance.gameInformation.allPvpTeams[i].teamColor;
                        }
                    }

                    //Redraw
                    for (int i = 0; i < drd.teamPoints.Count; i++)
                    {
                        pointsActive[i].text = drd.teamPoints[i].ToString();
                    }
                }
            }
            else
            {
                //Hide all UI
                timer.enabled = false;
            }
        }
    }
}
