
using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// Use this to implement your own voice manager
    /// </summary>
    public abstract class Kit_VoiceManagerBase : ScriptableObject
    {
        public enum DamageType { Projectile, Other }

        /// <summary>
        /// Called when setting up as the owner (or bot)
        /// </summary>
        /// <param name="pb"></param>
        public abstract void SetupOwner(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called when setting up not as the owner
        /// </summary>
        /// <param name="pb"></param>
        public abstract void SetupOthers(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called when we received a voice RPC
        /// </summary>
        /// <param name="catId"></param>
        /// <param name="id"></param>
        public abstract void PlayVoiceRpcReceived(Kit_PlayerBehaviour pb, int catId, int id, int idTwo = 0);

        /// <summary>
        /// Called when we spotted a new enemy
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="enemy"></param>
        public abstract void SpottedEnemy(Kit_PlayerBehaviour pb, Kit_PlayerBehaviour enemy);

        /// <summary>
        /// When we took damage
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="dt"></param>
        public abstract void DamageTaken(Kit_PlayerBehaviour pb, DamageType dt);

        /// <summary>
        /// Called when we have killed an enemy
        /// </summary>
        /// <param name="pb"></param>
        public abstract void EnemyKilled(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called when we start to reload
        /// </summary>
        /// <param name="pb"></param>
        public abstract void Reloading(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called when we threw a grenade
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="id"></param>
        public abstract void GrenadeThrown(Kit_PlayerBehaviour pb, int id);

        /// <summary>
        /// Called when we used a melee!
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="id"></param>
        public abstract void MeleeUsed(Kit_PlayerBehaviour pb, int id);

        /// <summary>
        /// Gets an int from a cat
        /// </summary>
        /// <param name=""></param>
        /// <param name="cat"></param>
        /// <returns></returns>
        public abstract int GetDeathSoundID(Kit_PlayerBehaviour pb, int cat);

        /// <summary>
        /// Play dat def sound
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="cat"></param>
        /// <param name="id"></param>
        public abstract void PlayDeathSound(Kit_PlayerBehaviour pb, int cat, int id);

        /// <summary>
        /// Enemy scan
        /// </summary>
        /// <param name="pb"></param>
        public abstract void ScanForEnemies(Kit_PlayerBehaviour pb);
    }
}
