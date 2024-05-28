
using UnityEngine;

namespace MarsFPSKit
{
    public abstract class Kit_VitalsBase : ScriptableObject
    {
        public GameObject networkData;

        /// <summary>
        /// Called to setup this system
        /// </summary>
        /// <param name=""></param>
        public abstract void InitializeServer(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called to setup this system
        /// </summary>
        /// <param name=""></param>
        public abstract void InitializeClient(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Apply fall damage!
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="dmg"></param>
        public abstract void ApplyFallDamage(Kit_PlayerBehaviour pb, float dmg);

        /// <summary>
        /// Apply damage from environment!
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="dmg"></param>
        public abstract void ApplyEnvironmentalDamage(Kit_PlayerBehaviour pb, float dmg, int deathSoundCategory);

        /// <summary>
        /// Commit suicide
        /// </summary>
        /// <param name="pb"></param>
        public abstract void Suicide(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called to apply damage using this system
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="runtimeData"></param>
        public abstract void ApplyDamage(Kit_PlayerBehaviour pb, float dmg, bool botShot, uint idWhoShot, int gunID, Vector3 shotFrom);

        /// <summary>
        /// Called to apply damage using this system
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="runtimeData"></param>
        public abstract void ApplyDamage(Kit_PlayerBehaviour pb, float dmg, bool botShot, uint idWhoShot, string deathCause, Vector3 shotFrom);

        /// <summary>
        /// Called to apply healing using this system!
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="heal"></param>
        public abstract void ApplyHeal(Kit_PlayerBehaviour pb, float heal);

        /// <summary>
        /// Update callback
        /// </summary>
        /// <param name="pb"></param>
        public abstract void CustomUpdate(Kit_PlayerBehaviour pb);
    }
}
