using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        public class Kit_MeleeRenderer : MonoBehaviour
        {
            /// <summary>
            /// The weapon animator
            /// </summary>
            public Animator anim;
            /// <summary>
            /// Support for legacy animation
            /// </summary>
            public Animation legacyAnim;
            /// <summary>
            /// Name of animations to play
            /// </summary>
            public Kit_MeleeRendererLegacyAnimations legacyAnimData;

            /// <summary>
            /// The weapon renderers that are not of attachments
            /// </summary>
            [Tooltip("Asssign all weapon renderers here that are not of attachments")]
            public Renderer[] allWeaponRenderers;
            /// <summary>
            /// These rendererers will be disabled in the customization menu. E.g. arms
            /// </summary>
            public Renderer[] hideInCustomiazionMenu;

            [Tooltip("Enable use of player dependent arms. Please note that the rigs need to match up. If you don't know what that means, you probably shouldn't use this.")]
            /// <summary>
            /// Do we use player model dependent arms?
            /// </summary>
            [Header("Player Model Dependent Arms")]
            public bool playerModelDependentArmsEnabled = false;
            /// <summary>
            /// The key for getting the arms
            /// </summary>
            public string playerModelDependentArmsKey = "Kit";
            [Tooltip("This is where the player model dependent arms will get parented to")]
            /// <summary>
            /// This is where the player model dependent arms will get parented to
            /// </summary>
            public Transform playerModelDependentArmsRoot;

            [Header("Run position / rotation")]
            /// <summary>
            /// Determines if the weapon should be moved when we are running
            /// </summary>
            public bool useRunPosRot;
            /// <summary>
            /// The run position to use
            /// </summary>
            public Vector3 runPos;
            /// <summary>
            /// The run rotation to use. Will be converted to Quaternion using <see cref="Quaternion.Euler(Vector3)"/>
            /// </summary>
            public Vector3 runRot;
            /// <summary>
            /// How fast is the weapon going to move / rotate towards the run pos / run rot?
            /// </summary>
            public float runSmooth = 3f;

            [Header("Camera Animation")]
            public bool cameraAnimationEnabled;
            /// <summary>
            /// If camera animation is enabled, which one should be used?
            /// </summary>
            public CameraAnimationType cameraAnimationType;
            /// <summary>
            /// The bone for the camera animation
            /// </summary>
            public Transform cameraAnimationBone;
            /// <summary>
            /// If the type is LookAt, this is the target
            /// </summary>
            public Transform cameraAnimationTarget;
            /// <summary>
            /// The reference rotation to add movemment to
            /// </summary>
            public Vector3 cameraAnimationReferenceRotation;

            [Header("Loadout")]
            /// <summary>
            /// Use this to correct the position in the customization menu
            /// </summary>
            public Vector3 customizationMenuOffset;

#if UNITY_EDITOR
            //Test if everything is correctly assigned, but only in the editor.
            void OnEnable()
            {
                for (int i = 0; i < allWeaponRenderers.Length; i++)
                {
                    if (!allWeaponRenderers[i])
                    {
                        Debug.LogError("Weapon renderer from " + gameObject.name + " at index " + i + " not assigned.");
                    }
                }
            }
#endif

            /// <summary>
            /// Visibility state of the weapon
            /// </summary>
            public bool visible
            {
                get
                {
                    for (int i = 0; i < allWeaponRenderers.Length; i++)
                    {
                        if (!allWeaponRenderers[i].enabled) return false;
                    }
                    return true;
                }
                set
                {
                    //Set renderers
                    for (int i = 0; i < allWeaponRenderers.Length; i++)
                    {
                        allWeaponRenderers[i].enabled = value;
                    }
                }
            }
        }
    }
}