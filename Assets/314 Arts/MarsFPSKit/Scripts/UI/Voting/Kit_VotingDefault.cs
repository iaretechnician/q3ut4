using MarsFPSKit.Networking;
using Mirror;
using UnityEngine;

namespace MarsFPSKit
{
    public class Kit_VotingDefault : Kit_VotingBase
    {
        [SyncVar]
        public float timer = 30f;

        /// <summary>
        /// Playerid to vote (0 = none, 1 = no, 2 = yes)
        /// </summary>
        public readonly SyncDictionary<uint, byte> playersToVote = new SyncDictionary<uint, byte>();

        private void OnEnable()
        {
            Kit_NetworkPlayerManager.instance.onPlayerLeft.AddListener(OnPlayerLeft);
        }

        private void OnDisable()
        {
            Kit_NetworkPlayerManager.instance.onPlayerLeft.RemoveListener(OnPlayerLeft);
        }

        void Start()
        {
            playersToVote.Callback += OnVotesChanged;
        }

        public override void OnStartServer()
        {
            //Assign it
            Kit_IngameMain.instance.currentVoting = this;

            //Check if enough time is left
            if (Kit_IngameMain.instance.timer > timer)
            {

            }
            else
            {
                VoteFailed();
            }
        }

        public override void OnStartClient()
        {
            //Assign it
            Kit_IngameMain.instance.currentVoting = this;

            //Redraw
            if (Kit_IngameMain.instance.votingMenu)
            {
                Kit_IngameMain.instance.votingMenu.RedrawVotingUI(this);
            }
        }

        void Update()
        {
            if (isServer)
            {
                //Decrease the timer
                if (timer > 0)
                {
                    timer -= Time.deltaTime;
                    if (timer <= 0)
                    {
                        TimeRanOut();
                    }
                }
            }

            //Check if we did not vote yet!
            if (myVote == 0)
            {
                if (Input.GetKeyDown(KeyCode.F1))
                {
                    VoteYes();
                }
                else if (Input.GetKeyDown(KeyCode.F2))
                {
                    VoteNo();
                }
            }
        }

        public override void OnVoteYes(Kit_Player player)
        {
            if (player != null)
            {
                if (playersToVote.ContainsKey(player.id))
                {
                    playersToVote[player.id] = 2;
                }
                else
                {
                    playersToVote.Add(player.id, 2);
                }
            }

            RecalculateVotes();
        }

        public override void OnVoteNo(Kit_Player player)
        {
            if (player != null)
            {
                if (playersToVote.ContainsKey(player.id))
                {
                    playersToVote[player.id] = 1;
                }
                else
                {
                    playersToVote.Add(player.id, 1);
                }
            }

            RecalculateVotes();
        }

        void OnDestroy()
        {
            //Tell the voting behaviour
            if (Kit_IngameMain.instance.votingMenu)
            {
                Kit_IngameMain.instance.votingMenu.VoteEnded(this);
            }
        }

        public override int GetYesVotes()
        {
            int yesVotes = 0;
            foreach (var vote in playersToVote)
            {
                if (vote.Value == 2)
                {
                    yesVotes++;
                }
            }

            return yesVotes;
        }

        public override int GetNoVotes()
        {
            int noVotes = 0;
            foreach (var vote in playersToVote)
            {
                if (vote.Value == 1)
                {
                    noVotes++;
                }
            }
            return noVotes;
        }

        private void OnVotesChanged(SyncIDictionary<uint, byte>.Operation op, uint key, byte item)
        {
            if (Kit_IngameMain.instance.votingMenu)
            {
                Kit_IngameMain.instance.votingMenu.RedrawVotingUI(this);
            }
            if (isServer)
            {
                RecalculateVotes();
            }
        }

        //Check if enough votes were gained
        void RecalculateVotes()
        {
            if (isServer)
            {
                //Have all players voted?
                if ((GetYesVotes() + GetNoVotes()) >= Kit_NetworkPlayerManager.instance.players.Count)
                {
                    //More yes than no votes?
                    if (GetYesVotes() > GetNoVotes())
                    {
                        VoteSucceeded();
                    }
                    else
                    {
                        VoteFailed();
                    }
                }
            }
        }

        void TimeRanOut()
        {
            if (isServer)
            {
                //More than 50% of all players need to vote and more yes votes than no votes!
                int totalVotes = GetYesVotes() + GetNoVotes();
                if (totalVotes > (Kit_NetworkPlayerManager.instance.players.Count / 2))
                {
                    if (GetYesVotes() > GetNoVotes())
                    {
                        VoteSucceeded();
                    }
                    else
                    {
                        VoteFailed();
                    }
                }
                else
                {
                    VoteFailed();
                }
            }
        }

        void VoteSucceeded()
        {
            if (isServer)
            {
                if (votingOn == VotingOn.Kick)
                {
                    //Get that player
                    Kit_Player toKick = Kit_NetworkPlayerManager.instance.GetPlayerById((uint)argument);
                    if (toKick != null)
                    {
                        //Kick that player
                        toKick.serverToClientConnection.Disconnect();
                    }
                }
                else if (votingOn == VotingOn.Map)
                {
                    //Switch map
                    Kit_IngameMain.instance.SwitchMap((int)argument);
                }
                else if (votingOn == VotingOn.GameMode)
                {
                    //Switch game mode
                    Kit_IngameMain.instance.SwitchGameMode((int)argument);
                }

                //Destroy this
                NetworkServer.Destroy(gameObject);
            }
        }

        void VoteFailed()
        {
            //Destroy this
            if (isServer)
            {
                NetworkServer.Destroy(gameObject);
            }
        }

        private void OnPlayerLeft(Kit_Player otherPlayer)
        {
            if (otherPlayer != null)
            {
                if (playersToVote.ContainsKey(otherPlayer.id))
                {
                    playersToVote.Remove(otherPlayer.id);
                }

                //If the player who started the vote left, abort!
                if (otherPlayer.id == voteStartedBy)
                {
                    VoteFailed();
                }
                //If we are voting on this player, also abort
                if (votingOn == VotingOn.Kick && otherPlayer.id == argument)
                {
                    VoteFailed();
                }
            }
        }
    }
}