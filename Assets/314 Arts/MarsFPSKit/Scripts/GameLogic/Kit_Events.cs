using UnityEngine;
using UnityEngine.Events;

namespace MarsFPSKit
{
    public class PlayerSpawnedEvent : UnityEvent<Kit_PlayerBehaviour>
    {

    }

    public class PlayerDiedEvent : UnityEvent<Kit_PlayerBehaviour>
    {

    }

    public class PlayerWonEvent : UnityEvent<Kit_Player>
    {

    }

    public class TeamWonEvent : UnityEvent<uint>
    {

    }

    public class TeamWonWithScoreEvent : UnityEvent<uint, int[]>
    {

    }

    public class TeamSwitchedEvent : UnityEvent<int>
    {

    }

    /// <summary>
    /// This is the kit's event system. You can subscribe to events here that will be fired at their time!
    /// </summary>
    public class Kit_Events
    {
        /// <summary>
        /// Called when a player spawns
        /// </summary>
        public static PlayerSpawnedEvent onPlayerSpawned = new PlayerSpawnedEvent();
        /// <summary>
        /// Called when a player dies
        /// </summary>
        public static PlayerDiedEvent onPlayerDied = new PlayerDiedEvent();
        /// <summary>
        /// Called when a single player wins, only on the master client!
        /// </summary>
        public static PlayerWonEvent onEndGamePlayerWin = new PlayerWonEvent();
        /// <summary>
        /// Called when a team wins, 2 = draw. Only called on master client!
        /// </summary>
        public static TeamWonEvent onEndGameTeamWin = new TeamWonEvent();
        /// <summary>
        /// Called when a team wins, T0 = team, T1 = score team 1, T2 = score team 2. Only called on master client!
        /// </summary>
        public static TeamWonWithScoreEvent onEndGameTeamWinWithScore = new TeamWonWithScoreEvent();
        /// <summary>
        /// Called when the local player switches his team
        /// </summary>
        public static TeamSwitchedEvent onTeamSwitched = new TeamSwitchedEvent();
    }
}