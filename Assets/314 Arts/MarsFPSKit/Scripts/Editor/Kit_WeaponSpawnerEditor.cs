using UnityEngine;
using UnityEditor;
using MarsFPSKit;
using MarsFPSKit.Weapons;
using System.Linq;
using System.Collections.Generic;

using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Kit_WeaponSpawner))]
public class Kit_WeaponSpawnerEditor : Editor
{
    public static bool foldoutWeapons;

    public static bool foldoutSettings;

    public override void OnInspectorGUI()
    {
        Kit_WeaponSpawner spawner = (Kit_WeaponSpawner)target;

        if (!spawner.dropPrefab)
        {
            spawner.dropPrefab = EditorGUILayout.ObjectField("Drop Prefab", spawner.dropPrefab, typeof(GameObject), false) as GameObject;
        }
        else
        {
            if (!spawner.dropPrefab.GetComponent<Kit_DropBehaviour>())
            {
                EditorGUILayout.HelpBox("Drop Prefab does not have Drop Behaviour on it", MessageType.Error);
                spawner.dropPrefab = EditorGUILayout.ObjectField("Drop Prefab", spawner.dropPrefab, typeof(GameObject), false) as GameObject;
            }
        }

        Kit_IngameMain instance = FindObjectOfType<Kit_IngameMain>();

        if (instance)
        {
            if (spawner.weaponsToSpawn.Length <= 0)
            {
                EditorGUILayout.HelpBox("Add a weapon! Otherwise this thing is useless.", MessageType.Warning);
                if (GUILayout.Button("Add new weapon"))
                {
                    List<WeaponToSpawn> wstList = new List<WeaponToSpawn>();
                    WeaponToSpawn wst = new WeaponToSpawn();
                    if (instance.gameInformation.allWeapons[wst.weaponID].GetType() == typeof(Kit_ModernWeaponScript))
                    {
                        wst.bulletsLeft = (instance.gameInformation.allWeapons[wst.weaponID] as Kit_ModernWeaponScript).bulletsPerMag;

                        wst.bulletsLeftToReload = (instance.gameInformation.allWeapons[wst.weaponID] as Kit_ModernWeaponScript).bulletsPerMag * 2;
                    }
                    wstList.Add(wst);
                    spawner.weaponsToSpawn = wstList.ToArray();
                }
            }
            else
            {
                foldoutWeapons = EditorGUILayout.Foldout(foldoutWeapons, "Weapons");

                if (foldoutWeapons)
                {
                    for (int i = 0; i < spawner.weaponsToSpawn.Length; i++)
                    {
                        //First, clamp ID
                        spawner.weaponsToSpawn[i].weaponID = Mathf.Clamp(spawner.weaponsToSpawn[i].weaponID, 0, instance.gameInformation.allWeapons.Length - 1);

                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                        if (GUILayout.Button("Remove Weapon"))
                        {
                            //Convert to list
                            List<WeaponToSpawn> weps = spawner.weaponsToSpawn.ToList();
                            //Remove at
                            int id = i;
                            weps.RemoveAt(id);
                            //Go back
                            spawner.weaponsToSpawn = weps.ToArray();
                            return;
                        }

                        GUILayout.Label("Selected Weapon: " + instance.gameInformation.allWeapons[spawner.weaponsToSpawn[i].weaponID].weaponName);
                        spawner.weaponsToSpawn[i].weaponID = EditorGUILayout.IntSlider("Weapon ID", spawner.weaponsToSpawn[i].weaponID, 0, instance.gameInformation.allWeapons.Length - 1);

                        if (instance.gameInformation.allWeapons[spawner.weaponsToSpawn[i].weaponID].GetType() == typeof(Kit_ModernWeaponScript))
                        {
                            Kit_ModernWeaponScript ws = instance.gameInformation.allWeapons[spawner.weaponsToSpawn[i].weaponID] as Kit_ModernWeaponScript;

                            if ((instance.gameInformation.allWeapons[spawner.weaponsToSpawn[i].weaponID] as Kit_ModernWeaponScript).reloadMode == ReloadMode.Chambered || (instance.gameInformation.allWeapons[spawner.weaponsToSpawn[i].weaponID] as Kit_ModernWeaponScript).reloadMode == ReloadMode.ProceduralChambered)
                            {
                                spawner.weaponsToSpawn[i].bulletsLeft = EditorGUILayout.IntSlider("Bullets Left", spawner.weaponsToSpawn[i].bulletsLeft, 0, (instance.gameInformation.allWeapons[spawner.weaponsToSpawn[i].weaponID] as Kit_ModernWeaponScript).bulletsPerMag + 1);
                            }
                            else
                            {
                                spawner.weaponsToSpawn[i].bulletsLeft = EditorGUILayout.IntSlider("Bullets Left", spawner.weaponsToSpawn[i].bulletsLeft, 0, (instance.gameInformation.allWeapons[spawner.weaponsToSpawn[i].weaponID] as Kit_ModernWeaponScript).bulletsPerMag);
                            }

                            spawner.weaponsToSpawn[i].bulletsLeftToReload = EditorGUILayout.IntField("Bullets Left to Reload", spawner.weaponsToSpawn[i]
                                .bulletsLeftToReload);

                            //Check if attachments line up
                            if (ws.attachmentSlots.Length != spawner.weaponsToSpawn[i].attachmentsOfThisWeapon.Length)
                            {
                                //Create new array
                                spawner.weaponsToSpawn[i].attachmentsOfThisWeapon = new int[ws.attachmentSlots.Length];
                            }

                            //Loop through attachments
                            for (int o = 0; o < spawner.weaponsToSpawn[i].attachmentsOfThisWeapon.Length; o++)
                            {
                                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                                //Display Attachment name
                                GUILayout.Label("Attachment selected in slot [" + o + "]: " + ws.attachmentSlots[o].availableAttachments[spawner.weaponsToSpawn[i].attachmentsOfThisWeapon[o]].name);

                                spawner.weaponsToSpawn[i].attachmentsOfThisWeapon[o] = EditorGUILayout.IntSlider("Attachment ID: ", spawner.weaponsToSpawn[i].attachmentsOfThisWeapon[o], 0, ws.attachmentSlots[o].availableAttachments.Length - 1);

                                EditorGUILayout.EndVertical();
                            }
                        }
                        else
                        {
                            spawner.weaponsToSpawn[i].bulletsLeft = EditorGUILayout.IntField("Bullets Left", spawner.weaponsToSpawn[i]
                                .bulletsLeft);
                            spawner.weaponsToSpawn[i].bulletsLeftToReload = EditorGUILayout.IntField("Bullets Left to Reload", spawner.weaponsToSpawn[i]
                                .bulletsLeftToReload);
                        }
                        EditorGUILayout.EndVertical();
                    }

                    if (GUILayout.Button("Add new weapon"))
                    {
                        //Convert to list
                        List<WeaponToSpawn> weps = spawner.weaponsToSpawn.ToList();
                        WeaponToSpawn wst = new WeaponToSpawn();
                        if (instance.gameInformation.allWeapons[wst.weaponID].GetType() == typeof(Kit_ModernWeaponScript))
                        {
                            wst.bulletsLeft = (instance.gameInformation.allWeapons[wst.weaponID] as Kit_ModernWeaponScript).bulletsPerMag;

                            wst.bulletsLeftToReload = (instance.gameInformation.allWeapons[wst.weaponID] as Kit_ModernWeaponScript).bulletsPerMag * 2;
                        }
                        weps.Add(wst);
                        //Go back
                        spawner.weaponsToSpawn = weps.ToArray();
                    }
                }
            }

            foldoutSettings = EditorGUILayout.Foldout(foldoutSettings, "Settings");
            if (foldoutSettings)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                spawner.spawnType = (WeaponSpawnType)EditorGUILayout.EnumPopup("Respawn type", spawner.spawnType);

                if (spawner.spawnType == WeaponSpawnType.RespawnAfterTaken)
                {
                    spawner.respawnTime = EditorGUILayout.FloatField("Respawn time after weapon was picked up (s): ", spawner.respawnTime);
                }
                else if (spawner.spawnType == WeaponSpawnType.RespawnAfterTime)
                {
                    spawner.respawnTime = EditorGUILayout.FloatField("Respawn every (s): ", spawner.respawnTime);
                }
                EditorGUILayout.EndVertical();
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(spawner);
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }
    }
}
