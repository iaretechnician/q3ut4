using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// This holds information for the hitbox
    /// </summary>
    public class Kit_PlayerDamageMultiplier : MonoBehaviour
    {
        /// <summary>
        /// With which number is the damage multiplied?
        /// </summary>
        public float damageMultiplier = 1f;

        /// <summary>
        /// Which ID does this part of the ragdoll have?
        /// </summary>
        public int ragdollId;
    }
}
