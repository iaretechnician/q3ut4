using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    namespace UI
    {
        /// <summary>
        /// This class contains references for the coop server browser
        /// </summary>
        public class Kit_CoopBrowserEntry : MonoBehaviour
        {
            /// <summary>
            /// The name of this room
            /// </summary>
            public TextMeshProUGUI serverName;
            /// <summary>
            /// The map that is currently played in this room
            /// </summary>
            public TextMeshProUGUI mapName;
            /// <summary>
            /// How many players are in this room
            /// </summary>
            public TextMeshProUGUI players;
            /// <summary>
            /// The ping of this room - The cloud
            /// </summary>
            public TextMeshProUGUI ping;
            /// <summary>
            /// Join Button
            /// </summary>
            public Button joinButton;

            private Services.GameInfo myRoom;

            /// <summary>
            /// Called from Main Menu to properly set this entry up
            /// </summary>
            public void Setup(Kit_MenuPveGameModeBase menu, Services.GameInfo curRoom)
            {
                myRoom = curRoom;

                if (myRoom != null)
                {
                    //Set Info
                    serverName.text = myRoom.name;
                    //Map
                    mapName.text = menu.menuManager.game.allCoopGameModes[myRoom.gameMode].maps[myRoom.map].mapName;
                    //Players
                    players.text = myRoom.players + "/" + myRoom.maxPlayers;
                    //Ping
                    ping.text = myRoom.ping.ToString();
                }

                //Reset scale (Otherwise it will be offset)
                transform.localScale = Vector3.one;
            }
        }
    }
}