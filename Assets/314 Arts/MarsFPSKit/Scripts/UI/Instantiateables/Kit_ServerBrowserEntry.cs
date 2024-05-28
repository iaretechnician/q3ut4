using MarsFPSKit.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    namespace UI
    {
        /// <summary>
        /// This class contains references for the server browser and acts as a sender  
        /// </summary>
        public class Kit_ServerBrowserEntry : MonoBehaviour
        {
            public TextMeshProUGUI serverName; //The name of this room
            public TextMeshProUGUI dedicated; //Displays if its a dedicated server
            public TextMeshProUGUI mapName; //The map that is currently played in this room
            public TextMeshProUGUI gameModeName; //The game mode that is currently played in this room
            public TextMeshProUGUI players; //How many players are in this room
            public TextMeshProUGUI ping; //The ping of this room - The cloud
            public TextMeshProUGUI password; //If this room is password protected
            private Kit_MenuServerBrowser msb;
            private GameInfo myRoom;

            /// <summary>
            /// Called from Main Menu to properly set this entry up
            /// </summary>
            public void Setup(Kit_MenuServerBrowser curMsb, GameInfo curRoom)
            {
                msb = curMsb;
                myRoom = curRoom;

                if (myRoom != null)
                {
                    //Set Info
                    serverName.text = myRoom.name;
                    int gameMode = myRoom.gameMode;
                    //Game Mode
                    gameModeName.text = msb.menuManager.game.allPvpGameModes[gameMode].gameModeName;
                    //Map
                    mapName.text = msb.menuManager.game.allPvpGameModes[gameMode].traditionalMaps[myRoom.map].mapName;
                    bool bots = myRoom.bots;
                    if (bots)
                    {
                        //Players
                        players.text = myRoom.players + "/" + myRoom.maxPlayers + " (bots)";
                    }
                    else
                    {
                        //Players
                        players.text = myRoom.players + "/" + myRoom.maxPlayers;
                    }
                    //Ping
                    ping.text = myRoom.ping.ToString();
                    //Password
                    if (myRoom.password) password.text = "Yes";
                    else password.text = "No";

                    if (myRoom.dedicated)
                    {
                        dedicated.text = "Yes";
                    }
                    else
                    {
                        dedicated.text = "No";
                    }
                }

                //Reset scale (Otherwise it will be offset)
                transform.localScale = Vector3.one;
            }

            //Called from the button that is on this prefab, to join this room (attempt)
            public void OnClick()
            {
                //Check if this button is ready
                if (msb)
                {
                    if (myRoom != null)
                    {
                        //Attempt to join
                        msb.JoinRoom(myRoom);
                    }
                }
            }
        }
    }
}