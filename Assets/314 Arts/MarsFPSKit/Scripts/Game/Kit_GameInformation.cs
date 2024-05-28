using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MarsFPSKit.Weapons;
using MarsFPSKit.Services;

namespace MarsFPSKit
{
    /// <summary>
    /// This is used to display unlocks
    /// </summary>
    public class UnlockInformation
    {
        /// <summary>
        /// Name of the unlocked item
        /// </summary>
        public string name;
        /// <summary>
        /// Image of the unlocked item
        /// </summary>
        public Sprite img;
    }

    [System.Serializable]
    public class AnimatorSetInformation
    {
        /// <summary>
        /// The prefix of that animation set
        /// </summary>
        public string prefix;
        /// <summary>
        /// The anim type to set to the animator
        /// </summary>
        public int type;
    }

    /// <summary>
    /// This Object contains the complete game information (Maps, GameModes, Weapons)
    /// </summary>
    [CreateAssetMenu(menuName = "MarsFPSKit/Critical/Game Information")]
    public class Kit_GameInformation : ScriptableObject
    {
        public enum PerspectiveMode { FirstPersonOnly, ThirdPersonOnly, Both }
        public enum Perspective { FirstPerson = 1, ThirdPerson = 3 }
        public enum ThirdPersonAiming { OverShoulder, GoIntoFirstPerson }

        /// <summary>
        /// Master server service to use
        /// </summary>
        [Tooltip("This registeres games and makes them available for others to join. Can be null but server browser will not work without it")]
        [Header("Services")]
        public Kit_MasterServerBase masterServer;
        [Tooltip("Transport service that we should use. This has to be assigned!")]
        /// <summary>
        /// Transport service that we should use. This has to be assigned!
        /// </summary>
        public Kit_TransportServiceBase transport;

        /// <summary>
        /// The ingame main prefab. This is the one that you need to put on your maps.
        /// </summary>
        [Tooltip("The ingame main prefab. This is the one that you need to put on your maps.")]
        [Header("Sync")]
        public GameObject ingameMainPrefab;
        [Tooltip("Make sure to change it when you publish a new update")]
        public string gameVersion = "1";
        /// <summary>
        /// Weapon categories!
        /// </summary>
        public string[] allWeaponCategories = new string[5] { "Primary", "Secondary", "Melee", "Lethal", "NonLethal" };
        /// <summary>
        /// The default weapons in the respective slot!
        /// </summary>
        public int[] defaultWeaponsInSlot = new int[5];
        [Tooltip("All Weapons that are available")]
        public Kit_WeaponBase[] allWeapons;
        /// <summary>
        /// Sets to use for the animator
        /// </summary>
        [Tooltip("These are the sets of animations that you can pick in the weapons. For example: Rifle, Pistol, RPG, Sword, etc..")]
        public AnimatorSetInformation[] allAnimatorAnimationSets;

        /// <summary>
        /// Singleplayer Game Modes
        /// </summary>
        [Header("PvE Config")]
        public Kit_PvE_GameModeBase[] allSingleplayerGameModes;
        /// <summary>
        /// COOP Game Modes
        /// </summary>
        public Kit_PvE_GameModeBase[] allCoopGameModes;

        [Header("PvP Config")]
        [Tooltip("All Game Modes that are avaialable")]
        /// <summary>
        /// PvP Game Modes
        /// </summary>
        public Kit_PvP_GameModeBase[] allPvpGameModes;
        /// <summary>
        /// Teams for PVP
        /// </summary>
        public Kit_Team[] allPvpTeams;
        /// <summary>
        /// How many points do we receive per kill?
        /// </summary>
        public int pointsPerKill = 100;

        [Header("Settings")]
        /// <summary>
        /// Check this if touchscreen input should be enabled
        /// </summary>
        public bool enableTouchscreenInput;
        /// <summary>
        /// The Kit's perspective
        /// </summary>
        public PerspectiveMode perspectiveMode;
        /// <summary>
        /// The default perspective if perspective mode is set to both
        /// </summary>
        public Perspective defaultPerspective;
        /// <summary>
        /// Third person aiming mode
        /// </summary>
        public ThirdPersonAiming thirdPersonAiming;
        //[Tooltip("If this is enabled, bullets will be fired from third person camera in third person mode. Warning: This can cause unbalances.")]
        /// <summary>
        /// If this is enabled, bullets will be fired from third person camera in third person mode. Warning: This can cause unbalances.
        /// </summary>
        //public bool thirdPersonCameraShooting;
        [Tooltip("If you want to have dropped weapons when picking up scene weapons, set this to true. WARNING: It can cause some weird behaviour with quickly respawning weapons")]
        /// <summary>
        /// If you want to have dropped weapons when picking up scene weapons, set this to true. WARNING: It can cause some weird behaviour with quickly respawning weapons
        /// </summary>
        public bool enableDropWeaponOnSceneSpawnedWeapons = false;
        /// <summary>
        /// Should we automatically reload when empty mag and still ammo left?
        /// </summary>
        public bool enableAutoReload;

