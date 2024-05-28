using UnityEngine;
using UnityEngine.Serialization;

namespace MarsFPSKit
{
    /// <summary>
    /// This class marks a spawn in the scene
    /// </summary>
    public class Kit_PlayerSpawn : MonoBehaviour
    {
        /// <summary>
        /// The Spawn Group ID of this spawn, used by <see cref="Kit_GameModeBase"/>
        /// </summary>
        public int spawnGroupID = 0;
        /// <summary>
        /// The game modes this spawn can be used for
        /// </summary>
        [Tooltip("Drag all game modes into this array that this spawn should be used for")]
        [FormerlySerializedAs("gameModes")]
        public Kit_PvP_GameModeBase[] pvpGameModes;
        /// <summary>
        /// Used in these singleplayer game modes
        /// </summary>
        public Kit_PvE_GameModeBase[] singleplayerGameModes;
        /// <summary>
        /// Used in these coop game modes
        /// </summary>
        public Kit_PvE_GameModeBase[] coopGameModes;

        void OnDrawGizmos()
        {
            //Color the spawn based on the group id
            if (spawnGroupID == 0)
                Gizmos.color = Color.black;
            else if (spawnGroupID == 1)
                Gizmos.color = Color.blue;
            else if (spawnGroupID == 2)
                Gizmos.color = Color.cyan;
            else if (spawnGroupID == 3)
                Gizmos.color = Color.gray;
            else if (spawnGroupID == 4)
                Gizmos.color = Color.green;
            else if (spawnGroupID == 5)
                Gizmos.color = Color.magenta;
            else if (spawnGroupID == 6)
                Gizmos.color = Color.red;
            else if (spawnGroupID == 7)
                Gizmos.color = Color.white;
            else
                Gizmos.color = Color.yellow;

            //Draw a cube to indicate
            Gizmos.DrawCube(transform.position, Vector3.one * 0.3f);
        }
    }
}
