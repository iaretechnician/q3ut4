using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace MarsFPSKit
{
    namespace Weapons
    {
        [CustomEditor(typeof(Kit_ModernWeaponScript))]
        public class Kit_ModernWeaponScriptEditor : UnityEditor.Editor
        {
            static bool prefabsFoldout;
            static bool settingsFoldout;
            static bool attachmentsFoldout;
            static bool animationSettingsFoldout;
            static bool damageFoldout;
            static bool crosshairFoldout;
            static bool bulletFoldout;
            static bool bulletSpreadFoldout;
            static bool recoilFoldout;
            static bool reloadFoldout;
            static bool aimingFoldout;
            static bool weaponDelayFoldout;
            static bool weaponTiltFoldout;
            static bool weaponFallFoldout;
            static bool shellEjectionFoldout;
            static bool walkFoldout;
            static bool genericAnimationsFoldout;
            static bool springFoldout;

            Kit_GameInformation information = null;

            SerializedProperty slotsProperty;
            SerializedProperty attachmentsPropery;

            void OnEnable()
            {
                // Fetch the objects from the MyScript script to display in the inspector
                slotsProperty = serializedObject.FindProperty("canFitIntoSlots");
                attachmentsPropery = serializedObject.FindProperty("attachmentSlotsSpecific");
            }

            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                EditorGUI.BeginChangeCheck();
                Kit_ModernWeaponScript script = target as Kit_ModernWeaponScript;

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
                        if (!script.firstPersonPrefab.GetComponent<Kit_WeaponRenderer>())
                        {
                            EditorGUILayout.HelpBox("First Person Prefab does not have the weapon renderer assigned!", MessageType.Error);
                        }
                    }
                    if (script.thirdPersonPrefab)
                    {
                        if (!script.thirdPersonPrefab.GetComponent<Kit_ThirdPersonWeaponRenderer>())
                        {
                            EditorGUILayout.HelpBox("Third Person Prefab does not have the third person weapon renderer assigned!", MessageType.Error);
                        }
                    }
                    if (script.dropPrefab)
                    {
                        if (!script.dropPrefab.GetComponent<Kit_DropRenderer>())
                        {
                            EditorGUILayout.HelpBox("Drop Prefab does not have drop renderer assigned!", MessageType.Error);
                        }
                    }
                    if (script.firstPersonPrefab && script.thirdPersonPrefab && script.dropPrefab)
                    {
                        if (script.firstPersonPrefab.GetComponent<Kit_WeaponRenderer>() && script.thirdPersonPrefab.GetComponent<Kit_ThirdPersonWeaponRenderer>() && script.dropPrefab.GetComponent<Kit_DropRenderer>())
                        {
                            Kit_WeaponRenderer wr = script.firstPersonPrefab.GetComponent<Kit_WeaponRenderer>();
                            Kit_ThirdPersonWeaponRenderer wrtp = script.thirdPersonPrefab.GetComponent<Kit_ThirdPersonWeaponRenderer>();
                            Kit_DropRenderer wrd = script.dropPrefab.GetComponent<Kit_DropRenderer>();
                            if (wr.attachmentAndSkinSockets.Length == wrtp.attachmentAndSkinSockets.Length && wrtp.attachmentAndSkinSockets.Length == wrd.attachmentAndSkinSockets.Length) { }
                            else
                            {
                                EditorGUILayout.HelpBox("Attachment Slots on the prefabs do not match.", MessageType.Error);
                            }
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
                    script.weaponKillfeedImage = EditorGUILayout.ObjectField("Killfeed image", script.weaponKillfeedImage, typeof(Sprite), false) as Sprite;
                    //script.weaponType = (WeaponType)EditorGUILayout.EnumPopup("Weapon Type", script.weaponType); //OLD
                    EditorGUILayout.PropertyField(slotsProperty, true);

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
                        script.levelToUnlockAt = EditorGUILayout.IntSlider("Unlocks at level", script.levelToUnlockAt, -1, information.leveling.GetMaxLevel());
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
                    if (script.fireMode != FireMode.BoltAction)
                        script.RPM = EditorGUILayout.IntField("RPM", script.RPM);
                    if (script.fireMode == FireMode.Burst)
                    {
                        script.burstBulletsPerShot = EditorGUILayout.IntField("Shots per burst", script.burstBulletsPerShot);
                        script.burstTimeBetweenShots = EditorGUILayout.FloatField("Time between Burst Shots", script.burstTimeBetweenShots);
                    }
                    script.fireMode = (FireMode)EditorGUILayout.EnumPopup("Fire Mode", script.fireMode);
                    script.fireSound = EditorGUILayout.ObjectField("Fire sound", script.fireSound, typeof(AudioClip), false) as AudioClip;
                    script.fireSoundThirdPerson = EditorGUILayout.ObjectField("Fire sound Third Person", script.fireSoundThirdPerson, typeof(AudioClip), false) as AudioClip;
                    script.dryFireSound = EditorGUILayout.ObjectField("Dry Fire sound", script.dryFireSound, typeof(AudioClip), false) as AudioClip;
                    script.fireSoundThirdPersonMaxRange = EditorGUILayout.FloatField("Fire sound third person max range", script.fireSoundThirdPersonMaxRange);
                    script.fireSoundThirdPersonRolloff = EditorGUILayout.CurveField("Fire sound third person rolloff", script.fireSoundThirdPersonRolloff);
                    if (script.fireMode == FireMode.BoltAction)
                    {
                        script.boltActionTimeNormal = EditorGUILayout.FloatField("Bolt action time", script.boltActionTimeNormal);
                        script.boltActionDelayNormal = EditorGUILayout.FloatField("Bolt action sound delay", script.boltActionDelayNormal);
                        script.boltActionSoundNormal = EditorGUILayout.ObjectField("Bolt action sound", script.boltActionSoundNormal, typeof(AudioClip), false) as AudioClip;
                        script.boltActionTimeLast = EditorGUILayout.FloatField("Bolt action time last", script.boltActionTimeLast);
                        script.boltActionDelayLast = EditorGUILayout.FloatField("Bolt action sound last delay", script.boltActionDelayLast);
                        script.boltActionSoundLast = EditorGUILayout.ObjectField("Bolt action sound last", script.boltActionSoundLast, typeof(AudioClip), false) as AudioClip;
                        script.boltActionSoundThirdPersonMaxRange = EditorGUILayout.FloatField("Bolt action sound third person max range", script.boltActionSoundThirdPersonMaxRange);
                        script.boltActionSoundThirdPersonRolloff = EditorGUILayout.CurveField("Bolt action sound third person rolloff", script.boltActionSoundThirdPersonRolloff);
                    }
                    script.bulletsPerMag = EditorGUILayout.IntField("Bullets Per Mag", script.bulletsPerMag);
                    script.bulletsToReloadAtStart = EditorGUILayout.IntField("Bullets To Reload At Start", script.bulletsToReloadAtStart);
                    script.speedMultiplierBase = EditorGUILayout.FloatField("Speed Multiplier", script.speedMultiplierBase);
                    script.ragdollForce = EditorGUILayout.FloatField("Ragdoll Force", script.ragdollForce);
                    script.timeCannotFireAfterRun = EditorGUILayout.FloatField("Weapon unusable after run in seconds", script.timeCannotFireAfterRun);
                    EditorGUILayout.EndVertical();
                }
                attachmentsFoldout = EditorGUILayout.Foldout(attachmentsFoldout, "Attachments");
                if (attachmentsFoldout)
                {
                    if (!script.attachmentSlotOverride)
                    {
                        EditorGUILayout.PropertyField(attachmentsPropery, true);
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        EditorGUILayout.HelpBox("You can copy weapon specific attachments over from another weapon by dragging that weapon into this slot:", MessageType.Info);
                        Kit_ModernWeaponScript copyFrom = null;
                        copyFrom = EditorGUILayout.ObjectField("Copy Attachments from weapon", copyFrom, typeof(Kit_ModernWeaponScript), false) as Kit_ModernWeaponScript;

                        if (copyFrom)
                        {
                            Undo.RecordObject(target, "Attachment Copy");
                            script.attachmentSlotsSpecific = new AttachmentSlot[copyFrom.attachmentSlotsSpecific.Length];

                            for (int i = 0; i < script.attachmentSlotsSpecific.Length; i++)
                            {
                                int id = i;
                                script.attachmentSlotsSpecific[id] = copyFrom.attachmentSlotsSpecific[id].Clone();
                            }
                            copyFrom = null;
                        }
                        EditorGUILayout.EndVertical();
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("A universal override file is used", MessageType.Warning);
                    }

                    script.attachmentSlotOverride = EditorGUILayout.ObjectField("Attachments override", script.attachmentSlotOverride, typeof(Kit_AttachmentsUniversal), false) as Kit_AttachmentsUniversal;
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
                animationSettingsFoldout = EditorGUILayout.Foldout(animationSettingsFoldout, "Animator Settings");
                if (animationSettingsFoldout)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    script.setEmptyBool = GUILayout.Toggle(script.setEmptyBool, "Set Empty Bool");
                    script.setRunningBool = GUILayout.Toggle(script.setRunningBool, "Set Running Bool");
                    script.sendRunningEvents = GUILayout.Toggle(script.sendRunningEvents, "Send Running Events");
                    script.setAimingBool = GUILayout.Toggle(script.setAimingBool, "Set Aiming Bool");
                    script.setAimingProgress = GUILayout.Toggle(script.setAimingProgress, "Set Aiming float value");
                    script.setMovementDirection = GUILayout.Toggle(script.setMovementDirection, "Set movement direction");
                    EditorGUILayout.EndVertical();
                }
                damageFoldout = EditorGUILayout.Foldout(damageFoldout, "Damage");
                if (damageFoldout)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    script.baseDamage = EditorGUILayout.FloatField("Base Damage", script.baseDamage);
                    script.range = EditorGUILayout.FloatField("Max Range", script.range);
                    script.damageDropoff = EditorGUILayout.CurveField("Damage Dropoff", script.damageDropoff);
                    script.fireTypeMode = (FireTypeMode)EditorGUILayout.EnumPopup("Fire Type", script.fireTypeMode);
                    if (script.fireTypeMode == FireTypeMode.Pellets)
                    {
                        script.amountOfPellets = EditorGUILayout.IntField("Pellets", script.amountOfPellets);
                    }
                    EditorGUILayout.EndVertical();
                }
                crosshairFoldout = EditorGUILayout.Foldout(crosshairFoldout, "Crosshair");
                if (crosshairFoldout)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    script.crosshairEnabled = GUILayout.Toggle(script.crosshairEnabled, "Enabled");
                    if (script.crosshairEnabled)
                    {
                        script.crosshairSizeMultiplier = EditorGUILayout.FloatField("Size Multiplier", script.crosshairSizeMultiplier);
                    }
                    EditorGUILayout.EndVertical();
                }
                bulletFoldout = EditorGUILayout.Foldout(bulletFoldout, "Bullets");
                if (bulletFoldout)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    script.bulletsMode = (BulletMode)EditorGUILayout.EnumPopup("Bullet Mode", script.bulletsMode);
                    if (script.bulletsMode == BulletMode.Physical)
                    {
                        script.bulletPrefab = EditorGUILayout.ObjectField("Bullet Prefab", script.bulletPrefab, typeof(GameObject), false) as GameObject;
                        script.bulletSpeed = EditorGUILayout.FloatField("Bullet Speed (m/s)", script.bulletSpeed);
                        script.bulletHideForFrames = EditorGUILayout.IntField("Bullet Hide for frames after existance", script.bulletHideForFrames);
                        script.bulletGravityMultiplier = EditorGUILayout.FloatField("Bullet gravity modifier (current= " + (Physics.gravity.y * script.bulletGravityMultiplier).ToString() + " m/s²)", script.bulletGravityMultiplier);
                        script.bulletLifeTime = EditorGUILayout.FloatField("Bullet Life time (s)", script.bulletLifeTime);
                        script.bulletStaysAliveAfterDeath = EditorGUILayout.Toggle("Bullet Stays Alive after death", script.bulletStaysAliveAfterDeath);
                        if (script.bulletStaysAliveAfterDeath)
                        {
                            script.bulletStaysAliveAfterDeathTime = EditorGUILayout.FloatField("Time it stays alive after death", script.bulletStaysAliveAfterDeathTime);
                        }
                    }
                    script.bulletsPenetrationEnabled = EditorGUILayout.Toggle("Bullet Penetration Enabled", script.bulletsPenetrationEnabled);
                    if (script.bulletsPenetrationEnabled)
                    {
                        script.bulletsPenetrationForce = EditorGUILayout.IntField("Bullet Penetration Force", script.bulletsPenetrationForce);
                    }
                    EditorGUILayout.EndVertical();
                }
                bulletSpreadFoldout = EditorGUILayout.Foldout(bulletSpreadFoldout, "Bullet Spread");
                if (bulletSpreadFoldout)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    script.bulletSpreadMode = (SpreadMode)EditorGUILayout.EnumPopup("Spread Mode", script.bulletSpreadMode);

                    script.bulletSpreadHipBase = EditorGUILayout.FloatField("Hip base ", script.bulletSpreadHipBase);
                    script.bulletSpreadHipVelocityAdd = EditorGUILayout.FloatField("Hip velocity add max", script.bulletSpreadHipVelocityAdd);
                    script.bulletSpreadHipVelocityReference = EditorGUILayout.FloatField("Hip velocity reference speed", script.bulletSpreadHipVelocityReference);

                    script.bulletSpreadAimBase = EditorGUILayout.FloatField("Aim base ", script.bulletSpreadAimBase);
                    script.bulletSpreadAimVelocityAdd = EditorGUILayout.FloatField("Aim velocity add max", script.bulletSpreadAimVelocityAdd);
                    script.bulletSpreadAimVelocityReference = EditorGUILayout.FloatField("Aim velocity reference speed", script.bulletSpreadAimVelocityReference);

                    if (script.bulletSpreadMode == SpreadMode.SprayPattern)
                    {
                        SerializedProperty pattern = serializedObject.FindProperty("bulletSpreadSprayPattern");
                        EditorGUILayout.PropertyField(pattern, true);
                        script.bulletSpreadSprayPatternRecoverySpeed = EditorGUILayout.FloatField("Spray Pattern Recovery speed", script.bulletSpreadSprayPatternRecoverySpeed);
                    }
                    EditorGUILayout.EndVertical();
                }
                recoilFoldout = EditorGUILayout.Foldout(recoilFoldout, "Recoil");
                if (recoilFoldout)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    script.recoilPerShotMin = EditorGUILayout.Vector2Field("Min recoil", script.recoilPerShotMin);
                    script.recoilPerShotMax = EditorGUILayout.Vector2Field("Max recoil", script.recoilPerShotMax);
                    script.recoilApplyTime = EditorGUILayout.FloatField("Apply time", script.recoilApplyTime);
                    script.recoilReturnSpeed = EditorGUILayout.FloatField("Return speed", script.recoilReturnSpeed);
                    EditorGUILayout.EndVertical();
                }
                reloadFoldout = EditorGUILayout.Foldout(reloadFoldout, "Reloading");
                if (reloadFoldout)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    script.reloadMode = (ReloadMode)EditorGUILayout.EnumPopup("Reload mode", script.reloadMode);
                    if (script.reloadMode == ReloadMode.Simple || script.reloadMode == ReloadMode.Chambered || script.reloadMode == ReloadMode.FullEmpty)
                    {
                        GUILayout.Label("Reload normal");
                        script.reloadTimeOne = EditorGUILayout.FloatField("Reload time one", script.reloadTimeOne);
                        script.reloadTimeTwo = EditorGUILayout.FloatField("Reload time two", script.reloadTimeTwo);
                        script.reloadSound = EditorGUILayout.ObjectField("Reload sound", script.reloadSound, typeof(AudioClip), false) as AudioClip;
                    }
                    if (script.reloadMode == ReloadMode.FullEmpty || script.reloadMode == ReloadMode.Chambered)
                    {
                        GUILayout.Label("Reload Empty");
                        script.reloadEmptyTimeOne = EditorGUILayout.FloatField("Reload empty time one", script.reloadEmptyTimeOne);
                        script.reloadEmptyTimeTwo = EditorGUILayout.FloatField("Reload empty time two", script.reloadEmptyTimeTwo);
                        script.reloadSoundEmpty = EditorGUILayout.ObjectField("Reload sound empty", script.reloadSoundEmpty, typeof(AudioClip), false) as AudioClip;
                    }

                    if (script.reloadMode == ReloadMode.Procedural || script.reloadMode == ReloadMode.ProceduralChambered)
                    {
                        GUILayout.Label("Start");
                        script.reloadProceduralStartTime = EditorGUILayout.FloatField("Reload procedural start time", script.reloadProceduralStartTime);
                        script.reloadProceduralStartSound = EditorGUILayout.ObjectField("Reload procedural start sound", script.reloadProceduralStartSound, typeof(AudioClip), false) as AudioClip;
                        GUILayout.Label("Start Empty");
                        script.reloadProceduralAddBulletDuringStartEmpty = GUILayout.Toggle(script.reloadProceduralAddBulletDuringStartEmpty, "Reload procedural add bullet during empty start");
                        if (script.reloadProceduralAddBulletDuringStartEmpty)
                        {
                            script.reloadProceduralEmptyInsertStartTimeOne = EditorGUILayout.FloatField("Reload procedural empty time one", script.reloadProceduralEmptyInsertStartTimeOne);
                            script.reloadProceduralEmptyInsertStartTimeTwo = EditorGUILayout.FloatField("Reload procedural empty time two", script.reloadProceduralEmptyInsertStartTimeTwo);
                        }
                        else
                        {
                            script.reloadProceduralEmptyStartTime = EditorGUILayout.FloatField("Reload procedural empty time", script.reloadProceduralEmptyStartTime);
                        }
                        script.reloadProceduralStartEmptySound = EditorGUILayout.ObjectField("Reload procedural start empty sound", script.reloadProceduralStartEmptySound, typeof(AudioClip), false) as AudioClip;
                        GUILayout.Label("Insert");
                        script.reloadProceduralInsertTimeOne = EditorGUILayout.FloatField("Reload procedural insert time one", script.reloadProceduralInsertTimeOne);
                        script.reloadProceduralInsertTimeTwo = EditorGUILayout.FloatField("Reload procedural insert time two", script.reloadProceduralInsertTimeTwo);
                        script.reloadProceduralInsertSound = EditorGUILayout.ObjectField("Reload procedural insert sound", script.reloadProceduralInsertSound, typeof(AudioClip), false) as AudioClip;
                        GUILayout.Label("End");
                        script.reloadProceduralEndTime = EditorGUILayout.FloatField("Reload procedural end time", script.reloadProceduralEndTime);
                        script.reloadProceduralEndSound = EditorGUILayout.ObjectField("Reload procedural end sound", script.reloadProceduralEndSound, typeof(AudioClip), false) as AudioClip;
                    }

                    script.reloadSoundThirdPersonMaxRange = EditorGUILayout.FloatField("Reload sound third person max range", script.reloadSoundThirdPersonMaxRange);
                    script.reloadSoundThirdPersonRolloff = EditorGUILayout.CurveField("Reload sound third person rolloff", script.reloadSoundThirdPersonRolloff);
                    EditorGUILayout.EndVertical();
                }
                aimingFoldout = EditorGUILayout.Foldout(aimingFoldout, "Aiming");
                if (aimingFoldout)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    script.aimEnabled = EditorGUILayout.Toggle("Aim enabled", script.aimEnabled);
                    script.aimInTime = EditorGUILayout.FloatField("Aim in time", script.aimInTime);
                    script.aimOutTime = EditorGUILayout.FloatField("Aim out time", script.aimOutTime);
                    script.aimSpeedMultiplier = EditorGUILayout.FloatField("Aim speed in %", script.aimSpeedMultiplier);
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
                        script.weaponTiltEnabledWhileAiming = EditorGUILayout.Toggle("Enabled while aiming", script.weaponTiltEnabledWhileAiming);
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
                shellEjectionFoldout = EditorGUILayout.Foldout(shellEjectionFoldout, "Shell Ejection");
                if (shellEjectionFoldout)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    script.shellEjectionPrefab = EditorGUILayout.ObjectField("Shell prefab", script.shellEjectionPrefab, typeof(GameObject), false) as GameObject;
                    if (script.shellEjectionPrefab)
                    {
                        script.shellEjectionTime = Mathf.Clamp(EditorGUILayout.FloatField("Ejection delay", script.shellEjectionTime), 0f, float.MaxValue);
                        script.shellEjectionMinForce = EditorGUILayout.Vector3Field("Min force", script.shellEjectionMinForce);
                        script.shellEjectionMaxForce = EditorGUILayout.Vector3Field("Max force", script.shellEjectionMaxForce);
                        script.shellEjectionMinTorque = EditorGUILayout.Vector3Field("Min torque", script.shellEjectionMinTorque);
                        script.shellEjectionMaxTorque = EditorGUILayout.Vector3Field("Max torque", script.shellEjectionMaxTorque);
                    }
                    EditorGUILayout.EndVertical();
                }
                walkFoldout = EditorGUILayout.Foldout(walkFoldout, "Walk Animation");
                if (walkFoldout)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    script.enableWalkAnimationWhileAiming = GUILayout.Toggle(script.enableWalkAnimationWhileAiming, "Enable walk animation while aiming");
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

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Weapon script modification");
                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(target);
                }

                /*
                if (GUI.changed)
                {
                    Undo.RecordObject(target, "Weapon script modification");
                    serializedObject.ApplyModifiedProperties();
                }
                */
            }
        }
    }
}
