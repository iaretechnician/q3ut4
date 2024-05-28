using MarsFPSKit.Networking;
using Mirror;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace MarsFPSKit
{
    [System.Serializable]
    public class Kit_Bot
    {
        /// <summary>
        /// The ID of this bot
        /// </summary>
        public uint id;
        /// <summary>
        /// The name of this bot
        /// </summary>
        public string name;
        /// <summary>
        /// Team of this bot
        /// </summary>
        public sbyte team;
        /// <summary>
        /// Kills of this bot
        /// </summary>
        public ushort kills;
        /// <summary>
        /// Assists of this bot
        /// </summary>
        public ushort assists;
        /// <summary>
        /// Deaths of this bot
        /// </summary>
        public ushort deaths;
        /// <summary>
        /// Can this bot spawn?
        /// </summary>
        public bool canSpawn = true;
        /// <summary>
        /// Custom per-bot data that can be used by the game mode for example!
        /// </summary>
        public object customData;
        /// <summary>
        /// Custom data per bot for bots
        /// </summary>
        public List<object> pluginCustomBotData = new List<object>();
    }

    public class Kit_BotManager : NetworkBehaviour
    {
        public readonly SyncList<Kit_Bot> bots = new SyncList<Kit_Bot>();
        public List<Kit_PlayerBehaviour> activeBots = new List<Kit_PlayerBehaviour>();

        /// <summary>
        /// Bot name manager to use
        /// </summary>
        public Kit_BotNameManager nameManager;

        public Kit_BotLoadoutManager loadoutManager;

        public float spawnFrequency = 1f;
        private float lastSpawn;
        private uint lastId;

        private Kit_Bot dirtyBot = new Kit_Bot();

        void Awake()
        {
            //Assign
            Kit_IngameMain.instance.currentBotManager = this;
        }

        void Update()
        {
            if (isServer)
            {
                if (Time.time > lastSpawn)
                {
                    lastSpawn = Time.time + spawnFrequency;
                    SpawnBots();
                }
            }
        }

        void SpawnBots()
        {
            for (int i = 0; i < bots.Count; i++)
            {
                if (!IsBotAlive(bots[i]) && bots[i].canSpawn)
                {
                    SpawnBot(bots[i]);
                    break;
                }
            }
        }

        void SpawnBot(Kit_Bot bot)
        {
            if (Kit_IngameMain.instance.gameInformation.allPvpGameModes[Kit_IngameMain.instance.currentGameMode].CanSpawn(bot))
            {
                //Get a spawn
                Transform spawnLocation = Kit_IngameMain.instance.gameInformation.allPvpGameModes[Kit_IngameMain.instance.currentGameMode].GetSpawn(bot);
                if (spawnLocation)
                {
                    //Assign the values
                    if (!Kit_IngameMain.instance.currentPvPGameModeBehaviour.UsesCustomSpawn())
                    {
                        Loadout loadout = loadoutManager.GetBotLoadout();
                        GameObject go = Instantiate(Kit_IngameMain.instance.playerPrefab, spawnLocation.position, spawnLocation.rotation);
                        Kit_PlayerBehaviour pb = go.GetComponent<Kit_PlayerBehaviour>();
                        pb.myTeam = bot.team;
                        pb.isBot = true;
                        pb.id = bot.id;
                        pb.thirdPersonPlayerModelID = loadout.teamLoadout[bot.team].playerModelID;
                        pb.thirdPersonPlayerModelCustomizations.AddRange(loadout.teamLoadout[bot.team].playerModelCustomizations);
                        pb.weaponManager.SetupSpawnData(pb, loadout);
                        NetworkServer.Spawn(go);
                    }
                    else
                    {
                        //Game mode is not loadout driven
                        //Get the current loadout
                        Loadout loadout = Kit_IngameMain.instance.currentPvPGameModeBehaviour.DoCustomSpawnBot(bot);
                        GameObject go = Instantiate(Kit_IngameMain.instance.playerPrefab, spawnLocation.position, spawnLocation.rotation);
                        Kit_PlayerBehaviour pb = go.GetComponent<Kit_PlayerBehaviour>();
                        pb.myTeam = bot.team;
                        pb.isBot = true;
                        pb.id = bot.id;
                        pb.thirdPersonPlayerModelID = loadout.teamLoadout[bot.team].playerModelID;
                        pb.thirdPersonPlayerModelCustomizations.AddRange(loadout.teamLoadout[bot.team].playerModelCustomizations);
                        pb.weaponManager.SetupSpawnData(pb, loadout);
                        NetworkServer.Spawn(go);
                    }
                }
            }
        }

        /// <summary>
        /// Adds a new, empty bot
        /// </summary>
        /// <returns></returns>
        public Kit_Bot AddNewBot()
        {
            Kit_Bot newBot = new Kit_Bot();
            newBot.id = lastId;
            lastId++;
            //Get a new name
            newBot.name = nameManager.GetRandomName(this);
            //Send chat message
            Kit_IngameMain.instance.chat.SendBotMessage(newBot, "Hey guys", 0);
            bots.Add(newBot);

            for (int i = 0; i < Kit_IngameMain.instance.gameInformation.plugins.Length; i++)
            {
                Kit_IngameMain.instance.gameInformation.plugins[i].BotWasCreated(this, newBot);
            }

            return newBot;
        }

        /// <summary>
        /// Removes a random bot
        /// </summary>
        public void RemoveRandomBot()
        {
            Kit_Bot toRemove = GetBotWithID((uint)Random.Range(0, bots.Count));
            //Send chat message
            Kit_IngameMain.instance.chat.SendBotMessage(toRemove, "Bye guys", 1);
            if (IsBotAlive(toRemove))
            {
                NetworkServer.Destroy(GetAliveBot(toRemove).gameObject);
            }
            bots.Remove(toRemove);
        }

        /// <summary>
        /// Removes a bot that is in this team
        /// </summary>
        /// <param name="team"></param>
        public void RemoveBotInTeam(int team)
        {
            Kit_Bot toRemove = bots.Find(x => x.team == team);
            if (toRemove != null)
            {
                //Send chat message
                Kit_IngameMain.instance.chat.SendBotMessage(toRemove, "Bye guys", 1);
                if (IsBotAlive(toRemove))
                {
                    NetworkServer.Destroy(GetAliveBot(toRemove).gameObject);
                }
                bots.Remove(toRemove);
            }
        }

        public Kit_Bot GetBotWithID(uint id)
        {
            for (int i = 0; i < bots.Count; i++)
            {
                if (bots[i].id == id)
                {
                    return bots[i];
                }
            }
            return null;
        }

        public bool IsBotAlive(Kit_Bot bot)
        {
            for (int i = 0; i < activeBots.Count; i++)
            {
                if (activeBots[i] && activeBots[i].isBot && activeBots[i].id == bot.id)
                {
                    return true;
                }
            }
            return false;
        }

        public Kit_PlayerBehaviour GetAliveBot(Kit_Bot bot)
        {
            for (int i = 0; i < activeBots.Count; i++)
            {
                if (activeBots[i] && activeBots[i].isBot && activeBots[i].id == bot.id)
                {
                    return activeBots[i];
                }
            }
            return null;
        }

        public void AddActiveBot(Kit_PlayerBehaviour bot)
        {
            activeBots.Add(bot);
        }

        /// <summary>
        /// How many bots are there?
        /// </summary>
        public int GetAmountOfBots()
        {
            return bots.Count;
        }

        /// <summary>
        /// How many bots are in team one?
        /// </summary>
        /// <returns></returns>
        public int GetBotsInTeamX(int team)
        {
            int toReturn = 0;
            for (int i = 0; i < bots.Count; i++)
            {
                if (bots[i].team == team) toReturn++;
            }
            return toReturn;
        }

        /// <summary>
        /// How many players are there?
        /// </summary>
        /// <returns></returns>
        public int GetAmountOfPlayers()
        {
            return Kit_NetworkPlayerManager.instance.players.Count;
        }

        /// <summary>
        /// How many players are in team one?
        /// </summary>
        /// <returns></returns>
        public int GetPlayersInTeamX(int team)
        {
            int toReturn = 0;
            for (int i = 0; i < Kit_NetworkPlayerManager.instance.players.Count; i++)
            {
                if (Kit_NetworkPlayerManager.instance.players[i].team == team) toReturn++;
            }
            return toReturn;
        }

        public void KillAllBots()
        {
            if (NetworkServer.active)
            {
                Kit_PlayerBehaviour[] players = FindObjectsOfType<Kit_PlayerBehaviour>();

                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i] && players[i].isBot)
                    {
                        NetworkServer.Destroy(players[i].gameObject);
                    }
                }
            }
        }

        /// <summary>
        /// Every time a player is modified, he has to be re set, otherwise Mirror will not update his data to the other clients in the list.
        /// </summary>
        /// <param name="player"></param>
        public void ModifyBotData(Kit_Bot bot)
        {
            int index = bots.IndexOf(bot);
            bots[index] = dirtyBot;
            bots[index] = bot;
        }
    }
}