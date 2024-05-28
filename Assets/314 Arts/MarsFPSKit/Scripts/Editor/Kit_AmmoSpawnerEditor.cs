using UnityEngine;
using UnityEditor;
using MarsFPSKit;
using System.Collections.Generic;


[CustomEditor(typeof(Kit_AmmoSpawner))]
public class Kit_AmmoSpawnerEditor : Editor
{
    public static bool foldoutSettings;

    public override void OnInspectorGUI()
    {
        Kit_AmmoSpawner spawner = (Kit_AmmoSpawner)target;

        foldoutSettings = EditorGUILayout.Foldout(foldoutSettings, "Settings");

        if (!spawner.ammoPrefab ||spawner.ammoPrefab && !spawner.ammoPrefab.GetComponent<Kit_AmmoPickup>())
        {
            if (spawner.ammoPrefab && !spawner.ammoPrefab.GetComponent<Kit_AmmoPickup>())
            {
                EditorGUILayout.HelpBox("Object does not have necessary scripts!", MessageType.Error);
            }

            spawner.ammoPrefab = EditorGUILayout.ObjectField("Ammo Prefab", spawner.ammoPrefab, typeof(GameObject), false) as GameObject;
        }

        if (foldoutSettings)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            spawner.spawnType = (AmmoSpawnType)EditorGUILayout.EnumPopup("Respawn type", spawner.spawnType);
            spawner.amountOfClipsToPickup = EditorGUILayout.IntField("Amount of clips", spawner.amountOfClipsToPickup);

            if (spawner.spawnType == AmmoSpawnType.RespawnAfterTaken)
            {
                spawner.respawnTime = EditorGUILayout.FloatField("Respawn time after ammo was picked up (s): ", spawner.respawnTime);
            }
            EditorGUILayout.EndVertical();
        }
    }
}