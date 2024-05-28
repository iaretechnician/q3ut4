using Mirror;
using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        public class Kit_AttachmentSyncDataLaser : Kit_AttachmentSyncDataBase
        {
            /// <summary>
            /// Is the laser turned on?
            /// </summary>
            [SyncVar]
            public bool on;
        }
    }
}