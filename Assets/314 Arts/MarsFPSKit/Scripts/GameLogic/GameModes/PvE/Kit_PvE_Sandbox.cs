using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

using Random = UnityEngine.Random;


namespace MarsFPSKit
{
    [CreateAssetMenu(menuName = "MarsFPSKit/Gamemodes/PvE/Sandbox (SP&COOP)")]
    public class Kit_PvE_Sandbox : Kit_PvE_GameModeBase
    {
        /// <summary>
        /// Possible loadouts we can spawn with
        /// </summary>
        public Loadout[] spawnLoadouts;

        public PlayerModelConfig[] spawnPlayerModels;

        public override PlayerModelConfig GetPlayerModel(Kit_PlayerBehaviour pb)
        {
            //Get random player model
            return spawnPlayerModels[Random.Range(0, spawnPlayerModels.Length)];
        }

        public override Loadout GetSpawnLoadout()
        {
            //Get random loadout
            return spawnLoadouts[Random.Range(0, spawnLoadouts.Length)];
        }

        public override bool CanControlPlayer()
        {
            return true;
        }

        public override bool CanSpawn(Kit_Player player)
        {
            return true;
        }

        public override bool CanStartVote()
        {
            return false;
        }

        public override void GameModeProceed()
        {
            //No team selection here
            Kit_IngameMain.instance.pauseMenuState = PauseMenuState.main;
            Kit_IngameMain.instance.Spawn();
        }

        public override void OnLocalPlayerDeathCameraEnded()
        {
            //Just respawn
            Kit_IngameMain.instance.Spawn();
        }

        public override void GamemodeSetupServer()
        {
            base.GamemodeSetupServer();

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
                if (allSpawns[id].singleplayerGameModes.Contains(this) && Kit_IngameMain.instance.currentGameModeType == 0)
                {
                    //Add it to the list
                    filteredSpawns.Add(allSpawns[id]);
                    //Set highest index
                    if (allSpawns[id].spawnGroupID > highestIndex) highestIndex = allSpawns[id].spawnGroupID;
                }
                else if (allSpawns[id].coopGameModes.Contains(this) && Kit_IngameMain.instance.currentGameModeType == 1)
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
        }

        public override void GameModeUpdate()
        {

        }

#if UNITY_EDITOR
        public override string[] GetSceneCheckerMessages()
        {
            throw new System.NotImplementedException();
        }

        public override MessageType[] GetSceneCheckerMessageTypes()
        {
            throw new System.NotImplementedException();
        }
#endif

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

        }

        public override void TimeRunOut()
        {

        }
    }
}