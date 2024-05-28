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
    [CreateAssetMenu(menuName = ("MarsFPSKit/Gamemodes/Deathmatch Logic"))]
    public class Kit_PvP_GMB_Deathmatch : Kit_PvP_GameModeBase
    {
        /// <summary>
        /// How many kills does a player need to win the match?
        /// </summary>
        public int killLimit = 30;

        /// <summary>
        /// How many seconds apart is being checked if someone won?
        /// </summary>
        public float winnerCheckTime = 1f;

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

        public int players
        {
            get
            {
                return Kit_NetworkPlayerManager.instance.players.Count;
            }
        }

        public override Spectateable GetSpectateable()
        {
            return Spectateable.All;
        }

        public override bool CanJoinTeam(NetworkConnectionToClient player, int team)
        {
            //As this is Deathmatch, any player can join any team
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
            for (int i = 0; i < allSpawns.Length; i++)
            {
                //Check if that spawn is useable for this game mode logic
                if (allSpawns[i].pvpGameModes.Contains(this))
                {
                    //Add it to the list
                    filteredSpawns.Add(allSpawns[i]);
                }
            }

            //This game mode doesn't use different game mode for teams, so just keep it one layered
            Kit_IngameMain.instance.internalSpawns = new List<InternalSpawns>();
            //Create a new InternalSpawns instance
            InternalSpawns dmSpawns = new InternalSpawns();
            dmSpawns.spawns = filteredSpawns;
            Kit_IngameMain.instance.internalSpawns.Add(dmSpawns);

            //Set stage and timer
            Kit_IngameMain.instance.gameModeStage = 0;
            Kit_IngameMain.instance.timer = preGameTime;
        }

        public override void GameModeUpdate()
        {
            Kit_PvP_GMB_DeathmatchNetworkData drd = Kit_IngameMain.instance.currentGameModeRuntimeData as Kit_PvP_GMB_DeathmatchNetworkData;
            if (drd)
            {
                if (Time.time > drd.lastWinnerCheck)
                {
                    CheckForWinner();
                    drd.lastWinnerCheck = Time.time + winnerCheckTime;
                }
            }
        }


        /// <summary>
        /// Checks all players in <see cref="PhotonNetwork.PlayerList"/> if they reached the kill limit, if the game is not over already
        /// </summary>
        void CheckForWinner()
        {
            //Check if someone can still win
            if (Kit_IngameMain.instance.gameModeStage < 2)
            {
                List<Kit_Player> tempPlayers = new List<Kit_Player>();

                //Add normal players
                tempPlayers.AddRange(Kit_NetworkPlayerManager.instance.players);

                //Convert all bots
                if (Kit_IngameMain.instance.currentBotManager)
                {
                    for (int i = 0; i < Kit_IngameMain.instance.currentBotManager.bots.Count; i++)
                    {
                        Kit_Player player = new Kit_Player();
                        player.isBot = true;
                        player.id = Kit_IngameMain.instance.currentBotManager.bots[i].id;
                        player.kills = Kit_IngameMain.instance.currentBotManager.bots[i].kills;
                        tempPlayers.Add(player);
                    }
                }

                //Loop through all players
                for (int i = 0; i < tempPlayers.Count; i++)
                {
                    //Check how many kills he has
                    //Compare with kill limit
                    if (tempPlayers[i].kills >= killLimit)
                    {
                        //He has won. Tell the world about it!
                        Kit_IngameMain.instance.timer = endGameTime;
                        Kit_IngameMain.instance.gameModeStage = 2;

                        //Tell the world about it
                        Kit_IngameMain.instance.EndGame(tempPlayers[i]);
                        break;
                    }
                }
            }
        }


        //This can be used for a BR, made it real quick lol

        /*
        /// <summary>
        /// Checks all players in <see cref="PhotonNetwork.PlayerList"/> if they reached the kill limit, if the game is not over already
        /// </summary>
        void CheckForWinner()
        {
            //Check if someone can still win
            if (Kit_IngameMain.instance.gameModeStage < 2 && Kit_IngameMain.instance.gameModeStage > 0)
            {
                //Get all player sleft
                Kit_PlayerBehaviour[] players = FindObjectsOfType<Kit_PlayerBehaviour>();

                if (players.Length == 1)
                {
                    //That player won
                    Kit_Player winPlayer = new Kit_Player();
                    winPlayer.isBot = players[0].isBot;

                    if (players[0].isBot)
                    {
                        winPlayer.id = players[0].botId;
                    }
                    else
                    {
                        winPlayer.id = players[0].photonView.OwnerActorNr;
                    }

                    Kit_IngameMain.instance.EndGame(winPlayer);
                }
            }
        }
        */

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
                //As this is deathmatch, we only have one layer of spawns so we use [0]
                Transform spawnToTest = Kit_IngameMain.instance.internalSpawns[0].spawns[UnityEngine.Random.Range(0, Kit_IngameMain.instance.internalSpawns[0].spawns.Count)].transform;
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
                //As this is deathmatch, we only have one layer of spawns so we use [0]
                Transform spawnToTest = Kit_IngameMain.instance.internalSpawns[0].spawns[UnityEngine.Random.Range(0, Kit_IngameMain.instance.internalSpawns[0].spawns.Count)].transform;
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
            Debug.Log("Game Mode received kill");
            //Check if someone won
            CheckForWinner();
        }

        public override void TimeRunOut()
        {
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

                Kit_Player wonPlayer = GetPlayerWithMostKills();

                if (wonPlayer != null)
                {
                    Debug.Log("Deathmatch ended. Winner: " + wonPlayer);
                    //We have a winner, tell the world (other players) about it
                    Kit_IngameMain.instance.EndGame(wonPlayer);
                }
                else
                {
                    Debug.Log("Deathmatch ended. No winner");
                    //There is no winner. Tell the world about it.
                    Kit_IngameMain.instance.EndGame(999);
                }
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

        /// <summary>
        /// Returns the Photon.Realtime.Player with the most kills. If there are two or more players with the same amount of kills, noone will be returned
        /// </summary>
        /// <returns></returns>
        Kit_Player GetPlayerWithMostKills()
        {
            int maxKills = 0;
            Kit_Player toReturn = null;

            List<Kit_Player> tempPlayers = new List<Kit_Player>();

            tempPlayers.AddRange(Kit_NetworkPlayerManager.instance.players);

            //Convert all bots
            if (Kit_IngameMain.instance.currentBotManager)
            {
                for (int i = 0; i < Kit_IngameMain.instance.currentBotManager.bots.Count; i++)
                {
                    Kit_Player player = new Kit_Player();
                    player.isBot = true;
                    player.id = Kit_IngameMain.instance.currentBotManager.bots[i].id;
                    player.kills = Kit_IngameMain.instance.currentBotManager.bots[i].kills;
                    tempPlayers.Add(player);
                }
            }

            //Loop through all players
            for (int i = 0; i < tempPlayers.Count; i++)
            {
                //Compare
                if (tempPlayers[i].kills > maxKills)
                {
                    maxKills = tempPlayers[i].kills;
                    toReturn = tempPlayers[i];
                }
            }

            int amountOfPlayersWithMaxKills = 0;

            if (toReturn != null)
            {
                //If we have a player with most kills, check if two players have the same amount of kills (which would be a draw)
                for (int i = 0; i < tempPlayers.Count; i++)
                {
                    //Compare
                    if (tempPlayers[i].kills == maxKills)
                    {
                        amountOfPlayersWithMaxKills++;
                    }
                }
            }

            //If theres more than one player with most kills, return none
            if (amountOfPlayersWithMaxKills > 1) toReturn = null;

            return toReturn;
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
                    if (players >= Kit_IngameMain.instance.currentPvPGameModeBehaviour.lobbyMinimumPlayersNeeded)
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
                    if (players >= Kit_IngameMain.instance.currentPvPGameModeBehaviour.traditionalPlayerNeeded[Kit_NetworkGameInformation.instance.playersNeeded])
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
            base.GameModeBeginMiddle();
        }

        public override bool ArePlayersEnemies(Kit_PlayerBehaviour playerOne, Kit_PlayerBehaviour playerTwo)
        {
            if (playerOne == playerTwo) return false;
            //Everyone can kill everyone
            return true;
        }

        public override bool ArePlayersEnemies(uint playerOneID, bool playerOneBot, Kit_PlayerBehaviour playerTwo, bool canKillSelf)
        {
            if (playerOneBot == playerTwo.isBot && playerOneID == playerTwo.id && !canKillSelf) return false;
            //Everyone can kill everyone
            return true;
        }

        public override bool ArePlayersEnemies(uint playerOneID, bool playerOneBot, uint playerTwoID, bool playerTwoBot, bool canKillSelf = false)
        {
            if (playerOneBot && playerTwoBot && playerOneID == playerTwoID && !canKillSelf) return false;
            if (!playerOneBot && !playerTwoBot && playerOneID == playerTwoID && !canKillSelf) return false;
            //Everyone can kill everyone
            return true;
        }

        public override bool AreWeEnemies(bool botEnemy, uint enemyId)
        {
            if (!NetworkClient.active) return true;

            if (!botEnemy && enemyId == Kit_NetworkPlayerManager.instance.myId) return false;
            //Everyone is enemies
            return true;
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
