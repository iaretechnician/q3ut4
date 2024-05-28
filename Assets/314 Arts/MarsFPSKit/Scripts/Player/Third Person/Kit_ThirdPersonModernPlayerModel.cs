using System;
using System.Collections.Generic;
using MarsFPSKit.Weapons;
using UnityEngine;
using UnityEngine.Rendering;

using System.Collections;
using Mirror;

namespace MarsFPSKit
{
    public struct RewindSnapshot
    {
        public Vector3 position;
        public Quaternion rotation;
    }

    /// <summary>
    /// A modern animating player model, which calculates velocity locally. It will read data from <see cref="Kit_Movement_BootsOnGround"/>
    /// </summary>
    public class Kit_ThirdPersonModernPlayerModel : Kit_ThirdPersonPlayerModel
    {
        public AnimatorStateName animatorStates;

        /// <summary>
        /// These are all renderers that will be set to "shadows only" when using first person
        /// </summary>
        public Renderer[] fpShadowOnlyRenderers;
        /// <summary>
        /// These are colliders for hit detection
        /// </summary>
        public Collider[] raycastColliders;

        public Rigidbody[] rigidbodies;

        /// <summary>
        /// Implements the death camera
        /// </summary>
        [Header("Death Camera")]
        public Kit_DeathCameraBase deathCamera;
        /// <summary>
        /// How long does our ragdoll live?
        /// </summary>
        public float ragdollLiveTime = 30f;

        [HideInInspector]
        /// <summary>
        /// This is the reference to our player
        /// </summary>
        public Kit_PlayerBehaviour kpb;
        //Cache movement transform
        private Transform movementTransform;
        //To calculate movement
        private Vector3 position;
        private Vector3 oldPosition;
        private string oldAnimType;

        //Speed
        public float smoothedSpeed;
        private float rawSpeed;
        public Vector3 direction;
        public Vector3 localDirection;

        [Header("Lag Compensated Raycast")]
        public float histroyLength = 1f;
        public float snapShotInterval = 0.01f;

        public List<List<RewindSnapshot>> rewindSnapshots;

        private void OnEnable()
        {
            int snapShots = Mathf.CeilToInt(histroyLength / snapShotInterval);
            rewindSnapshots = new List<List<RewindSnapshot>>(raycastColliders.Length);

            for (int i = 0; i < raycastColliders.Length; i++)
            {
                rewindSnapshots.Add(new List<RewindSnapshot>(snapShots));
                for (int o = 0; o < snapShots; o++)
                {
                    rewindSnapshots[i].Add(new RewindSnapshot { position = raycastColliders[i].transform.position, rotation = raycastColliders[i].transform.rotation });
                }
            }

            StartCoroutine(CaptureSnapShots());
        }

#if UNITY_EDITOR
        public float debugTime = 0.05f;

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                int index = Mathf.CeilToInt(debugTime / snapShotInterval);

                if (rewindSnapshots != null)
                {
                    for (int i = 0; i < rewindSnapshots.Count; i++)
                    {
                        Collider col = raycastColliders[i];
                        Gizmos.DrawCube(rewindSnapshots[i][index].position, col.bounds.size);
                    }
                }
            }
        }
