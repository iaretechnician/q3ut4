
using Mirror;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#pragma warning disable 0219

namespace MarsFPSKit
{
    namespace Weapons
    {
        public enum GrenadeMode { QuickUse, IndividualWeapon, Both }

        [CreateAssetMenu(menuName = ("MarsFPSKit/Weapons/Modern Grenade Script"))]
        public class Kit_ModernGrenadeScript : Kit_WeaponBase
        {
            [Header("Spring")]
            /// <summary>
            /// Config for positional spring
            /// </summary>
            public Kit_Spring.SpringConfig springPosConfig;
            /// <summary>
            /// Config for rotational spring
            /// </summary>
            public Kit_Spring.SpringConfig springRotConfig;

            public GrenadeMode grenadeMode = GrenadeMode.Both;
            /// <summary>
            /// With how many grenades do we start?
            /// </summary>
            public int amountOfGrenadesAtStart = 3;
            [Header("Normal use")]
            /// <summary>
            /// How long does the pull pin take?
            /// </summary>
            public float pullPinTime = 0.5f;
            /// <summary>
            /// How long does the throw animation last?
            /// </summary>
            public float throwTime = 0.5f;
            /// <summary>
            /// Redraw time after throw
            /// </summary>
            public float redrawTime = 0.5f;
            /// <summary>
            /// Quick use pull pin time!
            /// </summary>
            [Header("Quick Use")]
            public float quickUsePullPinTime = 0.5f;
            /// <summary>
            /// Quick use throw time!
            /// </summary>
            public float quickUseThrowTime = 0.5f;

            [Header("Throw")]
            /// <summary>
            /// Force that is being applied when throwing the grenade
            /// </summary>
            public Vector3 throwForce;
            /// <summary>
            /// Torque that is being applied when throwing the grenade
            /// </summary>
            public Vector3 throwTorque;
            /// <summary>
            /// Prefab that is being thrown! Has to be in Resources Folder and has to have a PhotonView on it!
            /// </summary>
            public GameObject grenadePrefab;

            #region Sounds
            [Header("Sounds")]
            /// <summary>
            /// Sound used for draw
            /// </summary>
            public AudioClip drawSound;
            /// <summary>
            /// Sound used for putaway
            /// </summary>
            public AudioClip putawaySound;
            /// <summary>
            /// Sound that plays when pulling the pin!
            /// </summary>
            public AudioClip pullPinSound;
            /// <summary>
            /// Sound that plays when throwing
            /// </summary>
            public AudioClip throwSound;
            /// <summary>
            /// Sound that plays when pulling the pin!
            /// </summary>
            public AudioClip pullPinQuickSound;
            /// <summary>
            /// Sound that plays when throwing
            /// </summary>
            public AudioClip throwQuickSound;
            /// <summary>
            /// Third person rolloff
            /// </summary>
            public float thirdPersonRange = 30f;
            /// <summary>
            /// Sound rolloff for third person fire
            /// </summary>
            [HideInInspector]
            public AnimationCurve thirdPersonRolloff = AnimationCurve.EaseInOut(0f, 1f, 30f, 0f);
            /// <summary>
            /// As grenades a two layer audio, this is its id!
            /// </summary>
            public int voiceGrenadeSoundID;
            #endregion

            #region Weapon Delay
            [Header("Weapon Delay")]
            /// <summary>
            /// Base amount for weapon delay
            /// </summary>
            public float weaponDelayBaseAmount = 1f;
            /// <summary>
            /// Max amount for weapon delay
            /// </summary>
            public float weaponDelayMaxAmount = 0.02f;
            /// <summary>
            /// Multiplier that is applied when we are aiming
            /// </summary>
            public float weaponDelayAimingMultiplier = 0.3f;
            /// <summary>
            /// How fast does the weapon delay update?
            /// </summary>
            public float weaponDelaySmooth = 3f;
            #endregion

            #region Weapon Tilt
            /// <summary>
            /// Should the weapon tilt sideways when we are walking sideways?
            /// </summary>
            public bool weaponTiltEnabled = true;
            /// <summary>
            /// By how many degrees should the weapon tilt?
            /// </summary>
            public float weaponTiltIntensity = 5f;
            /// <summary>
            /// How fast should it return to 0,0,0 when weapon tilt is disabled?
            /// </summary>
            public float weaponTiltReturnSpeed = 3f;
            #endregion

            #region Weapon Fall
            [Header("Fall Down effect")]
            public float fallDownAmount = 10.0f;
            public float fallDownMinOffset = -6.0f;
            public float fallDownMaxoffset = 6.0f;
            public float fallDownTime = 0.1f;
            public float fallDownReturnSpeed = 1f;
            #endregion

            #region Generic Animations
            [Header("Generic Animations")]
            /// <summary>
            /// This animation controller holds the animations for generic gun movement (Idle, Walk, Run)
            /// </summary>
            public GameObject genericGunAnimatorControllerPrefab;

            /// <summary>
            /// Uses the generic walk anim if true
            /// </summary>
            public bool useGenericWalkAnim = true;

            /// <summary>
            /// Uses the generic run anim if true
            /// </summary>
            public bool useGenericRunAnim = true;
            #endregion

            public override void PredictionInput(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, Kit_PlayerInput input, float delta)
            {
                AuthorativeInput(pb, runtimeData, input, delta, 0);
            }

