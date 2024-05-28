using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        public class Kit_ThirdPersonWeaponRenderer : MonoBehaviour
        {
            /// <summary>
            /// The weapon renderers that are not of attachments
            /// </summary>
            [Tooltip("Asssign all weapon renderers here that are not of attachments")]
            public Renderer[] allWeaponRenderers;

            [Header("Shell Ejection")]
            /// <summary>
            /// This is where the ejected shell will spawn, if assigned
            /// </summary>
            public Transform shellEjectTransform;

            [Header("Muzzle Flash")]
            /// <summary>
            /// The muzzle flash particle system to use
            /// </summary>
            public ParticleSystem muzzleFlash;

            /// <summary>
            /// An array of inverse kinematic positions for individual player models
            /// </summary>
            [Header("Inverse Kinematics")]
            public Transform[] leftHandIK;

            #region New Attachment System
            /// <summary>
            /// Assign the sockets for the attachments to spawn in here aswell as the renderers that skins can change here
            /// </summary>
            [Tooltip("Assign the sockets for the attachments to spawn in here aswell as the renderers that skins can change here")]
            public AttachmentSocket[] attachmentAndSkinSockets;
            #endregion

            [HideInInspector]
            /// <summary>
            /// The currently selected attachments
            /// </summary>
            public int[] cachedAttachments;

            #region Cached values
            //This caches values from the attachments!
            [HideInInspector]
            public bool cachedMuzzleFlashEnabled;
            /// <summary>
            /// Tick this if weapon is silenced
            /// </summary>
            public bool isWeaponSilenced;
            [HideInInspector]
            /// <summary>
            /// Cached player reference
            /// </summary>
            public Kit_PlayerBehaviour cachedPlayer;
            /// <summary>
            /// Fire sound used for first person
            /// </summary>
            [HideInInspector]
            public AudioClip cachedFireSound;
            /// <summary>
            /// Fire sound used for third person
            /// </summary>
            [HideInInspector]
            public AudioClip cachedFireSoundThirdPerson;

            /// <summary>
            /// Max sound distance for third person fire
            /// </summary>
            [HideInInspector]
            public float cachedFireSoundThirdPersonMaxRange = 300f;
            /// <summary>
            /// Sound rolloff for third person fire
            /// </summary>
            [HideInInspector]
            public AnimationCurve cachedFireSoundThirdPersonRolloff = AnimationCurve.EaseInOut(0f, 1f, 300f, 0f);
            #endregion

#if UNITY_EDITOR
            //Test if everything is correctly assigned, but only in the editor.
            void OnEnable()
            {
                for (int i = 0; i < allWeaponRenderers.Length; i++)
                {
                    if (!allWeaponRenderers[i])
                    {
                        Debug.LogError("Third person weapon renderer from " + gameObject.name + " at index " + i + " not assigned.");
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


                    /*
                    //Loop through all slots
                    for (int i = 0; i < cachedAttachments.Length; i++)
                    {
                        if (i < attachmentSlots.Length)
                        {
                            //Loop through all attachments for that slot
                            for (int o = 0; o < attachmentSlots[i].attachments.Length; o++)
                            {
                                //Check if this attachment is enabled
                                if (o == cachedAttachments[i])
                                {
                                    //Tell the behaviours they are active!
                                    for (int p = 0; p < attachmentSlots[i].attachments[o].attachmentBehaviours.Length; p++)
                                    {
                                        attachmentSlots[i].attachments[o].attachmentBehaviours[p].SetVisibility(cachedPlayer, AttachmentUseCase.ThirdPerson, value);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Debug.LogWarning("Something must have gone wrong with the attachments. Enabled attachments is longer than all slots.");
                        }
                    }
                    */
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

                        /*
                        //Attachment renderers
                        for (int i = 0; i < attachmentSlots.Length; i++)
                        {
                            for (int o = 0; o < attachmentSlots[i].attachments.Length; o++)
                            {
                                for (int p = 0; p < attachmentSlots[i].attachments[o].attachmentBehaviours.Length; p++)
                                {
                                    if (attachmentSlots[i].attachments[o].attachmentBehaviours[p].GetType() == typeof(Kit_AttachmentRenderer))
                                    {
                                        Kit_AttachmentRenderer ar = attachmentSlots[i].attachments[o].attachmentBehaviours[p] as Kit_AttachmentRenderer;
                                        for (int a = 0; a < ar.renderersToActivate.Length; a++)
                                        {
                                            ar.renderersToActivate[a].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
                                        }
                                    }
                                }
                            }
                        }
                        */
                    }
                    else
                    {
                        //Set renderers
                        for (int i = 0; i < allWeaponRenderers.Length; i++)
                        {
                            allWeaponRenderers[i].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                        }

                        /*
                        //Attachment renderers
                        for (int i = 0; i < attachmentSlots.Length; i++)
                        {
                            for (int o = 0; o < attachmentSlots[i].attachments.Length; o++)
                            {
                                for (int p = 0; p < attachmentSlots[i].attachments[o].attachmentBehaviours.Length; p++)
                                {
                                    if (attachmentSlots[i].attachments[o].attachmentBehaviours[p].GetType() == typeof(Kit_AttachmentRenderer))
                                    {
                                        Kit_AttachmentRenderer ar = attachmentSlots[i].attachments[o].attachmentBehaviours[p] as Kit_AttachmentRenderer;
                                        for (int a = 0; a < ar.renderersToActivate.Length; a++)
                                        {
                                            ar.renderersToActivate[a].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                                        }
                                    }
                                }
                            }
                        }
                        */
                    }
                }
            }

            /// <summary>
            /// Enables the given attachments.
            /// </summary>
            /// <param name="enabledAttachments"></param>
            public void SetAttachments(int[] enabledAttachments, Kit_ModernWeaponScript ws, Kit_PlayerBehaviour pb, Kit_ModernWeaponScriptRuntimeData data)
            {
                cachedAttachments = enabledAttachments;
                cachedMuzzleFlashEnabled = true;
                cachedPlayer = pb;
                if (ws)
                {
                    cachedFireSound = ws.fireSound;
                    cachedFireSoundThirdPerson = ws.fireSoundThirdPerson;
                    cachedFireSoundThirdPersonMaxRange = ws.fireSoundThirdPersonMaxRange;
                    cachedFireSoundThirdPersonRolloff = ws.fireSoundThirdPersonRolloff;
                }

                List<Kit_SkinInfo> skinsInUse = new List<Kit_SkinInfo>();

                for (int i = 0; i < enabledAttachments.Length; i++)
                {
                    if (ws.attachmentSlots[i].availableAttachments[enabledAttachments[i]].GetType() == typeof(Kit_SkinInfo))
                    {
                        Kit_SkinInfo skin = ws.attachmentSlots[i].availableAttachments[enabledAttachments[i]] as Kit_SkinInfo;
                        skinsInUse.Add(skin);
                    }
                }

                //Create temporary list of synced attachments
                List<Kit_AttachmentVisualBase> syncAttachments = new List<Kit_AttachmentVisualBase>();

                //Loop through all slots
                for (int i = 0; i < enabledAttachments.Length; i++)
                {
                    if (ws.attachmentSlots[i].availableAttachments[enabledAttachments[i]].GetType() == typeof(Kit_AttachmentInfo))
                    {
                        Kit_AttachmentInfo attachment = ws.attachmentSlots[i].availableAttachments[enabledAttachments[i]] as Kit_AttachmentInfo;
                        AttachmentPrefabSet prefabsToUse = attachment.defaultSet;

                        for (int o = 0; o < skinsInUse.Count; o++)
                        {
                            if (attachment.skinOverride.ContainsKey(skinsInUse[o]))
                            {
                                prefabsToUse = attachment.skinOverride[skinsInUse[o]];
                            }
                        }

                        if (prefabsToUse.fpPrefab)
                        {
                            if (attachmentAndSkinSockets[i].socket)
                            {
                                GameObject creation = null;

                                if (prefabsToUse.tpPrefabOverride)
                                {
                                    Instantiate(prefabsToUse.tpPrefabOverride, attachmentAndSkinSockets[i].socket, false);
                                }
                                else
                                {
                                    creation = Instantiate(prefabsToUse.fpPrefab, attachmentAndSkinSockets[i].socket, false);
                                }
                                Kit_AttachmentVisualBase[] attachmentVisualBehaviours = creation.GetComponentsInChildren<Kit_AttachmentVisualBase>();

                                //Tell the behaviours they are active!
                                for (int p = 0; p < attachmentVisualBehaviours.Length; p++)
                                {
                                    int id = i;
                                    attachmentVisualBehaviours[p].Selected(pb, AttachmentUseCase.ThirdPerson, ws, data, id);

                                    //For loadout, automatically enable renders
                                    if (!pb && data == null) attachmentVisualBehaviours[p].SetVisibility(pb, AttachmentUseCase.ThirdPerson, true);

                                    //Check for sync
                                    if (attachmentVisualBehaviours[p].RequiresSyncing())
                                    {
                                        int add = p;
                                        syncAttachments.Add(attachmentVisualBehaviours[add]);
                                    }

                                    //Check what it is
                                    if (attachmentVisualBehaviours[p].GetType() == typeof(Kit_AttachmentDisableMuzzleFlash))
                                    {
                                        cachedMuzzleFlashEnabled = false;
                                    }
                                    else if (attachmentVisualBehaviours[p].GetType() == typeof(Kit_AttachmentOverrideSounds))
                                    {
                                        Kit_AttachmentOverrideSounds soundOverride = attachmentVisualBehaviours[p] as Kit_AttachmentOverrideSounds;
                                        cachedFireSound = soundOverride.fireSound;
                                        cachedFireSoundThirdPerson = soundOverride.fireSoundThirdPerson;
                                        cachedFireSoundThirdPersonMaxRange = soundOverride.fireSoundThirdPersonMaxRange;
                                        cachedFireSoundThirdPersonRolloff = soundOverride.fireSoundThirdPersonRolloff;
                                    }
                                    else if (attachmentVisualBehaviours[p].GetType() == typeof(Kit_AttachmentRenderer))
                                    {
                                        Kit_AttachmentRenderer ar = attachmentVisualBehaviours[p] as Kit_AttachmentRenderer;
                                        allWeaponRenderers = allWeaponRenderers.Concat(ar.renderersToActivate).ToArray();
                                    }
                                    else if (attachmentVisualBehaviours[p].GetType() == typeof(Kit_AttachmentAimOverride))
                                    {
                                        Kit_AttachmentAimOverride ar = attachmentVisualBehaviours[p] as Kit_AttachmentAimOverride;
                                        if (ar.dualRenderScopeCam) ar.dualRenderScopeCam.enabled = false;
                                    }
                                }
                            }
                        }
                    }
                    else if (ws.attachmentSlots[i].availableAttachments[enabledAttachments[i]].GetType() == typeof(Kit_SkinInfo))
                    {
                        Kit_SkinInfo skin = ws.attachmentSlots[i].availableAttachments[enabledAttachments[i]] as Kit_SkinInfo;
                        if (skin.setToRendererWithIndexTpOverride.Length > 0)
                        {
                            if (skin.setToRendererWithIndexTpOverride.Length != attachmentAndSkinSockets[i].renderersToChange.Length) Debug.LogWarning("Skin material length and the amount of renderers in slot #" + i + " do not match up. Trying best to assign.");

                            for (int o = 0; o < Mathf.Min(skin.setToRendererWithIndexTpOverride.Length, attachmentAndSkinSockets[i].renderersToChange.Length); o++)
                            {
                                attachmentAndSkinSockets[i].renderersToChange[o].sharedMaterials = skin.setToRendererWithIndexTpOverride[o].materialsToSet;
                            }
                        }
                        else
                        {
                            if (skin.setToRendererWithIndexFp.Length != attachmentAndSkinSockets[i].renderersToChange.Length) Debug.LogWarning("Skin material length and the amount of renderers in slot #" + i + " do not match up. Trying best to assign.");

                            for (int o = 0; o < Mathf.Min(skin.setToRendererWithIndexFp.Length, attachmentAndSkinSockets[i].renderersToChange.Length); o++)
                            {
                                attachmentAndSkinSockets[i].renderersToChange[o].sharedMaterials = skin.setToRendererWithIndexFp[o].materialsToSet;
                            }
                        }
                    }
                }

                //Match with the ones on fp
                if (data != null && data.weaponRenderer)
                {
                    if (data.weaponRenderer.cachedSyncAttachments.Length == syncAttachments.Count)
                    {
                        for (int i = 0; i < syncAttachments.Count; i++)
                        {
                            int id = i;
                            data.weaponRenderer.cachedSyncAttachments[id].thirdPersonEquivalent = syncAttachments[id];
                        }
                    }
                    else
                    {
                        Debug.LogWarning("FP / TP  Attachment sync lists don't match! Check your attachment's TP Override script");
                    }
                }


                //OLD SYSTEM
                /*
                //Set default cached values
                cachedAttachments = enabledAttachments;
                cachedMuzzleFlashEnabled = true;
                cachedPlayer = pb;
                if (ws)
                {
                    cachedFireSound = ws.fireSound;
                    cachedFireSoundThirdPerson = ws.fireSoundThirdPerson;
                    cachedFireSoundThirdPersonMaxRange = ws.fireSoundThirdPersonMaxRange;
                    cachedFireSoundThirdPersonRolloff = ws.fireSoundThirdPersonRolloff;
                }

                //Create temporary list of synced attachments
                List<Kit_AttachmentBehaviour> syncAttachments = new List<Kit_AttachmentBehaviour>();

                try
                { 
                    //Loop through all slots
                    for (int i = 0; i < enabledAttachments.Length; i++)
                    {
                        if (i < attachmentSlots.Length)
                        {
                            //Loop through all attachments for that slot
                            for (int o = 0; o < attachmentSlots[i].attachments.Length; o++)
                            {
                                //Check if this attachment is enabled
                                if (o == enabledAttachments[i])
                                {
                                    //Tell the behaviours they are active!
                                    for (int p = 0; p < attachmentSlots[i].attachments[o].attachmentBehaviours.Length; p++)
                                    {
                                        attachmentSlots[i].attachments[o].attachmentBehaviours[p].Selected(pb, AttachmentUseCase.ThirdPerson);

                                        //Check for sync
                                        if (attachmentSlots[i].attachments[o].attachmentBehaviours[p].RequiresSyncing())
                                        {
                                            int add = i;
                                            int addTwo = o;
                                            int addThree = p;
                                            syncAttachments.Add(attachmentSlots[add].attachments[addTwo].attachmentBehaviours[addThree]);

                                            if (pb && pb.isController && data != null)
                                            {
                                                for (int a = 0; a < data.weaponRenderer.attachmentSlots[i].attachments[o].attachmentBehaviours.Length; a++)
                                                {
                                                    //Woah this got pretty complex real quick
                                                    if (data.weaponRenderer.attachmentSlots[i].attachments[o].attachmentBehaviours[a].GetType() == attachmentSlots[i].attachments[o].attachmentBehaviours[p].GetType())
                                                    {
                                                        data.weaponRenderer.attachmentSlots[i].attachments[o].attachmentBehaviours[a].thirdPersonEquivalent = attachmentSlots[add].attachments[addTwo].attachmentBehaviours[addThree];
                                                    }
                                                }
                                            }
                                        }

                                        //Check what it is
                                        if (attachmentSlots[i].attachments[o].attachmentBehaviours[p].GetType() == typeof(Kit_AttachmentAimOverride))
                                        {
                                            Kit_AttachmentAimOverride aimOverride = attachmentSlots[i].attachments[o].attachmentBehaviours[p] as Kit_AttachmentAimOverride;
                                            //Override aim
                                            cachedAimingPos = aimOverride.aimPos;
                                            cachedAimingRot = aimOverride.aimRot;
                                        }
                                        else if (attachmentSlots[i].attachments[o].attachmentBehaviours[p].GetType() == typeof(Kit_AttachmentDisableMuzzleFlash))
                                        {
                                            cachedMuzzleFlashEnabled = false;
                                        }
                                        else if (attachmentSlots[i].attachments[o].attachmentBehaviours[p].GetType() == typeof(Kit_AttachmentOverrideSounds))
                                        {
                                            Kit_AttachmentOverrideSounds soundOverride = attachmentSlots[i].attachments[o].attachmentBehaviours[p] as Kit_AttachmentOverrideSounds;
                                            cachedFireSound = soundOverride.fireSound;
                                            cachedFireSoundThirdPerson = soundOverride.fireSoundThirdPerson;
                                            cachedFireSoundThirdPersonMaxRange = soundOverride.fireSoundThirdPersonMaxRange;
                                            cachedFireSoundThirdPersonRolloff = soundOverride.fireSoundThirdPersonRolloff;
                                            isWeaponSilenced = soundOverride.silencesWeapon;
                                        }
                                    }
                                }
                                else
                                {
                                    //Tell the behaviours they are not active!
                                    for (int p = 0; p < attachmentSlots[i].attachments[o].attachmentBehaviours.Length; p++)
                                    {
                                        attachmentSlots[i].attachments[o].attachmentBehaviours[p].Unselected(pb, AttachmentUseCase.ThirdPerson);
                                    }
                                }
                            }
                        }
                        else
                        {
                            Debug.LogWarning("Something must have gone wrong with the attachments. Enabled attachments is longer than all slots.");
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("There was an error with the attachments: " + e);
                }

                cachedSyncAttachments = syncAttachments.ToArray();
                */
            }
        }
    }
}