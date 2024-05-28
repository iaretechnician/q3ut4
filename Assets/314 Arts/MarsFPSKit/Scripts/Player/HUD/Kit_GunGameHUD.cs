using MarsFPSKit.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    /// <summary>
    /// This is used for <see cref="Kit_PvP_GMB_GunGame"/>
    /// </summary>
    public class Kit_GunGameHUD : Kit_GameModeHUDBase
    {
        public TextMeshProUGUI timer;

        public TextMeshProUGUI nextWeaponDisplay;

        private int roundedRestSeconds;
        private int displaySeconds;
        private int displayMinutes;

        public override void HUDUpdate()
        {
            if (Kit_IngameMain.instance.currentPvPGameModeBehaviour.AreEnoughPlayersThere() || Kit_IngameMain.instance.hasGameModeStarted)
            {
                roundedRestSeconds = Mathf.CeilToInt(Kit_IngameMain.instance.timer);
                displaySeconds = roundedRestSeconds % 60; //Get seconds
                displayMinutes = roundedRestSeconds / 60; //Get minutes
                                                          //Update text
                timer.text = string.Format("{0:00} : {1:00}", displayMinutes, displaySeconds);
                timer.enabled = true;

                //Only display next weapon if the player is spawned
                if (Kit_IngameMain.instance.myPlayer)
                {
                    Kit_PvP_GMB_GunGame gameMode = Kit_IngameMain.instance.currentPvPGameModeBehaviour as Kit_PvP_GMB_GunGame;
                    //Get Game Mode data
                    Kit_PvP_GMB_GunGameNetworkData ggrd = Kit_IngameMain.instance.currentGameModeRuntimeData as Kit_PvP_GMB_GunGameNetworkData;
                    if (ggrd != null)
                    {
                        int currentGun = 0;

                        if (ggrd.currentGun.ContainsKey(Kit_NetworkPlayerManager.instance.myId))
                        {
                            currentGun = ggrd.currentGun[Kit_NetworkPlayerManager.instance.myId];
                        }

                        if (currentGun + 1 < gameMode.weaponOrders[ggrd.currentGunOrder].weapons.Length)
                        {
                            nextWeaponDisplay.text = "Next Weapon: " + Kit_IngameMain.instance.gameInformation.allWeapons[gameMode.weaponOrders[ggrd.currentGunOrder].weapons[currentGun + 1]].name;
                        }
                        else
                        {
                            nextWeaponDisplay.text = "Final level reached";
                        }
                    }
                    nextWeaponDisplay.enabled = true;
                }
                else
                {
                    nextWeaponDisplay.enabled = false;
                }
            }
            else
            {
                timer.enabled = false;
                //Disable next weapon
                nextWeaponDisplay.enabled = false;
            }
        }
    }
}
