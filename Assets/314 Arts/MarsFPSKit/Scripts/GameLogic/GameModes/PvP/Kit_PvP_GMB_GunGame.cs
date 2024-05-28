using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using MarsFPSKit.Weapons;
using Random = UnityEngine.Random;
using MarsFPSKit.Spectating;
using Mirror;
using MarsFPSKit.Networking;
using System.Net.Mail;
using Unity.Services.Lobbies.Models;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MarsFPSKit
{

    [System.Serializable]
    public class GunGameOrder
    {
        /// <summary>
        /// The weapons to be used in their order
        /// </summary>
        public int[] weapons;
    }

    [CreateAssetMenu(menuName = ("MarsFPSKit/Gamemodes/Gun Game Logic"))]
    public class Kit_PvP_GMB_GunGame : Kit_PvP_GameModeBase
    {
        public GunGameOrder[] weaponOrders;

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

        public override Spectateable GetSpectateable()
        {
            return Spectateable.All;
        }

        public override bool CanJoinTeam(NetworkConnectionToClient player, int team)
        {
            //As this is GunGame, any player can join any team
            return true;
        }

        public override void GamemodeSetupServer()
        {
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

            if (filteredSpawns.Count <= 0) throw new Exception("This scene has no spawns for this game mode");

            //This game mode doesn't use different game mode for teams, so just keep it one layered
            Kit_IngameMain.instance.internalSpawns = new List<InternalSpawns>();
            //Create a new InternalSpawns instance
            InternalSpawns dmSpawns = new InternalSpawns();
            dmSpawns.spawns = filteredSpawns;
            Kit_IngameMain.instance.internalSpawns.Add(dmSpawns);

            //Set stage and timer
            Kit_IngameMain.instance.gameModeStage = 0;
            Kit_IngameMain.instance.timer = preGameTime;

            if (NetworkServer.active)
            {
                //Create network data
                GameObject nData = Instantiate(networkData, Vector3.zero, Quaternion.identity);
                Kit_IngameMain.instance.currentGameModeRuntimeData = nData.GetComponent<Kit_GameModeNetworkDataBase>();
                Kit_PvP_GMB_GunGameNetworkData ggrd = nData.GetComponent<Kit_PvP_GMB_GunGameNetworkData>();
                ggrd.currentGunOrder = UnityEngine.Random.Range(0, weaponOrders.Length);
                NetworkServer.Spawn(nData);
            }
        }

        public override void GameModeUpdate()
        {
            Kit_PvP_GMB_GunGameNetworkData drd = Kit_IngameMain.instance.currentGameModeRuntimeData as Kit_PvP_GMB_GunGameNetworkData;
            if (Time.time > drd.lastWinnerCheck)
            {
                CheckForWinner();
                drd.lastWinnerCheck = Time.time + winnerCheckTime;
            }
        }

        /// <summary>
        /// Checks all players in <see cref="PhotonNetwork.PlayerList"/> if they reached the kill limit, if the game is not over already
        /// </summary>
        void CheckForWinner()
        {
            Kit_PvP_GMB_GunGameNetworkData drd = Kit_IngameMain.instance.currentGameModeRuntimeData as Kit_PvP_GMB_GunGameNetworkData;
            //Check if someone can still win
            if (Kit_IngameMain.instance.gameModeStage < 2)
            {
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
                    //Check how many kills he has
                    //Compare with kill limit
                    if (tempPlayers[i].kills >= weaponOrders[drd.currentGunOrder].weapons.Length)
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
                //As this is GunGame, we only have one layer of spawns so we use [0]
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
                //As this is GunGame, we only have one layer of spawns so we use [0]
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

            if (botKiller)
            {

            }
            else
            {
                Kit_PvP_GMB_GunGameNetworkData drd = Kit_IngameMain.instance.currentGameModeRuntimeData as Kit_PvP_GMB_GunGameNetworkData;

                if (!drd.currentGun.ContainsKey(killer))
                {
                    drd.currentGun.Add(killer, 0);
                }
                else
                {
                    //Increase gun
                    drd.currentGun[killer]++;
                }
                //Check if we have not reached the end yet
                if (drd.currentGun[killer] < weaponOrders[drd.currentGunOrder].weapons.Length)
                {
                    Kit_PlayerBehaviour playerInstance = Kit_NetworkPlayerManager.instance.GetPlayerBehaviourById(killer);
                    //Set weapon on our player
                    if (playerInstance)
                    {
                        Kit_WeaponBase weaponInfo = Kit_IngameMain.instance.gameInformation.allWeapons[weaponOrders[drd.currentGunOrder].weapons[drd.currentGun[killer]]];
                        if (weaponInfo.GetType() == typeof(Kit_ModernWeaponScript))
                        {
                            Kit_ModernWeaponScript weapon = weaponInfo as Kit_ModernWeaponScript;
                            int[] attachments = new int[weapon.attachmentSlots.Length];

                            playerInstance.ReplaceWeapon(0, weaponOrders[drd.currentGunOrder].weapons[drd.currentGun[killer]], weapon.bulletsPerMag, weapon.bulletsToReloadAtStart, attachments);
                        }
                        else
                        {
                            playerInstance.ReplaceWeapon(0, weaponOrders[drd.currentGunOrder].weapons[drd.currentGun[killer]], 0, 0, new int[0]);
                        }
                    }
                }
            }
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
                    Debug.Log("GunGame ended. Winner: " + wonPlayer);
                    //We have a winner, tell the world (other players) about it
                    Kit_IngameMain.instance.EndGame(wonPlayer);
                }
                else
                {
                    Debug.Log("GunGame ended. No winner");
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

        public override bool CanSpawn(Kit_Bot bot)
        {
            //Check if game stage allows spawning
            if (Kit_IngameMain.instance.gameModeStage < 2)
            {
                //Check if it is a valid team
                if (bot.team >= 0 && bot.team < Kit_IngameMain.instance.gameInformation.allPvpTeams.Length) return true;
            }
            return false;
        }

        public override bool UsesCustomSpawn()
        {
            return true;
        }

        public override GameObject DoCustomSpawn(Kit_Player player, Loadout selectedLoadout)
        {
            if (CanSpawn(player))
            {
                Kit_PvP_GMB_GunGameNetworkData drd = Kit_IngameMain.instance.currentGameModeRuntimeData as Kit_PvP_GMB_GunGameNetworkData;
                Transform spawnLocation = GetSpawn(player);
                if (spawnLocation)
                {
                    //Make SURE we are not spectating.
                    if (Kit_IngameMain.instance.spectatorManager)
                    {
                        Kit_IngameMain.instance.spectatorManager.EndSpectating();
                    }

                    Loadout loadout = Kit_IngameMain.instance.loadoutMenu.GetCurrentLoadout();

                    int currentGun = 0;

                    if (drd.currentGun.ContainsKey(player.id))
                    {
                        currentGun = drd.currentGun[player.id];
                    }
                    else
                    {
                        drd.currentGun.Add(player.id, 0);
                    }

                    Debug.Log("Current gun:" + currentGun);
                    if (weaponOrders[drd.currentGunOrder].weapons.Length > currentGun)
                    {
                        loadout.loadoutWeapons = new LoadoutWeapon[1];
                        loadout.loadoutWeapons[0] = new LoadoutWeapon();
                        loadout.loadoutWeapons[0].weaponID = weaponOrders[drd.currentGunOrder].weapons[currentGun];

                        if (Kit_IngameMain.instance.gameInformation.allWeapons[loadout.loadoutWeapons[0].weaponID].GetType() == typeof(Weapons.Kit_ModernWeaponScript))
                        {
                            Weapons.Kit_ModernWeaponScript weapon = Kit_IngameMain.instance.gameInformation.allWeapons[weaponOrders[drd.currentGunOrder].weapons[currentGun]] as Weapons.Kit_ModernWeaponScript;
                            loadout.loadoutWeapons[0].attachments = new int[weapon.attachmentSlots.Length];
                        }

                        GameObject go = Instantiate(Kit_IngameMain.instance.playerPrefab, spawnLocation.position, spawnLocation.rotation);
                        Kit_PlayerBehaviour pb = go.GetComponent<Kit_PlayerBehaviour>();
                        pb.thirdPersonPlayerModelID = loadout.teamLoadout[player.team].playerModelID;
                        pb.thirdPersonPlayerModelCustomizations.AddRange(loadout.teamLoadout[player.team].playerModelCustomizations);
                        pb.weaponManager.SetupSpawnData(pb, loadout, player.serverToClientConnection);

                        return go;
                    }
                }
            }
            return null;
        }

        public override Loadout DoCustomSpawnBot(Kit_Bot bot)
        {
            Kit_PvP_GMB_GunGameNetworkData drd = Kit_IngameMain.instance.currentGameModeRuntimeData as Kit_PvP_GMB_GunGameNetworkData;
            //Get the current loadout
            Loadout curLoadout = new Loadout();
            if (weaponOrders[drd.currentGunOrder].weapons.Length > bot.kills)
            {
                curLoadout.loadoutWeapons = new LoadoutWeapon[1];
                curLoadout.loadoutWeapons[0] = new LoadoutWeapon();
                curLoadout.loadoutWeapons[0].weaponID = weaponOrders[drd.currentGunOrder].weapons[bot.kills];

                if (Kit_IngameMain.instance.gameInformation.allWeapons[curLoadout.loadoutWeapons[0].weaponID].GetType() == typeof(Weapons.Kit_ModernWeaponScript))
                {
                    Weapons.Kit_ModernWeaponScript weapon = Kit_IngameMain.instance.gameInformation.allWeapons[weaponOrders[drd.currentGunOrder].weapons[bot.kills]] as Weapons.Kit_ModernWeaponScript;
                    curLoadout.loadoutWeapons[0].attachments = new int[weapon.attachmentSlots.Length];
                }

                curLoadout.teamLoadout = new TeamLoadout[Kit_IngameMain.instance.gameInformation.allPvpTeams.Length];

                for (int i = 0; i < curLoadout.teamLoadout.Length; i++)
                {
                    curLoadout.teamLoadout[i] = new TeamLoadout();
                    curLoadout.teamLoadout[i].playerModelID = Random.Range(0, Kit_IngameMain.instance.gameInformation.allPvpTeams[i].playerModels.Length);
                    curLoadout.teamLoadout[i].playerModelCustomizations = new int[Kit_IngameMain.instance.gameInformation.allPvpTeams[i].playerModels[curLoadout.teamLoadout[i].playerModelID].prefab.GetComponent<Kit_ThirdPersonPlayerModel>().customizationSlots.Length];
                }
            }
            else
            {
                curLoadout.loadoutWeapons = new LoadoutWeapon[1];
                curLoadout.loadoutWeapons[0] = new LoadoutWeapon();
                curLoadout.loadoutWeapons[0].weaponID = weaponOrders[drd.currentGunOrder].weapons[0];

                if (Kit_IngameMain.instance.gameInformation.allWeapons[curLoadout.loadoutWeapons[0].weaponID].GetType() == typeof(Weapons.Kit_ModernWeaponScript))
                {
                    Weapons.Kit_ModernWeaponScript weapon = Kit_IngameMain.instance.gameInformation.allWeapons[weaponOrders[drd.currentGunOrder].weapons[bot.kills]] as Weapons.Kit_ModernWeaponScript;
                    curLoadout.loadoutWeapons[0].attachments = new int[weapon.attachmentSlots.Length];
                }

                curLoadout.teamLoadout = new TeamLoadout[Kit_IngameMain.instance.gameInformation.allPvpTeams.Length];

                for (int i = 0; i < curLoadout.teamLoadout.Length; i++)
                {
                    curLoadout.teamLoadout[i] = new TeamLoadout();
                    curLoadout.teamLoadout[i].playerModelID = Random.Range(0, Kit_IngameMain.instance.gameInformation.allPvpTeams[i].playerModels.Length);
                    curLoadout.teamLoadout[i].playerModelCustomizations = new int[Kit_IngameMain.instance.gameInformation.allPvpTeams[i].playerModels[curLoadout.teamLoadout[i].playerModelID].prefab.GetComponent<Kit_ThirdPersonPlayerModel>().customizationSlots.Length];
                }
            }

            return curLoadout;
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
            base.GameModeBeginMiddle();
        }

        public override bool ArePlayersEnemies(Kit_PlayerBehaviour playerOne, Kit_PlayerBehaviour playerTwo)
        {
            //Everyone can kill everyone
            return true;
        }

        public override bool ArePlayersEnemies(uint playerOneID, bool playerOneBot, uint playerTwoID, bool playerTwoBot, bool canKillSelf = false)
        {
            if (playerOneBot == playerTwoBot && playerOneID == playerTwoID && !canKillSelf) return false;
            //Everyone can kill everyone
            return true;
        }

        public override bool ArePlayersEnemies(uint playerOneID, bool playerOneBot, Kit_PlayerBehaviour playerTwo, bool canKillSelf)
        {
            if (playerOneBot == playerTwo.isBot && playerOneID == playerTwo.id && !canKillSelf) return false;
            //Everyone can kill everyone
            return true;
        }

        public override bool CanDropWeapons()
        {
            return false;
        }

        public override bool AreWeEnemies(bool botEnemy, uint enemyId)
        {
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

        public override bool LoadoutMenuSupported()
        {
            return false;
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
