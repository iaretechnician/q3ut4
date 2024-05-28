using UnityEngine;
using TMPro;

namespace MarsFPSKit
{
    namespace UI
    {
        /// <summary>
        /// Displays the player prefs based statistics in the menu
        /// </summary>
        public class Kit_MenuStatisticsForPlayerPrefs : MonoBehaviour
        {
            /// <summary>
            /// Menu Manager reference
            /// </summary>
            public Kit_MenuManager menuManager;

            /// <summary>
            /// Kills
            /// </summary>
            public TextMeshProUGUI kills;
            /// <summary>
            /// Assists
            /// </summary>
            public TextMeshProUGUI assists;
            /// <summary>
            /// Deaths
            /// </summary>
            public TextMeshProUGUI deaths;
            /// <summary>
            /// K/D
            /// </summary>
            public TextMeshProUGUI kd;

            private void Awake()
            {
                menuManager.onLogin.AddListener(delegate { RedrawStatistics(); });
            }

            public void RedrawStatistics()
            {
                if (menuManager.game.statistics && menuManager.game.statistics.GetType() == typeof(Kit_StatisticsPlayerPrefs))
                {
                    Kit_StatisticsPlayerPrefs kspp = menuManager.game.statistics as Kit_StatisticsPlayerPrefs;

                    //Just set texts
                    kills.text = "Kills: " + kspp.kills;
                    assists.text = "Assists: " + kspp.assists;
                    deaths.text = "Deaths: " + kspp.deaths;
                    if (kspp.deaths > 0) kd.text = "K/D: " + ((float)kspp.kills / kspp.deaths).ToString("F1");
                    else kd.text = "K/D: " + kspp.kills;
                }
            }
        }
    }
}