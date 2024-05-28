using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MarsFPSKit.Networking;
using Unity.Services.Lobbies.Models;
using Mirror;

namespace MarsFPSKit
{
    public class Kit_VotingUIDefault : Kit_VotingUIBase
    {
        public GameObject votingPrefab;

        public enum MenuState { Category, Player, Map, GameMode }
        private MenuState currentMenuState = MenuState.Category;
        /// <summary>
        /// Id of this menu
        /// </summary>
        public int menuIdMainPage;
        /// <summary>
        /// Sub page
        /// </summary>
        public int menuIdSubPage;

        [Header("Vote Start")]
        /// <summary>
        /// The prefab for the selection menu (Players, Maps, Game Modes)
        /// </summary>
        public GameObject voteMenuSelectionPrefab;
        /// <summary>
        /// Where the prefab is going to be parented to
        /// </summary>
        public RectTransform voteMenuSelectionGO;
        /// <summary>
        /// Currently active entries
        /// </summary>
        public List<GameObject> voteMenuSelectionEntries = new List<GameObject>();
        /// <summary>
        /// The back button in the selection list
        /// </summary>
        public GameObject voteMenuSelectionBack;

        /// <summary>
        /// How many seconds need to pass until we can start another votE?
        /// </summary>
        public float votingCooldown = 60f;

        /// <summary>
        /// When have we started a vote for the last time?
        /// </summary>
        private float lastVote;

        [Header("Mid Round Vote")]
        public GameObject mrvRoot;
        /// <summary>
        /// Displays the username who started
        /// </summary>
        public TextMeshProUGUI voteStartedBy;
        /// <summary>
        /// Displays what is being voted on
        /// </summary>
        public TextMeshProUGUI voteDescription;
        /// <summary>
        /// Displays our own vote OR the controls
        /// </summary>
        public TextMeshProUGUI voteOwn;
        /// <summary>
        /// Displays the amount of yes votes
        /// </summary>
        public TextMeshProUGUI yesVotes;
        /// <summary>
        /// Displays the amount of no votes
        /// </summary>
        public TextMeshProUGUI noVotes;

        private void Start()
        {
            Kit_NetworkPlayerManager.instance.onPlayerJoined.AddListener(OnPlayerEnteredRoom);
            Kit_NetworkPlayerManager.instance.onPlayerLeft.AddListener(OnPlayerLeftRoom);
        }

        private void OnDestroy()
        {
            Kit_NetworkPlayerManager.instance.onPlayerJoined.RemoveListener(OnPlayerEnteredRoom);
            Kit_NetworkPlayerManager.instance.onPlayerLeft.RemoveListener(OnPlayerLeftRoom);
        }

        public override void OpenVotingMenu()
        {
            if (Kit_IngameMain.instance.currentPvPGameModeBehaviour.CanStartVote())
            {
                if (Time.time > lastVote)
                {
                    if (Kit_GameSettings.currentNetworkingMode == KitNetworkingMode.Traditional)
                    {
                        if (Kit_IngameMain.instance.SwitchMenu(menuIdMainPage))
                        {
                            //Set state
                            currentMenuState = MenuState.Category;
                        }
                    }
                }
                else
                {
                    Kit_IngameMain.instance.DisplayMessage("You need to wait " + (lastVote - Time.time).ToString("F0") + " seconds before you can start another vote!");
                    BackToPauseMenu();
                }
            }
            else
            {
                Kit_IngameMain.instance.DisplayMessage("A vote can currently not be started!");
                BackToPauseMenu();
            }
        }

        public override void CloseVotingMenu()
        {

        }

