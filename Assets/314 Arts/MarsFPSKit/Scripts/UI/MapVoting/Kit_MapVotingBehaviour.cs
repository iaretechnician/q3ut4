using UnityEngine;
using System.Collections.Generic;
using Mirror;
using MarsFPSKit.Networking;

namespace MarsFPSKit
{
    /// <summary>
    /// Helper class for the map voting
    /// </summary>
    public class MapGameModeCombo
    {
        /// <summary>
        /// The map of this combo
        /// </summary>
        public int map;
        /// <summary>
        /// The game mode of this combo
        /// </summary>
        public int gameMode;
    }

    public class Kit_MapVotingBehaviour : NetworkBehaviour
    {
        /// <summary>
        /// The combos that can currently be voted for
        /// </summary>
        public readonly SyncList<MapGameModeCombo> combos = new SyncList<MapGameModeCombo>();
        /// <summary>
        /// Votes for each map are stored here and updated frequently. This list is synced.
        /// </summary>
        public readonly SyncDictionary<uint, uint> currentVotes = new SyncDictionary<uint, uint>();

        public override void OnStartServer()
        {
            //Assign
            Kit_IngameMain.instance.currentMapVoting = this;
            //Callback
            Kit_IngameMain.instance.MapVotingOpened();
            //Reset player stats for next round.
            Kit_IngameMain.instance.ResetAllStatsEndOfRound();
        }

        public override void OnStartClient()
        {
            //Assign
            Kit_IngameMain.instance.currentMapVoting = this;
            //Callback
            Kit_IngameMain.instance.MapVotingOpened();
            //Setup the UI
            Kit_IngameMain.instance.mapVotingUI.SetupVotes(this);

            currentVotes.Callback += OnVotesChanged;
        }

        void OnDestroy()
        {
            Kit_IngameMain.instance.mapVotingUI.Hide();
        }

        #region Custom Calls
        /// <summary>
        /// Returns the combo with the most votes
        /// </summary>
        /// <returns></returns>
        public MapGameModeCombo GetComboWithMostVotes()
        {
            MapGameModeCombo toReturn = combos[0];
            uint[] votes = GetVotesPerCombo();

            uint mostVotes = 0;
            int mostVotesIndex = 0;

            //Check which one has the most votes
            for (int i = 0; i < votes.Length; i++)
            {
                if (votes[i] > mostVotes)
                {
                    mostVotes = votes[i];
                    mostVotesIndex = i;
                }
            }

            //Set
            toReturn = combos[mostVotesIndex];

            //Return it
            return toReturn;
        }

        public uint[] GetVotesPerCombo()
        {
            uint[] votes = new uint[combos.Count];

            foreach (KeyValuePair<uint, uint> pair in currentVotes)
            {
                votes[pair.Value]++;
            }
            return votes;
        }

        private void OnVotesChanged(SyncIDictionary<uint, uint>.Operation op, uint key, uint item)
        {
            if (isClient)
            {
                //Update UI if we are a client
                Kit_IngameMain.instance.mapVotingUI.RedrawVotes(this);
            }
        }
        #endregion

        #region Static functions
        /// <summary>
        /// Get a new map and game mode combo. It will try to avoid things already in the used list. Depending on the amount you want that might not be possible.
        /// </summary>
        /// <param name="game">Game information to use</param>
        /// <param name="used">List of combos that are already used</param>
        /// <returns></returns>
        public static MapGameModeCombo GetMapGameModeCombo(Kit_GameInformation game, List<MapGameModeCombo> used)
        {
            //First select a random game mode and map
            int gameMode = Random.Range(0, game.allPvpGameModes.Length);
            int map = 0;

            if (Kit_GameSettings.currentNetworkingMode == KitNetworkingMode.Traditional)
            {
                map = Random.Range(0, game.allPvpGameModes[gameMode].traditionalMaps.Length);
            }
            else if (Kit_GameSettings.currentNetworkingMode == KitNetworkingMode.Lobby)
            {
                map = Random.Range(0, game.allPvpGameModes[gameMode].lobbyMaps.Length);
            }

            //To prevent an infite loop if all game modes are already used
            int tries = 0;
            while (IsGameModeUsed(gameMode, used) && tries < 10)
            {
                gameMode = Random.Range(0, game.allPvpGameModes.Length);
                tries++;
            }

            //Reset tries
            tries = 0;
            while (IsMapUsed(map, used) && tries < 10)
            {
                if (Kit_GameSettings.currentNetworkingMode == KitNetworkingMode.Traditional)
                {
                    map = Random.Range(0, game.allPvpGameModes[gameMode].traditionalMaps.Length);
                }
                else if (Kit_GameSettings.currentNetworkingMode == KitNetworkingMode.Lobby)
                {
                    map = Random.Range(0, game.allPvpGameModes[gameMode].lobbyMaps.Length);
                }
                tries++;
            }

            //Create class and return it
            return new MapGameModeCombo { map = map, gameMode = gameMode };
        }

        /// <summary>
        /// Checks if gameMode is used in the list
        /// </summary>
        /// <param name="gameMode"></param>
        /// <param name="used"></param>
        /// <returns></returns>
        static bool IsGameModeUsed(int gameMode, List<MapGameModeCombo> used)
        {
            for (int i = 0; i < used.Count; i++)
            {
                if (used[i].gameMode == gameMode)
                {
                    return true;
                }
            }
            return false;
        }

        static bool IsMapUsed(int map, List<MapGameModeCombo> used)
        {
            for (int i = 0; i < used.Count; i++)
            {
                if (used[i].map == map)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        [Command(requiresAuthority = false)]
        public void CmdVoteForMap(uint index, NetworkConnectionToClient who = null)
        {
            Kit_Player player = Kit_NetworkPlayerManager.instance.GetPlayerByConnection(who);

            if (player != null)
            {
                if (!currentVotes.ContainsKey(player.id))
                {
                    currentVotes.Add(player.id, index);
                }
                else
                {
                    currentVotes[player.id] = index;
                }
            }
        }
    }
}
