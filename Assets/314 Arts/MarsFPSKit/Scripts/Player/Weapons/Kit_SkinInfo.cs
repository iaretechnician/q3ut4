using System;
using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        [Serializable]
        public class SkinSlot
        {
            /// <summary>
            /// These materials will be set to the renderer with the index of this skin slot
            /// </summary>
            [Tooltip("These materials will be set to the renderer with the index of this skin slot")]
            public Material[] materialsToSet;
        }

        [CreateAssetMenu(menuName = ("MarsFPSKit/Weapons/Attachments/New Skin info"))]
        public class Kit_SkinInfo : Kit_AttachmentBase
        {
            /// <summary>
            /// The materials in this array will be set to the corresponding slot in the 'renderersToChange' in the 'attachmentAndSkinSlots' array.
            /// </summary>
            [Tooltip("The materials in this array will be set to the corresponding slot in the 'renderersToChange' in the 'attachmentAndSkinSlots' array.")]
            public SkinSlot[] setToRendererWithIndexFp;

            /// <summary>
            /// If the length of this array is > 0 this will be used instead for tp.
            /// </summary>
            [Tooltip("If the length of this array is > 0 this will be used instead for tp.")]
            public SkinSlot[] setToRendererWithIndexTpOverride;

            /// <summary>
            /// If the length of this array is > 0 this will be used instead for drops.
            /// </summary>
            [Tooltip("If the length of this array is > 0 this will be used instead for drops.")]
            public SkinSlot[] setToRendererWithIndexDropOverride;
        }
    }
}