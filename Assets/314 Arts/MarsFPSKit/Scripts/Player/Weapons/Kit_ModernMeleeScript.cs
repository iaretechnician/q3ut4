
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#pragma warning disable 0219

namespace MarsFPSKit
{
    namespace Weapons
    {
        public enum AttackType { Stab, Charge, None, Heal, Combo }

        [System.Serializable]
        public class ComboAttack
        {
            [Header("Combo")]
            /// <summary>
            /// If there is a next combo, this is the time we have to initiate the next combo attack
            /// </summary>
            public float comboTimeForNextCombo;
            /// <summary>
            /// Can we proceed if we miss?
            /// </summary>
            public bool canAchieveNextComboOnMiss = true;
            /// <summary>
            /// Can we proceed if we hit an enemy?
            /// </summary>
            public bool canAchieveNextComboOnHit = true;
            /// <summary>
            /// Can we proceed if we hit an object?
            /// </summary>
            public bool canAchieveNextComboOnHitObject = true;
            /// <summary>
            /// Damage of combo attack
            /// </summary>
            public float comboDamage = 50f;
            /// <summary>
            /// Penetration power for combo
            /// </summary>
            public int comboPenetrationPower = 2;
            /// <summary>
            /// How far can we combo someone?
            /// </summary>
            public float comboReach = 2f;
            /// <summary>
            /// Half extents
            /// </summary>
            public Vector3 comboHalfExtents = new Vector3(0.1f, 0.1f, 1f);
            /// <summary>
            /// Applied force to ragdoll when combobing.
            /// </summary>
            public float comboRagdollForce = 500f;
            [Header("Windup")]
            /// <summary>
            /// Windup animation name
            /// </summary>
            public string comboWindupAnimationName = "combo Windup";
            /// <summary>
            /// Windup sound
            /// </summary>
            public AudioClip comboWindupSound;
            /// <summary>
            /// Time until actual damage is tested
            /// </summary>
            public float comboWindupTime = 0.3f;
            [Header("Hit")]
            /// <summary>
            /// Name of this combo's animation
            /// </summary>
            public string comboAnimationHitName = "combo Hit";
            /// <summary>
            /// Sound that plays when combobing and hitting
            /// </summary>
            public AudioClip comboHitSound;
            /// <summary>
            /// How long does one combo take?
            /// </summary>
            public float comboHitTime = 0.5f;
            [Header("Hit Object")]
            /// <summary>
            /// Name of this combo's animation
            /// </summary>
            public string comboAnimationHitObjectName = "combo Hit Object";
            /// <summary>
            /// Sound that plays when combobing and hitting
            /// </summary>
            public AudioClip comboHitObjectSound;
            /// <summary>
            /// How long does one combo take?
            /// </summary>
            public float comboHitObjectTime = 0.5f;
            [Header("Miss")]
            /// <summary>
            /// Animation that plays when missing
            /// </summary>
            public string comboAnimationMissName = "combo Miss";
            /// <summary>
            /// Sound that plays when combobing and missing
            /// </summary>
            public AudioClip comboMissSound;
            /// <summary>
            /// How long does a missed combo take?
            /// </summary>
            public float comboMissTime = 0.5f;
        }

        [System.Serializable]
        public class AttackSettings
        {
            /// <summary>
            /// Damage of stab attack
            /// </summary>
            [Header("Stab")]
            public float stabDamage = 50f;
            /// <summary>
            /// Penetration power for stab
            /// </summary>
            public int stabPenetrationPower = 2;
            /// <summary>
            /// How far can we stab someone?
            /// </summary>
            public float stabReach = 2f;
            /// <summary>
            /// Half extents
            /// </summary>
            public Vector3 stabHalfExtents = new Vector3(0.1f, 0.1f, 1f);
            /// <summary>
            /// Applied force to ragdoll when stabbing.
            /// </summary>
            public float stabRagdollForce = 500f;
            [Header("Windup")]
            /// <summary>
            /// Windup animation name
            /// </summary>
            public string stabWindupAnimationName = "Stab Windup";
            /// <summary>
            /// Windup sound
            /// </summary>
            public AudioClip stabWindupSound;
            /// <summary>
            /// Time until actual damage is tested
            /// </summary>
            public float stabWindupTime = 0.3f;
            [Header("Hit")]
            /// <summary>
            /// Name of this stab's animation
            /// </summary>
            public string stabAnimationHitName = "Stab Hit";
            /// <summary>
            /// Sound that plays when stabbing and hitting
            /// </summary>
            public AudioClip stabHitSound;
            /// <summary>
            /// How long does one stab take?
            /// </summary>
            public float stabHitTime = 0.5f;
            [Header("Hit Object")]
            /// <summary>
            /// Name of this stab's animation
            /// </summary>
            public string stabAnimationHitObjectName = "Stab Hit Object";
            /// <summary>
            /// Sound that plays when stabbing and hitting
            /// </summary>
            public AudioClip stabHitObjectSound;
            /// <summary>
            /// How long does one stab take?
            /// </summary>
            public float stabHitObjectTime = 0.5f;
            [Header("Miss")]
            /// <summary>
            /// Animation that plays when missing
            /// </summary>
            public string stabAnimationMissName = "Stab Miss";
            /// <summary>
            /// Sound that plays when stabbing and missing
            /// </summary>
            public AudioClip stabMissSound;
            /// <summary>
            /// How long does a missed stab take?
            /// </summary>
            public float stabMissTime = 0.5f;

            /// <summary>
            /// Damage at start (0% charge)
            /// </summary>
            [Header("Charge")]
            public float chargeDamageStart = 10f;
            /// <summary>
            /// Damage at fully charged (100% charge)
            /// </summary>
            public float chargeDamageCharged = 90f;
            /// <summary>
            /// Penetration power for charge
            /// </summary>
            public int chargePenetrationPower = 2;
            /// <summary>
            /// How far can we charge someone?
            /// </summary>
            public float chargeReach = 2f;
            /// <summary>
            /// Half extents
            /// </summary>
            public Vector3 chargeHalfExtents = new Vector3(0.1f, 0.1f, 1f);
            /// <summary>
            /// Applied force to ragdoll when chargebing.
            /// </summary>
            public float chargeRagdollForce = 500f;
            [Header("Charge")]
            /// <summary>
            /// Windup animation name
            /// </summary>
            public string chargeChargeAnimation = "Charge Windup";
            /// <summary>
            /// Windup sound
            /// </summary>
            public AudioClip chargeChargeSound;
            /// <summary>
            /// Time until actual damage is tested
            /// </summary>
            public float chargeChargeTime = 0.3f;
            [Header("Windup")]
            /// <summary>
            /// Windup animation name
            /// </summary>
            public string chargeWindupAnimationName = "Charge Windup";
            /// <summary>
            /// Windup sound
            /// </summary>
            public AudioClip chargeWindupSound;
            /// <summary>
            /// Time until actual damage is tested
            /// </summary>
            public float chargeWindupTime = 0.3f;
            [Header("Hit")]
            /// <summary>
            /// Name of this charge's animation
            /// </summary>
            public string chargeAnimationHitName = "Charge Hit";
            /// <summary>
            /// Sound that plays when chargebing and hitting
            /// </summary>
            public AudioClip chargeHitSound;
            /// <summary>
            /// How long does one charge take?
            /// </summary>
            public float chargeHitTime = 0.5f;
            [Header("Hit Object")]
            /// <summary>
            /// Name of this charge's animation
            /// </summary>
            public string chargeAnimationHitObjectName = "Charge Hit Object";
            /// <summary>
            /// Sound that plays when chargebing and hitting
            /// </summary>
            public AudioClip chargeHitObjectSound;
            /// <summary>
            /// How long does one charge take?
            /// </summary>
            public float chargeHitObjectTime = 0.5f;
            [Header("Miss")]
            /// <summary>
            /// Animation that plays when missing
            /// </summary>
            public string chargeAnimationMissName = "Charge Miss";
            /// <summary>
            /// Sound that plays when chargebing and missing
            /// </summary>
            public AudioClip chargeMissSound;
            /// <summary>
            /// How long does a missed charge take?
            /// </summary>
            public float chargeMissTime = 0.5f;

            /// <summary>
            /// Time before heal is applied
            /// </summary>
            [Header("Heal")]
            public float healTimeOne = 2f;
            /// <summary>
            /// Time after healing where we cant do other things
            /// </summary>
            public float healTimeTwo = 2f;
            /// <summary>
            /// How much do we heal?
            /// </summary>
            public float healAmount = 5f;
            /// <summary>
            /// Start amount of healing
            /// </summary>
            public int healStartAmount = 2;
            /// <summary>
            /// Name of the animation
            /// </summary>
            public string healAnimationName = "Heal";
            /// <summary>
            /// Sound for healing
            /// </summary>
            public AudioClip healSound;

            /// <summary>
            /// Combo attack
            /// </summary>
            [Header("Combo")]
            public ComboAttack[] combos;
        }

