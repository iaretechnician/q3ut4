using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// This class marks a bot navigation point in the scene
    /// </summary>
    public class Kit_BotNavPoint : MonoBehaviour
    {
        /// <summary>
        /// ID of this spawn point
        /// </summary>
        public int navPointGroupID;

        /// <summary>
        /// The game modes this spawn can be used for
        /// </summary>
        [Tooltip("Drag all game modes into this array that this spawn should be used for")]
        public Kit_PvP_GameModeBase[] gameModes;

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;

            //Draw a cube to indicate
            Gizmos.DrawCube(transform.position, Vector3.one * 0.3f);
        }
    }
}