        public void KickPlayer()
        {
            if (Kit_NetworkPlayerManager.instance.players.Count > 1)
            {
                //Clear list
                for (int i = 0; i < voteMenuSelectionEntries.Count; i++)
                {
                    Destroy(voteMenuSelectionEntries[i]);
                }
                voteMenuSelectionEntries = new List<GameObject>();

                //Loop through all players and list them
                for (int i = 0; i < Kit_NetworkPlayerManager.instance.players.Count; i++)
                {
                    //Check if its not us
                    if (!Kit_NetworkPlayerManager.instance.players[i].isLocal)
                    {
                        //Instantiate
                        GameObject go = Instantiate(voteMenuSelectionPrefab, voteMenuSelectionGO, false);
                        //Get Entry
                        Kit_VotingSelectionEntry entry = go.GetComponent<Kit_VotingSelectionEntry>();
                        int current = i; //This is necessary, otherwise 'i' would change.
                                         //Set name
                        entry.text.text = Kit_NetworkPlayerManager.instance.players[i].name;
                        //Add delegate
                        entry.btn.onClick.AddListener(delegate { StartVotePlayer(Kit_NetworkPlayerManager.instance.players[current]); });

                        //Add to list
                        voteMenuSelectionEntries.Add(go);
                    }
                }

                //Move back button to the lower part
                voteMenuSelectionBack.transform.SetAsLastSibling();

                LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);

                if (Kit_IngameMain.instance.SwitchMenu(menuIdSubPage))
                {
                    //Set menu state
                    currentMenuState = MenuState.Player;
                }
            }
            else
            {
                BackToCategory();
                Kit_IngameMain.instance.DisplayMessage("Only you are in this room!");
            }
        }

        public void ChangeMap()
        {
            if (Kit_GameSettings.currentNetworkingMode == KitNetworkingMode.Traditional)
            {
                if (Kit_IngameMain.instance.currentPvPGameModeBehaviour.traditionalMaps.Length > 1)
                {
                    //Clear list
                    for (int i = 0; i < voteMenuSelectionEntries.Count; i++)
                    {
                        Destroy(voteMenuSelectionEntries[i]);
                    }
                    voteMenuSelectionEntries = new List<GameObject>();

                    int currentMap = Kit_IngameMain.instance.gameInformation.GetCurrentLevel();

                    //Loop through all maps and list them
                    for (uint i = 0; i < Kit_IngameMain.instance.currentPvPGameModeBehaviour.traditionalMaps.Length; i++)
                    {
                        //Check if its not the current map
                        if (i != currentMap)
                        {
                            //Instantiate
                            GameObject go = Instantiate(voteMenuSelectionPrefab, voteMenuSelectionGO, false);
                            //Get Entry
                            Kit_VotingSelectionEntry entry = go.GetComponent<Kit_VotingSelectionEntry>();
                            uint current = i; //This is necessary, otherwise 'i' would change.
                                             //Set name
                            entry.text.text = Kit_IngameMain.instance.currentPvPGameModeBehaviour.traditionalMaps[i].mapName;
                            //Add delegate
                            entry.btn.onClick.AddListener(delegate { StartVoteMap(current); });
                            //Add to list
                            voteMenuSelectionEntries.Add(go);
                        }
                    }

                    //Move back button to the lower part
                    voteMenuSelectionBack.transform.SetAsLastSibling();

                    if (Kit_IngameMain.instance.SwitchMenu(menuIdSubPage))
                    {
                        //Set state
                        currentMenuState = MenuState.Map;
                    }
                }
                else
                {
                    Kit_IngameMain.instance.DisplayMessage("Only one map is in this game!");
                    BackToCategory();
                }
            }
            else if (Kit_GameSettings.currentNetworkingMode == KitNetworkingMode.Lobby)
            {
                if (Kit_IngameMain.instance.currentPvPGameModeBehaviour.lobbyMaps.Length > 1)
                {
                    //Clear list
                    for (int i = 0; i < voteMenuSelectionEntries.Count; i++)
                    {
                        Destroy(voteMenuSelectionEntries[i]);
                    }
                    voteMenuSelectionEntries = new List<GameObject>();

                    int currentMap = Kit_IngameMain.instance.gameInformation.GetCurrentLevel();

                    //Loop through all maps and list them
                    for (uint i = 0; i < Kit_IngameMain.instance.currentPvPGameModeBehaviour.lobbyMaps.Length; i++)
                    {
                        //Check if its not the current map
                        if (i != currentMap)
                        {
                            //Instantiate
                            GameObject go = Instantiate(voteMenuSelectionPrefab, voteMenuSelectionGO, false);
                            //Get Entry
                            Kit_VotingSelectionEntry entry = go.GetComponent<Kit_VotingSelectionEntry>();
                            uint current = i; //This is necessary, otherwise 'i' would change.
                                             //Set name
                            entry.text.text = Kit_IngameMain.instance.currentPvPGameModeBehaviour.lobbyMaps[i].mapName;
                            //Add delegate
                            entry.btn.onClick.AddListener(delegate { StartVoteMap(current); });
                            //Add to list
                            voteMenuSelectionEntries.Add(go);
                        }
                    }

                    if (Kit_IngameMain.instance.SwitchMenu(menuIdSubPage))
                    {
                        //Move back button to the lower part
                        voteMenuSelectionBack.transform.SetAsLastSibling();

                        //Set state
                        currentMenuState = MenuState.Map;
                    }
                }
                else
                {
                    Kit_IngameMain.instance.DisplayMessage("Only one map is in this game!");
                    BackToCategory();
                }
            }
        }

