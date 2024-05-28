using Mirror;
using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        public abstract class Kit_AttachmentSyncDataBase : NetworkBehaviour
        {
            /// <summary>
            /// Slot of this attachment
            /// </summary>
            [SyncVar]
            public int slot;
        }
    }
}