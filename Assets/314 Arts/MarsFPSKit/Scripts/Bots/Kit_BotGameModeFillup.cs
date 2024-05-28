
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFPSKit
{
    [CreateAssetMenu(menuName = "MarsFPSKit/Bots/Game Mode Behaviour/Simple Fillup")]
    /// <summary>
    /// This script fills up bots to the player limit, supports team based game modes too
    /// </summary>
    public class Kit_BotGameModeFillup : Kit_BotGameModeManagerBase
    {
        public int MaxPlayers
        {
            get
            {
                return Kit_NetworkManager.instance.maxConnections;
            }
        }

        public override void Inizialize(Kit_BotManager manager)
        {
            if (Kit_IngameMain.instance.currentPvPGameModeBehaviour.isTeamGameMode)
            {
                int tries = 0;

                for (sbyte i = 0; i < Mathf.Clamp(Kit_IngameMain.instance.gameInformation.allPvpTeams.Length, 0, Kit_IngameMain.instance.currentPvPGameModeBehaviour.maximumAmountOfTeams); i++)
                {
                    sbyte team = i;
                    //Reset tries
                    tries = 0;
                    if (manager.GetPlayersInTeamX(i) + manager.GetBotsInTeamX(i) < MaxPlayers / Mathf.Clamp(Kit_IngameMain.instance.gameInformation.allPvpTeams.Length, 0, Kit_IngameMain.instance.currentPvPGameModeBehaviour.maximumAmountOfTeams))
                    {
                        while (manager.GetPlayersInTeamX(i) + manager.GetBotsInTeamX(i) < MaxPlayers / 2 && tries <= 20)
                        {
                            Kit_Bot bot = manager.AddNewBot();
                            bot.team = team;
                            tries++;
                        }
                    }
                    else if (manager.GetPlayersInTeamX(i) + manager.GetBotsInTeamX(i) > MaxPlayers / Mathf.Clamp(Kit_IngameMain.instance.gameInformation.allPvpTeams.Length, 0, Kit_IngameMain.instance.currentPvPGameModeBehaviour.maximumAmountOfTeams))
                    {
                        while (manager.GetPlayersInTeamX(i) + manager.GetBotsInTeamX(i) > MaxPlayers / Mathf.Clamp(Kit_IngameMain.instance.gameInformation.allPvpTeams.Length, 0, Kit_IngameMain.instance.currentPvPGameModeBehaviour.maximumAmountOfTeams) && tries <= 20)
                        {
                            manager.RemoveBotInTeam(team);
                            tries++;
                        }
                    }
                }
            }
            else
            {
                if (manager.GetAmountOfBots() + manager.GetAmountOfPlayers() < MaxPlayers)
                {
                    //Fill up bots till the limit
                    while (manager.GetAmountOfBots() + manager.GetAmountOfPlayers() < MaxPlayers)
                    {
                        manager.AddNewBot();
                    }
                }
                else if (manager.GetAmountOfBots() + manager.GetAmountOfPlayers() > MaxPlayers)
                {
                    //Fill up bots till the limit
                    while (manager.GetAmountOfBots() + manager.GetAmountOfPlayers() > MaxPlayers)
                    {
                        manager.RemoveRandomBot();
                    }
                }
            }
        }

        public override void PlayerJoinedTeam(Kit_BotManager manager)
        {
            if (Kit_IngameMain.instance.currentPvPGameModeBehaviour.isTeamGameMode)
            {
                int tries = 0;

                for (sbyte i = 0; i < Mathf.Clamp(Kit_IngameMain.instance.gameInformation.allPvpTeams.Length, 0, Kit_IngameMain.instance.currentPvPGameModeBehaviour.maximumAmountOfTeams); i++)
                {
                    sbyte team = i;
                    //Reset tries
                    tries = 0;
                    if (manager.GetPlayersInTeamX(i) + manager.GetBotsInTeamX(i) < MaxPlayers / Mathf.Clamp(Kit_IngameMain.instance.gameInformation.allPvpTeams.Length, 0, Kit_IngameMain.instance.currentPvPGameModeBehaviour.maximumAmountOfTeams))
                    {
                        while (manager.GetPlayersInTeamX(i) + manager.GetBotsInTeamX(i) < MaxPlayers / 2 && tries <= 20)
                        {
                            Kit_Bot bot = manager.AddNewBot();
                            bot.team = team;
                            tries++;
                        }
                    }
                    else if (manager.GetPlayersInTeamX(i) + manager.GetBotsInTeamX(i) > MaxPlayers / Mathf.Clamp(Kit_IngameMain.instance.gameInformation.allPvpTeams.Length, 0, Kit_IngameMain.instance.currentPvPGameModeBehaviour.maximumAmountOfTeams))
                    {
                        while (manager.GetPlayersInTeamX(i) + manager.GetBotsInTeamX(i) > MaxPlayers / Mathf.Clamp(Kit_IngameMain.instance.gameInformation.allPvpTeams.Length, 0, Kit_IngameMain.instance.currentPvPGameModeBehaviour.maximumAmountOfTeams) && tries <= 20)
                        {
                            manager.RemoveBotInTeam(team);
                            tries++;
                        }
                    }
                }
            }
            else
            {
                int tries = 0;
                if (manager.GetAmountOfBots() + manager.GetAmountOfPlayers() < MaxPlayers)
                {
                    //Fill up bots till the limit
                    while (manager.GetAmountOfBots() + manager.GetAmountOfPlayers() < MaxPlayers && tries <= 20)
                    {
                        manager.AddNewBot();
                        tries++;
                    }
                }
                else if (manager.GetAmountOfBots() + manager.GetAmountOfPlayers() > MaxPlayers && tries <= 20)
                {
                    //Fill up bots till the limit
                    while (manager.GetAmountOfBots() + manager.GetAmountOfPlayers() > MaxPlayers)
                    {
                        manager.RemoveRandomBot();
                        tries++;
                    }
                }
            }
        }

        public override void PlayerLeftTeam(Kit_BotManager manager)
        {
            if (Kit_IngameMain.instance.currentPvPGameModeBehaviour.isTeamGameMode)
            {
                int tries = 0;

                for (sbyte i = 0; i < Mathf.Clamp(Kit_IngameMain.instance.gameInformation.allPvpTeams.Length, 0, Kit_IngameMain.instance.currentPvPGameModeBehaviour.maximumAmountOfTeams); i++)
                {
                    sbyte team = i;
                    //Reset tries
                    tries = 0;
                    if (manager.GetPlayersInTeamX(i) + manager.GetBotsInTeamX(i) < MaxPlayers / Mathf.Clamp(Kit_IngameMain.instance.gameInformation.allPvpTeams.Length, 0, Kit_IngameMain.instance.currentPvPGameModeBehaviour.maximumAmountOfTeams))
                    {
                        while (manager.GetPlayersInTeamX(i) + manager.GetBotsInTeamX(i) < MaxPlayers / 2 && tries <= 20)
                        {
                            Kit_Bot bot = manager.AddNewBot();
                            bot.team = team;
                            tries++;
                        }
                    }
                    else if (manager.GetPlayersInTeamX(i) + manager.GetBotsInTeamX(i) > MaxPlayers / Mathf.Clamp(Kit_IngameMain.instance.gameInformation.allPvpTeams.Length, 0, Kit_IngameMain.instance.currentPvPGameModeBehaviour.maximumAmountOfTeams))
                    {
                        while (manager.GetPlayersInTeamX(i) + manager.GetBotsInTeamX(i) > MaxPlayers / Mathf.Clamp(Kit_IngameMain.instance.gameInformation.allPvpTeams.Length, 0, Kit_IngameMain.instance.currentPvPGameModeBehaviour.maximumAmountOfTeams) && tries <= 20)
                        {
                            manager.RemoveBotInTeam(team);
                            tries++;
                        }
                    }
                }
            }
            else
            {
                if (manager.GetAmountOfBots() + manager.GetAmountOfPlayers() < MaxPlayers)
                {
                    //Fill up bots till the limit
                    while (manager.GetAmountOfBots() + manager.GetAmountOfPlayers() < MaxPlayers)
                    {
                        manager.AddNewBot();
                    }
                }
                else if (manager.GetAmountOfBots() + manager.GetAmountOfPlayers() > MaxPlayers)
                {
                    //Fill up bots till the limit
                    while (manager.GetAmountOfBots() + manager.GetAmountOfPlayers() > MaxPlayers)
                    {
                        manager.RemoveRandomBot();
                    }
                }
            }
        }
    }
}
