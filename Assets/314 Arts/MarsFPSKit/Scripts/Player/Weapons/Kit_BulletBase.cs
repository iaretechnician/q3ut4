using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        /// <summary>
        /// Use this class to implement your own physical bullets
        /// </summary>
        public abstract class Kit_BulletBase : MonoBehaviour
        {
            /// <summary>
            /// Called after the bullet was instantiated by weapon script
            /// </summary>
            public abstract void Setup(Kit_ModernWeaponScript ws, Kit_ModernWeaponScriptRuntimeData data, Kit_PlayerBehaviour pb, Vector3 dir);
        }
    }
}