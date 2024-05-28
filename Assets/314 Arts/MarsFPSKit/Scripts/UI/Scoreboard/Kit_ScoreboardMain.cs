using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MarsFPSKit.Networking;

namespace MarsFPSKit
{
    namespace Scoreboard
    {
        [System.Serializable]
        /// <summary>
        /// This is a helper class to combine Photon.Realtime.Player and Kit_Bot
        /// </summary>
        public class Kit_ScoreboardHelper
        {
            /// <summary>
            /// Name of this player
            /// </summary>
            public string name;
            /// <summary>
            /// Team of this player
            /// </summary>
            public sbyte team;
            /// <summary>
            /// Kills of this player
            /// </summary>
            public uint kills;
            /// <summary>
            /// Deaths of this player
            /// </summary>
            public uint deaths;
            /// <summary>
            /// Assists of this player
            /// </summary>
            public uint assists;
            /// <summary>
            /// Ping of this player
            /// </summary>
            public ushort ping;
            /// <summary>
            /// To pool these, they need to have a used parameter
            /// </summary>
            public bool used;
        }

        public class Kit_ScoreboardMain : Kit_ScoreboardBase
        {
            /// <summary>
            /// The root object of the scoreboard
            /// </summary>
            public GameObject scoreboardRoot;

            /// <summary>
            /// Prefab for one team
            /// </summary>
            public GameObject teamPrefab;
            /// <summary>
            /// Where the team prefab does go
            /// </summary>
            public RectTransform teamGo;
            /// <summary>
            /// Active teams list
            /// </summary>
            public List<RectTransform> teamActive = new List<RectTransform>();
            /// <summary>
            /// Scoreboard entry prefab
            /// </summary>
            public GameObject entryPrefab;
            /// <summary>
            /// A list of active entries so we can pool them
            /// </summary>
            public List<Kit_ScoreboardUIEntry> activeEntries = new List<Kit_ScoreboardUIEntry>();

            [Header("Settings")]
            /// <summary>
            /// After how many seconds should the scoreboard be redrawn, if it is open? So we can see updated values while the scoreboard is open.
            /// </summary>
            public float redrawFrequency = 1f;

            /// <summary>
            /// When was the scoreboard redrawn for the last time?
            /// </summary>
            private float lastRedraw;

            //[HideInInspector]
            public bool canUseScoreboard;

            #region Runtime
            public List<Kit_ScoreboardHelper> rt_ScoreboardEntries = new List<Kit_ScoreboardHelper>();
            #endregion

            void Update()
            {
                //Check if we can use the scoreboard
                if (canUseScoreboard)
                {
                    //Check for input
                    if (Input.GetKey(KeyCode.Tab))
                    {
                        //Check if its not already open
                        if (!isOpen)
                        {
                            isOpen = true;
                            //Redraw
                            Redraw();
                        }
                    }
                    else
                    {
                        //Check if it is open
                        if (isOpen)
                        {
                            isOpen = false;
                            //Redraw
                            Redraw();
                        }
                    }

                    if (isOpen)
                    {
                        if (Time.time > lastRedraw)
                        {
                            Redraw();
                        }
                    }
                }
            }

            public override void Disable()
            {
                //Disable use of scoreboard
                canUseScoreboard = false;
                //Force scoreboard to close
                isOpen = false;
                //Close
                scoreboardRoot.SetActive(false);
            }

            public override void Enable()
            {
                //Enable use of scoreboard
                canUseScoreboard = true;
            }

