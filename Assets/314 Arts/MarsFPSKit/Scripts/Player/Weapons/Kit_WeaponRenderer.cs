using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        public enum CameraAnimationType { Copy, LookAt }

        [Serializable]
        public class AttachmentSocket
        {
            /// <summary>
            /// Assign the socket for the attachment to spawn in here
            /// </summary>
            [Tooltip("Assign the socket for the attachment to spawn in here")]
            public Transform socket;

            /// <summary>
            /// Assign the renderers that will be changed by the skin in that slot here
            /// </summary>
            [Tooltip("Assign the renderers that will be changed by the skin in that slot here")]
            public Renderer[] renderersToChange;
        }

        public class Kit_WeaponRenderer : MonoBehaviour
        {
            /// <summary>
            /// The weapon animator
            /// </summary>
            public Animator anim;
            /// <summary>
            /// Action layer on default animator
            /// </summary>
            public int animActionLayer = 0;
            [Tooltip("Some weapon packs animate arms and gun individually, for this purpose, this array was added. Assign additional animators here")]
            /// <summary>
            /// Some weapon packs animate arms and gun individually, for this purpose, this array was added.
            /// Assign additional animators here
            /// </summary>
            public Animator[] animAdditionals;
            /// <summary>
            /// Action layers on anims
            /// </summary>
            public int[] animAdditionalsActionLayer;

            /// <summary>
            /// Support for legacy animation
            /// </summary>
            public Animation legacyAnim;
            /// <summary>
            /// Name of animations to play
            /// </summary>
            public Kit_WeaponRendererLegacyAnimations legacyAnimData;

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
            /// Aiming position that is used if aim transform is disabled
            /// </summary>
            [Header("Aiming")]
            public Vector3 aimingPos;
            /// <summary>
            /// Aiming rotation
            /// </summary>
            public Vector3 aimingRot;
            /// <summary>
            /// Enable the aim transform?
            /// </summary>
            public bool enableAimTransform = true;
            /// <summary>
            /// Aiming transform target
            /// </summary>
            public Transform aimTransform;
            /// <summary>
            /// If assigned, this transform will be used to calculate the aiming position instead of the fp prefab itself
            /// </summary>
            [Tooltip("If assigned, this transform will be used to calculate the aiming position instead of the fp prefab itself")]
            public Transform aimTransformRelativeOffsetOverride;
            /// <summary>
            /// How far from the camera should the distance be?
            /// </summary>
            [Tooltip("How far from the camera should the distance be?")]
            public float aimDistanceFromCamera = 0.2f;
            /// <summary>
            /// Which fov to go to when we are aiming
            /// </summary>
            public float aimingFov = 40f;
            /// <summary>
            /// Should we use fullscreen aiming?
            /// </summary>
            public bool aimingFullscreen;
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

            #region New Attachment System
            /// <summary>
            /// Assign the sockets for the attachments to spawn in here aswell as the renderers that skins can change here
            /// </summary>
            [Tooltip("Assign the sockets for the attachments to spawn in here aswell as the renderers that skins can change here")]
            public AttachmentSocket[] attachmentAndSkinSockets;
            /// <summary>
            /// These are all attachments that need syncing
            /// </summary>
            [HideInInspector]
            public Kit_AttachmentVisualBase[] cachedSyncAttachments;

            /// <summary>
            /// These are all attachments that need interaction
            /// </summary>
            [HideInInspector]
            public Kit_AttachmentVisualBase[] cachedInteractionAttachments;
            #endregion

            #region Cached values
            //This caches values from the attachments!
            /// <summary>
            /// Which position to move to when we are aiming
            /// </summary>
            //[HideInInspector]
            public Vector3 cachedAimingPos;
            /// <summary>
            /// Which rotation to rotate to when we are aming
            /// </summary>
            [HideInInspector]
            public Vector3 cachedAimingRot;
            [HideInInspector]
            /// <summary>
            /// Aiming FOV
            /// </summary>
            public float cachedAimingFov = 40f;
            /// <summary>
            /// Should we use fullscreen scope?
            /// </summary>
            [HideInInspector]
            public bool cachedUseFullscreenScope;
            [HideInInspector]
            public bool cachedMuzzleFlashEnabled;
            /// <summary>
            /// Cached player
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

            public void WeaponUpdate(Kit_ModernWeaponScript ws, Kit_ModernWeaponScriptRuntimeData data)
            {
                //If we want to use dual render scope cam
                if (dualRenderScopeCam)
                {
                    if (cachedPlayer.isFirstPersonActive)
                    {
                        //Enable it at the given scope percentage only
                        dualRenderScopeCam.enabled = data.aimingProgress >= activateCameraAt;
                    }
                    else
                    {
                        dualRenderScopeCam.enabled = false;
                    }
                }
            }

            public void WeaponDraw()
            {
                if (dualRenderScopeCam) dualRenderScopeCam.enabled = false;
            }

            public void WeaponPutaway()
            {

            }

            public void WeaponPutawayHide()
            {
                if (dualRenderScopeCam) dualRenderScopeCam.enabled = false;
            }

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
                                        attachmentSlots[i].attachments[o].attachmentBehaviours[p].SetVisibility(cachedPlayer, AttachmentUseCase.FirstPerson, value);
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
            /// Enables the given attachments.
            /// </summary>
            /// <param name="enabledAttachments"></param>
            public void SetAttachments(int[] enabledAttachments, Kit_ModernWeaponScript ws, Kit_PlayerBehaviour pb, Kit_ModernWeaponScriptRuntimeData data)
            {
                //Set default cached values
                //cachedAttachments = enabledAttachments;
                cachedAimingPos = aimingPos;
                cachedAimingRot = aimingRot;
                cachedAimingFov = aimingFov;
                cachedUseFullscreenScope = aimingFullscreen;
                cachedMuzzleFlashEnabled = true;
                cachedPlayer = pb;
                if (ws)
                {
                    cachedFireSound = ws.fireSound;
                    cachedFireSoundThirdPerson = ws.fireSoundThirdPerson;
                    cachedFireSoundThirdPersonMaxRange = ws.fireSoundThirdPersonMaxRange;
                    cachedFireSoundThirdPersonRolloff = ws.fireSoundThirdPersonRolloff;
                }

                //Make sure this camera is disabled at this point.
                if (dualRenderScopeCam) dualRenderScopeCam.enabled = false;

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
                List<Kit_AttachmentVisualBase> interactionAttachments = new List<Kit_AttachmentVisualBase>();

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
                                GameObject creation = Instantiate(prefabsToUse.fpPrefab, attachmentAndSkinSockets[i].socket, false);
                                Kit_AttachmentVisualBase[] attachmentVisualBehaviours = creation.GetComponentsInChildren<Kit_AttachmentVisualBase>();

                                //Tell the behaviours they are active!
                                for (int p = 0; p < attachmentVisualBehaviours.Length; p++)
                                {
                                    int id = i;
                                    attachmentVisualBehaviours[p].Selected(pb, AttachmentUseCase.FirstPerson, ws, data, id);

                                    //For loadout menu, automatically enable renderers
                                    if (!pb) attachmentVisualBehaviours[p].SetVisibility(pb, AttachmentUseCase.FirstPerson, true);

                                    //Check for sync
                                    if (attachmentVisualBehaviours[p].RequiresSyncing())
                                    {
                                        int add = p;
                                        syncAttachments.Add(attachmentVisualBehaviours[add]);
                                    }

                                    //Check for interaction
                                    if (attachmentVisualBehaviours[p].RequiresInteraction())
                                    {
                                        int add = p;
                                        interactionAttachments.Add(attachmentVisualBehaviours[add]);
                                    }

                                    //Check what it is
                                    if (attachmentVisualBehaviours[p].GetType() == typeof(Kit_AttachmentAimOverride))
                                    {
                                        Kit_AttachmentAimOverride aimOverride = attachmentVisualBehaviours[p] as Kit_AttachmentAimOverride;
                                        //Override aim
                                        cachedAimingPos = aimOverride.aimPos;
                                        cachedAimingRot = aimOverride.aimRot;
                                        aimTransform = aimOverride.aimTransform;
                                        aimDistanceFromCamera = aimOverride.aimDistanceFromCamera;
                                        cachedAimingFov = aimOverride.aimFov;
                                        cachedUseFullscreenScope = aimOverride.useFullscreenScope;
                                        dualRenderScopeCam = aimOverride.dualRenderScopeCam;
                                        activateCameraAt = aimOverride.activateCameraAt;
                                    }
                                    else if (attachmentVisualBehaviours[p].GetType() == typeof(Kit_AttachmentDisableMuzzleFlash))
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
                                    else if (attachmentVisualBehaviours[p].GetType() == typeof(Kit_AttachmentAnimatorOverride))
                                    {
                                        Kit_AttachmentAnimatorOverride animatorOverride = attachmentVisualBehaviours[p] as Kit_AttachmentAnimatorOverride;
                                        if (anim)
                                        {
                                            anim.runtimeAnimatorController = animatorOverride.animatorOverride;
                                        }

                                        for (int a = 0; a < animatorOverride.animatorAdditionalsOverride.Length; a++)
                                        {
                                            animAdditionals[a].runtimeAnimatorController = animatorOverride.animatorAdditionalsOverride[a];
                                        }
                                    }
                                    else if (attachmentVisualBehaviours[p].GetType() == typeof(Kit_AttachmentRenderer))
                                    {
                                        Kit_AttachmentRenderer ar = attachmentVisualBehaviours[p] as Kit_AttachmentRenderer;

                                        allWeaponRenderers = allWeaponRenderers.Concat(ar.renderersToActivate).ToArray();
                                    }
                                }
                            }

                            if (data != null)
                            {

                                AttachmentDataArray dataChanges = attachment.generalStatChanges;

                                if (attachment.statChangeWeaponOverride.ContainsKey(ws))
                                {
                                    dataChanges = attachment.statChangeWeaponOverride[ws];
                                }

                                if (dataChanges != null)
                                {
                                    for (int p = 0; p < dataChanges.changes.Length; p++)
                                    {
                                        dataChanges.changes[p].ChangeStats(ws, data);
                                    }
                                }
                            }
                        }
                    }
                    else if (ws.attachmentSlots[i].availableAttachments[enabledAttachments[i]].GetType() == typeof(Kit_SkinInfo))
                    {
                        Kit_SkinInfo skin = ws.attachmentSlots[i].availableAttachments[enabledAttachments[i]] as Kit_SkinInfo;
                        if (skin.setToRendererWithIndexFp.Length != attachmentAndSkinSockets[i].renderersToChange.Length) Debug.LogWarning("Skin material length and the amount of renderers in slot #" + i + " do not match up. Trying best to assign.");

                        for (int o = 0; o < Mathf.Min(skin.setToRendererWithIndexFp.Length, attachmentAndSkinSockets[i].renderersToChange.Length); o++)
                        {
                            attachmentAndSkinSockets[i].renderersToChange[o].sharedMaterials = skin.setToRendererWithIndexFp[o].materialsToSet;
                        }
                    }
                }

                //Disable cam upon spawn
                if (dualRenderScopeCam) dualRenderScopeCam.enabled = false;

                cachedSyncAttachments = syncAttachments.ToArray();
                cachedInteractionAttachments = interactionAttachments.ToArray();

                if (enableAimTransform && pb)
                {
                    //Calculate aim position and aim rotation
                    Vector3 offsetPos = Vector3.zero;
                    if (aimTransformRelativeOffsetOverride) offsetPos = transform.InverseTransformPoint(aimTransformRelativeOffsetOverride.position);

                    Vector3 cameraPos = transform.InverseTransformPoint(pb.playerCameraTransform.position);
                    Vector3 aimPos = transform.InverseTransformPoint(aimTransform.position);

                    float existingAimDistanceFromCam = aimPos.z - cameraPos.z;

                    cameraPos.z = transform.localPosition.z;
                    aimPos.z = transform.localPosition.z;

                    cachedAimingPos = offsetPos + (cameraPos - aimPos);
                    cachedAimingPos.z = aimDistanceFromCamera - existingAimDistanceFromCam;

                }
            }
        }
    }
}