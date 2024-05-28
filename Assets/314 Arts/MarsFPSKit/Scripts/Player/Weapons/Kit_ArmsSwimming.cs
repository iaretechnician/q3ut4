
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#pragma warning disable 0219

namespace MarsFPSKit
{
    namespace Weapons
    {

        [CreateAssetMenu(menuName = ("MarsFPSKit/Weapons/Arms Swimming Script"))]
        public class Kit_ArmsSwimming : Kit_WeaponBase
        {
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
            /// As ArmsSwimming is a two layer sound, this is the sound ID!
            /// </summary>
            public int voiceArmsSwimmingSoundID;
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

            }

            public override void AuthorativeInput(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, Kit_PlayerInput input, float delta, double revertTime)
            {

            }

            public override WeaponDisplayData GetWeaponDisplayData(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                return null;
            }

            public override WeaponQuickUseDisplayData GetWeaponQuickUseDisplayData(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                return null;
            }

            public override bool SupportsCustomization()
            {
                return false;
            }

            public override bool CanBeSelected(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                return true;
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
                    if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ArmsSwimmingRuntimeData))
                    {
                        Kit_ArmsSwimmingRuntimeData data = runtimeData as Kit_ArmsSwimmingRuntimeData;

                        //Camera animation
                        if (data.armsSwimmingRenderer.cameraAnimationEnabled)
                        {
                            if (data.armsSwimmingRenderer.cameraAnimationType == CameraAnimationType.Copy)
                            {
                                pb.playerCameraAnimationTransform.localRotation = Quaternion.Euler(data.armsSwimmingRenderer.cameraAnimationReferenceRotation) * data.armsSwimmingRenderer.cameraAnimationBone.localRotation;
                            }
                            else if (data.armsSwimmingRenderer.cameraAnimationType == CameraAnimationType.LookAt)
                            {
                                pb.playerCameraAnimationTransform.localRotation = Quaternion.Euler(data.armsSwimmingRenderer.cameraAnimationReferenceRotation) * Quaternion.LookRotation(data.armsSwimmingRenderer.cameraAnimationTarget.localPosition - data.armsSwimmingRenderer.cameraAnimationBone.localPosition);
                            }
                        }
                        else
                        {
                            //Go back to 0,0,0
                            pb.playerCameraAnimationTransform.localRotation = Quaternion.Slerp(pb.playerCameraAnimationTransform.localRotation, Quaternion.identity, Time.deltaTime * 5f);
                        }

                        //Weapon delay calculation
                        if ((MarsScreen.lockCursor || pb.isBot) && pb.canControlPlayer)
                        {
                            //Get input from the mouse
                            data.weaponDelayCurrentX = -pb.input.mouseX * weaponDelayBaseAmount * Time.deltaTime;
                            if (!pb.looking.ReachedYMax(pb)) //Check if we should have delay on y looking
                                data.weaponDelayCurrentY = -pb.input.mouseY * weaponDelayBaseAmount * Time.deltaTime;
                            else //If not, just set it to zero
                                data.weaponDelayCurrentY = 0f;
                        }
                        else
                        {
                            //Cursor is not locked, set values to zero
                            data.weaponDelayCurrentX = 0f;
                            data.weaponDelayCurrentY = 0f;
                        }

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
                            data.armsSwimmingRenderer.transform.localPosition = Vector3.Lerp(data.armsSwimmingRenderer.transform.localPosition, data.armsSwimmingRenderer.runPos, Time.deltaTime * data.armsSwimmingRenderer.runSmooth);
                            //Move to run rot
                            data.armsSwimmingRenderer.transform.localRotation = Quaternion.Slerp(data.armsSwimmingRenderer.transform.localRotation, Quaternion.Euler(data.armsSwimmingRenderer.runRot), Time.deltaTime * data.armsSwimmingRenderer.runSmooth);
                            //Set time
                            data.lastRun = Time.time;
                        }
                        else
                        {
                            //Move back to idle pos
                            data.armsSwimmingRenderer.transform.localPosition = Vector3.Lerp(data.armsSwimmingRenderer.transform.localPosition, Vector3.zero, Time.deltaTime * data.armsSwimmingRenderer.runSmooth * 2f);
                            //Move back to idle rot
                            data.armsSwimmingRenderer.transform.localRotation = Quaternion.Slerp(data.armsSwimmingRenderer.transform.localRotation, Quaternion.identity, Time.deltaTime * data.armsSwimmingRenderer.runSmooth * 2f);
                        }

