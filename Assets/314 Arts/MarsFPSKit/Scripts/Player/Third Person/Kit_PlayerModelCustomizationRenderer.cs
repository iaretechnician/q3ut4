using UnityEngine;

namespace MarsFPSKit
{
    public class Kit_PlayerModelCustomizationRenderer : Kit_PlayerModelCustomizationBehaviour
    {
        public Renderer[] renderers;

        public override void Selected(Kit_PlayerBehaviour pb, Kit_ThirdPersonPlayerModel pm)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].enabled = true;
            }
        }

        public override void Unselected(Kit_PlayerBehaviour pb, Kit_ThirdPersonPlayerModel pm)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].enabled = false;
            }
        }
    }
}