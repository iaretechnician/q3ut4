using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    /// <summary>
    /// This class holds information for a voting entry
    /// </summary>
    public class Kit_MapVotingEntry : MonoBehaviour
    {
        /// <summary>
        /// Text to display the map with
        /// </summary>
        public Image mapImage;
        /// <summary>
        /// Text to display the map's name with
        /// </summary>
        public TextMeshProUGUI mapName;
        /// <summary>
        /// Text to display the game mode name with
        /// </summary>
        public TextMeshProUGUI gameModeName;

        /// <summary>
        /// Percentage filled background image
        /// </summary>
        public Image votePercentageImage;
        /// <summary>
        /// Text that displays voting percentage
        /// </summary>
        public TextMeshProUGUI votePercentageText;

        /// <summary>
        /// Which combo is this one voting for?
        /// </summary>
        public uint myVote;

        /// <summary>
        /// Updates our CustomProperties so that we vote for this combo
        /// </summary>
        public void VoteForThis()
        {
            Kit_IngameMain.instance.currentMapVoting.CmdVoteForMap(myVote);
        }
    }
}
