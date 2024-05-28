using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// This is used for two layer voices (e.g. grenades and melee!)
    /// </summary>
    [System.Serializable]
    public class TwoLayerAudio
    {
        public AudioClip[] clips;
    }

    /// <summary>
    /// This class holds voice files
    /// </summary>
    [CreateAssetMenu(menuName = "MarsFPSKit/Player Models/Voice/New")]
    public class Kit_VoiceInformation : ScriptableObject
    {
        /// <summary>
        /// Used when we spotted an enemy
        /// </summary>
        public AudioClip[] enemySpotted;
        /// <summary>
        /// Used when we took damage from projectiles
        /// </summary>
        public AudioClip[] damageTakenProjectile;
        /// <summary>
        /// Used when we took damage from other source
        /// </summary>
        public AudioClip[] damageTakenOthers;
        /// <summary>
        /// Used when we killed an enemy
        /// </summary>
        public AudioClip[] enemyKilled;
        /// <summary>
        /// Used when we are reloading
        /// </summary>
        public AudioClip[] reloading;
        /// <summary>
        /// Played when grenade is thrown
        /// </summary>
        public TwoLayerAudio[] grenadeThrown;
        /// <summary>
        /// Played when melee is used!
        /// </summary>
        public TwoLayerAudio[] meleeUsed;
        /// <summary>
        /// Death sounds
        /// </summary>
        public TwoLayerAudio[] deathSounds;
    }
}
