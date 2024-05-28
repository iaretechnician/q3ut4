using System.Collections.Generic;
using UnityEngine;

namespace MarsFPSKit
{
    public class Kit_MapVotingUI : Kit_MapVotingUIBase
    {
        /// <summary>
        /// Root object for the voting
        /// </summary>
        public GameObject root;

        [Header("List")]
        /// <summary>
        /// Where will all the votes go?
        /// </summary>
        public RectTransform listGo;
        /// <summary>
        /// Entry prefab
        /// </summary>
        public GameObject listPrefab;

        //Runtime
        List<Kit_MapVotingEntry> activeEntries = new List<Kit_MapVotingEntry>();

        public override void SetupVotes(Kit_MapVotingBehaviour behaviour)
        {
            //Delete old entries
            for (int i = 0; i < activeEntries.Count; i++)
            {
                Destroy(activeEntries[i].gameObject);
            }

            activeEntries = new List<Kit_MapVotingEntry>();

            for (int i = 0; i < behaviour.combos.Count; i++)
            {
                GameObject go = Instantiate(listPrefab, listGo, false);
                //Add to the entry list
                Kit_MapVotingEntry newEntry = go.GetComponent<Kit_MapVotingEntry>();
                activeEntries.Add(newEntry);
                //Setup the new entry
                newEntry.gameModeName.text = Kit_IngameMain.instance.gameInformation.allPvpGameModes[behaviour.combos[i].gameMode].gameModeName;
                if (Kit_GameSettings.currentNetworkingMode == KitNetworkingMode.Traditional)
                {
                    newEntry.mapName.text = Kit_IngameMain.instance.gameInformation.allPvpGameModes[behaviour.combos[i].gameMode].traditionalMaps[behaviour.combos[i].map].mapName;
                    newEntry.mapImage.sprite = Kit_IngameMain.instance.gameInformation.allPvpGameModes[behaviour.combos[i].gameMode].traditionalMaps[behaviour.combos[i].map].mapPicture;
                }
                else if (Kit_GameSettings.currentNetworkingMode == KitNetworkingMode.Lobby)
                {
                    newEntry.mapName.text = Kit_IngameMain.instance.gameInformation.allPvpGameModes[behaviour.combos[i].gameMode].lobbyMaps[behaviour.combos[i].map].mapName;
                    newEntry.mapImage.sprite = Kit_IngameMain.instance.gameInformation.allPvpGameModes[behaviour.combos[i].gameMode].lobbyMaps[behaviour.combos[i].map].mapPicture;
                }
                //Without this it would get changed as i changes.
                uint vote = (uint)i;
                newEntry.myVote = vote;
            }

            //Show the root
            root.SetActive(true);
        }

        public override void RedrawVotes(Kit_MapVotingBehaviour behaviour)
        {
            uint[] votes = behaviour.GetVotesPerCombo();

            //Get total votes
            uint totalVotes = 0;
            for (int i = 0; i < votes.Length; i++)
            {
                totalVotes += votes[i];
            }

            //Redraw all entries
            for (int i = 0; i < votes.Length; i++)
            {
                if (i < activeEntries.Count)
                {
                    if (totalVotes > 0)
                    {
                        activeEntries[i].votePercentageImage.fillAmount = votes[i] / (float)totalVotes;
                        activeEntries[i].votePercentageText.text = ((votes[i] / (float)totalVotes) * 100f).ToString("F0") + "%";
                    }
                    else
                    {
                        activeEntries[i].votePercentageImage.fillAmount = 0;
                        activeEntries[i].votePercentageText.text = "0%";
                    }
                }
            }

            Kit_IngameMain.instance.SetPauseMenuState(false);
        }

        public override void Hide()
        {
            //Disable root
            root.SetActive(false);
        }
    }
}
