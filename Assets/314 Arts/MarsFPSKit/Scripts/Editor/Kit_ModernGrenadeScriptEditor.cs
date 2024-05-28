using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace MarsFPSKit
{
    namespace Weapons
    {
        [CustomEditor(typeof(Kit_ModernGrenadeScript))]
        public class Kit_ModernGrenadeScriptEditor : UnityEditor.Editor
        {
            static bool prefabsFoldout;
            static bool settingsFoldout;
            static bool throwFoldout;
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
                Kit_ModernGrenadeScript script = target as Kit_ModernGrenadeScript;

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
                        if (!script.firstPersonPrefab.GetComponent<Kit_GrenadeRenderer>())
                        {
                            EditorGUILayout.HelpBox("First Person Prefab does not have the grenade renderer assigned!", MessageType.Error);
                        }
                    }
                    if (script.thirdPersonPrefab)
                    {
                        if (!script.thirdPersonPrefab.GetComponent<Kit_ThirdPersonGrenadeRenderer>())
                        {
                            EditorGUILayout.HelpBox("Third Person Prefab does not have the third person grenade renderer assigned!", MessageType.Error);
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

                    script.grenadeMode = (GrenadeMode)EditorGUILayout.EnumPopup("Grenade Mode", script.grenadeMode);
                    script.amountOfGrenadesAtStart = EditorGUILayout.IntField("Amount of grenades at start", script.amountOfGrenadesAtStart);
                    if (script.grenadeMode == GrenadeMode.IndividualWeapon || script.grenadeMode == GrenadeMode.Both)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        EditorGUILayout.LabelField("Equipped");
                        script.pullPinTime = EditorGUILayout.FloatField("Pull Pin Time", script.pullPinTime);
                        script.pullPinSound = EditorGUILayout.ObjectField("Pull Pin Sound", script.pullPinSound, typeof(AudioClip), false) as AudioClip;
                        script.throwTime = EditorGUILayout.FloatField("Throw Time", script.throwTime);
                        script.throwSound = EditorGUILayout.ObjectField("Throw Sound", script.throwSound, typeof(AudioClip), false) as AudioClip;
                        script.redrawTime = EditorGUILayout.FloatField("Re-draw time after throw", script.redrawTime);
                        EditorGUILayout.EndVertical();
                    }
                    if (script.grenadeMode == GrenadeMode.QuickUse || script.grenadeMode == GrenadeMode.Both)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        EditorGUILayout.LabelField("Quick use");
                        script.quickUsePullPinTime = EditorGUILayout.FloatField("Pull Pin Time", script.quickUsePullPinTime);
                        script.pullPinQuickSound = EditorGUILayout.ObjectField("Pull Pin Sound", script.pullPinQuickSound, typeof(AudioClip), false) as AudioClip;
                        script.quickUseThrowTime = EditorGUILayout.FloatField("Throw Time", script.quickUseThrowTime);
                        script.throwQuickSound = EditorGUILayout.ObjectField("Throw Sound", script.throwQuickSound, typeof(AudioClip), false) as AudioClip;
                        EditorGUILayout.EndVertical();
                    }
                    script.thirdPersonRange = EditorGUILayout.FloatField("Third person max range", script.thirdPersonRange);
                    script.thirdPersonRolloff = EditorGUILayout.CurveField("Third person rolloff", script.thirdPersonRolloff);

                    script.voiceGrenadeSoundID = EditorGUILayout.IntField("Voice Sound ID", script.voiceGrenadeSoundID);
                    EditorGUILayout.EndVertical();
                }
                throwFoldout = EditorGUILayout.Foldout(throwFoldout, "Throw");
                if (throwFoldout)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    script.grenadePrefab = EditorGUILayout.ObjectField("Grenade Prefab", script.grenadePrefab, typeof(GameObject), false) as GameObject;
                    script.throwForce = EditorGUILayout.Vector3Field("Throw Force", script.throwForce);
                    script.throwTorque = EditorGUILayout.Vector3Field("Throw Torque", script.throwTorque);
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
