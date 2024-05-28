using System;
using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        [System.Serializable]
        public class MaterialChange
        {
            /// <summary>
            /// Renderer that materials will be applied to
            /// </summary>
            public Renderer rendererToApplyTo;
            /// <summary>
            /// Materials that will be applied to <see cref="rendererToApplyTo"/>
            /// </summary>
            public Material[] materialsToApply;
        }

        public class Kit_AttachmentChangeMaterial : Kit_AttachmentVisualBase
        {
            public MaterialChange[] materialsToChange;

            public override void Selected(Kit_PlayerBehaviour pb, AttachmentUseCase auc, Kit_ModernWeaponScript script, Kit_ModernWeaponScriptRuntimeData data, int slot)
            {
                //Loop through
                for (int i = 0; i < materialsToChange.Length; i++)
                {
                    materialsToChange[i].rendererToApplyTo.sharedMaterials = materialsToChange[i].materialsToApply;
                }
            }
        }
    }
}