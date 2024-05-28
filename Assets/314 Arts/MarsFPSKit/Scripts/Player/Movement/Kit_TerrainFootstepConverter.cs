using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// This script gets index of terrain and converts them to strings used for footsteps!
    /// </summary>
    public class Kit_TerrainFootstepConverter : MonoBehaviour
    {
        [Tooltip("This has to be the same length as the amount of textures on the terrain. This will determine the 'tag' used for the footsteps.")]
        /// <summary>
        /// This has to be the same length as the amount of textures on the terrain. This will determine the 'tag' used for the footsteps.
        /// </summary>
        public string[] textureToString;
    }
}