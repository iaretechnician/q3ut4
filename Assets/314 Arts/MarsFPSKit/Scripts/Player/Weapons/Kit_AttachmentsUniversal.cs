using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Weapons/Attachments/Universal Attachment List")]
        public class Kit_AttachmentsUniversal : ScriptableObject
        {
            /// <summary>
            /// Attachment slots available on this override file
            /// </summary>
            public AttachmentSlot[] attachmentSlots;
        }
    }
}