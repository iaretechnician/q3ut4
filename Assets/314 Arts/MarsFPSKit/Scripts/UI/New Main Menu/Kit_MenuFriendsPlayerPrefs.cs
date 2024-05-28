using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

namespace MarsFPSKit
{
    namespace UI
    {
        /// <summary>
        /// Another leftover from PUN. This used PUN's friend service to implement a very basic friend service
        /// Mirror doesn't support this. Will implement third party service later (such as: Steam, Playfab etc)
        /// </summary>
        public class Kit_MenuFriendsPlayerPrefs : Kit_MenuFriendsBase
        {
            /// <summary>
            /// List of our friends
            /// </summary>
            public List<string> myFriends = new List<string>();

            /// <summary>
            /// Input field for adding a friend
            /// </summary>
            public TMP_InputField addFriendInput;

            /// <summary>
            /// Where the friends go
            /// </summary>
            public RectTransform friendGo;
            /// <summary>
            /// Prefab for the friend list
            /// </summary>
            public GameObject friendPrefab;
            /// <summary>
            /// Currently active buttons
            /// </summary>
            private List<Kit_MenuFriendsButton> currentbuttons = new List<Kit_MenuFriendsButton>();
            /// <summary>
            /// When friend is in a room
            /// </summary>
            public Color friendInGameColor = Color.blue;
            /// <summary>
            /// When friend is online
            /// </summary>
            public Color friendOnlineColor = Color.green;
            /// <summary>
            /// When friend is offline
            /// </summary>
            public Color friendOfflineColor = Color.red;
            /// <summary>
            /// How much time between auto updates?
            /// </summary>
            public float updateInterval = 5f;

            /*
            /// <summary>
            /// Update 
            /// </summary>
            private float lastUpdate;

            public void UpdateFriends()
            {
                if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InLobby)
                {
                    if (myFriends.Count > 0)
                    {
                        //Send to photon
                        PhotonNetwork.FindFriends(myFriends.ToArray());
                    }
                    else
                    {
                        OnFriendListUpdate(new List<FriendInfo>());
                    }
                }
            }

            public void SaveFriends()
            {
                PlayerPrefs.SetInt("friendsFor" + Kit_GameSettings.userName + "Amount", myFriends.Count);

                for (int i = 0; i < myFriends.Count; i++)
                {
                    PlayerPrefs.SetString("friendsFor" + Kit_GameSettings.userName + "Index" + i, myFriends[i]);
                }
            }

            public void LoadFriends()
            {
                int amount = PlayerPrefs.GetInt("friendsFor" + Kit_GameSettings.userName + "Amount");

                myFriends = new List<string>();

                for (int i = 0; i < amount; i++)
                {
                    myFriends.Add( PlayerPrefs.GetString("friendsFor" + Kit_GameSettings.userName + "Index" + i));
                }
            }

            public override void AfterLogin()
            {
                LoadFriends();
                UpdateFriends();
            }

            public override void BeforeOpening()
            {
                UpdateFriends();
            }

            public override void OnFriendListUpdate(List<FriendInfo> friendList)
            {
                //Remove currents
                for (int i = 0; i < currentbuttons.Count; i++)
                {
                    Destroy(currentbuttons[i].gameObject);
                }

                currentbuttons = new List<Kit_MenuFriendsButton>();

                for (int i = 0; i < friendList.Count; i++)
                {
                    int id = i;
                    GameObject go = Instantiate(friendPrefab, friendGo, false);
                    //Get button
                    Kit_MenuFriendsButton btn = go.GetComponent<Kit_MenuFriendsButton>();

                    //Set name
                    btn.playerName.text = friendList[id].UserId;

                    if (friendList[i].IsOnline)
                    {
                        if (friendList[i].IsInRoom)
                        {
                            btn.onlineState.color = friendInGameColor;
                            btn.joinButton.gameObject.SetActive(true);
                            btn.joinButton.onClick.AddListener(delegate { JoinRoom(friendList[id].Room); });
                        }
                        else
                        {
                            btn.onlineState.color = friendOnlineColor;
                            btn.joinButton.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        btn.onlineState.color = friendOfflineColor;
                        btn.joinButton.gameObject.SetActive(false);
                    }

                    btn.removeButton.onClick.AddListener(delegate { RemoveFriend(friendList[id].UserId); });

                    //Add to list
                    currentbuttons.Add(btn);
                }
            }
            public void JoinRoom(string room)
            {
                PhotonNetwork.JoinRoom(room);
            }

            public void RemoveFriend(string str)
            {
                if (myFriends.Contains(str))
                {
                    myFriends.Remove(str);
                    UpdateFriends();
                    SaveFriends();
                }
            }

            public void AddFriend(string str)
            {
                if (!myFriends.Contains(str))
                {
                    myFriends.Add(str);
                    UpdateFriends();
                    SaveFriends();
                }
            }

            public void AddFriend()
            {
                if (!addFriendInput.text.IsNullOrWhiteSpace())
                {
                    //Call
                    AddFriend(addFriendInput.text);
                    //Reset input
                    addFriendInput.text = "";
                }
            }

            private void Update()
            {
                if (PhotonNetwork.IsConnectedAndReady && Time.time > lastUpdate)
                {
                    lastUpdate = Time.time + updateInterval;
                    UpdateFriends();
                }
            }
            */
        }
    }
}