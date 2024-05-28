using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MarsFPSKit.Networking;

namespace MarsFPSKit
{
    [System.Serializable]
    public class AssistedKillData
    {
        /// <summary>
        /// ID of the bot / Photon Actor Number of the player (if not a bot)
        /// </summary>
        public uint id;
        /// <summary>
        /// Represents a bot?
        /// </summary>
        public bool bot;
    }

    [CreateAssetMenu(menuName = "MarsFPSKit/Assists/Default")]
    public class Kit_AssistManagerDefault : Kit_AssistManagerBase
    {
        /// <summary>
        /// How much xp is gained per assist?
        /// </summary>
        public int xpPerAssist = 20;

        public override void OnStart()
        {

        }

        public override void PlayerDamaged(bool botShot, uint shotId, Kit_PlayerBehaviour damagedPlayer, float dmg)
        {
            //Assists only for team gamemodes
            if (Kit_IngameMain.instance.currentPvPGameModeBehaviour && Kit_IngameMain.instance.currentPvPGameModeBehaviour.isTeamGameMode)
            {
                if (damagedPlayer.damagedBy.Where(x => x.bot == botShot && x.id == shotId).Count() <= 0)
                {
                    damagedPlayer.damagedBy.Add(new AssistedKillData { bot = botShot, id = shotId });
                }
            }
        }

        public override void PlayerKilled(bool botKiller, uint idKiller, Kit_PlayerBehaviour killedPlayer)
        {
            if (Kit_IngameMain.instance.currentPvPGameModeBehaviour && Kit_IngameMain.instance.currentPvPGameModeBehaviour.isTeamGameMode)
            {
                for (int i = 0; i < killedPlayer.damagedBy.Count; i++)
                {
                    //Check if it counts as assist
                    if (!(killedPlayer.damagedBy[i].bot == botKiller && killedPlayer.damagedBy[i].id == idKiller))
                    {
                        int killerTeam = -1;
                        int assistTeam = -2;

                        if (botKiller)
                        {
                            Kit_Bot killerBot = Kit_IngameMain.instance.currentBotManager.GetBotWithID(idKiller);
                            if (killerBot != null)
                            {
                                killerTeam = killerBot.team;
                            }
                        }
                        else
                        {
                            Kit_Player killerPlayer = Kit_NetworkPlayerManager.instance.GetPlayerById(idKiller);

                            if (killerPlayer != null)
                            {
                                killerTeam = killerPlayer.team;
                            }
                        }

                        //Assist for bot
                        if (killedPlayer.damagedBy[i].bot)
                        {
                            Kit_Bot bot = Kit_IngameMain.instance.currentBotManager.GetBotWithID(killedPlayer.damagedBy[i].id);
                            if (bot != null)
                            {
                                assistTeam = bot.team;

                                if (assistTeam == killerTeam)
                                {
                                    bot.assists++;
                                    Kit_IngameMain.instance.currentBotManager.ModifyBotData(bot);
                                }
                            }
                        }
                        else
                        {
                            Kit_Player player = Kit_NetworkPlayerManager.instance.GetPlayerById(killedPlayer.damagedBy[i].id);

                            if (player != null)
                            {
                                assistTeam = player.team;
                            }

                            if (assistTeam == killerTeam)
                            {
                                player.assists++;
                                Kit_NetworkPlayerManager.instance.ModifyPlayerData(player);

                                Kit_IngameMain.instance.RpcGenericEvent(0, 0);
                            }
                        }
                    }
                }
            }
        }

        public override void OnGenericEvent(byte eventCode, int content)
        {
            if (eventCode == 0)
            {
                if (Kit_IngameMain.instance.gameInformation.leveling)
                {
                    Kit_IngameMain.instance.gameInformation.leveling.AddXp(xpPerAssist);
                }

                Kit_IngameMain.instance.pointsUI.DisplayPoints(xpPerAssist, PointType.Assist);

                if (Kit_IngameMain.instance.gameInformation.statistics)
                {
                    //Call
                    Kit_IngameMain.instance.gameInformation.statistics.OnAssist();
                }
            }
        }
    }
}