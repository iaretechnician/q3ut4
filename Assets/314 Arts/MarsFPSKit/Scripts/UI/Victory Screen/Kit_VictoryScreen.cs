
using MarsFPSKit.Networking;
using Mirror;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// This script handles the victory screen. Since there is no special functions for this, it is not abstract. You can replace it with your own.
    /// </summary>
    public class Kit_VictoryScreen : NetworkBehaviour
    {
        [SyncVar]
        /// <summary>
        /// Winner type. 0 = Player/Bot, 1 = Team, 2 = Team with score
        /// </summary>
        public int winnerType;
        [SyncVar]
        /// <summary>
        /// Winner id (either the player/bot or the team that won)
        /// </summary>
        public uint winnerId;
        [SyncVar]
        /// <summary>
        /// If <see cref="winnerType"/> is 0, is the winner a bot?
        /// </summary>
        public bool winnerBot;
        /// <summary>
        /// Scores if a team won
        /// </summary>
        public readonly SyncList<int> winnerScores = new SyncList<int>();

        public override void OnStartClient()
        {
            //Assign to main behaviour
            Kit_IngameMain.instance.currentVictoryScreen = this;
            //Callback
            Kit_IngameMain.instance.VictoryScreenOpened();
            //Close Pause Menu
            Kit_IngameMain.instance.SetPauseMenuState(false);
            //Unlock cursor
            if (MarsScreen.lockCursor)
            {
                MarsScreen.lockCursor = false;
            }
            //Disable scoreboard
            Kit_IngameMain.instance.scoreboard.Disable();

            //Player won
            if (winnerType == 0)
            {
                if (winnerBot)
                {
                    if (Kit_IngameMain.instance.currentBotManager)
                    {
                        Kit_Bot winner = Kit_IngameMain.instance.currentBotManager.GetBotWithID(winnerId);
                        //Display UI
                        Kit_IngameMain.instance.victoryScreenUI.DisplayBotWinner(winner);
                    }
                }
                else
                {
                    Kit_Player winner = Kit_NetworkPlayerManager.instance.GetPlayerById(winnerId);
                    //Check if we won
                    if (winner.isLocal)
                    {
                        //We won this match!
                        Debug.Log("Victory Screen: We won");
                    }
                    else
                    {
                        //Someone else won :(
                        Debug.Log("Victory Screen: A different player won");
                    }
                    //Display UI
                    Kit_IngameMain.instance.victoryScreenUI.DisplayPlayerWinner(winner);
                }
            }
            //Team won (Or draw)
            else if (winnerType == 1)
            {
                //Check which team won
                if (winnerId == 999)
                {
                    //Draw
                    Debug.Log("Victory Screen: Draw");
                }
                else
                {
                    //Team x won
                    Debug.Log("Victory Screen: Team " + winnerId + " won");
                }
                //Check if the data inlcudes scores
                if (winnerScores.Count > 0)
                {
                    //Display UI
                    Kit_IngameMain.instance.victoryScreenUI.DisplayTeamWinnerWithScores(winnerId, winnerScores.ToArray());
                }
                else
                {
                    //Display UI
                    Kit_IngameMain.instance.victoryScreenUI.DisplayTeamWinner(winnerId);
                }
            }
        }

        void OnDestroy()
        {
            //Hide UI
            Kit_IngameMain.instance.victoryScreenUI.CloseUI();
            //Enable scoreboard
            Kit_IngameMain.instance.scoreboard.Enable();
        }
    }
}
