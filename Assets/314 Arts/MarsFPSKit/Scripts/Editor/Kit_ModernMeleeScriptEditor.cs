using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace MarsFPSKit
{
    namespace Weapons
    {
        [CustomEditor(typeof(Kit_ModernMeleeScript))]
        public class Kit_ModernMeleeScriptEditor : UnityEditor.Editor
        {
            static bool prefabsFoldout;
            static bool settingsFoldout;
            static bool attackSettingsFoldout;
            static bool weaponDelayFoldout;
            static bool weaponTiltFoldout;
            static bool weaponFallFoldout;
            static bool genericAnimationsFoldout;
            static bool springFoldout;

            Kit_GameInformation information = null;

            SerializedProperty slotsProperty;

            void OnEnable()
            {
                // Fetch the objects from the MyScript script to display in the inspector
                slotsProperty = serializedObject.FindProperty("canFitIntoSlots");
            }

            public override void OnInspectorGUI()
            {
                Kit_ModernMeleeScript script = target as Kit_ModernMeleeScript;

                if (!information)
                {
                    information = Resources.Load<Kit_GameInformation>("Game");
                }

                prefabsFoldout = EditorGUILayout.Foldout(prefabsFoldout, "Prefabs");
                if (prefabsFoldout)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    script.firstPersonPrefab = EditorGUILayout.ObjectField("First Person Prefab", script.firstPersonPrefab, typeof(GameObject), false) as GameObject;
                    script.thirdPersonPrefab = EditorGUILayout.ObjectField("Third Person Prefab", script.thirdPersonPrefab, typeof(GameObject), false) as GameObject;
                    script.dropPrefab = EditorGUILayout.ObjectField("Drop Prefab", script.dropPrefab, typeof(GameObject), false) as GameObject;
                    script.runtimeDataPrefab = EditorGUILayout.ObjectField("Runtime Data Prefab", script.runtimeDataPrefab, typeof(GameObject), false) as GameObject;

                    if (script.firstPersonPrefab)
                    {
                        if (!script.firstPersonPrefab.GetComponent<Kit_MeleeRenderer>())
                        {
                            EditorGUILayout.HelpBox("First Person Prefab does not have the melee renderer assigned!", MessageType.Error);
                        }
                    }
                    if (script.thirdPersonPrefab)
                    {
                        if (!script.thirdPersonPrefab.GetComponent<Kit_ThirdPersonMeleeRenderer>())
                        {
                            EditorGUILayout.HelpBox("Third Person Prefab does not have the third person melee renderer assigned!", MessageType.Error);
                        }
                    }
                    if (script.dropPrefab)
                    {
                        if (!script.dropPrefab.GetComponent<Kit_DropRenderer>())
                        {
                            EditorGUILayout.HelpBox("Drop Prefab does not have drop renderer assigned!", MessageType.Error);
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
                settingsFoldout = EditorGUILayout.Foldout(settingsFoldout, "Settings");
                if (settingsFoldout)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    script.weaponName = EditorGUILayout.TextField("Weapon Name", script.weaponName);
                    script.weaponPicture = EditorGUILayout.ObjectField("Weapon Picture", script.weaponPicture, typeof(Sprite), false) as Sprite;
                    script.weaponHudPicture = EditorGUILayout.ObjectField("Weapon HUD Picture", script.weaponHudPicture, typeof(Sprite), false) as Sprite;
                    script.weaponQuickUsePicture = EditorGUILayout.ObjectField("Weapon HUD (Quick Use) Picture", script.weaponQuickUsePicture, typeof(Sprite), false) as Sprite;
                    script.weaponKillfeedImage = EditorGUILayout.ObjectField("Killfeed image", script.weaponKillfeedImage, typeof(Sprite), false) as Sprite;
                    //script.weaponType = (WeaponType)EditorGUILayout.EnumPopup("Weapon Type", script.weaponType); //OLD
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(slotsProperty, true);
                    if (EditorGUI.EndChangeCheck())
                        serializedObject.ApplyModifiedProperties();

                    if (information)
                    {
                        if (information.allWeaponCategories.Length > 0)
                        {
                            List<string> displayedList = new List<string>();
                            for (int i = 0; i < information.allWeaponCategories.Length; i++)
                            {
                                displayedList.Add(information.allWeaponCategories[i]);
                            }
                            int currentIndex = displayedList.IndexOf(script.weaponType);
                            if (currentIndex < 0) currentIndex = 0;
                            string[] toDisplay = displayedList.ToArray();
                            int index = EditorGUILayout.Popup("Weapon Type", currentIndex, toDisplay);
                            script.weaponType = toDisplay[index];
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("Add some weapon types! to the game information file!", MessageType.Error);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Game file not found in resources", MessageType.Warning);
                        script.weaponType = EditorGUILayout.TextField("Weapon Type", script.weaponType);
                    }

                    script.weaponLoadoutSubCategory = EditorGUILayout.TextField("Loadout Weapon Subcategory", script.weaponLoadoutSubCategory);

                    if (information && information.leveling)
                    {
                        script.levelToUnlockAt = EditorGUILayout.IntSlider(script.levelToUnlockAt, -1, information.leveling.GetMaxLevel());
                        script.unlockImage = EditorGUILayout.ObjectField("Unlock Picture", script.unlockImage, typeof(Sprite), false) as Sprite;
                    }
                    else
                    {
                        script.levelToUnlockAt = EditorGUILayout.IntField("Level to unlock at", script.levelToUnlockAt);
                        script.unlockImage = EditorGUILayout.ObjectField("Unlock Picture", script.unlockImage, typeof(Sprite), false) as Sprite;
                    }
                    script.drawTime = EditorGUILayout.FloatField("Draw Time", script.drawTime);
                    script.drawSound = EditorGUILayout.ObjectField("Draw Sound", script.drawSound, typeof(AudioClip), false) as AudioClip;
                    script.putawayTime = EditorGUILayout.FloatField("Putaway Time", script.putawayTime);
                    script.putawaySound = EditorGUILayout.ObjectField("Draw Sound", script.putawaySound, typeof(AudioClip), false) as AudioClip;
                    script.deathSoundCategory = EditorGUILayout.IntField("Death Sound Category", script.deathSoundCategory);
                    if (information)
                    {
                        if (information.allAnimatorAnimationSets.Length > 0)
                        {
                            List<string> displayedList = new List<string>();
                            for (int i = 0; i < information.allAnimatorAnimationSets.Length; i++)
                            {
                                displayedList.Add(information.allAnimatorAnimationSets[i].prefix);
                            }
                            int currentIndex = displayedList.IndexOf(script.thirdPersonAnimType);
                            if (currentIndex < 0) currentIndex = 0;
                            string[] toDisplay = displayedList.ToArray();
                            int index = EditorGUILayout.Popup("Third Person Animation Type", currentIndex, toDisplay);
                            script.thirdPersonAnimType = toDisplay[index];
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("Add some animator sets to the game information file!", MessageType.Error);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Game file not found in resources", MessageType.Warning);
                        script.thirdPersonAnimType = EditorGUILayout.TextField("Third Person Animation Type", script.thirdPersonAnimType);
                    }

                    script.voiceMeleeSoundID = EditorGUILayout.IntField("Voice Sound ID", script.voiceMeleeSoundID);
                    EditorGUILayout.EndVertical();
                }
                attackSettingsFoldout = EditorGUILayout.Foldout(attackSettingsFoldout, "Attack Settings");
                if (attackSettingsFoldout)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    script.primaryAttack = (AttackType)EditorGUILayout.EnumPopup("Primary Attack (LMB)", script.primaryAttack);

                    if (script.primaryAttack != AttackType.None)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                        if (script.primaryAttack == AttackType.Stab)
                        {
                            EditorGUILayout.LabelField("Stab", EditorStyles.boldLabel);
                            script.primaryAttackSettings.stabDamage = EditorGUILayout.FloatField("Damage", script.primaryAttackSettings.stabDamage);
                            script.primaryAttackSettings.stabPenetrationPower = EditorGUILayout.IntField("Penetration Power", script.primaryAttackSettings.stabPenetrationPower);
                            script.primaryAttackSettings.stabReach = EditorGUILayout.FloatField("Reach", script.primaryAttackSettings.stabReach);
                            script.primaryAttackSettings.stabHalfExtents = EditorGUILayout.Vector3Field("Half Extents", script.primaryAttackSettings.stabHalfExtents);
                            script.primaryAttackSettings.stabRagdollForce = EditorGUILayout.FloatField("Ragdoll Force", script.primaryAttackSettings.stabRagdollForce);

                            EditorGUILayout.LabelField("Windup", EditorStyles.boldLabel);
                            script.primaryAttackSettings.stabWindupAnimationName = EditorGUILayout.TextField("Windup Animation Name", script.primaryAttackSettings.stabWindupAnimationName);
                            script.primaryAttackSettings.stabWindupSound = EditorGUILayout.ObjectField("Windup Sound", script.primaryAttackSettings.stabWindupSound, typeof(AudioClip), false) as AudioClip;
                            script.primaryAttackSettings.stabWindupTime = EditorGUILayout.FloatField("Windup Time", script.primaryAttackSettings.stabWindupTime);

                            EditorGUILayout.LabelField("Hit Player", EditorStyles.boldLabel);
                            script.primaryAttackSettings.stabAnimationHitName = EditorGUILayout.TextField("Hit Player Animation Name", script.primaryAttackSettings.stabAnimationHitName);
                            script.primaryAttackSettings.stabHitSound = EditorGUILayout.ObjectField("Hit Player Sound", script.primaryAttackSettings.stabHitSound, typeof(AudioClip), false) as AudioClip;
                            script.primaryAttackSettings.stabHitTime = EditorGUILayout.FloatField("Hit Player Time", script.primaryAttackSettings.stabHitTime);

                            EditorGUILayout.LabelField("Hit Object", EditorStyles.boldLabel);
                            script.primaryAttackSettings.stabAnimationHitObjectName = EditorGUILayout.TextField("Hit Object Animation Name", script.primaryAttackSettings.stabAnimationHitObjectName);
                            script.primaryAttackSettings.stabHitObjectSound = EditorGUILayout.ObjectField("Hit Object Sound", script.primaryAttackSettings.stabHitObjectSound, typeof(AudioClip), false) as AudioClip;
                            script.primaryAttackSettings.stabHitObjectTime = EditorGUILayout.FloatField("Hit Object Time", script.primaryAttackSettings.stabHitObjectTime);

                            EditorGUILayout.LabelField("Miss", EditorStyles.boldLabel);
                            script.primaryAttackSettings.stabAnimationMissName = EditorGUILayout.TextField("Miss Animation Name", script.primaryAttackSettings.stabAnimationMissName);
                            script.primaryAttackSettings.stabMissSound = EditorGUILayout.ObjectField("Miss Sound", script.primaryAttackSettings.stabMissSound, typeof(AudioClip), false) as AudioClip;
                            script.primaryAttackSettings.stabMissTime = EditorGUILayout.FloatField("Miss Time", script.primaryAttackSettings.stabMissTime);
                        }
                        else if (script.primaryAttack == AttackType.Charge)
                        {
                            EditorGUILayout.LabelField("Charge", EditorStyles.boldLabel);
                            script.primaryAttackSettings.chargeDamageStart = EditorGUILayout.FloatField("Damage (at start)", script.primaryAttackSettings.chargeDamageStart);
                            script.primaryAttackSettings.chargeDamageCharged = EditorGUILayout.FloatField("Damage (fully charged)", script.primaryAttackSettings.chargeDamageCharged);
                            script.primaryAttackSettings.chargePenetrationPower = EditorGUILayout.IntField("Penetration Power", script.primaryAttackSettings.chargePenetrationPower);
                            script.primaryAttackSettings.chargeReach = EditorGUILayout.FloatField("Reach", script.primaryAttackSettings.chargeReach);
                            script.primaryAttackSettings.chargeHalfExtents = EditorGUILayout.Vector3Field("Half Extents", script.primaryAttackSettings.chargeHalfExtents);
                            script.primaryAttackSettings.chargeRagdollForce = EditorGUILayout.FloatField("Ragdoll Force", script.primaryAttackSettings.chargeRagdollForce);

                            EditorGUILayout.LabelField("Charge", EditorStyles.boldLabel);
                            script.primaryAttackSettings.chargeChargeAnimation = EditorGUILayout.TextField("Charge Animation Name", script.primaryAttackSettings.chargeChargeAnimation);
                            script.primaryAttackSettings.chargeChargeSound = EditorGUILayout.ObjectField("Charge Sound", script.primaryAttackSettings.chargeChargeSound, typeof(AudioClip), false) as AudioClip;
                            script.primaryAttackSettings.chargeChargeTime = EditorGUILayout.FloatField("Charge Time", script.primaryAttackSettings.chargeChargeTime);

                            EditorGUILayout.LabelField("Windup", EditorStyles.boldLabel);
                            script.primaryAttackSettings.chargeWindupAnimationName = EditorGUILayout.TextField("Windup Animation Name", script.primaryAttackSettings.chargeWindupAnimationName);
                            script.primaryAttackSettings.chargeWindupSound = EditorGUILayout.ObjectField("Windup Sound", script.primaryAttackSettings.chargeWindupSound, typeof(AudioClip), false) as AudioClip;
                            script.primaryAttackSettings.chargeWindupTime = EditorGUILayout.FloatField("Windup Time", script.primaryAttackSettings.chargeWindupTime);

                            EditorGUILayout.LabelField("Hit Player", EditorStyles.boldLabel);
                            script.primaryAttackSettings.chargeAnimationHitName = EditorGUILayout.TextField("Hit Player Animation Name", script.primaryAttackSettings.chargeAnimationHitName);
                            script.primaryAttackSettings.chargeHitSound = EditorGUILayout.ObjectField("Hit Player Sound", script.primaryAttackSettings.chargeHitSound, typeof(AudioClip), false) as AudioClip;
                            script.primaryAttackSettings.chargeHitTime = EditorGUILayout.FloatField("Hit Player Time", script.primaryAttackSettings.chargeHitTime);

                            EditorGUILayout.LabelField("Hit Object", EditorStyles.boldLabel);
                            script.primaryAttackSettings.chargeAnimationHitObjectName = EditorGUILayout.TextField("Hit Object Animation Name", script.primaryAttackSettings.chargeAnimationHitObjectName);
                            script.primaryAttackSettings.chargeHitObjectSound = EditorGUILayout.ObjectField("Hit Object Sound", script.primaryAttackSettings.chargeHitObjectSound, typeof(AudioClip), false) as AudioClip;
                            script.primaryAttackSettings.chargeHitObjectTime = EditorGUILayout.FloatField("Hit Object Time", script.primaryAttackSettings.chargeHitObjectTime);

                            EditorGUILayout.LabelField("Miss", EditorStyles.boldLabel);
                            script.primaryAttackSettings.chargeAnimationMissName = EditorGUILayout.TextField("Miss Animation Name", script.primaryAttackSettings.chargeAnimationMissName);
                            script.primaryAttackSettings.chargeMissSound = EditorGUILayout.ObjectField("Miss Sound", script.primaryAttackSettings.chargeMissSound, typeof(AudioClip), false) as AudioClip;
                            script.primaryAttackSettings.chargeMissTime = EditorGUILayout.FloatField("Miss Time", script.primaryAttackSettings.chargeMissTime);
                        }
                        else if (script.primaryAttack == AttackType.Heal)
                        {
                            EditorGUILayout.LabelField("Heal", EditorStyles.boldLabel);
                            script.primaryAttackSettings.healTimeOne = EditorGUILayout.FloatField("Heal Time until heal (1)", script.primaryAttackSettings.healTimeOne);
                            script.primaryAttackSettings.healTimeTwo = EditorGUILayout.FloatField("Heal Time after heal (2)", script.primaryAttackSettings.healTimeTwo);
                            script.primaryAttackSettings.healAmount = EditorGUILayout.FloatField("Heal Amount", script.primaryAttackSettings.healAmount);
                            script.primaryAttackSettings.healStartAmount = EditorGUILayout.IntField("Heal Amount at start", script.primaryAttackSettings.healStartAmount);
                            script.primaryAttackSettings.healAnimationName = EditorGUILayout.TextField("Heal Animation Name", script.primaryAttackSettings.healAnimationName);
                            script.primaryAttackSettings.healSound = EditorGUILayout.ObjectField("Heal Sound", script.primaryAttackSettings.healSound, typeof(AudioClip), false) as AudioClip;
                        }
                        else if (script.primaryAttack == AttackType.Combo)
                        {
                            if (script.primaryAttackSettings.combos.Length == 0)
                            {
                                if (GUILayout.Button("Create new combo"))
                                {
                                    ComboAttack ca = new ComboAttack();
                                    List<ComboAttack> attacks = script.primaryAttackSettings.combos.ToList();
                                    attacks.Insert(0, ca);
                                    script.primaryAttackSettings.combos = attacks.ToArray();
                                }
                            }

                            for (int i = 0; i < script.primaryAttackSettings.combos.Length; i++)
                            {
                                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                                if (GUILayout.Button("Add new combo before"))
                                {
                                    ComboAttack ca = new ComboAttack();
                                    List<ComboAttack> attacks = script.primaryAttackSettings.combos.ToList();
                                    attacks.Insert(i, ca);
                                    script.primaryAttackSettings.combos = attacks.ToArray();
                                }

                                EditorGUILayout.LabelField("Combo #" + (i + 1).ToString(), EditorStyles.boldLabel);
                                script.primaryAttackSettings.combos[i].comboDamage = EditorGUILayout.FloatField("Damage", script.primaryAttackSettings.combos[i].comboDamage);
                                script.primaryAttackSettings.combos[i].comboPenetrationPower = EditorGUILayout.IntField("Penetration Power", script.primaryAttackSettings.combos[i].comboPenetrationPower);
                                script.primaryAttackSettings.combos[i].comboReach = EditorGUILayout.FloatField("Reach", script.primaryAttackSettings.combos[i].comboReach);
                                script.primaryAttackSettings.combos[i].comboHalfExtents = EditorGUILayout.Vector3Field("Half Extents", script.primaryAttackSettings.combos[i].comboHalfExtents);
                                script.primaryAttackSettings.combos[i].comboRagdollForce = EditorGUILayout.FloatField("Ragdoll Force", script.primaryAttackSettings.combos[i].comboRagdollForce);

                                EditorGUILayout.LabelField("Windup", EditorStyles.boldLabel);
                                script.primaryAttackSettings.combos[i].comboWindupAnimationName = EditorGUILayout.TextField("Windup Animation Name", script.primaryAttackSettings.combos[i].comboWindupAnimationName);
                                script.primaryAttackSettings.combos[i].comboWindupSound = EditorGUILayout.ObjectField("Windup Sound", script.primaryAttackSettings.combos[i].comboWindupSound, typeof(AudioClip), false) as AudioClip;
                                script.primaryAttackSettings.combos[i].comboWindupTime = EditorGUILayout.FloatField("Windup Time", script.primaryAttackSettings.combos[i].comboWindupTime);

                                EditorGUILayout.LabelField("Hit Player", EditorStyles.boldLabel);
                                script.primaryAttackSettings.combos[i].comboAnimationHitName = EditorGUILayout.TextField("Hit Player Animation Name", script.primaryAttackSettings.combos[i].comboAnimationHitName);
                                script.primaryAttackSettings.combos[i].comboHitSound = EditorGUILayout.ObjectField("Hit Player Sound", script.primaryAttackSettings.combos[i].comboHitSound, typeof(AudioClip), false) as AudioClip;
                                script.primaryAttackSettings.combos[i].comboHitTime = EditorGUILayout.FloatField("Hit Player Time", script.primaryAttackSettings.combos[i].comboHitTime);

                                EditorGUILayout.LabelField("Hit Object", EditorStyles.boldLabel);
                                script.primaryAttackSettings.combos[i].comboAnimationHitObjectName = EditorGUILayout.TextField("Hit Object Animation Name", script.primaryAttackSettings.combos[i].comboAnimationHitObjectName);
                                script.primaryAttackSettings.combos[i].comboHitObjectSound = EditorGUILayout.ObjectField("Hit Object Sound", script.primaryAttackSettings.combos[i].comboHitObjectSound, typeof(AudioClip), false) as AudioClip;
                                script.primaryAttackSettings.combos[i].comboHitObjectTime = EditorGUILayout.FloatField("Hit Object Time", script.primaryAttackSettings.combos[i].comboHitObjectTime);

                                EditorGUILayout.LabelField("Miss", EditorStyles.boldLabel);
                                script.primaryAttackSettings.combos[i].comboAnimationMissName = EditorGUILayout.TextField("Miss Animation Name", script.primaryAttackSettings.combos[i].comboAnimationMissName);
                                script.primaryAttackSettings.combos[i].comboMissSound = EditorGUILayout.ObjectField("Miss Sound", script.primaryAttackSettings.combos[i].comboMissSound, typeof(AudioClip), false) as AudioClip;
                                script.primaryAttackSettings.combos[i].comboMissTime = EditorGUILayout.FloatField("Miss Time", script.primaryAttackSettings.combos[i].comboMissTime);

                                EditorGUILayout.LabelField("Progress to next combo", EditorStyles.boldLabel);
                                script.primaryAttackSettings.combos[i].comboTimeForNextCombo = EditorGUILayout.FloatField("Time for next combo", script.primaryAttackSettings.combos[i].comboTimeForNextCombo);
                                script.primaryAttackSettings.combos[i].canAchieveNextComboOnMiss = EditorGUILayout.Toggle("Can achieve next combo on miss?", script.primaryAttackSettings.combos[i].canAchieveNextComboOnMiss);
                                script.primaryAttackSettings.combos[i].canAchieveNextComboOnHit = EditorGUILayout.Toggle("Can achieve next combo on hit?", script.primaryAttackSettings.combos[i].canAchieveNextComboOnHit);
                                script.primaryAttackSettings.combos[i].canAchieveNextComboOnHitObject = EditorGUILayout.Toggle("Can achieve next combo on hit object?", script.primaryAttackSettings.combos[i].canAchieveNextComboOnHitObject);

                                if (GUILayout.Button("Remove combo"))
                                {
                                    List<ComboAttack> attacks = script.primaryAttackSettings.combos.ToList();
                                    attacks.RemoveAt(i);
                                    script.primaryAttackSettings.combos = attacks.ToArray();
                                }
                                else if (GUILayout.Button("Add new combo after"))
                                {
                                    ComboAttack ca = new ComboAttack();
                                    List<ComboAttack> attacks = script.primaryAttackSettings.combos.ToList();
                                    attacks.Insert(i + 1, ca);
                                    script.primaryAttackSettings.combos = attacks.ToArray();
                                }

                                EditorGUILayout.EndVertical();
                            }
                        }

                        EditorGUILayout.EndVertical();
                    }

                    script.secondaryAttack = (AttackType)EditorGUILayout.EnumPopup("Secondary Attack (RMB)", script.secondaryAttack);

                    if (script.secondaryAttack != AttackType.None)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                        if (script.secondaryAttack == AttackType.Stab)
                        {
                            EditorGUILayout.LabelField("Stab", EditorStyles.boldLabel);
                            script.secondaryAttackSettings.stabDamage = EditorGUILayout.FloatField("Damage", script.secondaryAttackSettings.stabDamage);
                            script.secondaryAttackSettings.stabPenetrationPower = EditorGUILayout.IntField("Penetration Power", script.secondaryAttackSettings.stabPenetrationPower);
                            script.secondaryAttackSettings.stabReach = EditorGUILayout.FloatField("Reach", script.secondaryAttackSettings.stabReach);
                            script.secondaryAttackSettings.stabHalfExtents = EditorGUILayout.Vector3Field("Half Extents", script.secondaryAttackSettings.stabHalfExtents);
                            script.secondaryAttackSettings.stabRagdollForce = EditorGUILayout.FloatField("Ragdoll Force", script.secondaryAttackSettings.stabRagdollForce);

                            EditorGUILayout.LabelField("Windup", EditorStyles.boldLabel);
                            script.secondaryAttackSettings.stabWindupAnimationName = EditorGUILayout.TextField("Windup Animation Name", script.secondaryAttackSettings.stabWindupAnimationName);
                            script.secondaryAttackSettings.stabWindupSound = EditorGUILayout.ObjectField("Windup Sound", script.secondaryAttackSettings.stabWindupSound, typeof(AudioClip), false) as AudioClip;
                            script.secondaryAttackSettings.stabWindupTime = EditorGUILayout.FloatField("Windup Time", script.secondaryAttackSettings.stabWindupTime);

                            EditorGUILayout.LabelField("Hit Player", EditorStyles.boldLabel);
                            script.secondaryAttackSettings.stabAnimationHitName = EditorGUILayout.TextField("Hit Player Animation Name", script.secondaryAttackSettings.stabAnimationHitName);
                            script.secondaryAttackSettings.stabHitSound = EditorGUILayout.ObjectField("Hit Player Sound", script.secondaryAttackSettings.stabHitSound, typeof(AudioClip), false) as AudioClip;
                            script.secondaryAttackSettings.stabHitTime = EditorGUILayout.FloatField("Hit Player Time", script.secondaryAttackSettings.stabHitTime);

                            EditorGUILayout.LabelField("Hit Object", EditorStyles.boldLabel);
                            script.secondaryAttackSettings.stabAnimationHitObjectName = EditorGUILayout.TextField("Hit Object Animation Name", script.secondaryAttackSettings.stabAnimationHitObjectName);
                            script.secondaryAttackSettings.stabHitObjectSound = EditorGUILayout.ObjectField("Hit Object Sound", script.secondaryAttackSettings.stabHitObjectSound, typeof(AudioClip), false) as AudioClip;
                            script.secondaryAttackSettings.stabHitObjectTime = EditorGUILayout.FloatField("Hit Object Time", script.secondaryAttackSettings.stabHitObjectTime);

                            EditorGUILayout.LabelField("Miss", EditorStyles.boldLabel);
                            script.secondaryAttackSettings.stabAnimationMissName = EditorGUILayout.TextField("Miss Animation Name", script.secondaryAttackSettings.stabAnimationMissName);
                            script.secondaryAttackSettings.stabMissSound = EditorGUILayout.ObjectField("Miss Sound", script.secondaryAttackSettings.stabMissSound, typeof(AudioClip), false) as AudioClip;
                            script.secondaryAttackSettings.stabMissTime = EditorGUILayout.FloatField("Miss Time", script.secondaryAttackSettings.stabMissTime);
                        }
                        else if (script.secondaryAttack == AttackType.Charge)
                        {
                            EditorGUILayout.LabelField("Charge", EditorStyles.boldLabel);
                            script.secondaryAttackSettings.chargeDamageStart = EditorGUILayout.FloatField("Damage (at start)", script.secondaryAttackSettings.chargeDamageStart);
                            script.secondaryAttackSettings.chargeDamageCharged = EditorGUILayout.FloatField("Damage (fully charged)", script.secondaryAttackSettings.chargeDamageCharged);
                            script.secondaryAttackSettings.chargePenetrationPower = EditorGUILayout.IntField("Penetration Power", script.secondaryAttackSettings.chargePenetrationPower);
                            script.secondaryAttackSettings.chargeReach = EditorGUILayout.FloatField("Reach", script.secondaryAttackSettings.chargeReach);
                            script.secondaryAttackSettings.chargeHalfExtents = EditorGUILayout.Vector3Field("Half Extents", script.secondaryAttackSettings.chargeHalfExtents);
                            script.secondaryAttackSettings.chargeRagdollForce = EditorGUILayout.FloatField("Ragdoll Force", script.secondaryAttackSettings.chargeRagdollForce);

                            EditorGUILayout.LabelField("Charge", EditorStyles.boldLabel);
                            script.secondaryAttackSettings.chargeChargeAnimation = EditorGUILayout.TextField("Charge Animation Name", script.secondaryAttackSettings.chargeChargeAnimation);
                            script.secondaryAttackSettings.chargeChargeSound = EditorGUILayout.ObjectField("Charge Sound", script.secondaryAttackSettings.chargeChargeSound, typeof(AudioClip), false) as AudioClip;
                            script.secondaryAttackSettings.chargeChargeTime = EditorGUILayout.FloatField("Charge Time", script.secondaryAttackSettings.chargeChargeTime);

                            EditorGUILayout.LabelField("Windup", EditorStyles.boldLabel);
                            script.secondaryAttackSettings.chargeWindupAnimationName = EditorGUILayout.TextField("Windup Animation Name", script.secondaryAttackSettings.chargeWindupAnimationName);
                            script.secondaryAttackSettings.chargeWindupSound = EditorGUILayout.ObjectField("Windup Sound", script.secondaryAttackSettings.chargeWindupSound, typeof(AudioClip), false) as AudioClip;
                            script.secondaryAttackSettings.chargeWindupTime = EditorGUILayout.FloatField("Windup Time", script.secondaryAttackSettings.chargeWindupTime);

                            EditorGUILayout.LabelField("Hit Player", EditorStyles.boldLabel);
                            script.secondaryAttackSettings.chargeAnimationHitName = EditorGUILayout.TextField("Hit Player Animation Name", script.secondaryAttackSettings.chargeAnimationHitName);
                            script.secondaryAttackSettings.chargeHitSound = EditorGUILayout.ObjectField("Hit Player Sound", script.secondaryAttackSettings.chargeHitSound, typeof(AudioClip), false) as AudioClip;
                            script.secondaryAttackSettings.chargeHitTime = EditorGUILayout.FloatField("Hit Player Time", script.secondaryAttackSettings.chargeHitTime);

                            EditorGUILayout.LabelField("Hit Object", EditorStyles.boldLabel);
                            script.secondaryAttackSettings.chargeAnimationHitObjectName = EditorGUILayout.TextField("Hit Object Animation Name", script.secondaryAttackSettings.chargeAnimationHitObjectName);
                            script.secondaryAttackSettings.chargeHitObjectSound = EditorGUILayout.ObjectField("Hit Object Sound", script.secondaryAttackSettings.chargeHitObjectSound, typeof(AudioClip), false) as AudioClip;
                            script.secondaryAttackSettings.chargeHitObjectTime = EditorGUILayout.FloatField("Hit Object Time", script.secondaryAttackSettings.chargeHitObjectTime);

                            EditorGUILayout.LabelField("Miss", EditorStyles.boldLabel);
                            script.secondaryAttackSettings.chargeAnimationMissName = EditorGUILayout.TextField("Miss Animation Name", script.secondaryAttackSettings.chargeAnimationMissName);
                            script.secondaryAttackSettings.chargeMissSound = EditorGUILayout.ObjectField("Miss Sound", script.secondaryAttackSettings.chargeMissSound, typeof(AudioClip), false) as AudioClip;
                            script.secondaryAttackSettings.chargeMissTime = EditorGUILayout.FloatField("Miss Time", script.secondaryAttackSettings.chargeMissTime);
                        }
                        else if (script.secondaryAttack == AttackType.Heal)
                        {
                            EditorGUILayout.LabelField("Heal", EditorStyles.boldLabel);
                            script.secondaryAttackSettings.healTimeOne = EditorGUILayout.FloatField("Heal Time until heal (1)", script.secondaryAttackSettings.healTimeOne);
                            script.secondaryAttackSettings.healTimeTwo = EditorGUILayout.FloatField("Heal Time after heal (2)", script.secondaryAttackSettings.healTimeTwo);
                            script.secondaryAttackSettings.healAmount = EditorGUILayout.FloatField("Heal Amount", script.secondaryAttackSettings.healAmount);
                            script.secondaryAttackSettings.healStartAmount = EditorGUILayout.IntField("Heal Amount at start", script.secondaryAttackSettings.healStartAmount);
                            script.secondaryAttackSettings.healAnimationName = EditorGUILayout.TextField("Heal Animation Name", script.secondaryAttackSettings.healAnimationName);
                            script.secondaryAttackSettings.healSound = EditorGUILayout.ObjectField("Heal Sound", script.secondaryAttackSettings.healSound, typeof(AudioClip), false) as AudioClip;
                        }
                        else if (script.secondaryAttack == AttackType.Combo)
                        {
                            if (script.secondaryAttackSettings.combos.Length == 0)
                            {
                                if (GUILayout.Button("Create new combo"))
                                {
                                    ComboAttack ca = new ComboAttack();
                                    List<ComboAttack> attacks = script.secondaryAttackSettings.combos.ToList();
                                    attacks.Insert(0, ca);
                                    script.secondaryAttackSettings.combos = attacks.ToArray();
                                }
                            }

                            for (int i = 0; i < script.secondaryAttackSettings.combos.Length; i++)
                            {
                                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                                if (GUILayout.Button("Add new combo before"))
                                {
                                    ComboAttack ca = new ComboAttack();
                                    List<ComboAttack> attacks = script.secondaryAttackSettings.combos.ToList();
                                    attacks.Insert(i, ca);
                                    script.secondaryAttackSettings.combos = attacks.ToArray();
                                }

                                EditorGUILayout.LabelField("Combo #" + (i + 1).ToString(), EditorStyles.boldLabel);
                                script.secondaryAttackSettings.combos[i].comboDamage = EditorGUILayout.FloatField("Damage", script.secondaryAttackSettings.combos[i].comboDamage);
                                script.secondaryAttackSettings.combos[i].comboPenetrationPower = EditorGUILayout.IntField("Penetration Power", script.secondaryAttackSettings.combos[i].comboPenetrationPower);
                                script.secondaryAttackSettings.combos[i].comboReach = EditorGUILayout.FloatField("Reach", script.secondaryAttackSettings.combos[i].comboReach);
                                script.secondaryAttackSettings.combos[i].comboHalfExtents = EditorGUILayout.Vector3Field("Half Extents", script.secondaryAttackSettings.combos[i].comboHalfExtents);
                                script.secondaryAttackSettings.combos[i].comboRagdollForce = EditorGUILayout.FloatField("Ragdoll Force", script.secondaryAttackSettings.combos[i].comboRagdollForce);

                                EditorGUILayout.LabelField("Windup", EditorStyles.boldLabel);
                                script.secondaryAttackSettings.combos[i].comboWindupAnimationName = EditorGUILayout.TextField("Windup Animation Name", script.secondaryAttackSettings.combos[i].comboWindupAnimationName);
                                script.secondaryAttackSettings.combos[i].comboWindupSound = EditorGUILayout.ObjectField("Windup Sound", script.secondaryAttackSettings.combos[i].comboWindupSound, typeof(AudioClip), false) as AudioClip;
                                script.secondaryAttackSettings.combos[i].comboWindupTime = EditorGUILayout.FloatField("Windup Time", script.secondaryAttackSettings.combos[i].comboWindupTime);

                                EditorGUILayout.LabelField("Hit Player", EditorStyles.boldLabel);
                                script.secondaryAttackSettings.combos[i].comboAnimationHitName = EditorGUILayout.TextField("Hit Player Animation Name", script.secondaryAttackSettings.combos[i].comboAnimationHitName);
                                script.secondaryAttackSettings.combos[i].comboHitSound = EditorGUILayout.ObjectField("Hit Player Sound", script.secondaryAttackSettings.combos[i].comboHitSound, typeof(AudioClip), false) as AudioClip;
                                script.secondaryAttackSettings.combos[i].comboHitTime = EditorGUILayout.FloatField("Hit Player Time", script.secondaryAttackSettings.combos[i].comboHitTime);

                                EditorGUILayout.LabelField("Hit Object", EditorStyles.boldLabel);
                                script.secondaryAttackSettings.combos[i].comboAnimationHitObjectName = EditorGUILayout.TextField("Hit Object Animation Name", script.secondaryAttackSettings.combos[i].comboAnimationHitObjectName);
                                script.secondaryAttackSettings.combos[i].comboHitObjectSound = EditorGUILayout.ObjectField("Hit Object Sound", script.secondaryAttackSettings.combos[i].comboHitObjectSound, typeof(AudioClip), false) as AudioClip;
                                script.secondaryAttackSettings.combos[i].comboHitObjectTime = EditorGUILayout.FloatField("Hit Object Time", script.secondaryAttackSettings.combos[i].comboHitObjectTime);

                                EditorGUILayout.LabelField("Miss", EditorStyles.boldLabel);
                                script.secondaryAttackSettings.combos[i].comboAnimationMissName = EditorGUILayout.TextField("Miss Animation Name", script.secondaryAttackSettings.combos[i].comboAnimationMissName);
                                script.secondaryAttackSettings.combos[i].comboMissSound = EditorGUILayout.ObjectField("Miss Sound", script.secondaryAttackSettings.combos[i].comboMissSound, typeof(AudioClip), false) as AudioClip;
                                script.secondaryAttackSettings.combos[i].comboMissTime = EditorGUILayout.FloatField("Miss Time", script.secondaryAttackSettings.combos[i].comboMissTime);

                                EditorGUILayout.LabelField("Progress to next combo", EditorStyles.boldLabel);
                                script.secondaryAttackSettings.combos[i].comboTimeForNextCombo = EditorGUILayout.FloatField("Time for next combo", script.secondaryAttackSettings.combos[i].comboTimeForNextCombo);
                                script.secondaryAttackSettings.combos[i].canAchieveNextComboOnMiss = EditorGUILayout.Toggle("Can achieve next combo on miss?", script.secondaryAttackSettings.combos[i].canAchieveNextComboOnMiss);
                                script.secondaryAttackSettings.combos[i].canAchieveNextComboOnHit = EditorGUILayout.Toggle("Can achieve next combo on hit?", script.secondaryAttackSettings.combos[i].canAchieveNextComboOnHit);
                                script.secondaryAttackSettings.combos[i].canAchieveNextComboOnHitObject = EditorGUILayout.Toggle("Can achieve next combo on hit object?", script.secondaryAttackSettings.combos[i].canAchieveNextComboOnHitObject);

                                if (GUILayout.Button("Remove combo"))
                                {
                                    List<ComboAttack> attacks = script.secondaryAttackSettings.combos.ToList();
                                    attacks.RemoveAt(i);
                                    script.secondaryAttackSettings.combos = attacks.ToArray();
                                }
                                else if (GUILayout.Button("Add new combo after"))
                                {
                                    ComboAttack ca = new ComboAttack();
                                    List<ComboAttack> attacks = script.secondaryAttackSettings.combos.ToList();
                                    attacks.Insert(i + 1, ca);
                                    script.secondaryAttackSettings.combos = attacks.ToArray();
                                }

                                EditorGUILayout.EndVertical();
                            }
                        }

                        EditorGUILayout.EndVertical();
                    }

                    script.quickAttack = (AttackType)EditorGUILayout.EnumPopup("Quick Attack (definied in Weapon Manager and Input Manager)", script.quickAttack);

                    if (script.quickAttack == AttackType.Combo)
                    {
                        Debug.LogWarning("Quick use cannot be combo.");
                        script.quickAttack = AttackType.None;
                    }

                    if (script.quickAttack != AttackType.None)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                        script.quickAttackSkipsPutaway = EditorGUILayout.Toggle("Skip putaway animation", script.quickAttackSkipsPutaway);

                        if (script.quickAttack == AttackType.Stab)
                        {
                            EditorGUILayout.LabelField("Stab", EditorStyles.boldLabel);
                            script.quickAttackSettings.stabDamage = EditorGUILayout.FloatField("Damage", script.quickAttackSettings.stabDamage);
                            script.quickAttackSettings.stabPenetrationPower = EditorGUILayout.IntField("Penetration Power", script.quickAttackSettings.stabPenetrationPower);
                            script.quickAttackSettings.stabReach = EditorGUILayout.FloatField("Reach", script.quickAttackSettings.stabReach);
                            script.quickAttackSettings.stabHalfExtents = EditorGUILayout.Vector3Field("Half Extents", script.quickAttackSettings.stabHalfExtents);
                            script.quickAttackSettings.stabRagdollForce = EditorGUILayout.FloatField("Ragdoll Force", script.quickAttackSettings.stabRagdollForce);

                            EditorGUILayout.LabelField("Windup", EditorStyles.boldLabel);
                            script.quickAttackSettings.stabWindupAnimationName = EditorGUILayout.TextField("Windup Animation Name", script.quickAttackSettings.stabWindupAnimationName);
                            script.quickAttackSettings.stabWindupSound = EditorGUILayout.ObjectField("Windup Sound", script.quickAttackSettings.stabWindupSound, typeof(AudioClip), false) as AudioClip;
                            script.quickAttackSettings.stabWindupTime = EditorGUILayout.FloatField("Windup Time", script.quickAttackSettings.stabWindupTime);

                            EditorGUILayout.LabelField("Hit Player", EditorStyles.boldLabel);
                            script.quickAttackSettings.stabAnimationHitName = EditorGUILayout.TextField("Hit Player Animation Name", script.quickAttackSettings.stabAnimationHitName);
                            script.quickAttackSettings.stabHitSound = EditorGUILayout.ObjectField("Hit Player Sound", script.quickAttackSettings.stabHitSound, typeof(AudioClip), false) as AudioClip;
                            script.quickAttackSettings.stabHitTime = EditorGUILayout.FloatField("Hit Player Time", script.quickAttackSettings.stabHitTime);

                            EditorGUILayout.LabelField("Hit Object", EditorStyles.boldLabel);
                            script.quickAttackSettings.stabAnimationHitObjectName = EditorGUILayout.TextField("Hit Object Animation Name", script.quickAttackSettings.stabAnimationHitObjectName);
                            script.quickAttackSettings.stabHitObjectSound = EditorGUILayout.ObjectField("Hit Object Sound", script.quickAttackSettings.stabHitObjectSound, typeof(AudioClip), false) as AudioClip;
                            script.quickAttackSettings.stabHitObjectTime = EditorGUILayout.FloatField("Hit Object Time", script.quickAttackSettings.stabHitObjectTime);

                            EditorGUILayout.LabelField("Miss", EditorStyles.boldLabel);
                            script.quickAttackSettings.stabAnimationMissName = EditorGUILayout.TextField("Miss Animation Name", script.quickAttackSettings.stabAnimationMissName);
                            script.quickAttackSettings.stabMissSound = EditorGUILayout.ObjectField("Miss Sound", script.quickAttackSettings.stabMissSound, typeof(AudioClip), false) as AudioClip;
                            script.quickAttackSettings.stabMissTime = EditorGUILayout.FloatField("Miss Time", script.quickAttackSettings.stabMissTime);
                        }
                        else if (script.quickAttack == AttackType.Charge)
                        {
                            EditorGUILayout.LabelField("Charge", EditorStyles.boldLabel);
                            script.quickAttackSettings.chargeDamageStart = EditorGUILayout.FloatField("Damage (at start)", script.quickAttackSettings.chargeDamageStart);
                            script.quickAttackSettings.chargeDamageCharged = EditorGUILayout.FloatField("Damage (fully charged)", script.quickAttackSettings.chargeDamageCharged);
                            script.quickAttackSettings.chargePenetrationPower = EditorGUILayout.IntField("Penetration Power", script.quickAttackSettings.chargePenetrationPower);
                            script.quickAttackSettings.chargeReach = EditorGUILayout.FloatField("Reach", script.quickAttackSettings.chargeReach);
                            script.quickAttackSettings.chargeHalfExtents = EditorGUILayout.Vector3Field("Half Extents", script.quickAttackSettings.chargeHalfExtents);
                            script.quickAttackSettings.chargeRagdollForce = EditorGUILayout.FloatField("Ragdoll Force", script.quickAttackSettings.chargeRagdollForce);

                            EditorGUILayout.LabelField("Charge", EditorStyles.boldLabel);
                            script.quickAttackSettings.chargeChargeAnimation = EditorGUILayout.TextField("Charge Animation Name", script.quickAttackSettings.chargeChargeAnimation);
                            script.quickAttackSettings.chargeChargeSound = EditorGUILayout.ObjectField("Charge Sound", script.quickAttackSettings.chargeChargeSound, typeof(AudioClip), false) as AudioClip;
                            script.quickAttackSettings.chargeChargeTime = EditorGUILayout.FloatField("Charge Time", script.quickAttackSettings.chargeChargeTime);

                            EditorGUILayout.LabelField("Windup", EditorStyles.boldLabel);
                            script.quickAttackSettings.chargeWindupAnimationName = EditorGUILayout.TextField("Windup Animation Name", script.quickAttackSettings.chargeWindupAnimationName);
                            script.quickAttackSettings.chargeWindupSound = EditorGUILayout.ObjectField("Windup Sound", script.quickAttackSettings.chargeWindupSound, typeof(AudioClip), false) as AudioClip;
                            script.quickAttackSettings.chargeWindupTime = EditorGUILayout.FloatField("Windup Time", script.quickAttackSettings.chargeWindupTime);

                            EditorGUILayout.LabelField("Hit Player", EditorStyles.boldLabel);
                            script.quickAttackSettings.chargeAnimationHitName = EditorGUILayout.TextField("Hit Player Animation Name", script.quickAttackSettings.chargeAnimationHitName);
                            script.quickAttackSettings.chargeHitSound = EditorGUILayout.ObjectField("Hit Player Sound", script.quickAttackSettings.chargeHitSound, typeof(AudioClip), false) as AudioClip;
                            script.quickAttackSettings.chargeHitTime = EditorGUILayout.FloatField("Hit Player Time", script.quickAttackSettings.chargeHitTime);

                            EditorGUILayout.LabelField("Hit Object", EditorStyles.boldLabel);
                            script.quickAttackSettings.chargeAnimationHitObjectName = EditorGUILayout.TextField("Hit Object Animation Name", script.quickAttackSettings.chargeAnimationHitObjectName);
                            script.quickAttackSettings.chargeHitObjectSound = EditorGUILayout.ObjectField("Hit Object Sound", script.quickAttackSettings.chargeHitObjectSound, typeof(AudioClip), false) as AudioClip;
                            script.quickAttackSettings.chargeHitObjectTime = EditorGUILayout.FloatField("Hit Object Time", script.quickAttackSettings.chargeHitObjectTime);

                            EditorGUILayout.LabelField("Miss", EditorStyles.boldLabel);
                            script.quickAttackSettings.chargeAnimationMissName = EditorGUILayout.TextField("Miss Animation Name", script.quickAttackSettings.chargeAnimationMissName);
                            script.quickAttackSettings.chargeMissSound = EditorGUILayout.ObjectField("Miss Sound", script.quickAttackSettings.chargeMissSound, typeof(AudioClip), false) as AudioClip;
                            script.quickAttackSettings.chargeMissTime = EditorGUILayout.FloatField("Miss Time", script.quickAttackSettings.chargeMissTime);
                        }
                        else if (script.quickAttack == AttackType.Heal)
                        {
                            EditorGUILayout.LabelField("Heal", EditorStyles.boldLabel);
                            script.quickAttackSettings.healTimeOne = EditorGUILayout.FloatField("Heal Time until heal (1)", script.quickAttackSettings.healTimeOne);
                            script.quickAttackSettings.healTimeTwo = EditorGUILayout.FloatField("Heal Time after heal (2)", script.quickAttackSettings.healTimeTwo);
                            script.quickAttackSettings.healAmount = EditorGUILayout.FloatField("Heal Amount", script.quickAttackSettings.healAmount);
                            script.quickAttackSettings.healStartAmount = EditorGUILayout.IntField("Heal Amount at start", script.quickAttackSettings.healStartAmount);
                            script.quickAttackSettings.healAnimationName = EditorGUILayout.TextField("Heal Animation Name", script.quickAttackSettings.healAnimationName);
                            script.quickAttackSettings.healSound = EditorGUILayout.ObjectField("Heal Sound", script.quickAttackSettings.healSound, typeof(AudioClip), false) as AudioClip;
                        }

                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.EndVertical();
                }
                springFoldout = EditorGUILayout.Foldout(springFoldout, "Spring Settings");
                if (springFoldout)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.HelpBox("These springs are used for procedural animations", MessageType.Info);
                    script.springPosConfig.damping = EditorGUILayout.Vector3Field("Positional Damping", script.springPosConfig.damping);
                    script.springPosConfig.stiffness = EditorGUILayout.Vector3Field("Positional Stiffness", script.springPosConfig.stiffness);
                    script.springPosConfig.lerpSpeed = EditorGUILayout.FloatField("Positional Lerp Speed", script.springPosConfig.lerpSpeed);

                    script.springRotConfig.damping = EditorGUILayout.Vector3Field("Rotational Damping", script.springRotConfig.damping);
                    script.springRotConfig.stiffness = EditorGUILayout.Vector3Field("Rotational Stiffness", script.springRotConfig.stiffness);
                    script.springRotConfig.lerpSpeed = EditorGUILayout.FloatField("Rotational Lerp Speed", script.springRotConfig.lerpSpeed);

                    EditorGUILayout.EndVertical();
                }
                weaponDelayFoldout = EditorGUILayout.Foldout(weaponDelayFoldout, "Weapon Delay");
                if (weaponDelayFoldout)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    script.weaponDelayBaseAmount = EditorGUILayout.FloatField("Base amount", script.weaponDelayBaseAmount);
                    script.weaponDelayMaxAmount = EditorGUILayout.FloatField("Max amount", script.weaponDelayMaxAmount);
                    script.weaponDelayAimingMultiplier = EditorGUILayout.FloatField("Aiming multiplier", script.weaponDelayAimingMultiplier);
                    script.weaponDelaySmooth = EditorGUILayout.FloatField("Smooth", script.weaponDelaySmooth);
                    EditorGUILayout.EndVertical();
                }
                weaponTiltFoldout = EditorGUILayout.Foldout(weaponTiltFoldout, "Weapon Tilt");
                if (weaponTiltFoldout)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    script.weaponTiltEnabled = EditorGUILayout.Toggle("Enabled", script.weaponTiltEnabled);
                    if (script.weaponTiltEnabled)
                    {
                        script.weaponTiltIntensity = EditorGUILayout.FloatField("Intensity", script.weaponTiltIntensity);
                        script.weaponTiltReturnSpeed = EditorGUILayout.FloatField("Return Speed", script.weaponTiltReturnSpeed);
                    }
                    EditorGUILayout.EndVertical();
                }
                weaponFallFoldout = EditorGUILayout.Foldout(weaponFallFoldout, "Weapon Fall");
                if (weaponFallFoldout)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    script.fallDownAmount = EditorGUILayout.FloatField("Amount", script.fallDownAmount);
                    script.fallDownMinOffset = EditorGUILayout.FloatField("Min Offset", script.fallDownMinOffset);
                    script.fallDownMaxoffset = EditorGUILayout.FloatField("Max Offset", script.fallDownMaxoffset);
                    script.fallDownTime = EditorGUILayout.FloatField("Time", script.fallDownTime);
                    script.fallDownReturnSpeed = EditorGUILayout.FloatField("Return speed", script.fallDownReturnSpeed);
                    EditorGUILayout.EndVertical();
                }
                genericAnimationsFoldout = EditorGUILayout.Foldout(genericAnimationsFoldout, "Generic Animations");
                if (genericAnimationsFoldout)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    script.genericGunAnimatorControllerPrefab = EditorGUILayout.ObjectField("Animator prefab", script.genericGunAnimatorControllerPrefab, typeof(GameObject), false) as GameObject;
                    script.useGenericWalkAnim = GUILayout.Toggle(script.useGenericWalkAnim, "Use generic walk animation");
                    script.useGenericRunAnim = GUILayout.Toggle(script.useGenericRunAnim, "Use generic run animation");
                    EditorGUILayout.EndVertical();
                }

                if (GUI.changed)
                {
                    EditorUtility.SetDirty(target);
                }
            }
        }
    }
}
