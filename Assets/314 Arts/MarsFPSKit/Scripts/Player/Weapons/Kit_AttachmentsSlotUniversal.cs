using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        /// <summary>
        /// This can be used to override just one slot
        /// </summary>
        [CreateAssetMenu(menuName = "MarsFPSKit/Weapons/Attachments/Universal Attachment One Slot List")]
        public class Kit_AttachmentsSlotUniversal : ScriptableObject
        {
            /// <summary>
            /// These attachments are available in this slot
            /// </summary>
            public Kit_AttachmentBase[] availableAttachments;
        }
    }
}