using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// This class is used to display the victory screen from <see cref="Kit_VictoryScreen"/>
    /// </summary>
    public abstract class Kit_VictoryScreenUI : MonoBehaviour
    {
        /// <summary>
        /// Display a player who has won
        /// </summary>
        /// <param name="winner"></param>
        public abstract void DisplayPlayerWinner(Kit_Player winner);

        /// <summary>
        /// Display a bot who has won
        /// </summary>
        /// <param name="winner"></param>
        public abstract void DisplayBotWinner(Kit_Bot winner);

        /// <summary>
        /// Display a team who has won (or 2 for draw)
        /// </summary>
        /// <param name="winner"></param>
        public abstract void DisplayTeamWinner(uint winner);

        /// <summary>
        /// Displays  a team who has won (or 2 for draw) including scores
        /// </summary>
        /// <param name="winner"></param>
        /// <param name="teamOneScore"></param>
        /// <param name="teamTwoScore"></param>
        public abstract void DisplayTeamWinnerWithScores(uint winner, int[] scores);

        /// <summary>
        /// Close the UI completely. Called when the Victory Screen is deleted
        /// </summary>
        public abstract void CloseUI();
    }
}