        [CreateAssetMenu(menuName = ("MarsFPSKit/Weapons/Modern Melee Script"))]
        public class Kit_ModernMeleeScript : Kit_WeaponBase
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

            #region Attack
            /// <summary>
            /// Primary attack (lmb)
            /// </summary>
            [Header("Attacks")]
            public AttackType primaryAttack = AttackType.Stab;
            /// <summary>
            /// Settings of primary attack
            /// </summary>
            public AttackSettings primaryAttackSettings = new AttackSettings();
            /// <summary>
            /// Secondary attack (rmb)
            /// </summary>
            public AttackType secondaryAttack = AttackType.Charge;
            /// <summary>
            /// Settings of secondary attack
            /// </summary>
            public AttackSettings secondaryAttackSettings = new AttackSettings();
            /// <summary>
            /// Quick attack type
            /// </summary>
            public AttackType quickAttack = AttackType.Stab;
            /// <summary>
            /// Settings of quick attack
            /// </summary>
            public AttackSettings quickAttackSettings = new AttackSettings();
            /// <summary>
            /// When using quick attack, do we skip the weapon's putaway?
            /// </summary>
            public bool quickAttackSkipsPutaway = true;
            #endregion

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
            /// As melee is a two layer sound, this is the sound ID!
            /// </summary>
            public int voiceMeleeSoundID;
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
                Kit_ModernMeleeScriptRuntimeData data = runtimeData as Kit_ModernMeleeScriptRuntimeData;

                //Set this weapon to selected and ready (for other things)
                data.isSelectedAndReady = true;

                if (primaryAttack == AttackType.Stab)
                {
                    //Check for input
                    if (data.lastLmb != pb.input.lmb)
                    {
                        data.lastLmb = pb.input.lmb;
                        if (pb.input.lmb)
                        {
                            if (Time.time > data.nextActionPossibleAt && !data.isCharging)
                            {
                                Kit_ScriptableObjectCoroutineHelper.instance.StartCoroutine(Kit_ScriptableObjectCoroutineHelper.instance.MeleeExecuteStab(this, data, primaryAttackSettings, pb, (float)revertTime));
                            }
                        }
                    }
                }
                else if (primaryAttack == AttackType.Charge)
                {
                    if (!data.chargingSecondary)
                    {
                        if (pb.input.lmb)
                        {
                            if (Time.time > data.nextActionPossibleAt)
                            {
                                if (!data.isCharging)
                                {
                                    ///Start charging!
                                    if (pb.isFirstPersonActive)
                                    {
                                        if (primaryAttackSettings.chargeChargeAnimation != "")
                                        {
                                            //Play animation
                                            if (data.meleeRenderer.anim)
                                            {
                                                data.meleeRenderer.anim.Play(primaryAttackSettings.chargeChargeAnimation);
                                            }
                                            else if (data.meleeRenderer.legacyAnim)
                                            {
                                                data.meleeRenderer.legacyAnim.Play(secondaryAttackSettings.chargeChargeAnimation);
                                            }
                                        }
                                        //Play sound
                                        if (primaryAttackSettings.chargeChargeSound)
                                        {
                                            data.sounds.clip = primaryAttackSettings.chargeChargeSound;
                                            data.sounds.Play();
                                        }
                                    }

                                    if (pb.isServer)
                                    {
                                        //Call network
                                        pb.RpcMeleeChargeNetwork(0, 0);
                                    }
                                    //Play third person reload anim
                                    pb.thirdPersonPlayerModel.PlayMeleeAnimation(1, 0);
                                }
                                else
                                {
                                    //Increase progress
                                    if (data.chargingProgress < 1f)
                                    {
                                        data.chargingProgress += Time.deltaTime / primaryAttackSettings.chargeChargeTime;
                                    }
                                }

                                //Set bool
                                data.chargingPrimary = true;
                                data.isCharging = true;
                            }
                        }
                        else
                        {
                            if (data.isCharging)
                            {
                                //RELEASE...
                                Kit_ScriptableObjectCoroutineHelper.instance.StartCoroutine(Kit_ScriptableObjectCoroutineHelper.instance.MeleeExecuteCharge(this, data, primaryAttackSettings, pb, (float)revertTime));
                                //Reset bools
                                data.isCharging = false;
                                data.chargingPrimary = false;
                                //data.chargingProgress = 0f;
                            }
                        }
                    }
                }
                else if (primaryAttack == AttackType.Heal)
                {
                    //Check for input
                    if (data.lastLmb != pb.input.lmb)
                    {
                        data.lastLmb = pb.input.lmb;
                        if (pb.input.lmb)
                        {
                            if (Time.time > data.nextActionPossibleAt && data.primaryHealingAmount > 0)
                            {
                                Kit_ScriptableObjectCoroutineHelper.instance.StartCoroutine(Kit_ScriptableObjectCoroutineHelper.instance.MeleeExecuteHeal(this, data, primaryAttackSettings, pb, (float)revertTime));
                            }
                        }
                    }
                }
                else if (primaryAttack == AttackType.Combo)
                {
                    //Check for input
                    if (data.lastLmb != pb.input.lmb)
                    {
                        data.lastLmb = pb.input.lmb;
                        if (pb.input.lmb)
                        {
                            if (Time.time > data.nextActionPossibleAt && !data.isCharging)
                            {
                                if (primaryAttackSettings.combos.Length > 0)
                                {
                                    //Check if we can continue our combo
                                    if (Time.time > data.primaryComboNextOnePossibleUntil)
                                    {
                                        data.primaryComboCur = 0;
                                    }
                                    Kit_ScriptableObjectCoroutineHelper.instance.StartCoroutine(Kit_ScriptableObjectCoroutineHelper.instance.MeleeExecuteCombo(this, data, primaryAttackSettings, true, pb, (float)revertTime));
                                }
                                else
                                {
                                    Debug.LogWarning("No combos for primary attack specified!");
                                }
                            }
                        }
                    }
                }

                if (secondaryAttack == AttackType.Stab)
                {
                    //Check for input
                    if (data.lastRmb != pb.input.rmb)
                    {
                        data.lastRmb = pb.input.rmb;
                        if (pb.input.rmb)
                        {
                            if (Time.time > data.nextActionPossibleAt && !data.isCharging)
                            {
                                Kit_ScriptableObjectCoroutineHelper.instance.StartCoroutine(Kit_ScriptableObjectCoroutineHelper.instance.MeleeExecuteStab(this, data, secondaryAttackSettings, pb, (float)revertTime));
                            }
                        }
                    }
                }
                else if (secondaryAttack == AttackType.Charge)
                {
                    if (!data.chargingPrimary)
                    {
                        if (pb.input.rmb)
                        {
                            if (Time.time > data.nextActionPossibleAt)
                            {
                                if (!data.isCharging)
                                {
                                    //Start charging!
                                    if (pb.isFirstPersonActive)
                                    {
                                        if (secondaryAttackSettings.chargeChargeAnimation != "")
                                        {
                                            //Play animation
                                            if (data.meleeRenderer.anim)
                                            {
                                                data.meleeRenderer.anim.Play(secondaryAttackSettings.chargeChargeAnimation);
                                            }
                                            else if (data.meleeRenderer.legacyAnim)
                                            {
                                                data.meleeRenderer.legacyAnim.Play(secondaryAttackSettings.chargeChargeAnimation);
                                            }
                                        }
                                        //Play sound
                                        if (secondaryAttackSettings.chargeChargeSound)
                                        {
                                            data.sounds.clip = secondaryAttackSettings.chargeChargeSound;
                                            data.sounds.Play();
                                        }
                                    }

                                    if (pb.isServer)
                                    {
                                        //Call network
                                        pb.RpcMeleeChargeNetwork(0, 1);
                                    }

                                    //Play third person reload anim
                                    pb.thirdPersonPlayerModel.PlayMeleeAnimation(1, 0);
                                }
                                else
                                {
                                    //Increase progress
                                    if (data.chargingProgress < 1f)
                                    {
                                        data.chargingProgress += Time.deltaTime / secondaryAttackSettings.chargeChargeTime;
                                    }
                                }

                                //Set bool
                                data.chargingSecondary = true;
                                data.isCharging = true;
                            }
                        }
                        else
                        {
                            if (data.isCharging)
                            {
                                //RELEASE...
                                Kit_ScriptableObjectCoroutineHelper.instance.StartCoroutine(Kit_ScriptableObjectCoroutineHelper.instance.MeleeExecuteCharge(this, data, secondaryAttackSettings, pb, (float)revertTime));
                                //Reset bools
                                data.isCharging = false;
                                data.chargingSecondary = false;
                                //data.chargingProgress = 0f;
                            }
                        }
                    }
                }
                else if (secondaryAttack == AttackType.Heal)
                {
                    //Check for input
                    if (data.lastRmb != pb.input.rmb)
                    {
                        data.lastRmb = pb.input.rmb;
                        if (pb.input.rmb)
                        {
                            if (Time.time > data.nextActionPossibleAt && data.secondaryHealingAmount > 0)
                            {
                                Kit_ScriptableObjectCoroutineHelper.instance.StartCoroutine(Kit_ScriptableObjectCoroutineHelper.instance.MeleeExecuteHeal(this, data, secondaryAttackSettings, pb, (float)revertTime));
                            }
                        }
                    }
                }
                else if (secondaryAttack == AttackType.Combo)
                {
                    //Check for input
                    if (data.lastLmb != pb.input.lmb)
                    {
                        data.lastLmb = pb.input.lmb;
                        if (pb.input.lmb)
                        {
                            if (Time.time > data.nextActionPossibleAt && !data.isCharging)
                            {
                                if (secondaryAttackSettings.combos.Length > 0)
                                {
                                    //Check if we can continue our combo
                                    if (Time.time > data.secondaryComboNextOnePossibleUntil)
                                    {
                                        data.secondaryComboCur = 0;
                                    }
                                    Kit_ScriptableObjectCoroutineHelper.instance.StartCoroutine(Kit_ScriptableObjectCoroutineHelper.instance.MeleeExecuteCombo(this, data, secondaryAttackSettings, false, pb, (float)revertTime));
                                }
                                else
                                {
                                    Debug.LogWarning("No combos for secondary attack specified!");
                                }
                            }
                        }
                    }
                }
            }

