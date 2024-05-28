using UnityEngine;
using UnityEditor;
using MarsFPSKit;
using System.Collections.Generic;


[CustomEditor(typeof(Kit_HealthSpawner))]
public class Kit_HealthSpawnerEditor : Editor
{
    public static bool foldoutSettings;

    public override void OnInspectorGUI()
    {
        Kit_HealthSpawner spawner = (Kit_HealthSpawner)target;

        foldoutSettings = EditorGUILayout.Foldout(foldoutSettings, "Settings");

        if (!spawner.healthPrefab || spawner.healthPrefab && !spawner.healthPrefab.GetComponent<Kit_HealthPickup>())
        {
            if (spawner.healthPrefab && !spawner.healthPrefab.GetComponent<Kit_HealthPickup>())
            {
                EditorGUILayout.HelpBox("Object does not have necessary scripts!", MessageType.Error);
            }

            spawner.healthPrefab = EditorGUILayout.ObjectField("Health Prefab", spawner.healthPrefab, typeof(GameObject), false) as GameObject;
        }

        if (foldoutSettings)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            spawner.spawnType = (HealthSpawnType)EditorGUILayout.EnumPopup("Respawn type", spawner.spawnType);
            spawner.healthRestored = EditorGUILayout.Slider("Amount of health restored", spawner.healthRestored, 0f, 100f);

            if (spawner.spawnType == HealthSpawnType.RespawnAfterTaken)
            {
                spawner.respawnTime = EditorGUILayout.FloatField("Respawn time after health was picked up (s): ", spawner.respawnTime);
            }
            EditorGUILayout.EndVertical();
        }
    }
}