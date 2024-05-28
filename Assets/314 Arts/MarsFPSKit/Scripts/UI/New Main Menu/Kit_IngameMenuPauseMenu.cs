using MarsFPSKit.Networking;
using TMPro;
using UnityEngine;

namespace MarsFPSKit
{
    namespace UI
    {
        public class Kit_IngameMenuPauseMenu : MonoBehaviour
        {
            /// <summary>
            /// ID for the pause menu
            /// </summary>
            public int pauseMenuId = 1;
            /// <summary>
            /// Button that takes us to the loadout menu
            /// </summary>
            public GameObject loadoutButton;
            /// <summary>
            /// Spawn  Button
            /// </summary>
            public TextMeshProUGUI spawnButtonText;
            /// <summary>
            /// Because this button controls both, changing teams and commiting suicide, we need to adjust its text
            /// </summary>
            public TextMeshProUGUI changeTeamButtonText;
            /// <summary>
            /// The change team button
            /// </summary>
            public GameObject changeTeamButton;
            /// <summary>
            /// Button that lets us vote
            /// </summary>
            public GameObject voteButton;
            /// <summary>
            /// Transform where the button goes
            /// </summary>
            public RectTransform pluginButtonGo;
            /// <summary>
            /// Prefab for inejcting a button into the pause menu
            /// </summary>
            public GameObject pluginButtonPrefab;

            private void Start()
            {
                //Is loadout supported?
                if (Kit_IngameMain.instance.currentPvPGameModeBehaviour && Kit_IngameMain.instance.currentPvPGameModeBehaviour.LoadoutMenuSupported())
                {
                    loadoutButton.SetActive(true);
                }
                else
                {
                    loadoutButton.SetActive(false);
                }
            }

            private void Update()
            {
                #region Team - Suicide Button
                if (Kit_IngameMain.instance.currentPvEGameModeBehaviour)
                {
                    changeTeamButton.gameObject.SetActiveOptimized(false);
                }
                else if (Kit_IngameMain.instance.myPlayer)
                {
                    changeTeamButtonText.text = "Suicide";
                    changeTeamButton.gameObject.SetActiveOptimized(true);
                }
                else if (Kit_IngameMain.instance.currentPvPGameModeBehaviour)
                {
                    changeTeamButtonText.text = "Change Team";
                    changeTeamButton.gameObject.SetActiveOptimized(true);
                }
                else
                {
                    changeTeamButton.gameObject.SetActiveOptimized(false);
                }
                #endregion

                #region Spawn/Resume Button
                if (Kit_IngameMain.instance.myPlayer)
                {
                    spawnButtonText.text = "Resume";
                }
                else
                {
                    if (Kit_IngameMain.instance.currentPvPGameModeBehaviour && Kit_IngameMain.instance.currentPvPGameModeBehaviour.CanSpawn(Kit_NetworkPlayerManager.instance.GetLocalPlayer()))
                    {
                        spawnButtonText.text = "Spawn";
                    }
                    else
                    {
                        spawnButtonText.text = "Close";
                    }
                }
                #endregion

                #region Vote Button
                //Voting only in pvp game modes
                voteButton.SetActiveOptimized(Kit_IngameMain.instance.currentPvPGameModeBehaviour);
                #endregion
            }
        }
    }
}