
using UnityEngine;

namespace MarsFPSKit
{
    public abstract class Kit_SpawnProtectionBase : ScriptableObject
    {
        /// <summary>
        /// Called in Start from <see cref="Kit_PlayerBehaviour"/> if this system is assigned. Called for EVERYONE
        /// </summary>
        /// <param name="pb"></param>
        public virtual void CustomStart(Kit_PlayerBehaviour pb)
        {

        }

        /// <summary>
        /// Called in Update from<see cref="Kit_PlayerBehaviour"/> pb if this system is assigned. Only called for the controller.
        /// </summary>
        /// <param name="pb"></param>
        public virtual void CustomUpdate(Kit_PlayerBehaviour pb)
        {

        }

        /// <summary>
        /// The movement system should call this when the player moved. Look in the default system to see how it could be done
        /// </summary>
        /// <param name="pb"></param>
        public virtual void PlayerMoved(Kit_PlayerBehaviour pb)
        {

        }

        /// <summary>
        /// The weapon system should call this when the player uses his weapon. Look in the default system to see how it could be done
        /// </summary>
        /// <param name="pb"></param>
        public virtual void GunFired(Kit_PlayerBehaviour pb)
        {

        }

        /// <summary>
        /// Your damage system should check for this bool.
        /// </summary>
        /// <param name="pb"></param>
        /// <returns></returns>
        public abstract bool CanTakeDamage(Kit_PlayerBehaviour pb);
    }
}