        public void ChangeGameMode()
        {
            if (Kit_GameSettings.currentNetworkingMode == KitNetworkingMode.Traditional)
            {
                if (Kit_IngameMain.instance.gameInformation.allPvpGameModes.Length > 1)
                {
                    //Clear list
                    for (int i = 0; i < voteMenuSelectionEntries.Count; i++)
                    {
                        Destroy(voteMenuSelectionEntries[i]);
                    }
                    voteMenuSelectionEntries = new List<GameObject>();

                    //Loop through all game modes and list them
                    for (uint i = 0; i < Kit_IngameMain.instance.gameInformation.allPvpGameModes.Length; i++)
                    {
                        //Check if its not the current map
                        if (i != Kit_IngameMain.instance.currentGameMode)
                        {
                            //Instantiate
                            GameObject go = Instantiate(voteMenuSelectionPrefab, voteMenuSelectionGO, false);
                            //Get Entry
                            Kit_VotingSelectionEntry entry = go.GetComponent<Kit_VotingSelectionEntry>();
                            uint current = i; //This is necessary, otherwise 'i' would change.
                                             //Set name
                            entry.text.text = Kit_IngameMain.instance.gameInformation.allPvpGameModes[i].gameModeName;
                            //Add delegate
                            entry.btn.onClick.AddListener(delegate { StartVoteGameMode(current); });

                            //Add to list
                            voteMenuSelectionEntries.Add(go);
                        }
                    }

                    if (Kit_IngameMain.instance.SwitchMenu(menuIdSubPage))
                    {
                        //Move back button to the lower part
                        voteMenuSelectionBack.transform.SetAsLastSibling();

                        //Set state
                        currentMenuState = MenuState.GameMode;
                    }
                }
                else
                {
                    Kit_IngameMain.instance.DisplayMessage("This game only has one game mode!");
                }
            }
        }

        public void StartVotePlayer(Kit_Player player)
        {
            //Send Event
            if (player != null)
            {
                //Set timer
                lastVote = Time.time + votingCooldown;
                //Send request
                Kit_IngameMain.instance.CmdStartVote(0, player.id);
            }

            BackToPauseMenu();
        }

        public void StartVoteMap(uint map)
        {
            //Set timer
            lastVote = Time.time + votingCooldown;
            //Send request
            Kit_IngameMain.instance.CmdStartVote(1, map);
            BackToPauseMenu();
        }

        public void StartVoteGameMode(uint gameMode)
        {
            //Set timer
            lastVote = Time.time + votingCooldown;
            //Send request
            Kit_IngameMain.instance.CmdStartVote(2, gameMode);
            BackToPauseMenu();
        }

