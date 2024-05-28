using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        public class Kit_AttachmentRenderer : Kit_AttachmentVisualBase
        {
            [Tooltip("These renderers will be enabled if this attachment is selected.")]
            public Renderer[] renderersToActivate;

            public override void Selected(Kit_PlayerBehaviour pb, AttachmentUseCase auc, Kit_ModernWeaponScript script, Kit_ModernWeaponScriptRuntimeData data, int slot)
            {
                for (int i = 0; i < renderersToActivate.Length; i++)
                {
                    if (renderersToActivate[i])
                    {
                        renderersToActivate[i].enabled = false;

                        if (auc == AttachmentUseCase.FirstPerson)
                        {
                            renderersToActivate[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        }
                    }
                    else
                    {
                        Debug.LogError(gameObject.name + ": Renderer at " + i + " is not assigned.");
                    }
                }
            }

            public override void SetVisibility(Kit_PlayerBehaviour pb, AttachmentUseCase auc, bool visible)
            {
                for (int i = 0; i < renderersToActivate.Length; i++)
                {
                    if (renderersToActivate[i])
                    {
                        renderersToActivate[i].enabled = visible;
                    }
                    else
                    {
                        Debug.LogError(gameObject.name + ": Renderer at " + i + " is not assigned.");
                    }
                }
            }
        }
    }
}