            public override WeaponDisplayData GetWeaponDisplayData(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                if (primaryAttack != AttackType.None && secondaryAttack != AttackType.None)
                {
                    WeaponDisplayData wdd = new WeaponDisplayData();
                    wdd.sprite = weaponHudPicture;
                    wdd.name = weaponName;
                    return wdd;
                }
                else
                {
                    return null;
                }
            }

            public override WeaponQuickUseDisplayData GetWeaponQuickUseDisplayData(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                if (quickAttack != AttackType.None)
                {
                    WeaponQuickUseDisplayData wdd = new WeaponQuickUseDisplayData();
                    wdd.sprite = weaponQuickUsePicture;
                    wdd.name = weaponName;
                    if (quickAttack == AttackType.Heal)
                    {
                        if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernMeleeScriptRuntimeData))
                        {
                            Kit_ModernMeleeScriptRuntimeData data = runtimeData as Kit_ModernMeleeScriptRuntimeData;
                            wdd.amount = data.quickHealingAmount;
                        }
                        else
                        {
                            wdd.amount = 1;
                        }
                    }
                    else
                    {
                        wdd.amount = 1;
                    }
                    return wdd;
                }
                else
                {
                    return null;
                }
            }

            public override bool SupportsCustomization()
            {
                return false;
            }

            public override bool CanBeSelected(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                if (primaryAttack == AttackType.None && secondaryAttack == AttackType.None) return false;
                return true;
            }

            public override bool SupportsQuickUse(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                if (quickAttack == AttackType.None || quickAttack == AttackType.Combo) return false;

                if (quickAttack == AttackType.Heal)
                {
                    if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernMeleeScriptRuntimeData))
                    {
                        Kit_ModernMeleeScriptRuntimeData data = runtimeData as Kit_ModernMeleeScriptRuntimeData;

                        return data.quickHealingAmount > 0;
                    }

                    return false;
                }
                else
                {
                    return true;
                }
            }

            public override bool QuickUseSkipsPutaway(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                return quickAttackSkipsPutaway;
            }

            public override bool WaitForQuickUseButtonRelease(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                return primaryAttack == AttackType.Charge;
            }

            public override float BeginQuickUse(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, float revertBy)
            {
                if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernMeleeScriptRuntimeData))
                {
                    Kit_ModernMeleeScriptRuntimeData data = runtimeData as Kit_ModernMeleeScriptRuntimeData;

                    //Set third person anim type
                    pb.thirdPersonPlayerModel.SetAnimType(thirdPersonAnimType);
                    //Stop third person anims
                    pb.thirdPersonPlayerModel.AbortWeaponAnimations();
                    if (pb.isFirstPersonActive)
                    {
                        if (pb.looking.GetPerspective(pb) == Kit_GameInformation.Perspective.FirstPerson)
                        {
                            //Show weapon
                            data.meleeRenderer.visible = true;
                        }
                        else
                        {
                            data.meleeRenderer.visible = false;
                        }
                        if (data.meleeRenderer.anim)
                        {
                            data.meleeRenderer.anim.enabled = true;
                        }
                        else if (data.meleeRenderer.legacyAnim)
                        {
                            data.meleeRenderer.legacyAnim.enabled = true;
                        }
                    }

                    if (pb.looking.GetPerspective(pb) == Kit_GameInformation.Perspective.FirstPerson && pb.isFirstPersonActive)
                    {
                        //Show tp weapon and hide
                        data.tpMeleeRenderer.visible = true;
                        data.tpMeleeRenderer.shadowsOnly = true;
                    }
                    else
                    {
                        //Show tp weapon and show
                        data.tpMeleeRenderer.visible = true;
                        data.tpMeleeRenderer.shadowsOnly = false;
                    }

                    if (quickAttack == AttackType.Stab)
                    {
                        //Start Coroutine!
                        Kit_ScriptableObjectCoroutineHelper.instance.StartCoroutine(Kit_ScriptableObjectCoroutineHelper.instance.MeleeExecuteStab(this, data, quickAttackSettings, pb, revertBy));
                        return quickAttackSettings.stabWindupTime + quickAttackSettings.stabHitTime;
                    }
                    else if (quickAttack == AttackType.Charge)
                    {
                        ///Start charging!
                        if (pb.isFirstPersonActive)
                        {
                            if (quickAttackSettings.chargeChargeAnimation != "")
                            {
                                //Play animation
                                if (data.meleeRenderer.anim)
                                {
                                    data.meleeRenderer.anim.Play(quickAttackSettings.chargeChargeAnimation);
                                }
                                else if (data.meleeRenderer.legacyAnim)
                                {
                                    data.meleeRenderer.legacyAnim.Play(quickAttackSettings.chargeChargeAnimation);
                                }
                            }
                            //Play sound
                            if (quickAttackSettings.chargeChargeSound)
                            {
                                data.sounds.clip = quickAttackSettings.chargeChargeSound;
                                data.sounds.Play();
                            }
                        }

                        //Set charge time
                        data.quickChargeStartedAt = Time.time;

                        return 0f;
                    }
                    else if (quickAttack == AttackType.Heal)
                    {
                        //Start Coroutine!
                        Kit_ScriptableObjectCoroutineHelper.instance.StartCoroutine(Kit_ScriptableObjectCoroutineHelper.instance.MeleeExecuteHeal(this, data, quickAttackSettings, pb, revertBy));
                        return quickAttackSettings.healTimeOne + quickAttackSettings.healTimeTwo;
                    }
                }

                //In case of failure...
                return 0f;
            }

            public override float EndQuickUse(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, float revertBy)
            {
                if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernMeleeScriptRuntimeData))
                {
                    Kit_ModernMeleeScriptRuntimeData data = runtimeData as Kit_ModernMeleeScriptRuntimeData;

                    if (quickAttack == AttackType.Stab)
                    {
                        //Do nuthin'
                        return 0f;
                    }
                    else if (quickAttack == AttackType.Charge)
                    {
                        //Finish him.
                        Kit_ScriptableObjectCoroutineHelper.instance.StartCoroutine(Kit_ScriptableObjectCoroutineHelper.instance.MeleeExecuteCharge(this, data, quickAttackSettings, pb, revertBy));
                        return quickAttackSettings.chargeWindupTime + quickAttackSettings.chargeHitTime;
                    }
                    else if (quickAttack == AttackType.Heal)
                    {
                        //Do nothing
                        return 0f;
                    }
                }

                //In case of failure...
                return 0f;
            }

            public override void EndQuickUseAfter(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, float revertBy)
            {
                if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernMeleeScriptRuntimeData))
                {
                    Kit_ModernMeleeScriptRuntimeData data = runtimeData as Kit_ModernMeleeScriptRuntimeData;

                    if (pb.isFirstPersonActive)
                    {
                        data.meleeRenderer.visible = false;
                        if (data.meleeRenderer.anim)
                        {
                            data.meleeRenderer.anim.enabled = false;
                        }
                        else if (data.meleeRenderer.legacyAnim)
                        {
                            data.meleeRenderer.legacyAnim.enabled = false;
                        }
                    }
                    data.tpMeleeRenderer.visible = false;
                }
            }

            public override float AimInTime(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                return 0.5f;
            }

            public override void AnimateWeapon(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, int id, float speed)
            {
                if (pb.isFirstPersonActive)
                {
                    if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernMeleeScriptRuntimeData))
                    {
                        Kit_ModernMeleeScriptRuntimeData data = runtimeData as Kit_ModernMeleeScriptRuntimeData;

                        //Camera animation
                        if (data.meleeRenderer.cameraAnimationEnabled)
                        {
                            if (data.meleeRenderer.cameraAnimationType == CameraAnimationType.Copy)
                            {
                                pb.playerCameraAnimationTransform.localRotation = Quaternion.Euler(data.meleeRenderer.cameraAnimationReferenceRotation) * data.meleeRenderer.cameraAnimationBone.localRotation;
                            }
                            else if (data.meleeRenderer.cameraAnimationType == CameraAnimationType.LookAt)
                            {
                                pb.playerCameraAnimationTransform.localRotation = Quaternion.Euler(data.meleeRenderer.cameraAnimationReferenceRotation) * Quaternion.LookRotation(data.meleeRenderer.cameraAnimationTarget.localPosition - data.meleeRenderer.cameraAnimationBone.localPosition);
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
                            data.meleeRenderer.transform.localPosition = Vector3.Lerp(data.meleeRenderer.transform.localPosition, data.meleeRenderer.runPos, Time.deltaTime * data.meleeRenderer.runSmooth);
                            //Move to run rot
                            data.meleeRenderer.transform.localRotation = Quaternion.Slerp(data.meleeRenderer.transform.localRotation, Quaternion.Euler(data.meleeRenderer.runRot), Time.deltaTime * data.meleeRenderer.runSmooth);
                            //Set time
                            data.lastRun = Time.time;
                        }
                        else
                        {
                            //Move back to idle pos
                            data.meleeRenderer.transform.localPosition = Vector3.Lerp(data.meleeRenderer.transform.localPosition, Vector3.zero, Time.deltaTime * data.meleeRenderer.runSmooth * 2f);
                            //Move back to idle rot
                            data.meleeRenderer.transform.localRotation = Quaternion.Slerp(data.meleeRenderer.transform.localRotation, Quaternion.identity, Time.deltaTime * data.meleeRenderer.runSmooth * 2f);
                        }

                        if (data.meleeRenderer.legacyAnim && !data.meleeRenderer.legacyAnim.isPlaying && data.isSelectedAndReady && !data.isCharging)
                        {
                            data.meleeRenderer.legacyAnim[data.meleeRenderer.legacyAnimData.idle].wrapMode = WrapMode.Loop;
                            data.meleeRenderer.legacyAnim.CrossFade(data.meleeRenderer.legacyAnimData.idle, 0.3f, PlayMode.StopAll);
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
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.ResetTrigger("Start Run");
                                            data.meleeRenderer.anim.SetTrigger("End Run");
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim[data.meleeRenderer.legacyAnimData.idle].wrapMode = WrapMode.Loop;
                                            data.meleeRenderer.legacyAnim.CrossFade(data.meleeRenderer.legacyAnimData.idle, 0.3f, PlayMode.StopAll);
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
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.ResetTrigger("Start Run");
                                            data.meleeRenderer.anim.SetTrigger("End Run");
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim[data.meleeRenderer.legacyAnimData.idle].wrapMode = WrapMode.Loop;
                                            data.meleeRenderer.legacyAnim.CrossFade(data.meleeRenderer.legacyAnimData.idle, 0.3f, PlayMode.StopAll);
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
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.ResetTrigger("End Run");
                                            data.meleeRenderer.anim.SetTrigger("Start Run");
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim[data.meleeRenderer.legacyAnimData.run].wrapMode = WrapMode.Loop;
                                            data.meleeRenderer.legacyAnim.CrossFade(data.meleeRenderer.legacyAnimData.run, 0.3f, PlayMode.StopAll);
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
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.ResetTrigger("Start Run");
                                            data.meleeRenderer.anim.SetTrigger("End Run");
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim[data.meleeRenderer.legacyAnimData.idle].wrapMode = WrapMode.Loop;
                                            data.meleeRenderer.legacyAnim.CrossFade(data.meleeRenderer.legacyAnimData.idle, 0.3f, PlayMode.StopAll);
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
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.ResetTrigger("Start Run");
                                            data.meleeRenderer.anim.SetTrigger("End Run");
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim[data.meleeRenderer.legacyAnimData.idle].wrapMode = WrapMode.Loop;
                                            data.meleeRenderer.legacyAnim.CrossFade(data.meleeRenderer.legacyAnimData.idle, 0.3f, PlayMode.StopAll);
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
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.ResetTrigger("End Run");
                                            data.meleeRenderer.anim.SetTrigger("Start Run");
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim[data.meleeRenderer.legacyAnimData.run].wrapMode = WrapMode.Loop;
                                            data.meleeRenderer.legacyAnim.CrossFade(data.meleeRenderer.legacyAnimData.run, 0.3f, PlayMode.StopAll);
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
                if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernMeleeScriptRuntimeData))
                {
                    Kit_ModernMeleeScriptRuntimeData data = runtimeData as Kit_ModernMeleeScriptRuntimeData;

                    if (pb.isFirstPersonActive)
                    {
                        if (primaryAttack == AttackType.Heal && secondaryAttack == AttackType.Heal)
                        {
                            Kit_IngameMain.instance.hud.DisplayAmmo(data.primaryHealingAmount, data.secondaryHealingAmount);
                        }
                        else if (primaryAttack == AttackType.Heal)
                        {
                            Kit_IngameMain.instance.hud.DisplayAmmo(data.primaryHealingAmount, 0);
                        }
                        else if (secondaryAttack == AttackType.Heal)
                        {
                            Kit_IngameMain.instance.hud.DisplayAmmo(0, data.secondaryHealingAmount);
                        }
                        else
                        {
                            Kit_IngameMain.instance.hud.DisplayAmmo(0, 0, false);
                        }
                    }
                }
            }

            public override void DrawWeapon(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernMeleeScriptRuntimeData))
                {
                    Kit_ModernMeleeScriptRuntimeData data = runtimeData as Kit_ModernMeleeScriptRuntimeData;
                    //Set selected
                    data.isSelected = true;

                    if (pb.isFirstPersonActive)
                    {
                        //Reset pos & rot of the renderer
                        data.meleeRenderer.transform.localPosition = Vector3.zero;
                        data.meleeRenderer.transform.localRotation = Quaternion.identity;
                        //Reset delay
                        data.weaponDelayCur = Vector3.zero;
                        //Reset fov
                        Kit_IngameMain.instance.mainCamera.fieldOfView = Kit_GameSettings.baseFov;
                        //Play animation
                        if (data.meleeRenderer.anim)
                        {
                            //Enable anim
                            data.meleeRenderer.anim.enabled = true;
                            data.meleeRenderer.anim.Play("Draw", 0, 0f);
                        }
                        else if (data.meleeRenderer.legacyAnim)
                        {
                            //Enable anim
                            data.meleeRenderer.legacyAnim.enabled = true;
                            data.meleeRenderer.legacyAnim.Play(data.meleeRenderer.legacyAnimData.draw);
                        }
                        //Play sound if it is assigned
                        if (drawSound) data.sounds.PlayOneShot(drawSound);
                        if (pb.looking.GetPerspective(pb) == Kit_GameInformation.Perspective.FirstPerson)
                        {
                            //Show weapon
                            data.meleeRenderer.visible = true;
                        }
                        else
                        {
                            data.meleeRenderer.visible = false;
                        }
                    }

                    if (pb.looking.GetPerspective(pb) == Kit_GameInformation.Perspective.FirstPerson && pb.isFirstPersonActive)
                    {
                        //Show tp weapon and hide
                        data.tpMeleeRenderer.visible = true;
                        data.tpMeleeRenderer.shadowsOnly = true;
                    }
                    else
                    {
                        //Show tp weapon and show
                        data.tpMeleeRenderer.visible = true;
                        data.tpMeleeRenderer.shadowsOnly = false;
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
                if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernMeleeScriptRuntimeData))
                {
                    Kit_ModernMeleeScriptRuntimeData data = runtimeData as Kit_ModernMeleeScriptRuntimeData;
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
                if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernMeleeScriptRuntimeData))
                {
                    //Get runtime data
                    Kit_ModernMeleeScriptRuntimeData data = runtimeData as Kit_ModernMeleeScriptRuntimeData;
                    //Activate or deactivate based on bool
                    if (perspective == Kit_GameInformation.Perspective.ThirdPerson)
                    {
                        if (data.meleeRenderer)
                        {
                            data.meleeRenderer.visible = false;
                        }
                        if (data.tpMeleeRenderer)
                        {
                            data.tpMeleeRenderer.visible = true;
                            data.tpMeleeRenderer.shadowsOnly = false;
                        }
                    }
                    else
                    {
                        if (data.meleeRenderer)
                        {
                            data.meleeRenderer.visible = true;
                        }
                        if (data.tpMeleeRenderer)
                        {
                            data.tpMeleeRenderer.visible = true;
                            data.tpMeleeRenderer.shadowsOnly = true;
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

            public override void NetworkMeleeChargeRPCReceived(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, int id, int slot)
            {
                /*
                if (!pb.photonView.IsMine)
                {
                    if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernMeleeScriptRuntimeData))
                    {
                        Kit_ModernMeleeScriptRuntimeData data = runtimeData as Kit_ModernMeleeScriptRuntimeData;
                        if (slot == 0)
                        {
                            if (id == 0)
                            {
                                if (pb.isFirstPersonActive)
                                {
                                    if (primaryAttackSettings.chargeChargeAnimation != "")
                                    {
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.Play(primaryAttackSettings.chargeChargeAnimation);
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim.Play(primaryAttackSettings.chargeChargeAnimation);
                                        }
                                    }

                                    if (primaryAttackSettings.chargeChargeSound)
                                    {
                                        data.sounds.clip = primaryAttackSettings.chargeChargeSound;
                                        data.sounds.Play();
                                    }
                                }
                                else
                                {
                                    if (primaryAttackSettings.chargeChargeSound)
                                    {
                                        pb.thirdPersonPlayerModel.soundOther.clip = primaryAttackSettings.chargeChargeSound;
                                        pb.thirdPersonPlayerModel.soundOther.Play();
                                    }
                                }
                            }
                            else if (id == 1)
                            {
                                if (pb.isFirstPersonActive)
                                {
                                    if (primaryAttackSettings.chargeWindupAnimationName != "")
                                    {
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.Play(primaryAttackSettings.chargeWindupAnimationName);
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim.Play(primaryAttackSettings.chargeWindupAnimationName);
                                        }
                                    }

                                    if (primaryAttackSettings.chargeWindupSound)
                                    {
                                        data.sounds.clip = primaryAttackSettings.chargeWindupSound;
                                        data.sounds.Play();
                                    }
                                }
                                else
                                {
                                    if (primaryAttackSettings.chargeWindupSound)
                                    {
                                        pb.thirdPersonPlayerModel.soundOther.clip = primaryAttackSettings.chargeWindupSound;
                                        pb.thirdPersonPlayerModel.soundOther.Play();
                                    }
                                }
                            }
                            else if (id == 2)
                            {
                                if (pb.isFirstPersonActive)
                                {
                                    if (primaryAttackSettings.chargeAnimationMissName != "")
                                    {
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.Play(primaryAttackSettings.chargeAnimationMissName);
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim.Play(primaryAttackSettings.chargeAnimationMissName);
                                        }
                                    }

                                    if (primaryAttackSettings.chargeMissSound)
                                    {
                                        data.sounds.clip = primaryAttackSettings.chargeMissSound;
                                        data.sounds.Play();
                                    }
                                }
                                else
                                {
                                    if (primaryAttackSettings.chargeMissSound)
                                    {
                                        pb.thirdPersonPlayerModel.soundOther.clip = primaryAttackSettings.chargeMissSound;
                                        pb.thirdPersonPlayerModel.soundOther.Play();
                                    }
                                }
                            }
                            else if (id == 3)
                            {
                                if (pb.isFirstPersonActive)
                                {
                                    if (primaryAttackSettings.chargeAnimationHitName != "")
                                    {
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.Play(primaryAttackSettings.chargeAnimationHitName);
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim.Play(primaryAttackSettings.chargeAnimationHitName);
                                        }
                                    }

                                    if (primaryAttackSettings.chargeHitSound)
                                    {
                                        data.sounds.clip = primaryAttackSettings.chargeHitSound;
                                        data.sounds.Play();
                                    }
                                }
                                else
                                {
                                    if (primaryAttackSettings.chargeHitSound)
                                    {
                                        pb.thirdPersonPlayerModel.soundOther.clip = primaryAttackSettings.chargeHitSound;
                                        pb.thirdPersonPlayerModel.soundOther.Play();
                                    }
                                }
                            }
                            else if (id == 4)
                            {
                                if (pb.isFirstPersonActive)
                                {
                                    if (primaryAttackSettings.chargeAnimationHitObjectName != "")
                                    {
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.Play(primaryAttackSettings.chargeAnimationHitObjectName);
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim.Play(primaryAttackSettings.chargeAnimationHitObjectName);
                                        }
                                    }

                                    if (primaryAttackSettings.chargeHitObjectSound)
                                    {
                                        data.sounds.clip = primaryAttackSettings.chargeHitObjectSound;
                                        data.sounds.Play();
                                    }
                                }
                                else
                                {
                                    if (primaryAttackSettings.chargeHitObjectSound)
                                    {
                                        pb.thirdPersonPlayerModel.soundOther.clip = primaryAttackSettings.chargeHitObjectSound;
                                        pb.thirdPersonPlayerModel.soundOther.Play();
                                    }
                                }
                            }
                        }
                        else if (slot == 1)
                        {
                            if (id == 0)
                            {
                                if (pb.isFirstPersonActive)
                                {
                                    if (secondaryAttackSettings.chargeChargeAnimation != "")
                                    {
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.Play(secondaryAttackSettings.chargeChargeAnimation);
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim.Play(secondaryAttackSettings.chargeChargeAnimation);
                                        }
                                    }

                                    if (secondaryAttackSettings.chargeChargeSound)
                                    {
                                        data.sounds.clip = secondaryAttackSettings.chargeChargeSound;
                                        data.sounds.Play();
                                    }
                                }
                                else
                                {
                                    if (secondaryAttackSettings.chargeChargeSound)
                                    {
                                        pb.thirdPersonPlayerModel.soundOther.clip = secondaryAttackSettings.chargeChargeSound;
                                        pb.thirdPersonPlayerModel.soundOther.Play();
                                    }
                                }
                            }
                            else if (id == 1)
                            {
                                if (pb.isFirstPersonActive)
                                {
                                    if (secondaryAttackSettings.chargeWindupAnimationName != "")
                                    {
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.Play(secondaryAttackSettings.chargeWindupAnimationName);
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim.Play(secondaryAttackSettings.chargeWindupAnimationName);
                                        }
                                    }

                                    if (secondaryAttackSettings.chargeWindupSound)
                                    {
                                        data.sounds.clip = secondaryAttackSettings.chargeWindupSound;
                                        data.sounds.Play();
                                    }
                                }
                                else
                                {
                                    if (secondaryAttackSettings.chargeWindupSound)
                                    {
                                        pb.thirdPersonPlayerModel.soundOther.clip = secondaryAttackSettings.chargeWindupSound;
                                        pb.thirdPersonPlayerModel.soundOther.Play();
                                    }
                                }
                            }
                            else if (id == 2)
                            {
                                if (pb.isFirstPersonActive)
                                {
                                    if (secondaryAttackSettings.chargeAnimationMissName != "")
                                    {
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.Play(secondaryAttackSettings.chargeAnimationMissName);
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim.Play(secondaryAttackSettings.chargeAnimationMissName);
                                        }
                                    }

                                    if (secondaryAttackSettings.chargeMissSound)
                                    {
                                        data.sounds.clip = secondaryAttackSettings.chargeMissSound;
                                        data.sounds.Play();
                                    }
                                }
                                else
                                {
                                    if (secondaryAttackSettings.chargeMissSound)
                                    {
                                        pb.thirdPersonPlayerModel.soundOther.clip = secondaryAttackSettings.chargeMissSound;
                                        pb.thirdPersonPlayerModel.soundOther.Play();
                                    }
                                }
                            }
                            else if (id == 3)
                            {
                                if (pb.isFirstPersonActive)
                                {
                                    if (secondaryAttackSettings.chargeAnimationHitName != "")
                                    {
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.Play(secondaryAttackSettings.chargeAnimationHitName);
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim.Play(secondaryAttackSettings.chargeAnimationHitName);
                                        }
                                    }

                                    if (secondaryAttackSettings.chargeHitSound)
                                    {
                                        data.sounds.clip = secondaryAttackSettings.chargeHitSound;
                                        data.sounds.Play();
                                    }
                                }
                                else
                                {
                                    if (secondaryAttackSettings.chargeHitSound)
                                    {
                                        pb.thirdPersonPlayerModel.soundOther.clip = secondaryAttackSettings.chargeHitSound;
                                        pb.thirdPersonPlayerModel.soundOther.Play();
                                    }
                                }
                            }
                            else if (id == 4)
                            {
                                if (pb.isFirstPersonActive)
                                {
                                    if (secondaryAttackSettings.chargeAnimationHitObjectName != "")
                                    {
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.Play(secondaryAttackSettings.chargeAnimationHitObjectName);
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim.Play(secondaryAttackSettings.chargeAnimationHitObjectName);
                                        }
                                    }

                                    if (secondaryAttackSettings.chargeHitObjectSound)
                                    {
                                        data.sounds.clip = secondaryAttackSettings.chargeHitObjectSound;
                                        data.sounds.Play();
                                    }
                                }
                                else
                                {
                                    if (secondaryAttackSettings.chargeHitObjectSound)
                                    {
                                        pb.thirdPersonPlayerModel.soundOther.clip = secondaryAttackSettings.chargeHitObjectSound;
                                        pb.thirdPersonPlayerModel.soundOther.Play();
                                    }
                                }
                            }
                        }
                        else if (slot == 2)
                        {
                            if (id == 0)
                            {
                                if (pb.isFirstPersonActive)
                                {
                                    if (quickAttackSettings.chargeChargeAnimation != "")
                                    {
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.Play(quickAttackSettings.chargeChargeAnimation);
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim.Play(quickAttackSettings.chargeChargeAnimation);
                                        }
                                    }

                                    if (quickAttackSettings.chargeChargeSound)
                                    {
                                        data.sounds.clip = quickAttackSettings.chargeChargeSound;
                                        data.sounds.Play();
                                    }
                                }
                                else
                                {
                                    if (quickAttackSettings.chargeChargeSound)
                                    {
                                        pb.thirdPersonPlayerModel.soundOther.clip = quickAttackSettings.chargeChargeSound;
                                        pb.thirdPersonPlayerModel.soundOther.Play();
                                    }
                                }
                            }
                            else if (id == 1)
                            {
                                if (pb.isFirstPersonActive)
                                {
                                    if (quickAttackSettings.chargeWindupAnimationName != "")
                                    {
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.Play(quickAttackSettings.chargeWindupAnimationName);
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim.Play(quickAttackSettings.chargeWindupAnimationName);
                                        }
                                    }

                                    if (quickAttackSettings.chargeWindupSound)
                                    {
                                        data.sounds.clip = quickAttackSettings.chargeWindupSound;
                                        data.sounds.Play();
                                    }
                                }
                                else
                                {
                                    if (quickAttackSettings.chargeWindupSound)
                                    {
                                        pb.thirdPersonPlayerModel.soundOther.clip = quickAttackSettings.chargeWindupSound;
                                        pb.thirdPersonPlayerModel.soundOther.Play();
                                    }
                                }
                            }
                            else if (id == 2)
                            {
                                if (pb.isFirstPersonActive)
                                {
                                    if (quickAttackSettings.chargeAnimationMissName != "")
                                    {
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.Play(quickAttackSettings.chargeAnimationMissName);
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim.Play(quickAttackSettings.chargeAnimationMissName);
                                        }
                                    }

                                    if (quickAttackSettings.chargeMissSound)
                                    {
                                        data.sounds.clip = quickAttackSettings.chargeMissSound;
                                        data.sounds.Play();
                                    }
                                }
                                else
                                {
                                    if (quickAttackSettings.chargeMissSound)
                                    {
                                        pb.thirdPersonPlayerModel.soundOther.clip = quickAttackSettings.chargeMissSound;
                                        pb.thirdPersonPlayerModel.soundOther.Play();
                                    }
                                }
                            }
                            else if (id == 3)
                            {
                                if (pb.isFirstPersonActive)
                                {
                                    if (quickAttackSettings.chargeAnimationHitName != "")
                                    {
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.Play(quickAttackSettings.chargeAnimationHitName);
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim.Play(quickAttackSettings.chargeAnimationHitName);
                                        }
                                    }

                                    if (quickAttackSettings.chargeHitSound)
                                    {
                                        data.sounds.clip = quickAttackSettings.chargeHitSound;
                                        data.sounds.Play();
                                    }
                                }
                                else
                                {
                                    if (quickAttackSettings.chargeHitSound)
                                    {
                                        pb.thirdPersonPlayerModel.soundOther.clip = quickAttackSettings.chargeHitSound;
                                        pb.thirdPersonPlayerModel.soundOther.Play();
                                    }
                                }
                            }
                            else if (id == 4)
                            {
                                if (pb.isFirstPersonActive)
                                {
                                    if (quickAttackSettings.chargeAnimationHitObjectName != "")
                                    {
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.Play(quickAttackSettings.chargeAnimationHitObjectName);
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim.Play(quickAttackSettings.chargeAnimationHitObjectName);
                                        }
                                    }

                                    if (quickAttackSettings.chargeHitObjectSound)
                                    {
                                        data.sounds.clip = quickAttackSettings.chargeHitObjectSound;
                                        data.sounds.Play();
                                    }
                                }
                                else
                                {
                                    if (quickAttackSettings.chargeHitObjectSound)
                                    {
                                        pb.thirdPersonPlayerModel.soundOther.clip = quickAttackSettings.chargeHitObjectSound;
                                        pb.thirdPersonPlayerModel.soundOther.Play();
                                    }
                                }
                            }
                        }
                    }
                }
                */
            }

            public override void NetworkMeleeStabRPCReceived(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, int state, int slot)
            {
                /*
                if (!pb.photonView.IsMine)
                {
                    if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernMeleeScriptRuntimeData))
                    {
                        Kit_ModernMeleeScriptRuntimeData data = runtimeData as Kit_ModernMeleeScriptRuntimeData;
                        if (slot == 0)
                        {
                            if (state == 0)
                            {
                                if (pb.isFirstPersonActive)
                                {
                                    if (primaryAttackSettings.stabWindupAnimationName != "")
                                    {
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.Play(primaryAttackSettings.stabWindupAnimationName);
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim.Play(primaryAttackSettings.stabWindupAnimationName);
                                        }
                                    }

                                    if (primaryAttackSettings.stabWindupSound)
                                    {
                                        data.sounds.clip = primaryAttackSettings.stabWindupSound;
                                        data.sounds.Play();
                                    }
                                }
                                else
                                {
                                    if (primaryAttackSettings.stabWindupSound)
                                    {
                                        pb.thirdPersonPlayerModel.soundOther.clip = primaryAttackSettings.stabWindupSound;
                                        pb.thirdPersonPlayerModel.soundOther.Play();
                                    }
                                }
                            }
                            else if (state == 1)
                            {
                                if (pb.isFirstPersonActive)
                                {
                                    if (primaryAttackSettings.stabAnimationMissName != "")
                                    {
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.Play(primaryAttackSettings.stabAnimationMissName);
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim.Play(primaryAttackSettings.stabAnimationMissName);
                                        }
                                    }

                                    if (primaryAttackSettings.stabMissSound)
                                    {
                                        data.sounds.clip = primaryAttackSettings.stabMissSound;
                                        data.sounds.Play();
                                    }
                                }
                                else
                                {
                                    if (primaryAttackSettings.stabMissSound)
                                    {
                                        pb.thirdPersonPlayerModel.soundOther.clip = primaryAttackSettings.stabMissSound;
                                        pb.thirdPersonPlayerModel.soundOther.Play();
                                    }
                                }
                            }
                            else if (state == 2)
                            {
                                if (pb.isFirstPersonActive)
                                {
                                    if (primaryAttackSettings.stabAnimationHitName != "")
                                    {
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.Play(primaryAttackSettings.stabAnimationHitName);
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim.Play(primaryAttackSettings.stabAnimationHitName);
                                        }
                                    }

                                    if (primaryAttackSettings.stabHitSound)
                                    {
                                        data.sounds.clip = primaryAttackSettings.stabHitSound;
                                        data.sounds.Play();
                                    }
                                }
                                else
                                {
                                    if (primaryAttackSettings.stabHitSound)
                                    {
                                        pb.thirdPersonPlayerModel.soundOther.clip = primaryAttackSettings.stabHitSound;
                                        pb.thirdPersonPlayerModel.soundOther.Play();
                                    }
                                }
                            }
                            else if (state == 3)
                            {
                                if (pb.isFirstPersonActive)
                                {
                                    if (primaryAttackSettings.stabAnimationHitObjectName != "")
                                    {
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.Play(primaryAttackSettings.stabAnimationHitObjectName);
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim.Play(primaryAttackSettings.stabAnimationHitObjectName);
                                        }
                                    }

                                    if (primaryAttackSettings.stabHitObjectSound)
                                    {
                                        data.sounds.clip = primaryAttackSettings.stabHitObjectSound;
                                        data.sounds.Play();
                                    }
                                }
                                else
                                {
                                    if (primaryAttackSettings.stabHitObjectSound)
                                    {
                                        pb.thirdPersonPlayerModel.soundOther.clip = primaryAttackSettings.stabHitObjectSound;
                                        pb.thirdPersonPlayerModel.soundOther.Play();
                                    }
                                }
                            }
                        }
                        else if (slot == 1)
                        {
                            if (state == 0)
                            {
                                if (pb.isFirstPersonActive)
                                {
                                    if (secondaryAttackSettings.stabWindupAnimationName != "")
                                    {
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.Play(secondaryAttackSettings.stabWindupAnimationName);
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim.Play(secondaryAttackSettings.stabWindupAnimationName);
                                        }
                                    }

                                    if (secondaryAttackSettings.stabWindupSound)
                                    {
                                        data.sounds.clip = secondaryAttackSettings.stabWindupSound;
                                        data.sounds.Play();
                                    }
                                }
                                else
                                {
                                    if (secondaryAttackSettings.stabWindupSound)
                                    {
                                        pb.thirdPersonPlayerModel.soundOther.clip = secondaryAttackSettings.stabWindupSound;
                                        pb.thirdPersonPlayerModel.soundOther.Play();
                                    }
                                }
                            }
                            else if (state == 1)
                            {
                                if (pb.isFirstPersonActive)
                                {
                                    if (secondaryAttackSettings.stabAnimationMissName != "")
                                    {
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.Play(secondaryAttackSettings.stabAnimationMissName);
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim.Play(secondaryAttackSettings.stabAnimationMissName);
                                        }
                                    }

                                    if (secondaryAttackSettings.stabMissSound)
                                    {
                                        data.sounds.clip = secondaryAttackSettings.stabMissSound;
                                        data.sounds.Play();
                                    }
                                }
                                else
                                {
                                    if (secondaryAttackSettings.stabMissSound)
                                    {
                                        pb.thirdPersonPlayerModel.soundOther.clip = secondaryAttackSettings.stabMissSound;
                                        pb.thirdPersonPlayerModel.soundOther.Play();
                                    }
                                }
                            }
                            else if (state == 2)
                            {
                                if (pb.isFirstPersonActive)
                                {
                                    if (secondaryAttackSettings.stabAnimationHitName != "")
                                    {
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.Play(secondaryAttackSettings.stabAnimationHitName);
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim.Play(secondaryAttackSettings.stabAnimationHitName);
                                        }
                                    }

                                    if (secondaryAttackSettings.stabHitSound)
                                    {
                                        data.sounds.clip = secondaryAttackSettings.stabHitSound;
                                        data.sounds.Play();
                                    }
                                }
                                else
                                {
                                    if (secondaryAttackSettings.stabHitSound)
                                    {
                                        pb.thirdPersonPlayerModel.soundOther.clip = secondaryAttackSettings.stabHitSound;
                                        pb.thirdPersonPlayerModel.soundOther.Play();
                                    }
                                }
                            }
                            else if (state == 3)
                            {
                                if (pb.isFirstPersonActive)
                                {
                                    if (secondaryAttackSettings.stabAnimationHitObjectName != "")
                                    {
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.Play(secondaryAttackSettings.stabAnimationHitObjectName);
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim.Play(secondaryAttackSettings.stabAnimationHitObjectName);
                                        }
                                    }

                                    if (secondaryAttackSettings.stabHitObjectSound)
                                    {
                                        data.sounds.clip = secondaryAttackSettings.stabHitObjectSound;
                                        data.sounds.Play();
                                    }
                                }
                                else
                                {
                                    if (secondaryAttackSettings.stabHitObjectSound)
                                    {
                                        pb.thirdPersonPlayerModel.soundOther.clip = secondaryAttackSettings.stabHitObjectSound;
                                        pb.thirdPersonPlayerModel.soundOther.Play();
                                    }
                                }
                            }
                        }
                        else if (slot == 2)
                        {
                            if (state == 0)
                            {
                                if (pb.isFirstPersonActive)
                                {
                                    if (quickAttackSettings.stabWindupAnimationName != "")
                                    {
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.Play(quickAttackSettings.stabWindupAnimationName);
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim.Play(quickAttackSettings.stabWindupAnimationName);
                                        }
                                    }

                                    if (quickAttackSettings.stabWindupSound)
                                    {
                                        data.sounds.clip = quickAttackSettings.stabWindupSound;
                                        data.sounds.Play();
                                    }
                                }
                                else
                                {
                                    if (quickAttackSettings.stabWindupSound)
                                    {
                                        pb.thirdPersonPlayerModel.soundOther.clip = quickAttackSettings.stabWindupSound;
                                        pb.thirdPersonPlayerModel.soundOther.Play();
                                    }
                                }
                            }
                            else if (state == 1)
                            {
                                if (pb.isFirstPersonActive)
                                {
                                    if (quickAttackSettings.stabAnimationMissName != "")
                                    {
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.Play(quickAttackSettings.stabAnimationMissName);
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim.Play(quickAttackSettings.stabAnimationMissName);
                                        }
                                    }

                                    if (quickAttackSettings.stabMissSound)
                                    {
                                        data.sounds.clip = quickAttackSettings.stabMissSound;
                                        data.sounds.Play();
                                    }
                                }
                                else
                                {
                                    if (quickAttackSettings.stabMissSound)
                                    {
                                        pb.thirdPersonPlayerModel.soundOther.clip = quickAttackSettings.stabMissSound;
                                        pb.thirdPersonPlayerModel.soundOther.Play();
                                    }
                                }
                            }
                            else if (state == 2)
                            {
                                if (pb.isFirstPersonActive)
                                {
                                    if (quickAttackSettings.stabAnimationHitName != "")
                                    {
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.Play(quickAttackSettings.stabAnimationHitName);
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim.Play(quickAttackSettings.stabAnimationHitName);
                                        }
                                    }

                                    if (quickAttackSettings.stabHitSound)
                                    {
                                        data.sounds.clip = quickAttackSettings.stabHitSound;
                                        data.sounds.Play();
                                    }
                                }
                                else
                                {
                                    if (quickAttackSettings.stabHitSound)
                                    {
                                        pb.thirdPersonPlayerModel.soundOther.clip = quickAttackSettings.stabHitSound;
                                        pb.thirdPersonPlayerModel.soundOther.Play();
                                    }
                                }
                            }
                            else if (state == 3)
                            {
                                if (pb.isFirstPersonActive)
                                {
                                    if (quickAttackSettings.stabAnimationHitObjectName != "")
                                    {
                                        if (data.meleeRenderer.anim)
                                        {
                                            data.meleeRenderer.anim.Play(quickAttackSettings.stabAnimationHitObjectName);
                                        }
                                        else if (data.meleeRenderer.legacyAnim)
                                        {
                                            data.meleeRenderer.legacyAnim.Play(quickAttackSettings.stabAnimationHitObjectName);
                                        }
                                    }

                                    if (quickAttackSettings.stabHitObjectSound)
                                    {
                                        data.sounds.clip = quickAttackSettings.stabHitObjectSound;
                                        data.sounds.Play();
                                    }
                                }
                                else
                                {
                                    if (quickAttackSettings.stabHitObjectSound)
                                    {
                                        pb.thirdPersonPlayerModel.soundOther.clip = quickAttackSettings.stabHitObjectSound;
                                        pb.thirdPersonPlayerModel.soundOther.Play();
                                    }
                                }
                            }
                        }
                    }
                }
                */
            }

            public override void NetworkMeleeHealRPCReceived(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, int slot)
            {
                /*
                if (!pb.photonView.IsMine)
                {
                    if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernMeleeScriptRuntimeData))
                    {
                        Kit_ModernMeleeScriptRuntimeData data = runtimeData as Kit_ModernMeleeScriptRuntimeData;
                        if (slot == 0)
                        {
                            if (pb.isFirstPersonActive)
                            {
                                //Play Animation
                                if (data.meleeRenderer.anim)
                                {
                                    data.meleeRenderer.anim.Play(primaryAttackSettings.healAnimationName, 0, 0f);
                                }
                                else if (data.meleeRenderer.legacyAnim)
                                {
                                    data.meleeRenderer.legacyAnim.Play(primaryAttackSettings.healAnimationName);
                                }

                                if (primaryAttackSettings.healSound)
                                {
                                    data.sounds.clip = primaryAttackSettings.healSound;
                                    data.sounds.Play();
                                }
                            }
                            else
                            {
                                if (primaryAttackSettings.healSound)
                                {
                                    pb.thirdPersonPlayerModel.soundOther.clip = primaryAttackSettings.healSound;
                                    pb.thirdPersonPlayerModel.soundOther.Play();
                                }
                            }
                        }
                        else if (slot == 1)
                        {
                            if (pb.isFirstPersonActive)
                            {
                                //Play Animation
                                if (data.meleeRenderer.anim)
                                {
                                    data.meleeRenderer.anim.Play(secondaryAttackSettings.healAnimationName, 0, 0f);
                                }
                                else if (data.meleeRenderer.legacyAnim)
                                {
                                    data.meleeRenderer.legacyAnim.Play(secondaryAttackSettings.healAnimationName);
                                }

                                if (secondaryAttackSettings.healSound)
                                {
                                    data.sounds.clip = secondaryAttackSettings.healSound;
                                    data.sounds.Play();
                                }
                            }
                            else
                            {
                                if (secondaryAttackSettings.healSound)
                                {
                                    pb.thirdPersonPlayerModel.soundOther.clip = secondaryAttackSettings.healSound;
                                    pb.thirdPersonPlayerModel.soundOther.Play();
                                }
                            }
                        }
                        else if (slot == 2)
                        {
                            if (pb.isFirstPersonActive)
                            {
                                //Play Animation
                                if (data.meleeRenderer.anim)
                                {
                                    data.meleeRenderer.anim.Play(quickAttackSettings.healAnimationName, 0, 0f);
                                }
                                else if (data.meleeRenderer.legacyAnim)
                                {
                                    data.meleeRenderer.legacyAnim.Play(quickAttackSettings.healAnimationName);
                                }

                                if (quickAttackSettings.healSound)
                                {
                                    data.sounds.clip = quickAttackSettings.healSound;
                                    data.sounds.Play();
                                }
                            }
                            else
                            {
                                if (quickAttackSettings.healSound)
                                {
                                    pb.thirdPersonPlayerModel.soundOther.clip = quickAttackSettings.healSound;
                                    pb.thirdPersonPlayerModel.soundOther.Play();
                                }
                            }
                        }
                    }
                }
                */
            }

            public override void PutawayWeapon(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernMeleeScriptRuntimeData))
                {
                    Kit_ModernMeleeScriptRuntimeData data = runtimeData as Kit_ModernMeleeScriptRuntimeData;
                    if (pb.isFirstPersonActive)
                    {
                        //Reset fov
                        Kit_IngameMain.instance.mainCamera.fieldOfView = Kit_GameSettings.baseFov;
                        //Play animation
                        if (data.meleeRenderer.anim)
                        {
                            //Enable anim
                            data.meleeRenderer.anim.enabled = true;
                            data.meleeRenderer.anim.Play("Putaway", 0, 0f);
                        }
                        else if (data.meleeRenderer.legacyAnim)
                        {
                            //Enable anim
                            data.meleeRenderer.legacyAnim.enabled = true;
                            data.meleeRenderer.legacyAnim.Play(data.meleeRenderer.legacyAnimData.putaway);
                        }
                        //Play sound if it is assigned
                        if (putawaySound) data.sounds.PlayOneShot(putawaySound);
                        if (pb.looking.GetPerspective(pb) == Kit_GameInformation.Perspective.FirstPerson)
                        {
                            //Show weapon
                            data.meleeRenderer.visible = true;
                        }
                        else
                        {
                            //Hide
                            data.meleeRenderer.visible = false;
                        }
                    }
                    if (pb.looking.GetPerspective(pb) == Kit_GameInformation.Perspective.FirstPerson && pb.isFirstPersonActive)
                    {
                        //Show tp weapon
                        data.tpMeleeRenderer.visible = true;
                        data.tpMeleeRenderer.shadowsOnly = true;
                    }
                    else
                    {
                        data.tpMeleeRenderer.visible = true;
                        data.tpMeleeRenderer.shadowsOnly = false;
                    }
                    //Make sure it is not ready yet
                    data.isSelectedAndReady = false;
                    //Stop third person anims
                    pb.thirdPersonPlayerModel.AbortWeaponAnimations();
                }
            }

            public override void PutawayWeaponHide(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                if (runtimeData != null && runtimeData.GetType() == typeof(Kit_ModernMeleeScriptRuntimeData))
                {
                    Kit_ModernMeleeScriptRuntimeData data = runtimeData as Kit_ModernMeleeScriptRuntimeData;
                    //Set selected
                    data.isSelected = false;
                    //Reset run animation
                    data.startedRunAnimation = false;
                    if (pb.isFirstPersonActive)
                    {
                        //Hide weapon
                        data.meleeRenderer.visible = false;
                        //Disable anim
                        if (data.meleeRenderer.anim)
                        {
                            data.meleeRenderer.anim.enabled = false;
                        }
                        else if (data.meleeRenderer.legacyAnim)
                        {
                            data.meleeRenderer.legacyAnim.enabled = false;
                        }
                        //Reset pos & rot of the renderer
                        data.meleeRenderer.transform.localPosition = Vector3.zero;
                        data.meleeRenderer.transform.localRotation = Quaternion.identity;
                        //Reset delay
                        data.weaponDelayCur = Vector3.zero;
                    }
                    //Hide tp weapon
                    data.tpMeleeRenderer.visible = false;
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
                Kit_ModernMeleeScriptRuntimeData data = runtimeData as Kit_ModernMeleeScriptRuntimeData;

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
                data.meleeRenderer = fpRuntime.GetComponent<Kit_MeleeRenderer>();

                //Play Dependent arms
                if (data.meleeRenderer.playerModelDependentArmsEnabled && pb.thirdPersonPlayerModel.firstPersonArmsPrefab.Contains(data.meleeRenderer.playerModelDependentArmsKey))
                {
                    //Create Prefab
                    GameObject fpArms = Instantiate(pb.thirdPersonPlayerModel.firstPersonArmsPrefab[data.meleeRenderer.playerModelDependentArmsKey], fallTrans.transform, false);

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
                        data.meleeRenderer.cameraAnimationBone = fpa.cameraBoneOverride;
                    }

                    if (fpa.cameraBoneTargetOverride)
                    {
                        data.meleeRenderer.cameraAnimationTarget = fpa.cameraBoneTargetOverride;
                    }

                    //Reparent
                    for (int i = 0; i < fpa.reparents.Length; i++)
                    {
                        if (fpa.reparents[i])
                        {
                            fpa.reparents[i].transform.parent = data.meleeRenderer.playerModelDependentArmsRoot;
                        }
                        else
                        {
                            Debug.LogWarning("First Person Arm Renderer at index " + i + " is not assigned", fpa);
                        }
                    }
                    //Merge Array
                    data.meleeRenderer.allWeaponRenderers = data.meleeRenderer.allWeaponRenderers.Concat(fpa.renderers).ToArray();
                    //Rebind so that the animator animates our freshly reparented transforms too!
                    if (data.meleeRenderer.anim)
                    {
                        data.meleeRenderer.anim.Rebind();
                    }
                }

                data.meleeRenderer.visible = false;

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

                if (primaryAttack == AttackType.Heal)
                {
                    data.primaryHealingAmount = primaryAttackSettings.healStartAmount;
                }

                if (secondaryAttack == AttackType.Heal)
                {
                    data.secondaryHealingAmount = secondaryAttackSettings.healStartAmount;
                }

                if (quickAttack == AttackType.Heal)
                {
                    data.quickHealingAmount = quickAttackSettings.healStartAmount;
                }
            }

            public override void SetupThirdPerson(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                Kit_ModernMeleeScriptRuntimeData data = runtimeData as Kit_ModernMeleeScriptRuntimeData;

                //Setup the third person prefab
                GameObject tpRuntime = Instantiate(thirdPersonPrefab, pb.thirdPersonPlayerModel.weaponsInHandsGo, false);
                //Set Scale
                tpRuntime.transform.localScale = Vector3.one;

                //Setup renderer
                data.tpMeleeRenderer = tpRuntime.GetComponent<Kit_ThirdPersonMeleeRenderer>();
                data.tpMeleeRenderer.visible = false;
                if (pb.isFirstPersonActive)
                {
                    //Make it shadows only
                    data.tpMeleeRenderer.shadowsOnly = true;
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
                return 2;
            }
            public override void BeginSpectating(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, int[] attachments)
            {
                Kit_ModernMeleeScriptRuntimeData data = runtimeData as Kit_ModernMeleeScriptRuntimeData;

                //Create First Person renderer if it doesn't exist already
                if (!data.meleeRenderer)
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
                    data.meleeRenderer = fpRuntime.GetComponent<Kit_MeleeRenderer>();

                    //Play Dependent arms
                    if (data.meleeRenderer.playerModelDependentArmsEnabled && pb.thirdPersonPlayerModel.firstPersonArmsPrefab.Contains(data.meleeRenderer.playerModelDependentArmsKey))
                    {
                        //Create Prefab
                        GameObject fpArms = Instantiate(pb.thirdPersonPlayerModel.firstPersonArmsPrefab[data.meleeRenderer.playerModelDependentArmsKey], fallTrans.transform, false);

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
                                fpa.reparents[i].transform.parent = data.meleeRenderer.playerModelDependentArmsRoot;
                            }
                            else
                            {
                                Debug.LogWarning("First Person Arm Renderer at index " + i + " is not assigned", fpa);
                            }
                        }
                        //Merge Array
                        data.meleeRenderer.allWeaponRenderers = data.meleeRenderer.allWeaponRenderers.Concat(fpa.renderers).ToArray();
                        //Rebind so that the animator animates our freshly reparented transforms too!
                        if (data.meleeRenderer.anim)
                        {
                            data.meleeRenderer.anim.Rebind();
                        }
                    }

                    data.meleeRenderer.visible = false;

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
                    data.meleeRenderer.visible = data.isSelected;

                    //Hide tp
                    data.tpMeleeRenderer.shadowsOnly = true;
                }
                else
                {
                    //FP is definitely hidden, TP is already in the state that it should be in
                    data.meleeRenderer.visible = false;
                }
            }

            public override void EndSpectating(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                //Get data
                Kit_ModernMeleeScriptRuntimeData data = runtimeData as Kit_ModernMeleeScriptRuntimeData;

                //Hide melee renderer if present
                if (data.meleeRenderer)
                {
                    data.meleeRenderer.visible = false;
                }

                data.tpMeleeRenderer.shadowsOnly = false;
            }
        }
    }
}