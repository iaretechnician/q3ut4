using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;
using System.Collections.Generic;
using MarsFPSKit.UI;

namespace MarsFPSKit
{
    namespace Editor
    {
        public class Kit_EditorMenuExtensions : MonoBehaviour
        {
            [MenuItem("MMFPSE/Scene/Setup")]
            private static void SetupScene()
            {
                EditorSceneManager.MarkAllScenesDirty();

                if (EditorSceneManager.EnsureUntitledSceneHasBeenSaved("Scene needs to be saved in order to set it up"))
                {
                    if (!FindObjectOfType<Kit_MenuManager>())
                    {
                        //Scene is saved and ready for setup!
                        //Check if a main is present
                        if (!FindObjectOfType<Kit_IngameMain>())
                        {
                            //Load prefab
                            Object prefabLoaded = Resources.Load("MarsFPSKit_IngamePrefab");
                            if (prefabLoaded != null)
                            {
                                GameObject prefab = prefabLoaded as GameObject;
                                if (prefab)
                                {
                                    Kit_GameInformation[] gameInformations = Resources.FindObjectsOfTypeAll<Kit_GameInformation>();
                                    if (gameInformations.Length > 0)
                                    {
                                        Kit_GameInformation gameInformation = gameInformations[0];
                                        //Find all cameras and delete them
                                        Camera[] allCams = FindObjectsOfType<Camera>();
                                        Vector3 camPos = Vector3.zero;
                                        Quaternion camRot = Quaternion.identity;
                                        //Destroy them all
                                        for (int i = 0; i < allCams.Length; i++)
                                        {
                                            //Assign pos and rot
                                            camPos = allCams[i].transform.position;
                                            camRot = allCams[i].transform.rotation;
                                            DestroyImmediate(allCams[i].gameObject);
                                        }
                                        //Instantiate main prefab
                                        GameObject obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                                        FindObjectOfType<Kit_IngameMain>().spawnCameraPosition.position = camPos;
                                        FindObjectOfType<Kit_IngameMain>().spawnCameraPosition.rotation = camRot;
                                        if (EditorSceneManager.SaveOpenScenes())
                                        {
                                            //Continue, add to build manager
                                            List<EditorBuildSettingsScene> scenes = EditorBuildSettings.scenes.ToList();
                                            //Check if its there
                                            bool isAdded = false;
                                            foreach (EditorBuildSettingsScene scene in scenes)
                                            {
                                                if (scene.path == EditorSceneManager.GetActiveScene().path) isAdded = true;
                                            }
                                            if (!isAdded)
                                            {
                                                scenes.Add(new EditorBuildSettingsScene { enabled = true, path = EditorSceneManager.GetActiveScene().path });
                                            }
                                            //Set back
                                            EditorBuildSettings.scenes = scenes.ToArray();
                                            //Check if scene is already in the game information
                                            if (gameInformation.GetCurrentLevel() < 0)
                                            {
                                                Kit_MapInformation mapInfo = Kit_MapInformation.CreateInstance<Kit_MapInformation>();
                                                mapInfo.mapName = EditorSceneManager.GetActiveScene().name;
                                                mapInfo.sceneName = EditorSceneManager.GetActiveScene().name;
                                                string path = EditorSceneManager.GetActiveScene().path;
                                                path = path.Replace(".unity", "");
                                                path = path + "_MapInformation.asset";
                                                /*
                                                //Add to gameInformation
                                                List<Kit_MapInformation> maps = gameInformation.allMaps.ToList();
                                                maps.Add(mapInfo);
                                                gameInformation.allMaps = maps.ToArray();
                                                */
                                                //Save
                                                AssetDatabase.CreateAsset(mapInfo, path);
                                                //Save scene
                                                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                                                //Save Project
                                                AssetDatabase.SaveAssets();
                                            }
                                            //Save again
                                            EditorSceneManager.SaveOpenScenes();
                                        }
                                    }
                                    else
                                    {
                                        EditorUtility.DisplayDialog("Could not find game information", "Are you sure that you have one in your project?", "OK");
                                    }
                                }
                                else
                                {
                                    EditorUtility.DisplayDialog("Could not find prefab", "Did you rename the MarsFPSKit_IngamePrefab or move it out of the resources folder?", "OK");
                                }
                            }
                            else
                            {
                                EditorUtility.DisplayDialog("Could not find prefab", "Did you rename the MarsFPSKit_IngamePrefab or move it out of the resources folder?", "OK");
                            }
                        }
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Nuts", "Are you nuts? Don't setup the main menu.", "Nah");
                    }
                }
            }
        }
    }
}
