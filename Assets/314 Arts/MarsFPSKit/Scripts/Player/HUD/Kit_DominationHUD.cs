using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    /// <summary>
    /// This is used for <see cref="Kit_PvP_GMB_Domination"/>
    /// </summary>
    public class Kit_DominationHUD : Kit_GameModeHUDBase
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

        /// <summary>
        /// Prefab
        /// </summary>
        [Header("Flags")]
        public GameObject flagUiPrefab;
        /// <summary>
        /// Where the objects go
        /// </summary>
        public RectTransform flagUiGo;
        /// <summary>
        /// Currently instantiated flag UIs
        /// </summary>
        private List<Kit_DominationHUDFlag> flagUiInstantiated = new List<Kit_DominationHUDFlag>();

        /// <summary>
        /// Root of capture UI
        /// </summary>
        [Header("Capture UI")]
        public GameObject captureRoot;
        /// <summary>
        /// Image used to display progress of capture
        /// </summary>
        public Image captureProgress;
        /// <summary>
        /// Our canvas
        /// </summary>
        private Canvas myCanvas;
        /// <summary>
        /// Rect Transform of our canvas
        /// </summary>
        private RectTransform myCanvasRectTransform;

        public override void HUDInitialize()
        {
            //Cache it
            myCanvas = GetComponentInParent<Canvas>();
            myCanvasRectTransform = myCanvas.transform as RectTransform;
        }

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
                if (Kit_IngameMain.instance.currentGameModeRuntimeData != null && Kit_IngameMain.instance.currentGameModeRuntimeData.GetType() == typeof(Kit_PvP_GMB_DominationNetworkData))
                {
                    Kit_PvP_GMB_DominationNetworkData drd = Kit_IngameMain.instance.currentGameModeRuntimeData as Kit_PvP_GMB_DominationNetworkData;
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
                //Hide UI
                timer.enabled = false;
            }

            //Always updated UI
            //Ensure we are using the correct runtime data
            if (Kit_IngameMain.instance.currentGameModeRuntimeData != null && Kit_IngameMain.instance.currentGameModeRuntimeData.GetType() == typeof(Kit_PvP_GMB_DominationNetworkData))
            {
                Kit_PvP_GMB_DominationNetworkData drd = Kit_IngameMain.instance.currentGameModeRuntimeData as Kit_PvP_GMB_DominationNetworkData;
                Kit_PvP_GMB_Domination dominationGameMode = Kit_IngameMain.instance.currentPvPGameModeBehaviour as Kit_PvP_GMB_Domination;

                bool enableCap = false;

                if (flagUiInstantiated.Count != drd.flags.Count)
                {
                    foreach (Transform t in flagUiGo)
                    {
                        Destroy(t.gameObject);
                    }

                    //Instantiate flags
                    for (int i = 0; i < drd.flags.Count; i++)
                    {
                        GameObject go = Instantiate(flagUiPrefab, flagUiGo, false);
                        //Get
                        Kit_DominationHUDFlag runtime = go.GetComponent<Kit_DominationHUDFlag>();
                        //Hide
                        runtime.flagImage.enabled = false;
                        //Set netural color
                        runtime.flagImage.color = dominationGameMode.hudColorNeutral;
                        //Add
                        flagUiInstantiated.Add(runtime);
                    }
                }

                for (int i = 0; i < drd.flags.Count; i++)
                {
                    if (drd.flags[i])
                    {
                        if (drd.flags[i].currentState == -1)
                        {
                            //Both teams capturing
                            //Set color
                            drd.flags[i].minimapIcon.color = dominationGameMode.hudColorFlagFightedFor;
                            //Set HUD color
                            flagUiInstantiated[i].flagImage.color = dominationGameMode.hudColorFlagFightedFor;

                            //Check if we are team one
                            if (Kit_IngameMain.instance.myPlayer)
                            {
                                //Check if we are capturing
                                if (drd.flags[i].playersInTrigger.Contains(Kit_IngameMain.instance.myPlayer))
                                {
                                    enableCap = true;
                                    //Set progress
                                    captureProgress.fillAmount = drd.flags[i].smoothedCaptureProgress / 100f;
                                }
                            }
                        }
                        else if (drd.flags[i].currentState == 0)
                        {
                            if (drd.flags[i].currentOwner == 0)
                            {
                                //No one captures it and no one owns it
                                //Set color
                                drd.flags[i].minimapIcon.color = dominationGameMode.hudColorNeutral;
                                //Set HUD color
                                flagUiInstantiated[i].flagImage.color = dominationGameMode.hudColorNeutral;
                            }
                            else if (drd.flags[i].currentOwner >= 1)
                            {
                                //No one captures it and team one owns it
                                //Set color
                                drd.flags[i].minimapIcon.color = Kit_IngameMain.instance.gameInformation.allPvpTeams[drd.flags[i].currentOwner - 1].teamColor;
                                //Set HUD color
                                flagUiInstantiated[i].flagImage.color = Kit_IngameMain.instance.gameInformation.allPvpTeams[drd.flags[i].currentOwner - 1].teamColor;
                            }
                        }
                        else if (drd.flags[i].currentState >= 1)
                        {
                            //Check if already captured fully
                            if (drd.flags[i].currentOwner == drd.flags[i].currentState)
                            {
                                //Set color
                                drd.flags[i].minimapIcon.color = Kit_IngameMain.instance.gameInformation.allPvpTeams[drd.flags[i].currentState - 1].teamColor;
                                //Set HUD color
                                flagUiInstantiated[i].flagImage.color = Kit_IngameMain.instance.gameInformation.allPvpTeams[drd.flags[i].currentState - 1].teamColor;
                            }
                            else
                            {
                                //Team one capturing
                                //Set color
                                drd.flags[i].minimapIcon.color = Kit_IngameMain.instance.gameInformation.allPvpTeams[drd.flags[i].currentState - 1].teamColor;
                                //Set HUD color
                                flagUiInstantiated[i].flagImage.color = Kit_IngameMain.instance.gameInformation.allPvpTeams[drd.flags[i].currentState - 1].teamColor;

                                //Check if we are team one
                                if (Kit_IngameMain.instance.assignedTeamID + 1 == drd.flags[i].currentState && Kit_IngameMain.instance.myPlayer)
                                {
                                    //Check if we are capturing
                                    if (drd.flags[i].playersInTrigger.Contains(Kit_IngameMain.instance.myPlayer))
                                    {
                                        enableCap = true;
                                        //Set progress
                                        captureProgress.fillAmount = drd.flags[i].smoothedCaptureProgress / 100f;
                                    }
                                }
                            }
                        }
                    }
                }

                captureRoot.SetActiveOptimized(enableCap);
            }
        }

        private void LateUpdate()
        {
            Kit_PvP_GMB_DominationNetworkData drd = Kit_IngameMain.instance.currentGameModeRuntimeData as Kit_PvP_GMB_DominationNetworkData;

            for (int i = 0; i < drd.flags.Count; i++)
            {
                Vector3 screenPosition = Kit_IngameMain.instance.mainCamera.WorldToScreenPoint(drd.flags[i].uiPosition.position); // pass the world position

                if (screenPosition.z > 0)
                {
                    // Set the position and remove the screen offset
                    (flagUiInstantiated[i].transform as RectTransform).position = screenPosition;
                    flagUiInstantiated[i].flagImage.enabled = true;
                }
                else
                {
                    flagUiInstantiated[i].flagImage.enabled = false;
                }
            }
        }
    }
}
