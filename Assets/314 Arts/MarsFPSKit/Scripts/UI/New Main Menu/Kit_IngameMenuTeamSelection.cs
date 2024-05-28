using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    namespace UI
    {
        public class Kit_IngameMenuTeamSelection : MonoBehaviour
        {
            /// <summary>
            /// Id in the team selection
            /// </summary>
            public int teamSelectionId;
            /// <summary>
            /// Where the team selection goes
            /// </summary>
            public RectTransform teamGo;
            /// <summary>
            /// Prefab for team selection
            /// </summary>
            public GameObject teamPrefab;

            /// <summary>
            /// Action after team selection
            /// </summary>
            public AfterTeamSelection afterSelection;

            public void Setup()
            {
                for (sbyte i = 0; i < Mathf.Clamp(Kit_IngameMain.instance.gameInformation.allPvpTeams.Length, 0, Kit_IngameMain.instance.currentPvPGameModeBehaviour.maximumAmountOfTeams); i++)
                {
                    sbyte id = i;
                    GameObject go = Instantiate(teamPrefab, teamGo, false);
                    Button btn = go.GetComponentInChildren<Button>();
                    TextMeshProUGUI txt = go.GetComponentInChildren<TextMeshProUGUI>();

                    btn.onClick.AddListener(delegate { Kit_IngameMain.instance.JoinTeam(id); });
                    txt.text = Kit_IngameMain.instance.gameInformation.allPvpTeams[id].teamName;

                    //Move to right pos
                    go.transform.SetSiblingIndex(id);
                }

                if (Kit_IngameMain.instance.spectatorManager && Kit_IngameMain.instance.spectatorManager.IsSpectatingEnabled() && Kit_IngameMain.instance.currentPvPGameModeBehaviour.SpectatingEnabled())
                {
                    GameObject go = Instantiate(teamPrefab, teamGo, false);
                    Button btn = go.GetComponentInChildren<Button>();
                    TextMeshProUGUI txt = go.GetComponentInChildren<TextMeshProUGUI>();

                    btn.onClick.AddListener(delegate { Kit_IngameMain.instance.spectatorManager.BeginSpectating(true); });
                    txt.text = "Spectate";

                    //Move to right pos
                    go.transform.SetSiblingIndex(Mathf.Clamp(Kit_IngameMain.instance.gameInformation.allPvpTeams.Length, 0, Kit_IngameMain.instance.currentPvPGameModeBehaviour.maximumAmountOfTeams));
                }
            }
        }
    }
}