using UnityEngine;

namespace MarsFPSKit
{
    [System.Serializable]
    public class PlayerModelKillFeedInformation
    {
        /// <summary>
        /// When a player using this player model was killed using this ID, the configured sprite will be shown in the killfeed.
        /// </summary>
        public int idAtWhichToAppear;
        /// <summary>
        /// The sprite to show in the killfeed
        /// </summary>
        public Sprite toShow;
    }

    /// <summary>
    /// This object contains information for a Third Person Player model
    /// </summary>
    [CreateAssetMenu(menuName = "MarsFPSKit/Player Models/New")]
    public class Kit_PlayerModelInformation : ScriptableObject
    {
        /// <summary>
        /// This is displayed in the loadout. Human readable name
        /// </summary>
        public string displayName = "Random Player Model";

        [Tooltip("The prefab for this player model")]
        /// <summary>
        /// The prefab for this player model
        /// </summary>
        public GameObject prefab;
        [Tooltip("Which voices can be used by this player model?")]
        /// <summary>
        /// Which voices can be used by this player model?
        /// </summary>
        public Kit_VoiceInformation[] voices;
        [Tooltip("Killfeed configuration for this model")]
        /// <summary>
        /// Killfeed configuration for this model
        /// </summary>
        public PlayerModelKillFeedInformation[] killFeedConfig;
    }
}
