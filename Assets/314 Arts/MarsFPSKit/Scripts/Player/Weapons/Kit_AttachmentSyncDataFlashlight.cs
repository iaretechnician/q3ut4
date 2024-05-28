using Mirror;
using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        public class Kit_AttachmentSyncDataFlashlight : Kit_AttachmentSyncDataBase
        {
            /// <summary>
            /// Is the flashlight turned on?
            /// </summary>
            [SyncVar]
            public bool on;
        }
    }
}