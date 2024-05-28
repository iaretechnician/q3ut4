using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace MarsFPSKit
{
    namespace Weapons
    {
        /// <summary>
        /// One attachment slot for this weapon
        /// </summary>
        [Serializable]
        public class AttachmentSlot
        {
            /// <summary>
            /// Name of this slot
            /// </summary>
            public string slotName;
            [FormerlySerializedAs("availableAttachments")]
            /// <summary>
            /// These attachments are available in this slot
            /// </summary>
            public Kit_AttachmentBase[] availableAttachmentsSpecific;
            /// <summary>
            /// If assigned, this will be used instead of the specifics
            /// </summary>
            public Kit_AttachmentsSlotUniversal overrideSlot;

            public Kit_AttachmentBase[] availableAttachments
            {
                get
                {
                    if (overrideSlot) return overrideSlot.availableAttachments;
                    else return availableAttachmentsSpecific;
                }
            }

#if UNITY_EDITOR
            /// <summary>
            /// Creates a deep copy of the attachment slot
            /// </summary>
            /// <returns></returns>
            public AttachmentSlot Clone()
            {
                AttachmentSlot clone = new AttachmentSlot();
                clone.slotName = slotName;
                clone.availableAttachmentsSpecific = new Kit_AttachmentInfo[availableAttachmentsSpecific.Length];

                for (int i = 0; i < availableAttachmentsSpecific.Length; i++)
                {
                    int id = i;
                    clone.availableAttachmentsSpecific[id] = availableAttachmentsSpecific[id];
                }

                clone.overrideSlot = overrideSlot;

                return clone;
            }
#endif
        }

        [Serializable]
        public class AttachmentPrefabSet
        {
            /// <summary>
            /// Has to be assigned, is the main prefab that will be used
            /// </summary>
            public GameObject fpPrefab;
            /// <summary>
            /// If assigned, this will be used for TP weapon instead of the fp prefab
            /// </summary>
            public GameObject tpPrefabOverride;
            /// <summary>
            /// If assigned, this will be used for drop prefab instead of the fp prefab
            /// </summary>
            public GameObject dropPrefabOverride;
        }

        [Serializable]
        public class SkinToAttachmentPrefabSet : SerializableDictionary<Kit_SkinInfo, AttachmentPrefabSet> { }

        [Serializable]
        public class WeaponToAttachmentDataArray : SerializableDictionary<Kit_ModernWeaponScript, AttachmentDataArray> { }

        [Serializable]
        public class AttachmentDataArray
        {
            public Kit_AttachmentDataBase[] changes;
        }

        [CreateAssetMenu(menuName = ("MarsFPSKit/Weapons/Attachments/New attachment info"))]
        public class Kit_AttachmentInfo : Kit_AttachmentBase
        {
            /// <summary>
            /// Default set of prefabs to use
            /// </summary>
            public AttachmentPrefabSet defaultSet;

            /// <summary>
            /// If you want a skin to modify this attachment, you can make this skin override the set of prefabs to use for this weapon.
            /// </summary>
            public SkinToAttachmentPrefabSet skinOverride;

            /// <summary>
            /// If the statChangeWeaponOverride is not found, these statistic changes are used
            /// </summary>
            [Tooltip("If the statChangeWeaponOverride is not found, these statistic changes are used")]
            public AttachmentDataArray generalStatChanges;
            /// <summary>
            /// If the weapon where this attachment is on is in this dictionary these stat changes are used instead
            /// </summary>
            [Tooltip("If the weapon where this attachment is on is in this dictionary these stat changes are used instead")]
            public WeaponToAttachmentDataArray statChangeWeaponOverride;
        }
    }
}