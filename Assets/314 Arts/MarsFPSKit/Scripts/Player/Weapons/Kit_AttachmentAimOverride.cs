using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        public class Kit_AttachmentAimOverride : Kit_AttachmentVisualBase
        {
            /// <summary>
            /// Aiming position that is used when aim transform is disabled on weapon
            /// </summary>
            public Vector3 aimPos;
            /// <summary>
            /// Aiming rotation that is used when aim transform is disabled on weapon
            /// </summary>
            public Vector3 aimRot;
            /// <summary>
            /// New Aiming transform
            /// </summary>
            public Transform aimTransform;
            /// <summary>
            /// How far from the camera should this scope be?
            /// </summary>
            public float aimDistanceFromCamera = 0.3f;
            /// <summary>
            /// New Aiming FOV
            /// </summary>
            public float aimFov = 40f;
            /// <summary>
            /// Defines if this should use fullscreen aiming
            /// </summary>
            public bool useFullscreenScope;
            /// <summary>
            /// Use dual render scope camera?
            /// </summary>
            [Tooltip("Use dual render scope camera?")]
            public Camera dualRenderScopeCam;
            /// <summary>
            /// At which aim % (0.0 - 1.0) to activate the dual render cam?
            /// </summary>
            [Tooltip("At which aim % (0.0 - 1.0) to activate the dual render cam?")]
            public float activateCameraAt = 0.5f;
        }
    }
}
