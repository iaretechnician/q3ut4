
using MarsFPSKit.Networking;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    public class Kit_ModernAutoSpawnSystem : Kit_AutoSpawnSystemBase
    {
        /// <summary>
        /// How long until the auto respawn is over
        /// </summary>
        [Header("Settings")]
        public float autoRespawnTime = 6f;

        [Header("UI")]
        //UI Root
        public GameObject spawnSystemRoot;
        /// <summary>
        /// Time that remains displayed to the player
        /// </summary>
        public TextMeshProUGUI remainingTimeText;

        #region Runtime
        /// <summary>
        /// Is the system active
        /// </summary>
        private bool isAutoSpawnSystemOpen;
        /// <summary>
        /// At which time was the system activated?
        /// </summary>
        private float autoSpawnSystemActivatedAt;
        #endregion

        void Start()
        {
            //Disable
            //Close system
            isAutoSpawnSystemOpen = false;
            //Disable GUI
            spawnSystemRoot.SetActive(false);
        }

        void Update()
        {
            //Update ui if open
            if (isAutoSpawnSystemOpen)
            {
                //Update text
                remainingTimeText.text = "Auto respawn in " + ((autoSpawnSystemActivatedAt + autoRespawnTime) - Time.time).ToString("F2") + " seconds";
                //Check
                if (Time.time > (autoSpawnSystemActivatedAt + autoRespawnTime))
                {
                    //Spawn and close
                    Kit_IngameMain.instance.Spawn();
                    //Close system
                    isAutoSpawnSystemOpen = false;
                    //Disable GUI
                    spawnSystemRoot.SetActive(false);
                }

                //Check for input
                if (Input.GetKeyDown(KeyCode.F))
                {
                    //Spawn and close
                    Kit_IngameMain.instance.Spawn();
                    //Close system
                    isAutoSpawnSystemOpen = false;
                    //Disable GUI
                    spawnSystemRoot.SetActive(false);
                }
            }
        }

        public override void Interruption()
        {
            //Close system
            isAutoSpawnSystemOpen = false;
            //Disable GUI
            spawnSystemRoot.SetActive(false);
        }

        public override void LocalPlayerDied()
        {
            if (Kit_IngameMain.instance.currentPvPGameModeBehaviour.CanSpawn(Kit_NetworkPlayerManager.instance.GetLocalPlayer()) && (!Kit_IngameMain.instance.options || (Kit_IngameMain.instance.options && Kit_IngameMain.instance.currentScreen != Kit_IngameMain.instance.options.optionsScreenId)))
            {
                //Set time
                autoSpawnSystemActivatedAt = Time.time;
                //Activate system
                isAutoSpawnSystemOpen = true;
                //Activate GUI
                spawnSystemRoot.SetActive(true);
                //Close Pause Menu
                Kit_IngameMain.instance.SetPauseMenuState(false, false);
            }
        }

        public override void LocalPlayerSpawned()
        {
            //Close system
            isAutoSpawnSystemOpen = false;
            //Disable GUI
            spawnSystemRoot.SetActive(false);
        }
    }
}