            public override void AuthorativeInput(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, Kit_PlayerInput input, float delta, double revertTime)
            {
                Kit_ModernGrenadeScriptRuntimeData data = runtimeData as Kit_ModernGrenadeScriptRuntimeData;

                //Set this weapon to selected and ready (for other things)
                data.isSelectedAndReady = true;

                if (data.isThrowingGrenade)
                {
                    if (Time.time - pullPinTime > data.beganThrowingGrenade && !pb.input.lmb)
                    {
                        if (grenadePrefab)
                        {
                            if (pb.isServer)
                            {
                                //Throw Grenade
                                GameObject go = Instantiate(grenadePrefab, pb.playerCameraTransform.position, pb.playerCameraTransform.rotation);

                                //Get script
                                Kit_Grenade grenade = go.GetComponent<Kit_Grenade>();

                                if (grenade)
                                {
                                    grenade.gunId = gameGunID;
                                    grenade.ownerBot = pb.isBot;
                                    grenade.ownerId = pb.id;
                                    grenade.initialPlayerVelocity = pb.movement.GetVelocity(pb);
                                    grenade.forceToApply = throwForce;
                                    grenade.torqueToApply = throwTorque;
                                }

                                Kit_GrenadeSmoke smokeGrenade = go.GetComponent<Kit_GrenadeSmoke>();

                                if (smokeGrenade)
                                {
                                    smokeGrenade.ownerBot = pb.isBot;
                                    smokeGrenade.ownerId = pb.id;
                                    smokeGrenade.initialPlayerVelocity = pb.movement.GetVelocity(pb);
                                    smokeGrenade.forceToApply = throwForce;
                                    smokeGrenade.torqueToApply = throwTorque;
                                }

                                NetworkServer.Spawn(go);
                            }
                        }
                        else
                        {
                            Debug.LogError("No Grenade Prefab!");
                        }

                        //Subtract
                        data.amountOfGrenadesLeft--;
                        //Wait for return
                        data.hasThrownGrenadeAndIsWaitingForReturn = true;
                        //Set Time
                        data.grenadeThrownAt = Time.time;
                        //Don't throw anymore
                        data.isThrowingGrenade = false;

                        if (pb.isFirstPersonActive)
                        {
                            //Play Animation
                            if (data.grenadeRenderer.anim)
                            {
                                data.grenadeRenderer.anim.Play("Throw", 0, 0f);
                            }
                            else if (data.grenadeRenderer.legacyAnim)
                            {
                                data.grenadeRenderer.legacyAnim.Play(data.grenadeRenderer.legacyAnimData.throwNade);
                            }
                            //Play Sound
                            data.sounds.clip = throwSound;
                            data.sounds.Play();
                            //Play TP Aniamtion
                            pb.thirdPersonPlayerModel.PlayGrenadeAnimation(1);
                        }
                        else
                        {
                            //Play TP anim
                            pb.thirdPersonPlayerModel.PlayGrenadeAnimation(1);
                            //Play Sound
                            //Update range
                            pb.thirdPersonPlayerModel.soundFire.maxDistance = thirdPersonRange;
                            //Update sound rolloff
                            pb.thirdPersonPlayerModel.soundFire.SetCustomCurve(AudioSourceCurveType.CustomRolloff, thirdPersonRolloff);
                            pb.thirdPersonPlayerModel.soundFire.clip = throwSound;
                            pb.thirdPersonPlayerModel.soundFire.Play();
                        }

                        //Relay to voice manager
                        if (pb.voiceManager)
                        {
                            pb.voiceManager.GrenadeThrown(pb, voiceGrenadeSoundID);
                        }

                        if (pb.isServer)
                        {
                            //Send RPC
                            pb.RpcGrenadeThrowNetwork();
                        }
                    }
                }
                else
                {
                    if (!data.hasThrownGrenadeAndIsWaitingForReturn)
                    {
                        if (data.amountOfGrenadesLeft > 0)
                        {
                            if (pb.input.lmb && !data.isThrowingGrenade)
                            {
                                //Initiate throw
                                data.isThrowingGrenade = true;
                                //Set time
                                data.beganThrowingGrenade = Time.time;

                                if (pb.isFirstPersonActive)
                                {
                                    //Play Animation
                                    if (data.grenadeRenderer.anim)
                                    {
                                        data.grenadeRenderer.anim.Play("PullPin", 0, 0f);
                                    }
                                    else if (data.grenadeRenderer.legacyAnim)
                                    {
                                        data.grenadeRenderer.legacyAnim.Play(data.grenadeRenderer.legacyAnimData.pullPin);
                                    }
                                    //Play Sound
                                    data.sounds.clip = pullPinSound;
                                    data.sounds.Play();
                                    //Play TP Aniamtion
                                    pb.thirdPersonPlayerModel.PlayGrenadeAnimation(0);
                                }
                                else
                                {
                                    //Play TP anim
                                    pb.thirdPersonPlayerModel.PlayGrenadeAnimation(0);
                                    //Update range
                                    pb.thirdPersonPlayerModel.soundFire.maxDistance = thirdPersonRange;
                                    //Update sound rolloff
                                    pb.thirdPersonPlayerModel.soundFire.SetCustomCurve(AudioSourceCurveType.CustomRolloff, thirdPersonRolloff);
                                    //Play Sound
                                    pb.thirdPersonPlayerModel.soundFire.clip = pullPinSound;
                                    pb.thirdPersonPlayerModel.soundFire.Play();
                                }

                                if (pb.isServer)
                                {
                                    //Send RPC
                                    pb.RpcGrenadePullPinNetwork();
                                }
                            }
                        }
                    }
                }

                if (data.hasThrownGrenadeAndIsWaitingForReturn)
                {
                    if (!data.isRedrawInProgress)
                    {
                        if (Time.time - throwTime > data.grenadeThrownAt)
                        {
                            if (data.amountOfGrenadesLeft > 0)
                            {
                                //Set bool
                                data.isRedrawInProgress = true;
                                //Set Time
                                data.grenadeThrownAt = Time.time;
                            }
                            else
                            {
                                data.isRedrawInProgress = false;
                                data.hasThrownGrenadeAndIsWaitingForReturn = false;
                                if (pb.isFirstPersonActive)
                                {
                                    //Reset fov
                                    Kit_IngameMain.instance.mainCamera.fieldOfView = Kit_GameSettings.baseFov;
                                    if (data.grenadeRenderer.anim)
                                    {
                                        //Disable anim
                                        data.grenadeRenderer.anim.enabled = false;
                                    }
                                    else if (data.grenadeRenderer.legacyAnim)
                                    {
                                        //Disable anim
                                        data.grenadeRenderer.legacyAnim.enabled = false;
                                    }
                                    //Hide
                                    data.grenadeRenderer.visible = false;
                                }
                                //Hide tp weapon
                                data.tpGrenadeRenderer.visible = false;

                                //Make sure it is not ready yet
                                data.isSelectedAndReady = false;
                                //Stop third person anims
                                pb.thirdPersonPlayerModel.AbortWeaponAnimations();
                                //Force unselect
                                pb.weaponManager.ForceUnselectCurrentWeapon(pb);
                            }
                        }
                    }
                    else
                    {
                        if (Time.time - redrawTime > data.grenadeThrownAt)
                        {
                            //Done throwing! :)
                            data.hasThrownGrenadeAndIsWaitingForReturn = false;
                            data.isRedrawInProgress = false;
                        }
                    }
                }
            }

            public override WeaponDisplayData GetWeaponDisplayData(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernGrenadeScriptRuntimeData))
                {
                    Kit_ModernGrenadeScriptRuntimeData data = runtimeData as Kit_ModernGrenadeScriptRuntimeData;
                    if (data.amountOfGrenadesLeft > 0 && (grenadeMode == GrenadeMode.IndividualWeapon || grenadeMode == GrenadeMode.Both))
                    {
                        WeaponDisplayData wdd = new WeaponDisplayData();
                        wdd.sprite = weaponHudPicture;
                        wdd.name = weaponName;
                        return wdd;
                    }
                }
                return null;
            }

