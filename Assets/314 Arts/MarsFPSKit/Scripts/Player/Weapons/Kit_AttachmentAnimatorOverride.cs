using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        /// <summary>
        /// Changes the animator at runtime (e.g. useful for a grip)
        /// </summary>
        public class Kit_AttachmentAnimatorOverride : Kit_AttachmentVisualBase
        {
            /// <summary>
            /// Animator that will override the default one
            /// </summary>
            public RuntimeAnimatorController animatorOverride;
            /// <summary>
            /// This overrides anims in the additional slots
            /// </summary>
            public RuntimeAnimatorController[] animatorAdditionalsOverride;
        }
    }
}