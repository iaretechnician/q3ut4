using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    namespace Spectating
    {
        public class Kit_SpectatorUI : MonoBehaviour
        {
            /// <summary>
            /// Takes us to the previous player
            /// </summary>
            public Button previousPlayer;
            /// <summary>
            /// Takes us to the next player
            /// </summary>
            public Button nextPlayer;
            /// <summary>
            /// Displays name of our current player
            /// </summary>
            public TextMeshProUGUI currentPlayer;
        }
    }
}