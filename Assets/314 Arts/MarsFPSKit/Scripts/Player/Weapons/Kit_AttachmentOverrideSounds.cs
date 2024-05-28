using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        public class Kit_AttachmentOverrideSounds : Kit_AttachmentVisualBase
        {
            /// <summary>
            /// Fire sound used for first person
            /// </summary>
            public AudioClip fireSound;
            /// <summary>
            /// Fire sound used for third person
            /// </summary>
            public AudioClip fireSoundThirdPerson;
            /// <summary>
            /// Max sound distance for third person fire
            /// </summary>
            public float fireSoundThirdPersonMaxRange = 300f;
            /// <summary>
            /// Sound rolloff for third person fire
            /// </summary>
            public AnimationCurve fireSoundThirdPersonRolloff = AnimationCurve.EaseInOut(0f, 1f, 300f, 0f);
            /// <summary>
            /// Does this sound change silence the weapon?
            /// </summary>
            public bool silencesWeapon = true;
        }
    }
}
