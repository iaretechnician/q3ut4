using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MarsFPSKit
{
    namespace UI
    {
        public class Kit_MenuFriendsButton : MonoBehaviour
        {
            /// <summary>
            /// Online State image
            /// </summary>
            public Image onlineState;
            /// <summary>
            /// Name of the player
            /// </summary>
            public TextMeshProUGUI playerName;
            /// <summary>
            /// Button to join if player is online
            /// </summary>
            public Button joinButton;
            /// <summary>
            /// Removes the player from our list
            /// </summary>
            public Button removeButton;
            [HideInInspector]
            /// <summary>
            /// User ID of this button
            /// </summary>
            public string userId;
        }
    }
}