#endif

        IEnumerator CaptureSnapShots()
        {
            WaitForSeconds wait = new WaitForSeconds(snapShotInterval);
            while (true)
            {
                for (int i = 0; i < raycastColliders.Length; i++)
                {
                    RewindSnapshot oldSnapShot = rewindSnapshots[i][rewindSnapshots[i].Count - 1];
                    //Remove last one
                    rewindSnapshots[i].RemoveAt(rewindSnapshots[i].Count - 1);
                    oldSnapShot.position = raycastColliders[i].transform.position;
                    oldSnapShot.rotation = raycastColliders[i].transform.rotation;
                    //Add in front
                    rewindSnapshots[i].Insert(0, oldSnapShot);
                }

                yield return wait;
            }
        }

        public override void SetLagCompensationTo(float revertBy)
        {
            int index = Mathf.Clamp(Mathf.CeilToInt(revertBy / snapShotInterval), 0, rewindSnapshots[0].Count - 1);
            for (int i = 0; i < rewindSnapshots.Count; i++)
            {
                if (rigidbodies[i])
                {
                    rigidbodies[i].position = rewindSnapshots[i][index].position;
                    rigidbodies[i].rotation = rewindSnapshots[i][index].rotation;
                }
            }
        }

        public override void FirstPerson()
        {
            //Set all renderers to shadow only
            for (int i = 0; i < fpShadowOnlyRenderers.Length; i++)
            {
                fpShadowOnlyRenderers[i].shadowCastingMode = ShadowCastingMode.ShadowsOnly;
            }

            for (int i = 0; i < customizationSlots.Length; i++)
            {
                for (int o = 0; o < customizationSlots[i].customizations.Length; o++)
                {
                    for (int p = 0; p < customizationSlots[i].customizations[o].customizationBehaviours.Length; p++)
                    {
                        if (customizationSlots[i].customizations[o].customizationBehaviours[p] is Kit_PlayerModelCustomizationRenderer)
                        {
                            Kit_PlayerModelCustomizationRenderer render = customizationSlots[i].customizations[o].customizationBehaviours[p] as Kit_PlayerModelCustomizationRenderer;
                            for (int l = 0; l < render.renderers.Length; l++)
                            {
                                render.renderers[l].shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                            }
                        }
                    }
                }
            }
        }

        public override void FirstThirdPersonChanged(Kit_PlayerBehaviour pb, Kit_GameInformation.Perspective perspective)
        {
            if (perspective == Kit_GameInformation.Perspective.FirstPerson)
            {
                //Set all renderers to shadow only
                for (int i = 0; i < fpShadowOnlyRenderers.Length; i++)
                {
                    fpShadowOnlyRenderers[i].shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                }

                for (int i = 0; i < customizationSlots.Length; i++)
                {
                    for (int o = 0; o < customizationSlots[i].customizations.Length; o++)
                    {
                        for (int p = 0; p < customizationSlots[i].customizations[o].customizationBehaviours.Length; p++)
                        {
                            if (customizationSlots[i].customizations[o].customizationBehaviours[p] is Kit_PlayerModelCustomizationRenderer)
                            {
                                Kit_PlayerModelCustomizationRenderer render = customizationSlots[i].customizations[o].customizationBehaviours[p] as Kit_PlayerModelCustomizationRenderer;
                                for (int l = 0; l < render.renderers.Length; l++)
                                {
                                    render.renderers[l].shadowCastingMode = ShadowCastingMode.ShadowsOnly;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                //Set all renderers to shadow only
                for (int i = 0; i < fpShadowOnlyRenderers.Length; i++)
                {
                    fpShadowOnlyRenderers[i].shadowCastingMode = ShadowCastingMode.On;
                }

                for (int i = 0; i < customizationSlots.Length; i++)
                {
                    for (int o = 0; o < customizationSlots[i].customizations.Length; o++)
                    {
                        for (int p = 0; p < customizationSlots[i].customizations[o].customizationBehaviours.Length; p++)
                        {
                            if (customizationSlots[i].customizations[o].customizationBehaviours[p] is Kit_PlayerModelCustomizationRenderer)
                            {
                                Kit_PlayerModelCustomizationRenderer render = customizationSlots[i].customizations[o].customizationBehaviours[p] as Kit_PlayerModelCustomizationRenderer;
                                for (int l = 0; l < render.renderers.Length; l++)
                                {
                                    render.renderers[l].shadowCastingMode = ShadowCastingMode.On;
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void SetupModel(Kit_PlayerBehaviour newKpb)
        {
            //Store the reference to our player
            kpb = newKpb;
            //Cache movement transform
            movementTransform = kpb.transform;
            //Store position
            position = movementTransform.position;
            oldPosition = movementTransform.position;

            if (NetworkServer.active)
            {
                anim.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            }

            enabled = true;
        }

        public override void ThirdPerson()
        {
            //Set all renderers to normal shadows
            for (int i = 0; i < fpShadowOnlyRenderers.Length; i++)
            {
                fpShadowOnlyRenderers[i].shadowCastingMode = ShadowCastingMode.On;
            }

            for (int i = 0; i < customizationSlots.Length; i++)
            {
                for (int o = 0; o < customizationSlots[i].customizations.Length; o++)
                {
                    for (int p = 0; p < customizationSlots[i].customizations[o].customizationBehaviours.Length; p++)
                    {
                        if (customizationSlots[i].customizations[o].customizationBehaviours[p] is Kit_PlayerModelCustomizationRenderer)
                        {
                            Kit_PlayerModelCustomizationRenderer render = customizationSlots[i].customizations[o].customizationBehaviours[p] as Kit_PlayerModelCustomizationRenderer;
                            for (int l = 0; l < render.renderers.Length; l++)
                            {
                                render.renderers[l].shadowCastingMode = ShadowCastingMode.On;
                            }
                        }
                    }
                }
            }
        }

        public override void SetAnimType(string animType, bool noTrans = false)
        {
            /*
            Debug.Log(animType);
            string getAnimation = animatorStates.GetAnimatorState(anim.GetCurrentAnimatorStateInfo(0), oldAnimType);
            if (getAnimation != "")
            {
                if (noTrans)
                {
                    anim.Play(animType + "." + getAnimation, 0);
                }
                else
                {
                    anim.CrossFade(animType + "." + getAnimation, 0.1f, 0);
                }
            }
            else
            {
                if (noTrans)
                {
                    anim.Play(animType, 0);
                }
                else
                {
                    anim.CrossFade(animType, 0.1f, 0);
                }
            }
            oldAnimType = animType;
            */

            Kit_GameInformation game = null;
            if (Kit_IngameMain.instance) game = Kit_IngameMain.instance.gameInformation;
            else
            {
                UI.Kit_MenuManager mm = FindObjectOfType<UI.Kit_MenuManager>();
                if (mm)
                {
                    game = mm.game;
                }
            }

            if (game)
            {
                for (int i = 0; i < game.allAnimatorAnimationSets.Length; i++)
                {
                    if (game.allAnimatorAnimationSets[i].prefix == animType)
                    {
                        anim.SetInteger("animType", game.allAnimatorAnimationSets[i].type);
                        anim.SetTrigger("ChangeState");
                        break;
                    }
                }
            }
        }

        public override void PlayWeaponFireAnimation(string animType)
        {
            if (anim)
            {
                anim.Play(animType + " Fire", 2, 0f);
            }
        }

        public override void PlayWeaponReloadAnimation(string animType, int type)
        {
            if (anim)
            {
                if (type == 0)
                {
                    anim.CrossFade(animType + " Reload", 0.1f, 1, 0f);
                }
                else if (type == 1)
                {
                    anim.CrossFade(animType + " Reload Empty", 0.1f, 1, 0f);
                }
            }
        }

        public override void PlayWeaponAnimation(string animType, string anim)
        {
            this.anim.CrossFade(animType + " " + anim, 0.1f, 1, 0f);
        }

        public override void PlayWeaponChangeAnimation(string animType, bool draw, float length)
        {
            anim.SetFloat("actionSpeed", 1 / length);

            if (draw)
            {
                anim.CrossFade(animType + " Draw", 0.1f, 1, 0f);
            }
            else
            {
                anim.CrossFade(animType + " Holster", 0.1f, 1, 0f);
            }
        }

        public override void PlayMeleeAnimation(int animation, int state)
        {
            if (anim)
            {
                if (animation == 0)
                {
                    if (state == 0)
                    {
                        anim.CrossFade("Stab Windup", 0.1f, 1, 0f);
                    }
                    else if (state == 1)
                    {
                        anim.CrossFade("Stab Miss", 0.1f, 1, 0f);
                    }
                    else if (state == 2)
                    {
                        anim.CrossFade("Stab Hit", 0.1f, 1, 0f);
                    }
                    else if (state == 3)
                    {
                        anim.CrossFade("Stab Hit Object", 0.1f, 1, 0f);
                    }
                }
                else if (animation == 1)
                {
                    if (state == 0)
                    {
                        anim.CrossFade("Charge", 0.1f, 1, 0f);
                    }
                    else if (state == 1)
                    {
                        anim.CrossFade("Charge Windup", 0.1f, 1, 0f);
                    }
                    else if (state == 2)
                    {
                        anim.CrossFade("Charge Miss", 0.1f, 1, 0f);
                    }
                    else if (state == 3)
                    {
                        anim.CrossFade("Charge Hit", 0.1f, 1, 0f);
                    }
                    else if (state == 4)
                    {
                        anim.CrossFade("Charge Hit Object", 0.1f, 1, 0f);
                    }
                }
                else if (animation == 2)
                {
                    anim.CrossFade("Heal", 0.1f, 1, 0f);
                }
            }
        }

        public override void PlayGrenadeAnimation(int animation)
        {
            if (anim)
            {
                if (animation == 0)
                {
                    anim.CrossFade("PullPin", 0.1f, 1, 0f);
                }
                else if (animation == 1)
                {
                    anim.CrossFade("Throw", 0.1f, 1, 0f);
                }
            }
        }

        public override void AbortWeaponAnimations()
        {
            if (anim)
            {
                anim.CrossFade("Null", 0.05f, 1, 0f); //Reload layer
                anim.CrossFade("Null", 0.05f, 2, 0f); //Fire layer
            }
            //Also stop reload sounds
            soundReload.Stop();
        }

        public override void CreateRagdoll()
        {
            //Play Death sound
            if (kpb.voiceManager)
            {
                kpb.voiceManager.PlayDeathSound(kpb, kpb.deathSoundCategory, kpb.deathSoundID);
            }

            for (int i = 0; i < rigidbodies.Length; i++)
            {
                rigidbodies[i].isKinematic = false;
            }
            anim.enabled = false;
            for (int i = 0; i < raycastColliders.Length; i++)
            {
                raycastColliders[i].isTrigger = false;
                //Set Layer to ragdoll layer ! 
                raycastColliders[i].gameObject.layer = 11;
            }

            Vector3 velocity = kpb.movement.GetVelocity(kpb);

            for (int i = 0; i < rigidbodies.Length; i++)
            {
                rigidbodies[i].velocity = velocity;
            }

            if (kpb.isFirstPersonActive)
            {
                //Let death camera do its thing
                deathCamera.SetupDeathCamera(this);
            }

            transform.parent = null;
            StartCoroutine(ApplyForce());
        }

        IEnumerator ApplyForce()
        {
            Destroy(anim);
            Destroy(weaponsInHandsGo.gameObject);
            //We need to wait until physics registered that our rigidbodies are not kinematic anymore
            yield return new WaitForFixedUpdate();
            rigidbodies[kpb.ragdollId].AddForceAtPosition(kpb.ragdollForward * kpb.ragdollForce, kpb.ragdollPoint, ForceMode.Impulse);

            //Add destroy script
            gameObject.AddComponent<World.Kit_DestroyUponGameModeReset>();

            Destroy(gameObject, ragdollLiveTime);
            Destroy(this);
        }

        void Update()
        {
            if (kpb)
            {
                //Smooth speed
                smoothedSpeed = Mathf.Lerp(smoothedSpeed, rawSpeed, Time.deltaTime * 10f);

                #region Animator Update
                //Update the animator with our locally calculated values
                //Speed
                anim.SetFloat("walkSpeed", smoothedSpeed, 0.1f, Time.deltaTime);
                if (rawSpeed > 0.3f)
                {
                    //Direction
                    anim.SetFloat("walkX", localDirection.x, 0.1f, Time.deltaTime);
                    anim.SetFloat("walkZ", localDirection.z, 0.1f, Time.deltaTime);
                }
                else
                {
                    //Direction
                    anim.SetFloat("walkX", 0, 0.1f, Time.deltaTime);
                    anim.SetFloat("walkZ", 0, 0.1f, Time.deltaTime);
                }
                #endregion

                #region Movement Data
                if (kpb)
                {
                    Kit_Movement_BootsOnGroundNetworkData bogrd = kpb.movementNetworkData as Kit_Movement_BootsOnGroundNetworkData;
                    //Update state
                    anim.SetInteger("state", bogrd.state);
                    //Update blend
                    anim.SetFloat("stateBlend", bogrd.state, 0.1f, Time.deltaTime);
                    //Update aiming
                    anim.SetBool("aiming", bogrd.playSlowWalkAnimation);
                    //Update grounded
                    anim.SetBool("grounded", bogrd.isGrounded);
                }
                #endregion

                #region Looking Data
                if (kpb.customMouseLookData != null)
                {
                    if (kpb.customMouseLookData.GetType() == typeof(BasicMouseLookRuntimeData))
                    {
                        BasicMouseLookRuntimeData bmlrd = (BasicMouseLookRuntimeData)kpb.customMouseLookData;
                        //Smoothly update looking value
                        anim.SetFloat("lookY", bmlrd.finalMouseY, 0.1f, Time.deltaTime);
                        //Smoothly update leaning value
                        anim.SetFloat("leaning", bmlrd.leaningState, 0.1f, Time.deltaTime);
                    }
                }
                #endregion
            }
        }

        void FixedUpdate()
        {
            if (kpb)
            {
                //Pause Error
                if (Time.deltaTime <= float.Epsilon)
                {
                    smoothedSpeed = 0f;
                    rawSpeed = 0f;
                    return;
                }

                //Update position
                position = movementTransform.position;

                //Calculate speed
                rawSpeed = (position - oldPosition).magnitude / Time.deltaTime;

                //Update direction
                if (position != oldPosition)
                {
                    direction = Vector3.Normalize(position - oldPosition);
                }
                else
                {
                    direction = Vector3.zero;
                }

                //Update local direction
                localDirection = movementTransform.InverseTransformDirection(direction);

                //Update old position
                oldPosition = position;
            }
        }

        public void OnAnimatorIKRelay()
        {
            if (enabled && kpb && kpb.weaponManager)
            {
                kpb.weaponManager.OnAnimatorIKCallback(kpb, anim);
            }
        }
    }
}
