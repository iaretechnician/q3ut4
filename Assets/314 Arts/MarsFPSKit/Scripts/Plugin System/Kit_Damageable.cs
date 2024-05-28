using UnityEngine;

namespace MarsFPSKit
{
    /// <summary>
    /// This can be used to make other things damageable
    /// </summary>
    public interface IKitDamageable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dmg"></param>
        /// <param name="gunID"></param>
        /// <param name="shotPos"></param>
        /// <param name="forward"></param>
        /// <param name="force"></param>
        /// <param name="hitPos"></param>
        /// <param name="shotBot"></param>
        /// <param name="shotId"></param>
        bool LocalDamage(float dmg, int gunID, Vector3 shotPos, Vector3 forward, float force, Vector3 hitPos, bool shotBot, uint shotId);
    }
}