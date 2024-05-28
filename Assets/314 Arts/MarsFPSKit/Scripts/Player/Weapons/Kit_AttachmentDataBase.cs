using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        public abstract class Kit_AttachmentDataBase : ScriptableObject
        {
            /// <summary>
            /// Change the stats in the weapon
            /// </summary>
            /// <param name="ws"></param>
            /// <param name="data"></param>
            public abstract void ChangeStats(Kit_ModernWeaponScript ws, Kit_ModernWeaponScriptRuntimeData data);
        }
    }
}