            public override WeaponQuickUseDisplayData GetWeaponQuickUseDisplayData(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernGrenadeScriptRuntimeData))
                {
                    Kit_ModernGrenadeScriptRuntimeData data = runtimeData as Kit_ModernGrenadeScriptRuntimeData;
                    if (data.amountOfGrenadesLeft > 0 && (grenadeMode == GrenadeMode.QuickUse || grenadeMode == GrenadeMode.Both))
                    {
                        WeaponQuickUseDisplayData wdd = new WeaponQuickUseDisplayData();
                        wdd.sprite = weaponQuickUsePicture;
                        wdd.name = weaponName;
                        wdd.amount = data.amountOfGrenadesLeft;
                        return wdd;
                    }
                }
                return null;
            }

            public override float AimInTime(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                return 0.5f;
            }

            public override void AnimateWeapon(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, int id, float speed)
            {
                if (pb.isFirstPersonActive)
                {
                    if (pb.isBot) return;
                    if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernGrenadeScriptRuntimeData))
                    {
                        Kit_ModernGrenadeScriptRuntimeData data = runtimeData as Kit_ModernGrenadeScriptRuntimeData;

                        //Camera animation
                        if (data.grenadeRenderer.cameraAnimationEnabled)
                        {
                            if (data.grenadeRenderer.cameraAnimationType == CameraAnimationType.Copy)
                            {
                                pb.playerCameraAnimationTransform.localRotation = Quaternion.Euler(data.grenadeRenderer.cameraAnimationReferenceRotation) * data.grenadeRenderer.cameraAnimationBone.localRotation;
                            }
                            else if (data.grenadeRenderer.cameraAnimationType == CameraAnimationType.LookAt)
                            {
                                pb.playerCameraAnimationTransform.localRotation = Quaternion.Euler(data.grenadeRenderer.cameraAnimationReferenceRotation) * Quaternion.LookRotation(data.grenadeRenderer.cameraAnimationTarget.localPosition - data.grenadeRenderer.cameraAnimationBone.localPosition);
                            }
                        }
                        else
                        {
                            //Go back to 0,0,0
                            pb.playerCameraAnimationTransform.localRotation = Quaternion.Slerp(pb.playerCameraAnimationTransform.localRotation, Quaternion.identity, Time.deltaTime * 5f);
                        }

                        //Weapon delay calculation
                        data.weaponDelayLastDifference = data.weaponDelayLastRotation * Quaternion.Inverse(pb.mouseLookObject.rotation);
                        Vector3 differenceEulerAngles = data.weaponDelayLastDifference.eulerAngles;
                        float x = differenceEulerAngles.y;
                        float y = differenceEulerAngles.x;

                        if (x > 180) x -= 360f;
                        if (y > 180) y -= 360f;

                        //Get input from the mouse
                        data.weaponDelayCurrentX = x * weaponDelayBaseAmount * Time.deltaTime;
                        data.weaponDelayCurrentY = -y * weaponDelayBaseAmount * Time.deltaTime;

                        data.weaponDelayLastRotation = pb.mouseLookObject.rotation;

                        //Clamp
                        data.weaponDelayCurrentX = Mathf.Clamp(data.weaponDelayCurrentX, -weaponDelayMaxAmount, weaponDelayMaxAmount);
                        data.weaponDelayCurrentY = Mathf.Clamp(data.weaponDelayCurrentY, -weaponDelayMaxAmount, weaponDelayMaxAmount);

                        //Update Vector
                        data.weaponDelayCur.x = data.weaponDelayCurrentX;
                        data.weaponDelayCur.y = data.weaponDelayCurrentY;
                        data.weaponDelayCur.z = 0f;

                        //Smooth move towards the target
                        data.weaponDelayTransform.localPosition = Vector3.Lerp(data.weaponDelayTransform.localPosition, data.weaponDelayCur, Time.deltaTime * weaponDelaySmooth);

                        //Weapon tilt
                        if (weaponTiltEnabled)
                        {
                            data.weaponDelayTransform.localRotation = Quaternion.Slerp(data.weaponDelayTransform.localRotation, Quaternion.Euler(0, 0, -pb.movement.GetMovementDirection(pb).x * weaponTiltIntensity), Time.deltaTime * weaponTiltReturnSpeed);
                        }
                        else
                        {
                            data.weaponDelayTransform.localRotation = Quaternion.Slerp(data.weaponDelayTransform.localRotation, Quaternion.identity, Time.deltaTime * weaponTiltReturnSpeed);
                        }

                        //Weapon Fall
                        data.weaponFallTransform.localRotation = Quaternion.Slerp(data.weaponFallTransform.localRotation, Quaternion.identity, Time.deltaTime * fallDownReturnSpeed);

                        //Set speed
                        if (id != 0)
                        {
                            data.genericAnimator.SetFloat("speed", speed);
                        }
                        //If idle, set speed to 1
                        else
                        {
                            data.genericAnimator.SetFloat("speed", 1f);
                        }

                        //Run position and rotation
                        //Check state and if we can move
                        if (id == 2 && data.isSelectedAndReady)
                        {
                            //Move to run pos
                            data.grenadeRenderer.transform.localPosition = Vector3.Lerp(data.grenadeRenderer.transform.localPosition, data.grenadeRenderer.runPos, Time.deltaTime * data.grenadeRenderer.runSmooth);
                            //Move to run rot
                            data.grenadeRenderer.transform.localRotation = Quaternion.Slerp(data.grenadeRenderer.transform.localRotation, Quaternion.Euler(data.grenadeRenderer.runRot), Time.deltaTime * data.grenadeRenderer.runSmooth);
                            //Set time
                            data.lastRun = Time.time;
                        }
                        else
                        {
                            //Move back to idle pos
                            data.grenadeRenderer.transform.localPosition = Vector3.Lerp(data.grenadeRenderer.transform.localPosition, Vector3.zero, Time.deltaTime * data.grenadeRenderer.runSmooth * 2f);
                            //Move back to idle rot
                            data.grenadeRenderer.transform.localRotation = Quaternion.Slerp(data.grenadeRenderer.transform.localRotation, Quaternion.identity, Time.deltaTime * data.grenadeRenderer.runSmooth * 2f);
                        }

                        if (data.grenadeRenderer.legacyAnim && !data.grenadeRenderer.legacyAnim.isPlaying && !data.isThrowingGrenade && !data.hasThrownGrenadeAndIsWaitingForReturn && !data.isRedrawInProgress && data.isSelectedAndReady)
                        {
                            data.grenadeRenderer.legacyAnim[data.grenadeRenderer.legacyAnimData.idle].wrapMode = WrapMode.Loop;
                            data.grenadeRenderer.legacyAnim.CrossFade(data.grenadeRenderer.legacyAnimData.idle, 0.3f, PlayMode.StopAll);
                        }

                        //Check if state changed
                        if (id != data.lastWeaponAnimationID)
                        {
                            //Idle
                            if (id == 0)
                            {
                                //Play idle animation
                                data.genericAnimator.CrossFade("Idle", 0.3f);

                                if (!useGenericRunAnim)
                                {
                                    //End run animation on weapon animator
                                    if (data.startedRunAnimation)
                                    {
                                        data.startedRunAnimation = false;
                                        if (data.grenadeRenderer.anim)
                                        {
                                            data.grenadeRenderer.anim.ResetTrigger("Start Run");
                                            data.grenadeRenderer.anim.SetTrigger("End Run");
                                        }
                                        else if (data.grenadeRenderer.legacyAnim)
                                        {
                                            data.grenadeRenderer.legacyAnim[data.grenadeRenderer.legacyAnimData.idle].wrapMode = WrapMode.Loop;
                                            data.grenadeRenderer.legacyAnim.CrossFade(data.grenadeRenderer.legacyAnimData.idle, 0.3f, PlayMode.StopAll);
                                        }
                                    }
                                }
                            }
                            //Walk
                            else if (id == 1)
                            {
                                //Check if we should use generic anim
                                if (useGenericWalkAnim)
                                {
                                    //Play run animation
                                    data.genericAnimator.CrossFade("Walk", 0.2f);
                                }
                                //If not continue to play Idle
                                else
                                {
                                    //Play idle animation
                                    data.genericAnimator.CrossFade("Idle", 0.3f);
                                }

                                if (!useGenericRunAnim)
                                {
                                    //End run animation on weapon animator
                                    if (data.startedRunAnimation)
                                    {
                                        data.startedRunAnimation = false;
                                        if (data.grenadeRenderer.anim)
                                        {
                                            data.grenadeRenderer.anim.ResetTrigger("Start Run");
                                            data.grenadeRenderer.anim.SetTrigger("End Run");
                                        }
                                        else if (data.grenadeRenderer.legacyAnim)
                                        {
                                            data.grenadeRenderer.legacyAnim[data.grenadeRenderer.legacyAnimData.idle].wrapMode = WrapMode.Loop;
                                            data.grenadeRenderer.legacyAnim.CrossFade(data.grenadeRenderer.legacyAnimData.idle, 0.3f, PlayMode.StopAll);
                                        }
                                    }
                                }
                            }
                            //Run
                            else if (id == 2)
                            {
                                //Check if we should use generic anim
                                if (useGenericRunAnim)
                                {
                                    //Play run animation
                                    data.genericAnimator.CrossFade("Run", 0.2f);
                                }
                                //If not continue to play Idle
                                else
                                {
                                    //Play idle animation
                                    data.genericAnimator.CrossFade("Idle", 0.3f);
                                    //Start run animation on weapon animator
                                    if (!data.startedRunAnimation && data.isSelectedAndReady)
                                    {
                                        data.startedRunAnimation = true;
                                        if (data.grenadeRenderer.anim)
                                        {
                                            data.grenadeRenderer.anim.ResetTrigger("End Run");
                                            data.grenadeRenderer.anim.SetTrigger("Start Run");
                                        }
                                        else if (data.grenadeRenderer.legacyAnim)
                                        {
                                            data.grenadeRenderer.legacyAnim[data.grenadeRenderer.legacyAnimData.run].wrapMode = WrapMode.Loop;
                                            data.grenadeRenderer.legacyAnim.CrossFade(data.grenadeRenderer.legacyAnimData.run, 0.3f, PlayMode.StopAll);
                                        }
                                    }
                                }
                            }
                            //Update last state
                            data.lastWeaponAnimationID = id;
                        }
                        else
                        {
                            if (!useGenericRunAnim)
                            {
                                //Idle
                                if (id == 0)
                                {
                                    //End run animation on weapon animator
                                    if (data.startedRunAnimation)
                                    {
                                        data.startedRunAnimation = false;
                                        if (data.grenadeRenderer.anim)
                                        {
                                            data.grenadeRenderer.anim.ResetTrigger("Start Run");
                                            data.grenadeRenderer.anim.SetTrigger("End Run");
                                        }
                                        else if (data.grenadeRenderer.legacyAnim)
                                        {
                                            data.grenadeRenderer.legacyAnim[data.grenadeRenderer.legacyAnimData.idle].wrapMode = WrapMode.Loop;
                                            data.grenadeRenderer.legacyAnim.CrossFade(data.grenadeRenderer.legacyAnimData.idle, 0.3f, PlayMode.StopAll);
                                        }
                                    }
                                }
                                //Walk
                                else if (id == 1)
                                {
                                    //End run animation on weapon animator
                                    if (data.startedRunAnimation)
                                    {
                                        data.startedRunAnimation = false;
                                        if (data.grenadeRenderer.anim)
                                        {
                                            data.grenadeRenderer.anim.ResetTrigger("Start Run");
                                            data.grenadeRenderer.anim.SetTrigger("End Run");
                                        }
                                        else if (data.grenadeRenderer.legacyAnim)
                                        {
                                            data.grenadeRenderer.legacyAnim[data.grenadeRenderer.legacyAnimData.idle].wrapMode = WrapMode.Loop;
                                            data.grenadeRenderer.legacyAnim.CrossFade(data.grenadeRenderer.legacyAnimData.idle, 0.3f, PlayMode.StopAll);
                                        }
                                    }
                                }
                                //Run
                                else if (id == 2)
                                {
                                    //Start run animation on weapon animator
                                    if (!data.startedRunAnimation && data.isSelectedAndReady)
                                    {
                                        data.startedRunAnimation = true;
                                        if (data.grenadeRenderer.anim)
                                        {
                                            data.grenadeRenderer.anim.ResetTrigger("End Run");
                                            data.grenadeRenderer.anim.SetTrigger("Start Run");
                                        }
                                        else if (data.grenadeRenderer.legacyAnim)
                                        {
                                            data.grenadeRenderer.legacyAnim[data.grenadeRenderer.legacyAnimData.run].wrapMode = WrapMode.Loop;
                                            data.grenadeRenderer.legacyAnim.CrossFade(data.grenadeRenderer.legacyAnimData.run, 0.3f, PlayMode.StopAll);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            public override bool SupportsCustomization()
            {
                return false;
            }

            public override bool CanBeSelected(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                //Quick use only?
                if (grenadeMode == GrenadeMode.QuickUse) return false;
                if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernGrenadeScriptRuntimeData))
                {
                    Kit_ModernGrenadeScriptRuntimeData data = runtimeData as Kit_ModernGrenadeScriptRuntimeData;
                    if (data.amountOfGrenadesLeft > 0) return true;
                    else return false;
                }
                //If not data is to be found, then it cant be selected
                return false;
            }

            public override bool SupportsQuickUse(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                //Grenades DO support this, if it is enabled!
                if (grenadeMode == GrenadeMode.Both || grenadeMode == GrenadeMode.QuickUse)
                {
                    if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernGrenadeScriptRuntimeData))
                    {
                        Kit_ModernGrenadeScriptRuntimeData data = runtimeData as Kit_ModernGrenadeScriptRuntimeData;
                        if (data.amountOfGrenadesLeft > 0) return true;
                    }
                }
                return false;
            }

            public override float BeginQuickUse(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, float revertBy)
            {
                if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernGrenadeScriptRuntimeData))
                {
                    Kit_ModernGrenadeScriptRuntimeData data = runtimeData as Kit_ModernGrenadeScriptRuntimeData;
                    //Set third person anim type
                    pb.thirdPersonPlayerModel.SetAnimType(thirdPersonAnimType);
                    //Stop third person anims
                    pb.thirdPersonPlayerModel.AbortWeaponAnimations();
                    if (pb.isFirstPersonActive)
                    {
                        if (pb.looking.GetPerspective(pb) == Kit_GameInformation.Perspective.FirstPerson)
                        {
                            //Show weapon
                            data.grenadeRenderer.visible = true;
                        }
                        else
                        {
                            data.grenadeRenderer.visible = false;
                        }
                        if (data.grenadeRenderer.anim)
                        {
                            //Enable anim
                            data.grenadeRenderer.anim.enabled = true;
                        }
                        else if (data.grenadeRenderer.legacyAnim)
                        {
                            //Enable anim
                            data.grenadeRenderer.legacyAnim.enabled = true;
                        }
                        //Play Animation
                        if (data.grenadeRenderer.anim)
                        {
                            data.grenadeRenderer.anim.Play("Quick PullPin", 0, 0f);
                        }
                        else if (data.grenadeRenderer.legacyAnim)
                        {
                            data.grenadeRenderer.legacyAnim.Play(data.grenadeRenderer.legacyAnimData.quickPullPin);
                        }
                        //Play Sound
                        data.sounds.clip = pullPinQuickSound;
                        data.sounds.Play();
                        //Play TP Aniamtion
                        pb.thirdPersonPlayerModel.PlayGrenadeAnimation(0);
                    }

                    if (pb.looking.GetPerspective(pb) == Kit_GameInformation.Perspective.FirstPerson && pb.isFirstPersonActive)
                    {
                        //Show tp weapon and hide
                        data.tpGrenadeRenderer.visible = true;
                        data.tpGrenadeRenderer.shadowsOnly = true;
                    }
                    else
                    {
                        //Show tp weapon and show
                        data.tpGrenadeRenderer.visible = true;
                        data.tpGrenadeRenderer.shadowsOnly = false;
                    }

                    return quickUsePullPinTime;
                }

                //In case of failure...
                return 0f;
            }

            public override float EndQuickUse(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, float revertBy)
            {
                if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernGrenadeScriptRuntimeData))
                {
                    Kit_ModernGrenadeScriptRuntimeData data = runtimeData as Kit_ModernGrenadeScriptRuntimeData;

                    if (grenadePrefab)
                    {
                        if (pb.isServer)
                        {
                            //Throw Grenade
                            GameObject go = Instantiate(grenadePrefab, pb.playerCameraTransform.position, pb.playerCameraTransform.rotation);

                            //Get script
                            Kit_Grenade grenade = go.GetComponent<Kit_Grenade>();

                            if (grenade)
                            {
                                grenade.gunId = gameGunID;
                                grenade.ownerBot = pb.isBot;
                                grenade.ownerId = pb.id;
                                grenade.initialPlayerVelocity = pb.movement.GetVelocity(pb);
                                grenade.forceToApply = throwForce;
                                grenade.torqueToApply = throwTorque;
                            }

                            Kit_GrenadeSmoke smokeGrenade = go.GetComponent<Kit_GrenadeSmoke>();

                            if (smokeGrenade)
                            {
                                smokeGrenade.ownerBot = pb.isBot;
                                smokeGrenade.ownerId = pb.id;
                                smokeGrenade.initialPlayerVelocity = pb.movement.GetVelocity(pb);
                                smokeGrenade.forceToApply = throwForce;
                                smokeGrenade.torqueToApply = throwTorque;
                            }

                            NetworkServer.Spawn(go);
                        }
                    }
                    else
                    {
                        Debug.LogError("No Grenade Prefab!");
                    }

                    //Subtract
                    data.amountOfGrenadesLeft--;

                    if (pb.isFirstPersonActive)
                    {
                        //Play Animation
                        if (data.grenadeRenderer.anim)
                        {
                            data.grenadeRenderer.anim.Play("Quick Throw", 0, 0f);
                        }
                        else if (data.grenadeRenderer.legacyAnim)
                        {
                            data.grenadeRenderer.legacyAnim.Play(data.grenadeRenderer.legacyAnimData.quickThrow);
                        }
                        //Play Sound
                        data.sounds.clip = throwQuickSound;
                        data.sounds.Play();
                        //Play TP Aniamtion
                        pb.thirdPersonPlayerModel.PlayGrenadeAnimation(1);
                    }

                    //Relay to voice manager
                    if (pb.voiceManager)
                    {
                        pb.voiceManager.GrenadeThrown(pb, voiceGrenadeSoundID);
                    }

                    return quickUseThrowTime;
                }
                //In case of failure...
                return 0f;
            }

            public override void EndQuickUseAfter(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, float revertBy)
            {
                if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernGrenadeScriptRuntimeData))
                {
                    Kit_ModernGrenadeScriptRuntimeData data = runtimeData as Kit_ModernGrenadeScriptRuntimeData;
                    if (pb.isFirstPersonActive)
                    {
                        if (data.grenadeRenderer.anim)
                        {
                            //Disable anim
                            data.grenadeRenderer.anim.enabled = false;
                        }
                        else if (data.grenadeRenderer.legacyAnim)
                        {
                            //Disable anim
                            data.grenadeRenderer.legacyAnim.enabled = false;
                        }
                        data.grenadeRenderer.visible = false;
                    }
                    data.tpGrenadeRenderer.visible = false;
                }
            }

            public override bool WaitForQuickUseButtonRelease(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                return true;
            }

            public override void CalculateWeaponUpdate(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernGrenadeScriptRuntimeData))
                {
                    Kit_ModernGrenadeScriptRuntimeData data = runtimeData as Kit_ModernGrenadeScriptRuntimeData;

                    //Update HUD
                    if (pb.isFirstPersonActive)
                    {
                        Kit_IngameMain.instance.hud.DisplayAmmo(-1, data.amountOfGrenadesLeft, true);
                    }
                }
            }

            public override void NetworkGrenadePullPinRPCReceived(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                /*
                if (!pb.photonView.IsMine)
                {
                    if (runtimeData!= null && runtimeData.GetType() == typeof(Kit_ModernGrenadeScriptRuntimeData))
                    {
                        Kit_ModernGrenadeScriptRuntimeData data = runtimeData as Kit_ModernGrenadeScriptRuntimeData;
                        //Play TP anim
                        pb.thirdPersonPlayerModel.PlayGrenadeAnimation(0);

                        if (pb.isFirstPersonActive)
                        {
                            //Play Animation
                            if (data.grenadeRenderer.anim)
                            {
                                data.grenadeRenderer.anim.Play("PullPin", 0, 0f);
                            }
                            else if (data.grenadeRenderer.legacyAnim)
                            {
                                data.grenadeRenderer.legacyAnim.Play(data.grenadeRenderer.legacyAnimData.pullPin);
                            }
                            //Play Sound
                            data.sounds.clip = pullPinSound;
                            data.sounds.Play();
                        }
                        else
                        {
                            //Update range
                            pb.thirdPersonPlayerModel.soundFire.maxDistance = thirdPersonRange;
                            //Update sound rolloff
                            pb.thirdPersonPlayerModel.soundFire.SetCustomCurve(AudioSourceCurveType.CustomRolloff, thirdPersonRolloff);
                            //Play Sound
                            pb.thirdPersonPlayerModel.soundFire.clip = pullPinSound;
                            pb.thirdPersonPlayerModel.soundFire.Play();
                        }
                    }
                }
                */
            }

            public override void NetworkGrenadeThrowRPCReceived(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                /*
                if (!pb.photonView.IsMine)
                {
                    if (runtimeData!= null && runtimeData.GetType() == typeof(Kit_ModernGrenadeScriptRuntimeData))
                    {
                        Kit_ModernGrenadeScriptRuntimeData data = runtimeData as Kit_ModernGrenadeScriptRuntimeData;
                        //Play TP anim
                        pb.thirdPersonPlayerModel.PlayGrenadeAnimation(1);
                        if (pb.isFirstPersonActive)
                        {
                            //Play Animation
                            if (data.grenadeRenderer.anim)
                            {
                                data.grenadeRenderer.anim.Play("Throw", 0, 0f);
                            }
                            else if (data.grenadeRenderer.legacyAnim)
                            {
                                data.grenadeRenderer.legacyAnim.Play(data.grenadeRenderer.legacyAnimData.throwNade);
                            }
                            //Play Sound
                            data.sounds.clip = throwSound;
                            data.sounds.Play();
                        }
                        else
                        {
                            //Update range
                            pb.thirdPersonPlayerModel.soundFire.maxDistance = thirdPersonRange;
                            //Update sound rolloff
                            pb.thirdPersonPlayerModel.soundFire.SetCustomCurve(AudioSourceCurveType.CustomRolloff, thirdPersonRolloff);
                            //Play Sound
                            pb.thirdPersonPlayerModel.soundFire.clip = throwSound;
                            pb.thirdPersonPlayerModel.soundFire.Play();
                        }
                    }
                }
                */
            }

            public override void DrawWeapon(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernGrenadeScriptRuntimeData))
                {
                    Kit_ModernGrenadeScriptRuntimeData data = runtimeData as Kit_ModernGrenadeScriptRuntimeData;
                    //Set selected
                    data.isSelected = true;
                    if (pb.isFirstPersonActive)
                    {
                        //Reset pos & rot of the renderer
                        data.grenadeRenderer.transform.localPosition = Vector3.zero;
                        data.grenadeRenderer.transform.localRotation = Quaternion.identity;
                        //Reset fov
                        Kit_IngameMain.instance.mainCamera.fieldOfView = Kit_GameSettings.baseFov;
                        //Reset delay
                        data.weaponDelayCur = Vector3.zero;
                        if (data.grenadeRenderer.anim)
                        {
                            //Enable anim
                            data.grenadeRenderer.anim.enabled = true;
                        }
                        else if (data.grenadeRenderer.legacyAnim)
                        {
                            //Enable anim
                            data.grenadeRenderer.legacyAnim.enabled = true;
                        }
                        //Play animation
                        if (data.grenadeRenderer.anim)
                        {
                            data.grenadeRenderer.anim.Play("Draw", 0, 0f);
                        }
                        else if (data.grenadeRenderer.legacyAnim)
                        {
                            data.grenadeRenderer.legacyAnim.Play(data.grenadeRenderer.legacyAnimData.draw);
                        }
                        //Play sound if it is assigned
                        if (drawSound) data.sounds.PlayOneShot(drawSound);
                        if (pb.looking.GetPerspective(pb) == Kit_GameInformation.Perspective.FirstPerson)
                        {
                            //Show weapon
                            data.grenadeRenderer.visible = true;
                        }
                        else
                        {
                            data.grenadeRenderer.visible = false;
                        }
                    }
                    if (pb.looking.GetPerspective(pb) == Kit_GameInformation.Perspective.FirstPerson && pb.isFirstPersonActive)
                    {
                        //Show tp weapon and hide
                        data.tpGrenadeRenderer.visible = true;
                        data.tpGrenadeRenderer.shadowsOnly = true;
                    }
                    else
                    {
                        //Show tp weapon and show
                        data.tpGrenadeRenderer.visible = true;
                        data.tpGrenadeRenderer.shadowsOnly = false;
                    }
                    //Make sure it is not ready yet
                    data.isSelectedAndReady = false;
                    //Reset run animation
                    data.startedRunAnimation = false;
                    //Set third person anim type
                    pb.thirdPersonPlayerModel.SetAnimType(thirdPersonAnimType);
                    //Stop third person anims
                    pb.thirdPersonPlayerModel.AbortWeaponAnimations();
                }
            }

            public override void FallDownEffect(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, bool wasFallDamageApplied)
            {
                if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernGrenadeScriptRuntimeData))
                {
                    Kit_ModernGrenadeScriptRuntimeData data = runtimeData as Kit_ModernGrenadeScriptRuntimeData;
                    if (wasFallDamageApplied)
                    {
                        Kit_ScriptableObjectCoroutineHelper.instance.StartCoroutine(Kit_ScriptableObjectCoroutineHelper.instance.Kick(data.weaponFallTransform, new Vector3(fallDownAmount, Random.Range(fallDownMinOffset, fallDownMaxoffset), 0), fallDownTime));
                    }
                    else
                    {
                        Kit_ScriptableObjectCoroutineHelper.instance.StartCoroutine(Kit_ScriptableObjectCoroutineHelper.instance.Kick(data.weaponFallTransform, new Vector3(fallDownAmount / 3, Random.Range(fallDownMinOffset, fallDownMaxoffset) / 2, 0), fallDownTime));
                    }
                }
            }

            public override void FirstThirdPersonChanged(Kit_PlayerBehaviour pb, Kit_GameInformation.Perspective perspective, Kit_WeaponRuntimeDataBase runtimeData)
            {
                if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernGrenadeScriptRuntimeData))
                {
                    //Get runtime data
                    Kit_ModernGrenadeScriptRuntimeData data = runtimeData as Kit_ModernGrenadeScriptRuntimeData;
                    //Activate or deactivate based on bool
                    if (perspective == Kit_GameInformation.Perspective.ThirdPerson)
                    {
                        if (data.grenadeRenderer)
                        {
                            data.grenadeRenderer.visible = false;
                        }
                        if (data.tpGrenadeRenderer)
                        {
                            data.tpGrenadeRenderer.visible = true;
                            data.tpGrenadeRenderer.shadowsOnly = false;
                        }
                    }
                    else
                    {
                        if (data.grenadeRenderer)
                        {
                            data.grenadeRenderer.visible = true;
                        }
                        if (data.tpGrenadeRenderer)
                        {
                            data.tpGrenadeRenderer.visible = true;
                            data.tpGrenadeRenderer.shadowsOnly = true;
                        }
                    }
                }
            }

            public override bool ForceIntoFirstPerson(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                return false;
            }

            public override WeaponIKValues GetIK(Kit_PlayerBehaviour pb, Animator anim, Kit_WeaponRuntimeDataBase runtimeData)
            {
                return new WeaponIKValues();
            }

            public override WeaponStats GetStats()
            {
                return new WeaponStats();
            }

            public override bool SupportsStats()
            {
                return false;
            }

            public override bool IsWeaponAiming(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                return false;
            }

            public override void PutawayWeapon(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernGrenadeScriptRuntimeData))
                {
                    Kit_ModernGrenadeScriptRuntimeData data = runtimeData as Kit_ModernGrenadeScriptRuntimeData;
                    if (pb.isFirstPersonActive)
                    {
                        //Reset fov
                        Kit_IngameMain.instance.mainCamera.fieldOfView = Kit_GameSettings.baseFov;
                        if (data.grenadeRenderer.anim)
                        {
                            //Enable anim
                            data.grenadeRenderer.anim.enabled = true;
                        }
                        else if (data.grenadeRenderer.legacyAnim)
                        {
                            //Enable anim
                            data.grenadeRenderer.legacyAnim.enabled = true;
                        }
                        //Play animation
                        if (data.grenadeRenderer.anim)
                        {
                            data.grenadeRenderer.anim.Play("Putaway", 0, 0f);
                        }
                        else if (data.grenadeRenderer.legacyAnim)
                        {
                            data.grenadeRenderer.legacyAnim.Play(data.grenadeRenderer.legacyAnimData.putaway);
                        }
                        //Play sound if it is assigned
                        if (putawaySound) data.sounds.PlayOneShot(putawaySound);
                        if (pb.looking.GetPerspective(pb) == Kit_GameInformation.Perspective.FirstPerson)
                        {
                            //Show weapon
                            data.grenadeRenderer.visible = true;
                        }
                        else
                        {
                            //Hide
                            data.grenadeRenderer.visible = false;
                        }
                    }
                    if (pb.looking.GetPerspective(pb) == Kit_GameInformation.Perspective.FirstPerson && pb.isFirstPersonActive)
                    {
                        //Show tp weapon
                        data.tpGrenadeRenderer.visible = true;
                        data.tpGrenadeRenderer.shadowsOnly = true;
                    }
                    else
                    {
                        data.tpGrenadeRenderer.visible = true;
                        data.tpGrenadeRenderer.shadowsOnly = false;
                    }
                    //Make sure it is not ready yet
                    data.isSelectedAndReady = false;
                    //Abort any throws!
                    data.hasThrownGrenadeAndIsWaitingForReturn = false;
                    data.isThrowingGrenade = false;
                    data.isRedrawInProgress = false;
                    //Stop third person anims
                    pb.thirdPersonPlayerModel.AbortWeaponAnimations();
                }
            }

            public override void PutawayWeaponHide(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernGrenadeScriptRuntimeData))
                {
                    Kit_ModernGrenadeScriptRuntimeData data = runtimeData as Kit_ModernGrenadeScriptRuntimeData;
                    //Set selected
                    data.isSelected = false;
                    //Reset run animation
                    data.startedRunAnimation = false;
                    if (pb.isFirstPersonActive)
                    {
                        //Hide weapon
                        data.grenadeRenderer.visible = false;
                        if (data.grenadeRenderer.anim)
                        {
                            //Disable anim
                            data.grenadeRenderer.anim.enabled = false;
                        }
                        else if (data.grenadeRenderer.legacyAnim)
                        {
                            //Disable anim
                            data.grenadeRenderer.legacyAnim.enabled = false;
                        }
                        //Reset pos & rot of the renderer
                        data.grenadeRenderer.transform.localPosition = Vector3.zero;
                        data.grenadeRenderer.transform.localRotation = Quaternion.identity;
                        //Reset delay
                        data.weaponDelayCur = Vector3.zero;
                    }
                    //Hide tp weapon
                    data.tpGrenadeRenderer.visible = false;
                    //Make sure it is not ready
                    data.isSelectedAndReady = false;
                    //Abort any throws!
                    data.hasThrownGrenadeAndIsWaitingForReturn = false;
                    data.isThrowingGrenade = false;
                    data.isRedrawInProgress = false;
                    //Stop third person anims
                    pb.thirdPersonPlayerModel.AbortWeaponAnimations();
                }
            }

            public override float Sensitivity(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                return 1f;
            }

            public override void SetupFirstPerson(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                Kit_ModernGrenadeScriptRuntimeData data = runtimeData as Kit_ModernGrenadeScriptRuntimeData;

                //Setup root for this weapon
                GameObject root = new GameObject("Weapon root");
                root.transform.parent = pb.weaponsGo; //Set root
                root.transform.localPosition = Vector3.zero; //Reset position
                root.transform.localRotation = Quaternion.identity; //Reset rotation
                root.transform.localScale = Vector3.one; //Reset scale

                //Setup generic animations
                GameObject genericAnimations = Instantiate(genericGunAnimatorControllerPrefab);
                genericAnimations.transform.parent = root.transform;
                genericAnimations.transform.localPosition = Vector3.zero; //Reset position
                genericAnimations.transform.localRotation = Quaternion.identity; //Reset rotation
                genericAnimations.transform.localScale = Vector3.one; //Reset scale

                //Get animator
                Animator anim = genericAnimations.GetComponent<Animator>(); ;
                anim.Play("Idle");
                data.genericAnimator = anim;

                //Delay transform
                GameObject delayTrans = new GameObject("Weapon delay");
                delayTrans.transform.parent = genericAnimations.transform; //Set root
                delayTrans.transform.localPosition = Vector3.zero; //Reset position
                delayTrans.transform.localRotation = Quaternion.identity; //Reset rotation
                delayTrans.transform.localScale = Vector3.one; //Reset scale

                //Assign it
                data.weaponDelayTransform = delayTrans.transform;

                //Delay transform
                GameObject fallTrans = new GameObject("Weapon fall");
                fallTrans.transform.parent = delayTrans.transform; //Set root
                fallTrans.transform.localPosition = Vector3.zero; //Reset position
                fallTrans.transform.localRotation = Quaternion.identity; //Reset rotation
                fallTrans.transform.localScale = Vector3.one; //Reset scale

                //Assign it
                data.weaponFallTransform = fallTrans.transform;

                //Get Fire Audio (Needs to be consistent)
                if (pb.weaponsGo.GetComponent<AudioSource>()) data.sounds = pb.weaponsGo.GetComponent<AudioSource>();
                else data.sounds = pb.weaponsGo.gameObject.AddComponent<AudioSource>();

                //Setup the first person prefab
                GameObject fpRuntime = Instantiate(firstPersonPrefab, fallTrans.transform, false);
                fpRuntime.transform.localScale = Vector3.one; //Reset scale

                //Setup renderer
                data.grenadeRenderer = fpRuntime.GetComponent<Kit_GrenadeRenderer>();

                //Play Dependent arms
                if (data.grenadeRenderer.playerModelDependentArmsEnabled && pb.thirdPersonPlayerModel.firstPersonArmsPrefab.Contains(data.grenadeRenderer.playerModelDependentArmsKey))
                {
                    //Create Prefab
                    GameObject fpArms = Instantiate(pb.thirdPersonPlayerModel.firstPersonArmsPrefab[data.grenadeRenderer.playerModelDependentArmsKey], fallTrans.transform, false);

#if INTEGRATION_FPV2
                        //Set shaders
                        FirstPersonView.ShaderMaterialSolution.FPV_SM_Object armsObj = fpArms.GetComponent<FirstPersonView.ShaderMaterialSolution.FPV_SM_Object>();
                        if (armsObj)
                        {
                            //Set as first person object
                            armsObj.SetAsFirstPersonObject();
                        }
                        else
                        {
                            Debug.LogError("FPV2 is enabled but first person prefab does not have FPV_SM_Object assigned!");
                        }
#elif INTEGRATION_FPV3
                        //Set shaders
                        FirstPersonView.FPV_Object armsObj = fpArms.GetComponent<FirstPersonView.FPV_Object>();
                        if (armsObj)
                        {
                            //Set as first person object
                            armsObj.SetAsFirstPersonObject();
                        }
                        else
                        {
                            Debug.LogError("FPV3 is enabled but first person prefab does not have FPV_Object assigned!");
                        }
#endif

                    //Get Arms
                    Kit_FirstPersonArms fpa = fpArms.GetComponent<Kit_FirstPersonArms>();
                    //Reparent
                    for (int i = 0; i < fpa.reparents.Length; i++)
                    {
                        if (fpa.reparents[i])
                        {
                            fpa.reparents[i].transform.parent = data.grenadeRenderer.playerModelDependentArmsRoot;
                        }
                        else
                        {
                            Debug.LogWarning("First Person Arm Renderer at index " + i + " is not assigned", fpa);
                        }
                    }
                    //Merge Array
                    data.grenadeRenderer.allWeaponRenderers = data.grenadeRenderer.allWeaponRenderers.Concat(fpa.renderers).ToArray();
                    //Rebind so that the animator animates our freshly reparented transforms too!
                    if (data.grenadeRenderer.anim)
                    {
                        data.grenadeRenderer.anim.Rebind();
                    }
                }

                data.grenadeRenderer.visible = false;

#if INTEGRATION_FPV2
                    //Set shaders
                    FirstPersonView.ShaderMaterialSolution.FPV_SM_Object obj = fpRuntime.GetComponent<FirstPersonView.ShaderMaterialSolution.FPV_SM_Object>();
                    if (obj)
                    {
                        //Set as first person object
                        obj.SetAsFirstPersonObject();
                    }
                    else
                    {
                        Debug.LogError("FPV2 is enabled but first person prefab does not have FPV_SM_Object assigned!");
                    }
#elif INTEGRATION_FPV3
                    //Set shaders
                    FirstPersonView.FPV_Object obj = fpRuntime.GetComponent<FirstPersonView.FPV_Object>();
                    if (obj)
                    {
                        //Set as first person object
                        obj.SetAsFirstPersonObject();
                    }
                    else
                    {
                        Debug.LogError("FPV3 is enabled but first person prefab does not have FPV_Object assigned!");
                    }
#endif

                //Add to the list
                data.instantiatedObjects.Add(root);

                //Set data
                data.amountOfGrenadesLeft = amountOfGrenadesAtStart;
            }

            public override void SetupThirdPerson(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                Kit_ModernGrenadeScriptRuntimeData data = runtimeData as Kit_ModernGrenadeScriptRuntimeData;

                //Setup the third person prefab
                GameObject tpRuntime = Instantiate(thirdPersonPrefab, pb.thirdPersonPlayerModel.weaponsInHandsGo, false);
                //Set Scale
                tpRuntime.transform.localScale = Vector3.one;

                //Setup renderer
                data.tpGrenadeRenderer = tpRuntime.GetComponent<Kit_ThirdPersonGrenadeRenderer>();
                data.tpGrenadeRenderer.visible = false;
                if (pb.isFirstPersonActive)
                {
                    //Make it shadows only
                    data.tpGrenadeRenderer.shadowsOnly = true;
                }

                //Add to the list
                data.instantiatedObjects.Add(tpRuntime);
            }

            public override float SpeedMultiplier(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                return 1f;
            }

            public override int WeaponState(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                return 0;
            }

            public override int GetWeaponType(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                return 3;
            }


            public override void BeginSpectating(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, int[] attachments)
            {
                Kit_ModernGrenadeScriptRuntimeData data = runtimeData as Kit_ModernGrenadeScriptRuntimeData;

                //Create First Person renderer if it doesn't exist already
                if (!data.grenadeRenderer)
                {
                    //Setup root for this weapon
                    GameObject root = new GameObject("Weapon root");
                    root.transform.parent = pb.weaponsGo; //Set root
                    root.transform.localPosition = Vector3.zero; //Reset position
                    root.transform.localRotation = Quaternion.identity; //Reset rotation
                    root.transform.localScale = Vector3.one; //Reset scale

                    //Setup generic animations
                    GameObject genericAnimations = Instantiate(genericGunAnimatorControllerPrefab);
                    genericAnimations.transform.parent = root.transform;
                    genericAnimations.transform.localPosition = Vector3.zero; //Reset position
                    genericAnimations.transform.localRotation = Quaternion.identity; //Reset rotation
                    genericAnimations.transform.localScale = Vector3.one; //Reset scale

                    //Get animator
                    Animator anim = genericAnimations.GetComponent<Animator>(); ;
                    anim.Play("Idle");
                    data.genericAnimator = anim;

                    //Delay transform
                    GameObject delayTrans = new GameObject("Weapon delay");
                    delayTrans.transform.parent = genericAnimations.transform; //Set root
                    delayTrans.transform.localPosition = Vector3.zero; //Reset position
                    delayTrans.transform.localRotation = Quaternion.identity; //Reset rotation
                    delayTrans.transform.localScale = Vector3.one; //Reset scale

                    //Assign it
                    data.weaponDelayTransform = delayTrans.transform;

                    //Delay transform
                    GameObject fallTrans = new GameObject("Weapon fall");
                    fallTrans.transform.parent = delayTrans.transform; //Set root
                    fallTrans.transform.localPosition = Vector3.zero; //Reset position
                    fallTrans.transform.localRotation = Quaternion.identity; //Reset rotation
                    fallTrans.transform.localScale = Vector3.one; //Reset scale

                    //Assign it
                    data.weaponFallTransform = fallTrans.transform;

                    //Get Fire Audio (Needs to be consistent)
                    if (pb.weaponsGo.GetComponent<AudioSource>()) data.sounds = pb.weaponsGo.GetComponent<AudioSource>();
                    else data.sounds = pb.weaponsGo.gameObject.AddComponent<AudioSource>();

                    //Setup the first person prefab
                    GameObject fpRuntime = Instantiate(firstPersonPrefab, fallTrans.transform, false);
                    fpRuntime.transform.localScale = Vector3.one; //Reset scale

                    //Setup renderer
                    data.grenadeRenderer = fpRuntime.GetComponent<Kit_GrenadeRenderer>();

                    //Play Dependent arms
                    if (data.grenadeRenderer.playerModelDependentArmsEnabled && pb.thirdPersonPlayerModel.firstPersonArmsPrefab.Contains(data.grenadeRenderer.playerModelDependentArmsKey))
                    {
                        //Create Prefab
                        GameObject fpArms = Instantiate(pb.thirdPersonPlayerModel.firstPersonArmsPrefab[data.grenadeRenderer.playerModelDependentArmsKey], fallTrans.transform, false);

#if INTEGRATION_FPV2
                        //Set shaders
                        FirstPersonView.ShaderMaterialSolution.FPV_SM_Object armsObj = fpArms.GetComponent<FirstPersonView.ShaderMaterialSolution.FPV_SM_Object>();
                        if (armsObj)
                        {
                            //Set as first person object
                            armsObj.SetAsFirstPersonObject();
                        }
                        else
                        {
                            Debug.LogError("FPV2 is enabled but first person prefab does not have FPV_SM_Object assigned!");
                        }

#elif INTEGRATION_FPV3
                        //Set shaders
                        FirstPersonView.FPV_Object armsObj = fpArms.GetComponent<FirstPersonView.FPV_Object>();
                        if (armsObj)
                        {
                            //Set as first person object
                            armsObj.SetAsFirstPersonObject();
                        }
                        else
                        {
                            Debug.LogError("FPV3 is enabled but first person prefab does not have FPV_Object assigned!");
                        }
#endif

                        //Get Arms
                        Kit_FirstPersonArms fpa = fpArms.GetComponent<Kit_FirstPersonArms>();

                        if (fpa.cameraBoneOverride)
                        {
                            data.grenadeRenderer.cameraAnimationBone = fpa.cameraBoneOverride;
                        }

                        if (fpa.cameraBoneTargetOverride)
                        {
                            data.grenadeRenderer.cameraAnimationTarget = fpa.cameraBoneTargetOverride;
                        }

                        //Reparent
                        for (int i = 0; i < fpa.reparents.Length; i++)
                        {
                            if (fpa.reparents[i])
                            {
                                fpa.reparents[i].transform.parent = data.grenadeRenderer.playerModelDependentArmsRoot;
                            }
                            else
                            {
                                Debug.LogWarning("First Person Arm Renderer at index " + i + " is not assigned", fpa);
                            }
                        }
                        //Merge Array
                        data.grenadeRenderer.allWeaponRenderers = data.grenadeRenderer.allWeaponRenderers.Concat(fpa.renderers).ToArray();
                        //Rebind so that the animator animates our freshly reparented transforms too!
                        if (data.grenadeRenderer.anim)
                        {
                            data.grenadeRenderer.anim.Rebind();
                        }
                    }

                    data.grenadeRenderer.visible = false;

#if INTEGRATION_FPV2
                    //Set shaders
                    FirstPersonView.ShaderMaterialSolution.FPV_SM_Object obj = fpRuntime.GetComponent<FirstPersonView.ShaderMaterialSolution.FPV_SM_Object>();
                    if (obj)
                    {
                        //Set as first person object
                        obj.SetAsFirstPersonObject();
                    }
                    else
                    {
                        Debug.LogError("FPV2 is enabled but first person prefab does not have FPV_SM_Object assigned!");
                    }
#elif INTEGRATION_FPV3
                    //Set shaders
                    FirstPersonView.FPV_Object obj = fpRuntime.GetComponent<FirstPersonView.FPV_Object>();
                    if (obj)
                    {
                        //Set as first person object
                        obj.SetAsFirstPersonObject();
                    }
                    else
                    {
                        Debug.LogError("FPV3 is enabled but first person prefab does not have FPV_Object assigned!");
                    }
#endif

                    //Add to the list
                    data.instantiatedObjects.Add(root);
                }

                if (pb.looking.GetPerspective(pb) == Kit_GameInformation.Perspective.FirstPerson)
                {
                    //Show if selected
                    data.grenadeRenderer.visible = data.isSelected;

                    //Hide tp
                    data.tpGrenadeRenderer.shadowsOnly = true;
                }
                else
                {
                    //FP is definitely hidden, TP is already in the state that it should be in
                    data.grenadeRenderer.visible = false;
                }
            }

            public override void EndSpectating(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                //Get data
                Kit_ModernGrenadeScriptRuntimeData data = runtimeData as Kit_ModernGrenadeScriptRuntimeData;

                //Hide grenade renderer if present
                if (data.grenadeRenderer)
                {
                    data.grenadeRenderer.visible = false;
                }

                data.tpGrenadeRenderer.shadowsOnly = false;
            }

            public override void RegisterNetworkPrefabs()
            {
                NetworkClient.RegisterPrefab(grenadePrefab);
            }
        }
    }
}