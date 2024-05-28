using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace MarsFPSKit
{
    /// <summary>
    /// This window lets the user check the scene status
    /// </summary>
    public class Kit_EditorSceneStatusChecker : EditorWindow
    {
        Vector2 scroll;

        [MenuItem("MMFPSE/Scene/Check Status")]
        public static void Init()
        {
            Kit_EditorSceneStatusChecker window = (Kit_EditorSceneStatusChecker)GetWindow(typeof(Kit_EditorSceneStatusChecker));
            window.Show();
        }

        void OnGUI()
        {
            scroll = GUILayout.BeginScrollView(scroll);
            GUILayout.Label("Scene Status", EditorStyles.boldLabel);
            if (EditorSceneManager.GetActiveScene().path == "")
            {
                EditorGUILayout.HelpBox("Not saved", MessageType.Error);
                if (GUILayout.Button("Fix"))
                {
                    EditorSceneManager.EnsureUntitledSceneHasBeenSaved("Save scene");
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Saved", MessageType.Info);
            }

            EditorGUI.BeginDisabledGroup(EditorSceneManager.GetActiveScene().path == "");
            GUILayout.Label("Kit Setup Status", EditorStyles.boldLabel);
            if (!FindObjectOfType<Kit_IngameMain>())
            {
                EditorGUILayout.HelpBox("Kit_IngameMain not found", MessageType.Error);
                if (GUILayout.Button("Fix"))
                {
                    Object prefabLoaded = Resources.Load("MarsFPSKit_IngamePrefab");
                    if (prefabLoaded != null)
                    {
                        GameObject prefab = prefabLoaded as GameObject;
                        if (prefab)
                        {
                            //Instantiate main prefab
                            PrefabUtility.InstantiatePrefab(prefab);
                        }
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Kit_IngameMain found", MessageType.Info);
            }

            EditorGUI.BeginDisabledGroup(!FindObjectOfType<Kit_IngameMain>());
            //Get main
            Kit_GameInformation gameInformation = (Resources.Load("MarsFPSKit_IngamePrefab") as GameObject).GetComponent<Kit_IngameMain>().gameInformation;
            GUILayout.Label("Info status", EditorStyles.boldLabel);
            if (!IsMapAdded(gameInformation))
            {
                EditorGUILayout.HelpBox("Map is not added to the game information file", MessageType.Error);

                EditorGUILayout.HelpBox("To Fix, add the map to at least one game mode (traditional and/or lobby)", MessageType.Info);

                /*
                if (GUILayout.Button("Fix"))
                {
                    Kit_MapInformation mapInfo = Kit_MapInformation.CreateInstance<Kit_MapInformation>();
                    mapInfo.mapName = EditorSceneManager.GetActiveScene().name;
                    mapInfo.sceneName = EditorSceneManager.GetActiveScene().name;
                    string path = EditorSceneManager.GetActiveScene().path;
                    path = path.Replace(".unity", "");
                    path = path + "_MapInformation.asset";
                    //Add to gameInformation
                    List<Kit_MapInformation> maps = gameInformation.allMaps.ToList();
                    maps.Add(mapInfo);
                    gameInformation.allMaps = maps.ToArray();
                    //Save
                    AssetDatabase.CreateAsset(mapInfo, path);
                    //Save scene
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                    //Save Project
                    AssetDatabase.SaveAssets();
                }
                */
            }
            else
            {
                EditorGUILayout.HelpBox("Map is added", MessageType.Info);
            }

            EditorGUI.BeginDisabledGroup(!IsMapAdded(gameInformation));
            GUILayout.Label("Game Mode Status", EditorStyles.boldLabel);
            //Loop through all game modes
            for (int i = 0; i < gameInformation.allPvpGameModes.Length; i++)
            {
                if (gameInformation.allPvpGameModes[i])
                {
                    GUILayout.Label(gameInformation.allPvpGameModes[i].gameModeName, EditorStyles.miniBoldLabel);
                    if (gameInformation.allPvpGameModes[i])
                    {
                        string[] msg = gameInformation.allPvpGameModes[i].GetSceneCheckerMessages();
                        MessageType[] type = gameInformation.allPvpGameModes[i].GetSceneCheckerMessageTypes();
                        for (int o = 0; o < msg.Length; o++)
                        {
                            EditorGUILayout.HelpBox(msg[o], type[o]);
                        }
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Unassigned game mode behaviour at index " + i, MessageType.Error);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Unassigned game mode at index " + i, MessageType.Error);
                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.EndDisabledGroup();
            EditorGUI.EndDisabledGroup();

            GUILayout.EndScrollView();
        }

        public void OnInspectorUpdate()
        {
            //So that you see your changes in the scene instantly.
            Repaint();
        }

        public bool IsMapAdded(Kit_GameInformation info)
        {
            for (int i = 0; i < info.allPvpGameModes.Length; i++)
            {
                for (int o = 0; o < info.allPvpGameModes[i].traditionalMaps.Length; o++)
                {
                    if (info.allPvpGameModes[i].traditionalMaps[o])
                    {
                        if (info.allPvpGameModes[i].traditionalMaps[o].sceneName == EditorSceneManager.GetActiveScene().name) return true;
                    }
                    else
                    {
                        Debug.LogWarning("Traditional map for game mode " + info.allPvpGameModes[i] + " at index " + o + " is not assigned!");
                    }
                }

                for (int o = 0; o < info.allPvpGameModes[i].lobbyMaps.Length; o++)
                {
                    if (info.allPvpGameModes[i].lobbyMaps[o])
                    {
                        if (info.allPvpGameModes[i].lobbyMaps[o].sceneName == EditorSceneManager.GetActiveScene().name) return true;
                    }
                    else
                    {
                        Debug.LogWarning("Lobby map for game mode " + info.allPvpGameModes[i] + " at index " + o + " is not assigned!");
                    }
                }
            }
            return false;
        }
    }
}