using MarsFPSKit.UI;
using UnityEngine;

namespace MarsFPSKit
{
    [CreateAssetMenu(menuName = "MarsFPSKit/Statistics/PlayerPrefsBased")]
    /// <summary>
    /// Implements simple statistics with player prefs
    /// </summary>
    public class Kit_StatisticsPlayerPrefs : Kit_StatisticsBase
    {
        /// <summary>
        /// Our accumulated kills
        /// </summary>
        public int kills;
        /// <summary>
        /// Our accumulated deaths
        /// </summary>
        public int deaths;
        /// <summary>
        /// Our accumulated assists
        /// </summary>
        public int assists;

        public override void OnAssist()
        {
            assists++;
        }

        public override void OnDeath(int weapon)
        {
            deaths++;
        }

        public override void OnDeath(string weapon)
        {
            deaths++;
        }

        public override void OnKill(int weapon)
        {
            kills++;
        }

        public override void OnKill(string reason)
        {
            kills++;
        }

        public override void OnStart(Kit_MenuManager menu)
        {
            //Reset
            kills = 0;
            deaths = 0;
            assists = 0;

            //Then load
            kills = PlayerPrefs.GetInt(Kit_GameSettings.userName + "_kills", 0);
            deaths = PlayerPrefs.GetInt(Kit_GameSettings.userName + "_deaths", 0);
            assists = PlayerPrefs.GetInt(Kit_GameSettings.userName + "_assists", 0);
        }

        public override void Save()
        {
            //Save all
            PlayerPrefs.SetInt(Kit_GameSettings.userName + "_kills", kills);
            PlayerPrefs.SetInt(Kit_GameSettings.userName + "_deaths", deaths);
            PlayerPrefs.SetInt(Kit_GameSettings.userName + "_assists", assists);
        }

        public override void Save(Kit_MenuManager menu)
        {
            //Save all
            PlayerPrefs.SetInt(Kit_GameSettings.userName + "_kills", kills);
            PlayerPrefs.SetInt(Kit_GameSettings.userName + "_deaths", deaths);
            PlayerPrefs.SetInt(Kit_GameSettings.userName + "_assists", assists);
        }
    }
}