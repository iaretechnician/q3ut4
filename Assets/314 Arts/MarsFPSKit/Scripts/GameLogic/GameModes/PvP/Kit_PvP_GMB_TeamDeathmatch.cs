using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using MarsFPSKit.Spectating;
using Mirror;
using MarsFPSKit.Networking;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MarsFPSKit
{
    [CreateAssetMenu(menuName = ("MarsFPSKit/Gamemodes/Team Deathmatch Logic"))]
    public class Kit_PvP_GMB_TeamDeathmatch : Kit_PvP_GameModeBase
    {
        /// <summary>
        /// How many kills does a team need to win the match?
        /// </summary>
        public int killLimit = 75;

        [Tooltip("The maximum amount of difference the teams can have in player count")]
        /// <summary>
        /// The maximum amount of difference the teams can have in player count
        /// </summary>
        public int maxTeamDifference = 2;

        /// <summary>
        /// How many seconds need to be left in order to be able to start a vote?
        /// </summary>
        public float votingThreshold = 30f;

        [Header("Times")]
        /// <summary>
        /// How many seconds until we can start playing? This is the first countdown during which players cannot move or do anything other than spawn or chat.
        /// </summary>
        public float preGameTime = 20f;

        /// <summary>
        /// How many seconds until the map/gamemode voting menu is opened
        /// </summary>
        public float endGameTime = 10f;

        /// <summary>
        /// How many seconds do we have to vote on the next map and game mode?
        /// </summary>
        public float mapVotingTime = 20f;

        /// <summary>
        /// Spawn layer used for team one during countdown
        /// </summary>
        [Tooltip("Spawn layer used for the teams during countdown")]
        [Header("Spawns")]
        public int[] teamsInitialSpawnLayer;
        /// <summary>
        /// Spawn layer used for team two during gameplay
        /// </summary>
        [Tooltip("Spawn layer used for teams during gameplay")]
        public int[] teamsGameplaySpawnLayer;

        public override Spectateable GetSpectateable()
        {
            if (Kit_IngameMain.instance.assignedTeamID >= 0) return Spectateable.Friendlies;

            return Spectateable.All;
        }


        public override bool CanJoinTeam(NetworkConnectionToClient player, int team)
        {
            int amountOfPlayers = Kit_NetworkGameInformation.instance.playerLimit;

            //Check if the team has met its limits
            if (team == 0 && playersInTeamOne >= amountOfPlayers / 2)
            {
                return false;
            }
            else if (team == 1 && playersInTeamTwo >= amountOfPlayers / 2)
            {
                return false;
            }

            //Check if the difference is too big
            if (team == 0)
            {
                if (playersInTeamOne - playersInTeamTwo > maxTeamDifference) return false;
            }
            else if (team == 1)
            {
                if (playersInTeamTwo - playersInTeamOne > maxTeamDifference) return false;
            }

            //If none of the excluding factors were met, return true
            return true;
        }
        public override void GamemodeSetupServer()
        {
            //Create network data
            GameObject nData = Instantiate(networkData, Vector3.zero, Quaternion.identity);
            Kit_IngameMain.instance.currentGameModeRuntimeData = nData.GetComponent<Kit_GameModeNetworkDataBase>();
            NetworkServer.Spawn(nData);

            //Get all spawns
            Kit_PlayerSpawn[] allSpawns = FindObjectsOfType<Kit_PlayerSpawn>();
            //Are there any spawns at all?
            if (allSpawns.Length <= 0) throw new Exception("This scene has no spawns.");
            //Filter all spawns that are appropriate for this game mode
            List<Kit_PlayerSpawn> filteredSpawns = new List<Kit_PlayerSpawn>();
            //Highest spawn index
            int highestIndex = 0;
            for (int i = 0; i < allSpawns.Length; i++)
            {
                int id = i;
                //Check if that spawn is useable for this game mode logic
                if (allSpawns[id].pvpGameModes.Contains(this))
                {
                    //Add it to the list
                    filteredSpawns.Add(allSpawns[id]);
                    //Set highest index
                    if (allSpawns[id].spawnGroupID > highestIndex) highestIndex = allSpawns[id].spawnGroupID;
                }
            }

            Kit_IngameMain.instance.internalSpawns = new List<InternalSpawns>();
            for (int i = 0; i < (highestIndex + 1); i++)
            {
                Kit_IngameMain.instance.internalSpawns.Add(null);
            }

            for (int i = 0; i < Kit_IngameMain.instance.internalSpawns.Count; i++)
            {
                int id = i;
                Kit_IngameMain.instance.internalSpawns[id] = new InternalSpawns();
                Kit_IngameMain.instance.internalSpawns[id].spawns = new List<Kit_PlayerSpawn>();
                for (int o = 0; o < filteredSpawns.Count; o++)
                {
                    int od = o;
                    if (filteredSpawns[od].spawnGroupID == id)
                    {
                        Kit_IngameMain.instance.internalSpawns[id].spawns.Add(filteredSpawns[od]);
                    }
                }
            }

            //Set stage and timer
            Kit_IngameMain.instance.gameModeStage = 0;
            Kit_IngameMain.instance.timer = preGameTime;

            Kit_PvP_GMB_TeamDeathmatchNetworkData tdrd = Kit_IngameMain.instance.currentGameModeRuntimeData as Kit_PvP_GMB_TeamDeathmatchNetworkData;
            for (int i = 0; i < Mathf.Clamp(Kit_IngameMain.instance.gameInformation.allPvpTeams.Length, 0, maximumAmountOfTeams); i++)
            {
                tdrd.teamPoints.Add(0);
            }
            Kit_IngameMain.instance.currentGameModeRuntimeData = tdrd;
        }

        public override void GameModeUpdate()
        {

        }

        /// <summary>
        /// Checks all players in <see cref="PhotonNetwork.PlayerList"/> if they reached the kill limit, if the game is not over already
        /// </summary>
        void CheckForWinner()
        {
            //Check if someone can still win
            if (Kit_IngameMain.instance.gameModeStage < 2)
            {
                Kit_PvP_GMB_TeamDeathmatchNetworkData drd = Kit_IngameMain.instance.currentGameModeRuntimeData as Kit_PvP_GMB_TeamDeathmatchNetworkData;

                for (int i = 0; i < drd.teamPoints.Count; i++)
                {
                    if (drd.teamPoints[i] >= killLimit)
                    {
                        //End Game
                        Kit_IngameMain.instance.EndGame((uint)i, drd.teamPoints.ToArray());
                        //Set game stage
                        Kit_IngameMain.instance.timer = endGameTime;
                        Kit_IngameMain.instance.gameModeStage = 2;
                        break;
                    }
                }
            }
        }

        int players
        {
            get
            {
                return Kit_NetworkPlayerManager.instance.players.Count;
            }
        }

        /// <summary>
        /// How many players are in team one?
        /// </summary>
        int playersInTeamOne
        {
            get
            {
                int toReturn = 0;

                //Loop through all players
                for (int i = 0; i < Kit_NetworkPlayerManager.instance.players.Count; i++)
                {
                    if (Kit_NetworkPlayerManager.instance.players[i].team == 0) toReturn++;
                }

                //Return
                return toReturn;
            }
        }

        /// <summary>
        /// How many players are in team two?
        /// </summary>
        int playersInTeamTwo
        {
            get
            {
                int toReturn = 0;

                //Loop through all players
                for (int i = 0; i < Kit_NetworkPlayerManager.instance.players.Count; i++)
                {
                    if (Kit_NetworkPlayerManager.instance.players[i].team == 1) toReturn++;
                }

                //Return
                return toReturn;
            }
        }

        public override Transform GetSpawn(Kit_Player player)
        {
            //Define spawn tries
            int tries = 0;
            Transform spawnToReturn = null;
            //Try to get a spawn
            while (!spawnToReturn)
            {
                //To prevent an unlimited loop, only do it ten times
                if (tries >= 10)
                {
                    break;
                }

                int layer = 0;

                if (Kit_IngameMain.instance.gameModeStage == 0)
                {
                    layer = teamsInitialSpawnLayer[Mathf.Clamp(player.team, 0, teamsInitialSpawnLayer.Length - 1)];
                }
                else
                {
                    layer = teamsGameplaySpawnLayer[Mathf.Clamp(player.team, 0, teamsGameplaySpawnLayer.Length - 1)];
                }

                //Team deathmatch has no fixed spawns in this behaviour. Only use one layer
                Transform spawnToTest = Kit_IngameMain.instance.internalSpawns[layer].spawns[UnityEngine.Random.Range(0, Kit_IngameMain.instance.internalSpawns[layer].spawns.Count)].transform;
                //Test the spawn
                if (spawnToTest)
                {
                    if (spawnSystemToUse.CheckSpawnPosition(spawnToTest, player))
                    {
                        //Assign spawn
                        spawnToReturn = spawnToTest;
                        //Break the while loop
                        break;
                    }
                }
                tries++;
            }

            return spawnToReturn;
        }
        public override Transform GetSpawn(Kit_Bot bot)
        {
            //Define spawn tries
            int tries = 0;
            Transform spawnToReturn = null;
            //Try to get a spawn
            while (!spawnToReturn)
            {
                //To prevent an unlimited loop, only do it ten times
                if (tries >= 10)
                {
                    break;
                }
                int layer = 0;

                if (Kit_IngameMain.instance.gameModeStage == 0)
                {
                    layer = teamsInitialSpawnLayer[Mathf.Clamp(bot.team, 0, teamsInitialSpawnLayer.Length - 1)];
                }
                else
                {
                    layer = teamsInitialSpawnLayer[Mathf.Clamp(bot.team, 0, teamsGameplaySpawnLayer.Length - 1)];
                }

                //Team deathmatch has no fixed spawns in this behaviour. Only use one layer
                Transform spawnToTest = Kit_IngameMain.instance.internalSpawns[layer].spawns[UnityEngine.Random.Range(0, Kit_IngameMain.instance.internalSpawns[layer].spawns.Count)].transform;
                //Test the spawn
                if (spawnToTest)
                {
                    if (spawnSystemToUse.CheckSpawnPosition(spawnToTest, bot))
                    {
                        //Assign spawn
                        spawnToReturn = spawnToTest;
                        //Break the while loop
                        break;
                    }
                }
                tries++;
            }

            return spawnToReturn;
        }

        public override void PlayerDied(bool botKiller, uint killer, bool botKilled, uint killed)
        {
            Kit_PvP_GMB_TeamDeathmatchNetworkData drd = Kit_IngameMain.instance.currentGameModeRuntimeData as Kit_PvP_GMB_TeamDeathmatchNetworkData;
            if (botKiller)
            {
                if (Kit_IngameMain.instance.currentBotManager)
                {
                    //Check if he killed himself
                    if (!botKilled || killed != killer)
                    {
                        //Get bot
                        Kit_Bot killerBot = Kit_IngameMain.instance.currentBotManager.GetBotWithID(killer);
                        if (killerBot != null)
                        {
                            //Check in which team the killer is
                            int killerTeam = killerBot.team;
                            //Increase points
                            drd.teamPoints[killerTeam]++;
                        }
                    }
                }
            }
            else
            {
                //Check if he killed himself
                if (botKilled || killed != killer)
                {
                    Kit_Player playerKiller = null;
                    //Get player
                    for (int i = 0; i < Kit_NetworkPlayerManager.instance.players.Count; i++)
                    {
                        if (Kit_NetworkPlayerManager.instance.players[i].id == killer)
                        {
                            playerKiller = Kit_NetworkPlayerManager.instance.players[i];
                            break;
                        }
                    }

                    if (playerKiller != null)
                    {
                        //Check in which team the killer is
                        int killerTeam = playerKiller.team;
                        //Increase points
                        drd.teamPoints[killerTeam]++;
                    }
                }
            }
            //Check if a team has won
            CheckForWinner();
        }

        public override void TimeRunOut()
        {
            Kit_PvP_GMB_TeamDeathmatchNetworkData drd = Kit_IngameMain.instance.currentGameModeRuntimeData as Kit_PvP_GMB_TeamDeathmatchNetworkData;

            //Check stage
            if (Kit_IngameMain.instance.gameModeStage == 0)
            {
                if (Kit_GameSettings.currentNetworkingMode == KitNetworkingMode.Traditional)
                {
                    //Pre game time to main game
                    Kit_IngameMain.instance.timer = Kit_IngameMain.instance.currentPvPGameModeBehaviour.traditionalDurations[Kit_GameSettings.gameLength];
                }
                else if (Kit_GameSettings.currentNetworkingMode == KitNetworkingMode.Lobby)
                {
                    //Pre game time to main game
                    Kit_IngameMain.instance.timer = Kit_IngameMain.instance.currentPvPGameModeBehaviour.lobbyGameDuration;
                }
                Kit_IngameMain.instance.gameModeStage = 1;
            }
            //Time run out, determine winner
            else if (Kit_IngameMain.instance.gameModeStage == 1)
            {
                Kit_IngameMain.instance.timer = endGameTime;
                Kit_IngameMain.instance.gameModeStage = 2;

                //Get most points
                int mostPoints = drd.teamPoints.Max();

                int teamWon = 1000;

                for (int i = 0; i < drd.teamPoints.Count; i++)
                {
                    if (drd.teamPoints[i] == mostPoints)
                    {
                        //No other team has won yet
                        if (teamWon == 1000)
                        {
                            int id = i;
                            teamWon = id;
                        }
                        //Another team has as many points as we have
                        else
                        {
                            //That means draw!
                            teamWon = 999;
                            break;
                        }
                    }
                }

                //End game according to results
                Kit_IngameMain.instance.EndGame((uint)teamWon, drd.teamPoints.ToArray());
            }
            //Victory screen is over. Proceed to map voting.
            else if (Kit_IngameMain.instance.gameModeStage == 2)
            {
                //Destroy victory screen
                if (Kit_IngameMain.instance.currentVictoryScreen)
                {
                    NetworkServer.Destroy(Kit_IngameMain.instance.currentVictoryScreen.gameObject);
                }
                if (Kit_GameSettings.currentNetworkingMode == KitNetworkingMode.Traditional)
                {
                    //Set time and stage
                    Kit_IngameMain.instance.timer = mapVotingTime;
                    Kit_IngameMain.instance.gameModeStage = 3;
                    //Open the voting menu
                    Kit_IngameMain.instance.OpenVotingMenu();
                    //Delete all players
                    Kit_IngameMain.instance.DeleteAllPlayers();
                }
                else if (Kit_GameSettings.currentNetworkingMode == KitNetworkingMode.Lobby)
                {
                    //Delete all players
                    Kit_IngameMain.instance.DeleteAllPlayers();
                    Kit_IngameMain.instance.gameModeStage = 5;
                    //Load MM
                    Kit_SceneSyncer.instance.LoadScene("MainMenu");
                }
            }
            //End countdown is over, start new game
            else if (Kit_IngameMain.instance.gameModeStage == 3)
            {
                //Load new map / game mode
                Kit_IngameMain.instance.gameModeStage = 4;

                //Lets load the appropriate map
                //Get combo
                MapGameModeCombo nextCombo = Kit_IngameMain.instance.currentMapVoting.GetComboWithMostVotes();
                //Delete map voting
                NetworkServer.Destroy(Kit_IngameMain.instance.currentMapVoting.gameObject);
                Kit_NetworkGameInformation.instance.gameMode = nextCombo.gameMode;
                Kit_NetworkGameInformation.instance.map = nextCombo.map;

                //Load the map
                Kit_SceneSyncer.instance.LoadScene(Kit_IngameMain.instance.gameInformation.allPvpGameModes[nextCombo.gameMode].traditionalMaps[nextCombo.map].sceneName);
            }
        }

        public override bool CanSpawn(Kit_Player player)
        {
            //Check if game stage allows spawning
            if (Kit_IngameMain.instance.gameModeStage < 2)
            {
                //Check if it is a valid team
                if (player.team >= 0 && player.team < Kit_IngameMain.instance.gameInformation.allPvpTeams.Length) return true;
            }
            return false;
        }

        public override bool CanControlPlayer()
        {
            //While we are waiting for enough players, we can move!
            if (!AreEnoughPlayersThere() && !Kit_IngameMain.instance.hasGameModeStarted) return true;
            //We can only control our player if we are in the main phase
            return Kit_IngameMain.instance.gameModeStage == 1;
        }

        public override bool AreEnoughPlayersThere()
        {
            //If there are bots ...
            if (Kit_NetworkGameInformation.instance.bots)
            {
                return true;
            }
            else
            {
                if (Kit_NetworkGameInformation.instance.isLobby)
                {
                    if (Kit_NetworkPlayerManager.instance.players.Count >= Kit_IngameMain.instance.currentPvPGameModeBehaviour.lobbyMinimumPlayersNeeded)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    //If there are 2 or more players, we can play!
                    if (Kit_NetworkPlayerManager.instance.players.Count >= Kit_IngameMain.instance.currentPvPGameModeBehaviour.traditionalPlayerNeeded[Kit_NetworkGameInformation.instance.playersNeeded])
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public override void GameModeBeginMiddle()
        {
            if (Kit_IngameMain.instance.currentGameModeRuntimeData != null && Kit_IngameMain.instance.currentGameModeRuntimeData.GetType() == typeof(Kit_PvP_GMB_TeamDeathmatchNetworkData))
            {
                Kit_PvP_GMB_TeamDeathmatchNetworkData drd = Kit_IngameMain.instance.currentGameModeRuntimeData as Kit_PvP_GMB_TeamDeathmatchNetworkData;
                //Reset score
                for (int i = 0; i < drd.teamPoints.Count; i++)
                {
                    drd.teamPoints[i] = 0;
                }
            }

            base.GameModeBeginMiddle();
        }

        public override bool ArePlayersEnemies(uint playerOneID, bool playerOneBot, uint playerTwoID, bool playerTwoBot, bool canKillSelf = false)
        {
            if (playerTwoBot && playerOneBot && playerOneID == playerTwoID && canKillSelf) return true;

            int teamOne = -1;
            int teamTwo = -2;

            if (playerOneBot)
            {
                Kit_Bot bot = Kit_IngameMain.instance.currentBotManager.GetBotWithID(playerOneID);
                teamOne = bot.team;
            }
            else
            {
                Kit_Player player = Kit_NetworkPlayerManager.instance.GetPlayerById(playerOneID);
                teamOne = player.team;
            }

            if (playerTwoBot)
            {
                Kit_Bot bot = Kit_IngameMain.instance.currentBotManager.GetBotWithID(playerTwoID);
                teamTwo = bot.team;
            }
            else
            {
                Kit_Player player = Kit_NetworkPlayerManager.instance.GetPlayerById(playerTwoID);
                teamTwo = player.team;
            }

            if (teamOne != teamTwo) return true;
            return false;
        }

        public override bool ArePlayersEnemies(Kit_PlayerBehaviour playerOne, Kit_PlayerBehaviour playerTwo)
        {
            if (playerOne.myTeam != playerTwo.myTeam) return true;
            return false;
        }

        public override bool ArePlayersEnemies(uint playerOneID, bool playerOneBot, Kit_PlayerBehaviour playerTwo, bool canKillSelf)
        {
            if (playerTwo.isBot == playerOneBot && playerOneID == playerTwo.id && canKillSelf) return true;

            int oneTeam = -1;

            if (playerOneBot)
            {
                Kit_Bot bot = Kit_IngameMain.instance.currentBotManager.GetBotWithID(playerOneID);
                oneTeam = bot.team;
            }
            else
            {
                Kit_Player player = Kit_NetworkPlayerManager.instance.GetPlayerById(playerOneID);
                oneTeam = player.team;
            }

            if (oneTeam != playerTwo.myTeam) return true;

            return false;
        }

        public override bool AreWeEnemies(bool botEnemy, uint enemyId)
        {
            if (!NetworkClient.active) return true;

            //So that we can blind/kill ourselves with grenades
            if (!botEnemy && enemyId == Kit_NetworkPlayerManager.instance.GetLocalPlayer().id) return true;

            int enemyTeam = -1;

            if (botEnemy)
            {
                Kit_Bot bot = Kit_IngameMain.instance.currentBotManager.GetBotWithID(enemyId);
                if (bot != null)
                    enemyTeam = bot.team;
                else //If he doesn't exist, we can't be enemies
                    return false;
            }
            else
            {
                Kit_Player player = Kit_NetworkPlayerManager.instance.GetPlayerById(enemyId);
                enemyTeam = player.team;
            }

            if (Kit_IngameMain.instance.assignedTeamID != enemyTeam) return true;

            return false;
        }

        public override bool CanStartVote()
        {
            //While we are waiting for enough players, we can vote!
            if (!AreEnoughPlayersThere() && !Kit_IngameMain.instance.hasGameModeStarted) return true;
            //We can only vote during the main phase and if enough time is left
            return Kit_IngameMain.instance.gameModeStage == 1 && Kit_IngameMain.instance.timer > votingThreshold;
        }

#if UNITY_EDITOR
        public override string[] GetSceneCheckerMessages()
        {
            string[] toReturn = new string[2];
            //Find spawns
            Kit_PlayerSpawn[] spawns = FindObjectsOfType<Kit_PlayerSpawn>();
            //Get good spawns
            List<Kit_PlayerSpawn> spawnsForThisGameMode = new List<Kit_PlayerSpawn>();
            for (int i = 0; i < spawns.Length; i++)
            {
                if (spawns[i].pvpGameModes.Contains(this))
                {
                    spawnsForThisGameMode.Add(spawns[i]);
                }
            }

            if (spawnsForThisGameMode.Count <= 0)
            {
                toReturn[0] = "[Spawns] No spawns for this game mode found!";
            }
            else if (spawnsForThisGameMode.Count <= 6)
            {
                toReturn[0] = "[Spawns] Maybe you should add a few more";
            }
            else
            {
                toReturn[0] = "[Spawns] All good.";
            }

            Kit_BotNavPoint[] navPoints = FindObjectsOfType<Kit_BotNavPoint>();
            List<Kit_BotNavPoint> navPointsForThis = new List<Kit_BotNavPoint>();

            for (int i = 0; i < navPoints.Length; i++)
            {
                if (navPoints[i].gameModes.Contains(this))
                {
                    navPointsForThis.Add(navPoints[i]);
                }
            }

            if (navPointsForThis.Count <= 0)
            {
                toReturn[1] = "[Nav Points] No nav points for this game mode found!";
            }
            else if (navPointsForThis.Count <= 6)
            {
                toReturn[1] = "[Nav Points] Maybe you should add a few more";
            }
            else
            {
                toReturn[1] = "[Nav Points] All good.";
            }

            return toReturn;
        }

        public override MessageType[] GetSceneCheckerMessageTypes()
        {
            MessageType[] toReturn = new MessageType[2];
            //Find spawns
            Kit_PlayerSpawn[] spawns = FindObjectsOfType<Kit_PlayerSpawn>();
            //Get good spawns
            List<Kit_PlayerSpawn> spawnsForThisGameMode = new List<Kit_PlayerSpawn>();
            for (int i = 0; i < spawns.Length; i++)
            {
                if (spawns[i].pvpGameModes.Contains(this))
                {
                    spawnsForThisGameMode.Add(spawns[i]);
                }
            }

            if (spawnsForThisGameMode.Count <= 0)
            {
                toReturn[0] = MessageType.Error;
            }
            else if (spawnsForThisGameMode.Count <= 6)
            {
                toReturn[0] = MessageType.Warning;
            }
            else
            {
                toReturn[0] = MessageType.Info;
            }


            Kit_BotNavPoint[] navPoints = FindObjectsOfType<Kit_BotNavPoint>();
            List<Kit_BotNavPoint> navPointsForThis = new List<Kit_BotNavPoint>();

            for (int i = 0; i < navPoints.Length; i++)
            {
                if (navPoints[i].gameModes.Contains(this))
                {
                    navPointsForThis.Add(navPoints[i]);
                }
            }

            if (navPointsForThis.Count <= 0)
            {
                toReturn[1] = MessageType.Error;
            }
            else if (navPointsForThis.Count <= 6)
            {
                toReturn[1] = MessageType.Warning;
            }
            else
            {
                toReturn[1] = MessageType.Info;
            }

            return toReturn;
        }
#endif
    }
}
