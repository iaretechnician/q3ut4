using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    namespace Scoreboard
    {
        /// <summary>
        /// This class holds all references for a scoreboard entry
        /// </summary>
        public class Kit_ScoreboardUIEntry : MonoBehaviour
        {
            public TextMeshProUGUI nameText, kills, score, deaths, ping, assists;
        }
    }
}