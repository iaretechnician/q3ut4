using MarsFPSKit.Networking;
using Mirror;
using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// Base to implement your own voting behaviour
    /// </summary>
    public abstract class Kit_VotingBase : NetworkBehaviour
    {
        public enum VotingOn { Kick = 0, Map = 1, GameMode = 2 }
        [SyncVar]
        public VotingOn votingOn;
        [SyncVar]
        /// <summary>
        /// ID of player to kick, Map ID or Game Mode ID, depending on <see cref="votingOn"/>
        /// </summary>
        public uint argument;

        /// <summary>
        /// Our own vote. -1 = none, 0 = no, 1 = yes
        /// </summary>
        public int myVote = 0;

        [SyncVar]
        public uint voteStartedBy;

        /// <summary>
        /// Called to vote yes on the current vote
        /// </summary>
        public void VoteYes()
        {
            myVote = 2;
            CmdVoteYes();
        }

        /// <summary>
        /// Called to vote no on the current vote
        /// </summary>
        public void VoteNo()
        {
            myVote = 1;
            CmdVoteNo();
        }

        [Command(requiresAuthority = false)]
        public void CmdVoteYes(NetworkConnectionToClient sender = null)
        {
            OnVoteYes(Kit_NetworkPlayerManager.instance.GetPlayerByConnection(sender));
        }

        public virtual void OnVoteYes(Kit_Player player)
        {

        }

        [Command(requiresAuthority = false)]
        public void CmdVoteNo(NetworkConnectionToClient sender = null)
        {
            OnVoteNo(Kit_NetworkPlayerManager.instance.GetPlayerByConnection(sender));
        }

        public virtual void OnVoteNo(Kit_Player player)
        {

        }

        /// <summary>
        /// Returns the yes votes for the current vote
        /// </summary>
        /// <returns></returns>
        public abstract int GetYesVotes();

        /// <summary>
        /// Returns the no votes for the current vote
        /// </summary>
        /// <returns></returns>
        public abstract int GetNoVotes();
    }
}
