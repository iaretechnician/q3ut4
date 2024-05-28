using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        public class Kit_ThirdPersonGrenadeRenderer : MonoBehaviour
        {
            /// <summary>
            /// The weapon renderers that are not of attachments
            /// </summary>
            [Tooltip("Asssign all weapon renderers here that are not of attachments")]
            public Renderer[] allWeaponRenderers;

            //[Header("Inverse Kinematics")]
            //public Transform leftHandIK;

#if UNITY_EDITOR
            //Test if everything is correctly assigned, but only in the editor.
            void OnEnable()
            {
                for (int i = 0; i < allWeaponRenderers.Length; i++)
                {
                    if (!allWeaponRenderers[i])
                    {
                        Debug.LogError("Third person grenade renderer from " + gameObject.name + " at index " + i + " not assigned.");
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

            /// <summary>
            /// Is this weapon set to shadows only?
            /// </summary>
            public bool shadowsOnly
            {
                get
                {
                    for (int i = 0; i < allWeaponRenderers.Length; i++)
                    {
                        if (allWeaponRenderers[i].shadowCastingMode != UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly) return false;
                    }
                    return true;
                }
                set
                {
                    if (value)
                    {
                        //Set renderers
                        for (int i = 0; i < allWeaponRenderers.Length; i++)
                        {
                            allWeaponRenderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                        }
                    }
                    else
                    {
                        //Set renderers
                        for (int i = 0; i < allWeaponRenderers.Length; i++)
                        {
                            allWeaponRenderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                        }
                    }
                }
            }
        }
    }
}