        /// <summary>
        /// If set to true, ammo (bullets left) will not decrease
        /// </summary>
        [Header("Debug")]
        public bool debugEnableUnlimitedBullets;
        /// <summary>
        /// If set to true, reload ammo (bullets left to reload) will not decrease
        /// </summary>
        public bool debugEnableUnlimitedReloads;

        /// <summary>
        /// Leveling module
        /// </summary>
        [Header("Modules")]
        public Kit_LevelingBase leveling;
        /// <summary>
        /// Statistics module
        /// </summary>
        public Kit_StatisticsBase statistics;
        /// <summary>
        /// Use this to override the prefab used for the main camera in game.
        /// </summary>
        public GameObject mainCameraOverride;
        /// <summary>
        /// Plugins that are enabled!
        /// </summary>
        public Kit_Plugin[] plugins;


        public int GetCurrentLevel()
        {
            Scene currentScene = SceneManager.GetActiveScene();

            for (int i = 0; i < allPvpGameModes.Length; i++)
            {
                if (Kit_GameSettings.currentNetworkingMode == KitNetworkingMode.Traditional)
                {
                    for (int o = 0; o < allPvpGameModes[i].traditionalMaps.Length; o++)
                    {
                        if (allPvpGameModes[i].traditionalMaps[o].sceneName == currentScene.name)
                        {
                            return o;
                        }
                    }
                }
                else if (Kit_GameSettings.currentNetworkingMode == KitNetworkingMode.Lobby)
                {
                    for (int o = 0; o < allPvpGameModes[i].lobbyMaps.Length; o++)
                    {
                        if (allPvpGameModes[i].lobbyMaps[o].sceneName == currentScene.name)
                        {
                            return o;
                        }
                    }
                }
            }

            return -1;
        }

        public Kit_MapInformation GetMapInformationFromSceneName(string scene)
        {
            try
            {
                for (int i = 0; i < allPvpGameModes.Length; i++)
                {
                    if (Kit_GameSettings.currentNetworkingMode == KitNetworkingMode.Traditional)
                    {
                        for (int o = 0; o < allPvpGameModes[i].traditionalMaps.Length; o++)
                        {
                            if (allPvpGameModes[i].traditionalMaps[o].sceneName == scene)
                            {
                                return allPvpGameModes[i].traditionalMaps[o];
                            }
                        }
                    }
                    else if (Kit_GameSettings.currentNetworkingMode == KitNetworkingMode.Lobby)
                    {
                        for (int o = 0; o < allPvpGameModes[i].lobbyMaps.Length; o++)
                        {
                            if (allPvpGameModes[i].lobbyMaps[o].sceneName == scene)
                            {
                                return allPvpGameModes[i].lobbyMaps[o];
                            }
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Error when iterating through all pvp game modes and maps: " + e);
            }

            try
            {
                for (int i = 0; i < allSingleplayerGameModes.Length; i++)
                {
                    for (int o = 0; o < allSingleplayerGameModes[i].maps.Length; o++)
                    {
                        if (allSingleplayerGameModes[i].maps[o].sceneName == scene)
                        {
                            return allSingleplayerGameModes[i].maps[o];
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Error when iterating through all singleplayer game modes and maps: " + e);
            }

            try
            {
                for (int i = 0; i < allCoopGameModes.Length; i++)
                {
                    for (int o = 0; o < allCoopGameModes[i].maps.Length; o++)
                    {
                        if (allCoopGameModes[i].maps[o].sceneName == scene)
                        {
                            return allCoopGameModes[i].maps[o];
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Error when iterating through all coop game modes and maps: " + e);
            }

            return null;
        }

        public UnlockInformation[] GetUnlockedItemsAtLevel(int lvl)
        {
            List<UnlockInformation> toReturn = new List<UnlockInformation>();
            //Go through all weapons
            for (int i = 0; i < allWeapons.Length; i++)
            {
                //Check if its unlocked at that level
                if (allWeapons[i].levelToUnlockAt == lvl)
                {
                    //Create new Unlock Information
                    UnlockInformation ui = new UnlockInformation();
                    //Assign info
                    ui.name = allWeapons[i].weaponName;
                    ui.img = allWeapons[i].unlockImage;
                    //Add
                    toReturn.Add(ui);
                }
            }
            return toReturn.ToArray();
        }
    }
}