        public void BackToCategory()
        {
            if (Kit_IngameMain.instance.SwitchMenu(menuIdMainPage))
            {
                //Set state
                currentMenuState = MenuState.Category;
            }
        }

        public void BackToPauseMenu()
        {
            Kit_IngameMain.instance.SwitchMenu(Kit_IngameMain.instance.pauseMenu.pauseMenuId);
        }

        public override void RedrawVotingUI(Kit_VotingBase voting)
        {
            if (voting)
            {
                if (!mrvRoot.activeSelf)
                    mrvRoot.SetActive(true);
                if (voteStartedBy)
                {
                    //Set who started it
                    voteStartedBy.text = Kit_NetworkPlayerManager.instance.GetPlayerById(voting.voteStartedBy).name;
                }
                if (voteDescription)
                {
                    //Update description
                    if (voting.votingOn == Kit_VotingBase.VotingOn.Kick)
                    {
                        Kit_Player toKick = Kit_NetworkPlayerManager.instance.GetPlayerById(voting.argument);
                        if (toKick != null)
                        {
                            voteDescription.text = "Kick player: " + toKick.name;
                        }
                        else
                        {
                            voteDescription.text = "Kick player: ";
                        }
                    }
                    else if (voting.votingOn == Kit_VotingBase.VotingOn.Map)
                    {
                        if (Kit_GameSettings.currentNetworkingMode == KitNetworkingMode.Traditional)
                        {
                            voteDescription.text = "Switch map to: " + Kit_IngameMain.instance.currentPvPGameModeBehaviour.traditionalMaps[voting.argument].mapName;
                        }
                        else if (Kit_GameSettings.currentNetworkingMode == KitNetworkingMode.Lobby)
                        {
                            voteDescription.text = "Switch map to: " + Kit_IngameMain.instance.currentPvPGameModeBehaviour.lobbyMaps[voting.argument].mapName;
                        }
                    }
                    else if (voting.votingOn == Kit_VotingBase.VotingOn.GameMode)
                    {
                        voteDescription.text = "Switch Game Mode to: " + Kit_IngameMain.instance.gameInformation.allPvpGameModes[voting.argument].gameModeName;
                    }
                }
                if (yesVotes)
                {
                    //Yes Votes
                    yesVotes.text = voting.GetYesVotes().ToString();
                }
                if (noVotes)
                {
                    //No votes
                    noVotes.text = voting.GetNoVotes().ToString();
                }
                if (voteOwn)
                {
                    //Own vote
                    if (voting.myVote == 0)
                    {
                        voteOwn.text = "F1 <color=#00ff00ff>YES</color> F2 <color=#ff0000ff>NO</color>";
                    }
                    else if (voting.myVote == 1)
                    {
                        voteOwn.text = "You voted <color=#ff0000ff>NO</color>";
                    }
                    else if (voting.myVote == 2)
                    {
                        voteOwn.text = "You voted <color=#00ff00ff>YES</color>";
                    }
                }
            }
        }

        public override void VoteEnded(Kit_VotingBase voting)
        {
            //Hide the UI
            mrvRoot.SetActive(false);
        }

        public void OnPlayerEnteredRoom(Kit_Player newPlayer)
        {
            if (currentMenuState == MenuState.Player)
            {
                //Redraw
                KickPlayer();
            }
        }

        public void OnPlayerLeftRoom(Kit_Player otherPlayer)
        {
            if (currentMenuState == MenuState.Player)
            {
                if (Kit_NetworkPlayerManager.instance.players.Count > 1)
                {
                    //Redraw
                    KickPlayer();
                }
                else
                {
                    BackToCategory();
                }
            }
        }

        public override void StartVote(byte type, uint argument)
        {
            GameObject go = Instantiate(votingPrefab);
            Kit_VotingBase voting = go.GetComponent<Kit_VotingBase>();
            voting.argument = argument;
            voting.votingOn = (Kit_VotingBase.VotingOn)type;
            NetworkServer.Spawn(go);
        }
    }
}