                        if (data.armsSwimmingRenderer.legacyAnim && !data.armsSwimmingRenderer.legacyAnim.isPlaying && data.isSelectedAndReady && !data.isCharging)
                        {
                            data.armsSwimmingRenderer.legacyAnim[data.armsSwimmingRenderer.legacyAnimData.idle].wrapMode = WrapMode.Loop;
                            data.armsSwimmingRenderer.legacyAnim.CrossFade(data.armsSwimmingRenderer.legacyAnimData.idle, 0.3f, PlayMode.StopAll);
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
                                        if (data.armsSwimmingRenderer.anim)
                                        {
                                            data.armsSwimmingRenderer.anim.ResetTrigger("Start Run");
                                            data.armsSwimmingRenderer.anim.SetTrigger("End Run");
                                        }
                                        else if (data.armsSwimmingRenderer.legacyAnim)
                                        {
                                            data.armsSwimmingRenderer.legacyAnim[data.armsSwimmingRenderer.legacyAnimData.idle].wrapMode = WrapMode.Loop;
                                            data.armsSwimmingRenderer.legacyAnim.CrossFade(data.armsSwimmingRenderer.legacyAnimData.idle, 0.3f, PlayMode.StopAll);
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
                                        if (data.armsSwimmingRenderer.anim)
                                        {
                                            data.armsSwimmingRenderer.anim.ResetTrigger("Start Run");
                                            data.armsSwimmingRenderer.anim.SetTrigger("End Run");
                                        }
                                        else if (data.armsSwimmingRenderer.legacyAnim)
                                        {
                                            data.armsSwimmingRenderer.legacyAnim[data.armsSwimmingRenderer.legacyAnimData.idle].wrapMode = WrapMode.Loop;
                                            data.armsSwimmingRenderer.legacyAnim.CrossFade(data.armsSwimmingRenderer.legacyAnimData.idle, 0.3f, PlayMode.StopAll);
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
                                        if (data.armsSwimmingRenderer.anim)
                                        {
                                            data.armsSwimmingRenderer.anim.ResetTrigger("End Run");
                                            data.armsSwimmingRenderer.anim.SetTrigger("Start Run");
                                        }
                                        else if (data.armsSwimmingRenderer.legacyAnim)
                                        {
                                            data.armsSwimmingRenderer.legacyAnim[data.armsSwimmingRenderer.legacyAnimData.run].wrapMode = WrapMode.Loop;
                                            data.armsSwimmingRenderer.legacyAnim.CrossFade(data.armsSwimmingRenderer.legacyAnimData.run, 0.3f, PlayMode.StopAll);
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
                                        if (data.armsSwimmingRenderer.anim)
                                        {
                                            data.armsSwimmingRenderer.anim.ResetTrigger("Start Run");
                                            data.armsSwimmingRenderer.anim.SetTrigger("End Run");
                                        }
                                        else if (data.armsSwimmingRenderer.legacyAnim)
                                        {
                                            data.armsSwimmingRenderer.legacyAnim[data.armsSwimmingRenderer.legacyAnimData.idle].wrapMode = WrapMode.Loop;
                                            data.armsSwimmingRenderer.legacyAnim.CrossFade(data.armsSwimmingRenderer.legacyAnimData.idle, 0.3f, PlayMode.StopAll);
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
                                        if (data.armsSwimmingRenderer.anim)
                                        {
                                            data.armsSwimmingRenderer.anim.ResetTrigger("Start Run");
                                            data.armsSwimmingRenderer.anim.SetTrigger("End Run");
                                        }
                                        else if (data.armsSwimmingRenderer.legacyAnim)
                                        {
                                            data.armsSwimmingRenderer.legacyAnim[data.armsSwimmingRenderer.legacyAnimData.idle].wrapMode = WrapMode.Loop;
                                            data.armsSwimmingRenderer.legacyAnim.CrossFade(data.armsSwimmingRenderer.legacyAnimData.idle, 0.3f, PlayMode.StopAll);
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
                                        if (data.armsSwimmingRenderer.anim)
                                        {
                                            data.armsSwimmingRenderer.anim.ResetTrigger("End Run");
                                            data.armsSwimmingRenderer.anim.SetTrigger("Start Run");
                                        }
                                        else if (data.armsSwimmingRenderer.legacyAnim)
                                        {
                                            data.armsSwimmingRenderer.legacyAnim[data.armsSwimmingRenderer.legacyAnimData.run].wrapMode = WrapMode.Loop;
                                            data.armsSwimmingRenderer.legacyAnim.CrossFade(data.armsSwimmingRenderer.legacyAnimData.run, 0.3f, PlayMode.StopAll);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            public override void CalculateWeaponUpdate(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ArmsSwimmingRuntimeData))
                {
                    Kit_ArmsSwimmingRuntimeData data = runtimeData as Kit_ArmsSwimmingRuntimeData;

                    //Set this weapon to selected and ready (for other things)
                    data.isSelectedAndReady = true;

                    if (pb.isFirstPersonActive)
                    {
                        if (pb.input != null)
                        {
                            data.armsSwimmingRenderer.anim.SetFloat("moveX", pb.input.hor);
                            data.armsSwimmingRenderer.anim.SetFloat("moveZ", pb.input.ver);
                        }
                        else
                        {
                            data.armsSwimmingRenderer.anim.SetFloat("moveX", pb.movement.GetVelocity(pb).x);
                            data.armsSwimmingRenderer.anim.SetFloat("moveZ", pb.movement.GetVelocity(pb).z);
                        }

                        Kit_IngameMain.instance.hud.DisplayAmmo(0, 0, false);
                    }
                }
            }

            public override void DrawWeapon(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ArmsSwimmingRuntimeData))
                {
                    Kit_ArmsSwimmingRuntimeData data = runtimeData as Kit_ArmsSwimmingRuntimeData;
                    //Set selected
                    data.isSelected = true;

                    if (pb.isFirstPersonActive)
                    {
                        //Reset pos & rot of the renderer
                        data.armsSwimmingRenderer.transform.localPosition = Vector3.zero;
                        data.armsSwimmingRenderer.transform.localRotation = Quaternion.identity;
                        //Reset delay
                        data.weaponDelayCur = Vector3.zero;
                        //Reset fov
                        Kit_IngameMain.instance.mainCamera.fieldOfView = Kit_GameSettings.baseFov;
                        //Play animation
                        if (data.armsSwimmingRenderer.anim)
                        {
                            //Enable anim
                            data.armsSwimmingRenderer.anim.enabled = true;
                            data.armsSwimmingRenderer.anim.Play("Draw", 0, 0f);
                        }
                        else if (data.armsSwimmingRenderer.legacyAnim)
                        {
                            //Enable anim
                            data.armsSwimmingRenderer.legacyAnim.enabled = true;
                            data.armsSwimmingRenderer.legacyAnim.Play(data.armsSwimmingRenderer.legacyAnimData.draw);
                        }
                        //Play sound if it is assigned
                        if (drawSound) data.sounds.PlayOneShot(drawSound);
                        if (pb.looking.GetPerspective(pb) == Kit_GameInformation.Perspective.FirstPerson)
                        {
                            //Show weapon
                            data.armsSwimmingRenderer.visible = true;
                        }
                        else
                        {
                            data.armsSwimmingRenderer.visible = false;
                        }
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
                if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ArmsSwimmingRuntimeData))
                {
                    Kit_ArmsSwimmingRuntimeData data = runtimeData as Kit_ArmsSwimmingRuntimeData;
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
                if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ArmsSwimmingRuntimeData))
                {
                    //Get runtime data
                    Kit_ArmsSwimmingRuntimeData data = runtimeData as Kit_ArmsSwimmingRuntimeData;
                    //Activate or deactivate based on bool
                    if (perspective == Kit_GameInformation.Perspective.ThirdPerson)
                    {
                        if (data.armsSwimmingRenderer)
                        {
                            data.armsSwimmingRenderer.visible = false;
                        }
                    }
                    else
                    {
                        if (data.armsSwimmingRenderer)
                        {
                            data.armsSwimmingRenderer.visible = true;
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
                if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ArmsSwimmingRuntimeData))
                {
                    Kit_ArmsSwimmingRuntimeData data = runtimeData as Kit_ArmsSwimmingRuntimeData;
                    if (pb.isFirstPersonActive)
                    {
                        //Reset fov
                        Kit_IngameMain.instance.mainCamera.fieldOfView = Kit_GameSettings.baseFov;
                        //Play animation
                        if (data.armsSwimmingRenderer.anim)
                        {
                            //Enable anim
                            data.armsSwimmingRenderer.anim.enabled = true;
                            data.armsSwimmingRenderer.anim.Play("Putaway", 0, 0f);
                        }
                        else if (data.armsSwimmingRenderer.legacyAnim)
                        {
                            //Enable anim
                            data.armsSwimmingRenderer.legacyAnim.enabled = true;
                            data.armsSwimmingRenderer.legacyAnim.Play(data.armsSwimmingRenderer.legacyAnimData.putaway);
                        }
                        //Play sound if it is assigned
                        if (putawaySound) data.sounds.PlayOneShot(putawaySound);
                        if (pb.looking.GetPerspective(pb) == Kit_GameInformation.Perspective.FirstPerson)
                        {
                            //Show weapon
                            data.armsSwimmingRenderer.visible = true;
                        }
                        else
                        {
                            //Hide
                            data.armsSwimmingRenderer.visible = false;
                        }
                    }
                    //Make sure it is not ready yet
                    data.isSelectedAndReady = false;
                    //Stop third person anims
                    pb.thirdPersonPlayerModel.AbortWeaponAnimations();
                }
            }

            public override void PutawayWeaponHide(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ArmsSwimmingRuntimeData))
                {
                    Kit_ArmsSwimmingRuntimeData data = runtimeData as Kit_ArmsSwimmingRuntimeData;
                    //Set selected
                    data.isSelected = false;
                    //Reset run animation
                    data.startedRunAnimation = false;
                    if (pb.isFirstPersonActive)
                    {
                        //Hide weapon
                        data.armsSwimmingRenderer.visible = false;
                        //Disable anim
                        if (data.armsSwimmingRenderer.anim)
                        {
                            data.armsSwimmingRenderer.anim.enabled = false;
                        }
                        else if (data.armsSwimmingRenderer.legacyAnim)
                        {
                            data.armsSwimmingRenderer.legacyAnim.enabled = false;
                        }
                        //Reset pos & rot of the renderer
                        data.armsSwimmingRenderer.transform.localPosition = Vector3.zero;
                        data.armsSwimmingRenderer.transform.localRotation = Quaternion.identity;
                        //Reset delay
                        data.weaponDelayCur = Vector3.zero;
                    }
                    //Make sure it is not ready
                    data.isSelectedAndReady = false;
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
                Kit_ArmsSwimmingRuntimeData data = runtimeData as Kit_ArmsSwimmingRuntimeData;

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
                data.armsSwimmingRenderer = fpRuntime.GetComponent<Kit_MeleeRenderer>();

                //Play Dependent arms
                if (data.armsSwimmingRenderer.playerModelDependentArmsEnabled && pb.thirdPersonPlayerModel.firstPersonArmsPrefab.Contains(data.armsSwimmingRenderer.playerModelDependentArmsKey))
                {
                    //Create Prefab
                    GameObject fpArms = Instantiate(pb.thirdPersonPlayerModel.firstPersonArmsPrefab[data.armsSwimmingRenderer.playerModelDependentArmsKey], fallTrans.transform, false);

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
                            fpa.reparents[i].transform.parent = data.armsSwimmingRenderer.playerModelDependentArmsRoot;
                        }
                        else
                        {
                            Debug.LogWarning("First Person Arm Renderer at index " + i + " is not assigned", fpa);
                        }
                    }
                    //Merge Array
                    data.armsSwimmingRenderer.allWeaponRenderers = data.armsSwimmingRenderer.allWeaponRenderers.Concat(fpa.renderers).ToArray();
                    //Rebind so that the animator animates our freshly reparented transforms too!
                    if (data.armsSwimmingRenderer.anim)
                    {
                        data.armsSwimmingRenderer.anim.Rebind();
                    }
                }


                data.armsSwimmingRenderer.visible = false;

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

            public override void SetupThirdPerson(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                Kit_ArmsSwimmingRuntimeData data = runtimeData as Kit_ArmsSwimmingRuntimeData;
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
                return 2;
            }


            public override void BeginSpectating(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, int[] attachments)
            {
                Kit_ArmsSwimmingRuntimeData data = runtimeData as Kit_ArmsSwimmingRuntimeData;

                if (!data.armsSwimmingRenderer)
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
                    data.armsSwimmingRenderer = fpRuntime.GetComponent<Kit_MeleeRenderer>();

                    //Play Dependent arms
                    if (data.armsSwimmingRenderer.playerModelDependentArmsEnabled && pb.thirdPersonPlayerModel.firstPersonArmsPrefab.Contains(data.armsSwimmingRenderer.playerModelDependentArmsKey))
                    {
                        //Create Prefab
                        GameObject fpArms = Instantiate(pb.thirdPersonPlayerModel.firstPersonArmsPrefab[data.armsSwimmingRenderer.playerModelDependentArmsKey], fallTrans.transform, false);

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
                            data.armsSwimmingRenderer.cameraAnimationBone = fpa.cameraBoneOverride;
                        }

                        if (fpa.cameraBoneTargetOverride)
                        {
                            data.armsSwimmingRenderer.cameraAnimationTarget = fpa.cameraBoneTargetOverride;
                        }

                        //Reparent
                        for (int i = 0; i < fpa.reparents.Length; i++)
                        {
                            if (fpa.reparents[i])
                            {
                                fpa.reparents[i].transform.parent = data.armsSwimmingRenderer.playerModelDependentArmsRoot;
                            }
                            else
                            {
                                Debug.LogWarning("First Person Arm Renderer at index " + i + " is not assigned", fpa);
                            }
                        }
                        //Merge Array
                        data.armsSwimmingRenderer.allWeaponRenderers = data.armsSwimmingRenderer.allWeaponRenderers.Concat(fpa.renderers).ToArray();
                        //Rebind so that the animator animates our freshly reparented transforms too!
                        if (data.armsSwimmingRenderer.anim)
                        {
                            data.armsSwimmingRenderer.anim.Rebind();
                        }
                    }


                    data.armsSwimmingRenderer.visible = false;

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
                    data.armsSwimmingRenderer.visible = data.isSelected;
                }
                else
                {
                    //FP is definitely hidden, TP is already in the state that it should be in
                    data.armsSwimmingRenderer.visible = false;
                }
            }

            public override void EndSpectating(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                //Get data
                Kit_ArmsSwimmingRuntimeData data = runtimeData as Kit_ArmsSwimmingRuntimeData;

                //Hide
                data.armsSwimmingRenderer.visible = false;
            }
        }
    }
}