            void Redraw()
            {
                //Set time
                lastRedraw = Time.time + redrawFrequency;

                //Set root based on state
                if (isOpen)
                {
                    scoreboardRoot.SetActive(true);
                }
                else
                {
                    scoreboardRoot.SetActive(false);
                }

                //Reset entries
                for (int o = 0; o < rt_ScoreboardEntries.Count; o++)
                {
                    rt_ScoreboardEntries[o].used = false;
                }

                //Convert Photon.Realtime.Player to Scoreboard ready entries
                for (int i = 0; i < Kit_NetworkPlayerManager.instance.players.Count; i++)
                {
                    //Check if entry is available
                    if (rt_ScoreboardEntries.Count > i)
                    {
                        //Update
                        //Set name
                        rt_ScoreboardEntries[i].name = Kit_NetworkPlayerManager.instance.players[i].name;

                        rt_ScoreboardEntries[i].team = Kit_NetworkPlayerManager.instance.players[i].team;
                        rt_ScoreboardEntries[i].kills = Kit_NetworkPlayerManager.instance.players[i].kills;
                        rt_ScoreboardEntries[i].assists = Kit_NetworkPlayerManager.instance.players[i].assists;
                        rt_ScoreboardEntries[i].deaths = Kit_NetworkPlayerManager.instance.players[i].deaths;
                        rt_ScoreboardEntries[i].ping = Kit_NetworkPlayerManager.instance.players[i].ping;
                        rt_ScoreboardEntries[i].used = true;
                    }
                    else
                    {
                        //Create new
                        Kit_ScoreboardHelper entry = new Kit_ScoreboardHelper();

                        //Set name
                        entry.name = Kit_NetworkPlayerManager.instance.players[i].name;
                        entry.team = Kit_NetworkPlayerManager.instance.players[i].team;
                        entry.kills = Kit_NetworkPlayerManager.instance.players[i].kills;
                        entry.assists = Kit_NetworkPlayerManager.instance.players[i].assists;
                        entry.deaths = Kit_NetworkPlayerManager.instance.players[i].deaths;
                        entry.ping = Kit_NetworkPlayerManager.instance.players[i].ping;

                        //Set to used
                        entry.used = true;

                        //Add
                        rt_ScoreboardEntries.Add(entry);
                    }
                }

                //Convert Bots to List
                if (Kit_IngameMain.instance.currentBotManager)
                {
                    for (int i = 0; i < Kit_IngameMain.instance.currentBotManager.bots.Count; i++)
                    {
                        //Check if entry is available
                        if (rt_ScoreboardEntries.Count > i + Kit_NetworkPlayerManager.instance.players.Count)
                        {
                            //Update
                            //Set name
                            rt_ScoreboardEntries[i + Kit_NetworkPlayerManager.instance.players.Count].name = Kit_IngameMain.instance.currentBotManager.bots[i].name;
                            //Copy team
                            rt_ScoreboardEntries[i + Kit_NetworkPlayerManager.instance.players.Count].team = Kit_IngameMain.instance.currentBotManager.bots[i].team;
                            //Copy kills
                            rt_ScoreboardEntries[i + Kit_NetworkPlayerManager.instance.players.Count].kills = Kit_IngameMain.instance.currentBotManager.bots[i].kills;
                            //Copy assists
                            rt_ScoreboardEntries[i + Kit_NetworkPlayerManager.instance.players.Count].assists = Kit_IngameMain.instance.currentBotManager.bots[i].assists;
                            //Copy Deaths
                            rt_ScoreboardEntries[i + Kit_NetworkPlayerManager.instance.players.Count].deaths = Kit_IngameMain.instance.currentBotManager.bots[i].deaths;
                            //Set ping to 0
                            rt_ScoreboardEntries[i + Kit_NetworkPlayerManager.instance.players.Count].ping = 0;
                            //Set to used
                            rt_ScoreboardEntries[i + Kit_NetworkPlayerManager.instance.players.Count].used = true;
                        }
                        else
                        {
                            //Create new
                            Kit_ScoreboardHelper entry = new Kit_ScoreboardHelper();

                            //Set name
                            entry.name = Kit_IngameMain.instance.currentBotManager.bots[i].name;
                            //Copy team
                            entry.team = Kit_IngameMain.instance.currentBotManager.bots[i].team;
                            //Copy kills
                            entry.kills = Kit_IngameMain.instance.currentBotManager.bots[i].kills;
                            //Copy assists
                            entry.assists = Kit_IngameMain.instance.currentBotManager.bots[i].assists;
                            //Copy deaths
                            entry.deaths = Kit_IngameMain.instance.currentBotManager.bots[i].deaths;
                            //Set ping to 0
                            entry.ping = 0;

                            //Set to used
                            entry.used = true;

                            //Add
                            rt_ScoreboardEntries.Add(entry);
                        }
                    }
                }

                //Sort List
                rt_ScoreboardEntries = rt_ScoreboardEntries.OrderBy(x => x.kills).Reverse().ToList();

                //Different Scoreboard for team and non team game modes
                //Team Game Mode
                if (Kit_IngameMain.instance.currentPvPGameModeBehaviour.isTeamGameMode)
                {
                    //Create right amount of teams
                    if (teamActive.Count == 0)
                    {
                        for (int i = 0; i < Mathf.Clamp(Kit_IngameMain.instance.gameInformation.allPvpTeams.Length, 0, Kit_IngameMain.instance.currentPvPGameModeBehaviour.maximumAmountOfTeams); i++)
                        {
                            GameObject go = Instantiate(teamPrefab, teamGo, false);
                            teamActive.Add(go.transform as RectTransform);
                        }
                    }
                }
                //Non Team Game Mode
                else
                {
                    //Create right amount of teams
                    if (teamActive.Count == 0)
                    {
                        GameObject go = Instantiate(teamPrefab, teamGo, false);
                        teamActive.Add(go.transform as RectTransform);
                    }
                }

                int amountOfUsed = 0;

                for (int i = 0; i < rt_ScoreboardEntries.Count; i++)
                {
                    if (rt_ScoreboardEntries[i].used && rt_ScoreboardEntries[i].team >= 0)
                    {
                        if (activeEntries.Count < amountOfUsed + 1)
                        {
                            //Create new one
                            GameObject go = Instantiate(entryPrefab, teamActive[0], false);
                            //Get
                            Kit_ScoreboardUIEntry entry = go.GetComponent<Kit_ScoreboardUIEntry>();
                            //Add
                            activeEntries.Add(entry);
                        }

                        //Set it up
                        if (Kit_IngameMain.instance.currentPvPGameModeBehaviour && Kit_IngameMain.instance.currentPvPGameModeBehaviour.isTeamGameMode) activeEntries[amountOfUsed].transform.SetParent(teamActive[rt_ScoreboardEntries[i].team]);
                        else activeEntries[amountOfUsed].transform.SetParent(teamActive[0]);

                        activeEntries[amountOfUsed].nameText.text = rt_ScoreboardEntries[i].name;
                        activeEntries[amountOfUsed].kills.text = rt_ScoreboardEntries[i].kills.ToString();
                        activeEntries[amountOfUsed].ping.text = rt_ScoreboardEntries[i].ping.ToString(); ;
                        activeEntries[amountOfUsed].score.text = (rt_ScoreboardEntries[i].kills * Kit_IngameMain.instance.gameInformation.pointsPerKill).ToString();
                        activeEntries[amountOfUsed].deaths.text = rt_ScoreboardEntries[i].deaths.ToString();
                        activeEntries[amountOfUsed].assists.text = rt_ScoreboardEntries[i].assists.ToString();

                        activeEntries[amountOfUsed].gameObject.SetActiveOptimized(true);

                        amountOfUsed++;
                    }
                }

                //Deactivate other ones
                if (activeEntries.Count > amountOfUsed)
                {
                    for (int i = amountOfUsed; i < activeEntries.Count; i++)
                    {
                        activeEntries[i].gameObject.SetActiveOptimized(false);
                    }
                }
            }
        }
    }
}
