using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        public class Kit_ModernWeaponManagerNetworkData : Kit_WeaponManagerNetworkBase
        {
            /// <summary>
            /// Our currently selected weapon; [0] = slot; [1] = weapon In Slot
            /// </summary>
            public int currentWeapon;
            [SyncVar]
            /// <summary>
            /// The weapon we want to select
            /// </summary>
            public int desiredWeapon;
            [System.NonSerialized]
            /// <summary>
            /// Desired weapon is locked (by plugin?)
            /// </summary>
            public bool isDesiredWeaponLocked;
            [SyncVar]
            /// <summary>
            /// Is a quick use in progress?
            /// </summary>
            public bool quickUseInProgress;
            [SyncVar]
            /// <summary>
            /// Quick use that we want to do!
            /// </summary>
            public int desiredQuickUse;
            [System.NonSerialized]
            /// <summary>
            /// Current state of quick use.
            /// </summary>
            public int quickUseState;
            [System.NonSerialized]
            /// <summary>
            /// When is the next quick use state over?
            /// </summary>
            public float quickUseOverAt;
            [SyncVar]
            /// <summary>
            /// Sync!
            /// </summary>
            public bool quickUseSyncButtonWaitOver;
            /// <summary>
            /// These are the network ids of our weapon's prefabs that co-exist with out player
            /// </summary>
            public readonly SyncList<uint> weaponsInUseSync = new SyncList<uint>();
            /// <summary>
            /// Stores the objects of our list
            /// </summary>
            public Kit_WeaponRuntimeDataBase[] weaponsInUseDataObjects = new Kit_WeaponRuntimeDataBase[0];
            [System.NonSerialized]
            /// <summary>
            /// Last states for the slot buttons!
            /// </summary>
            public bool[] lastInputIDs;
            [System.NonSerialized]
            /// <summary>
            /// Last state for the drop weapon
            /// </summary>
            public bool lastInteract;
            [System.NonSerialized]
            /// <summary>
            /// Are we currently switching weapons?
            /// </summary>
            public bool switchInProgress;
            [System.NonSerialized]
            /// <summary>
            /// When is the next switching phase over?
            /// </summary>
            public float switchNextEnd; //This is only so we don't have to use a coroutine
            [System.NonSerialized]
            /// <summary>
            /// The current phase of switching
            /// </summary>
            public int switchPhase;
            [System.NonSerialized]
            /// <summary>
            /// Raycast hit for the pickup process
            /// </summary>
            public RaycastHit hit;
            [System.NonSerialized]
            /// <summary>
            /// To fire the interaction end trigger
            /// We store the object we are currently interacting with
            /// </summary>
            public Kit_InteractableObject holdingInteractableObject;

            private void Start()
            {
                weaponsInUseSync.Callback += OnWeaponsInUseSyncCallback;
            }

            private void OnWeaponsInUseSyncCallback(SyncList<uint>.Operation op, int itemIndex, uint oldItem, uint newItem)
            {
                //Make sure its assigned
                //Shouldn't happen..
                //But just in case, it should fix itself.
                if (!ownerPlayer)
                {
                    if (NetworkServer.active)
                    {
                        if (NetworkServer.spawned.ContainsKey(ownerPlayerNetworkId))
                        {
                            Kit_PlayerBehaviour pb = NetworkServer.spawned[ownerPlayerNetworkId].GetComponent<Kit_PlayerBehaviour>();
                            pb.weaponManagerNetworkData = this;
                            ownerPlayer = pb;
                        }
                    }
                    else
                    {
                        if (NetworkClient.spawned.ContainsKey(ownerPlayerNetworkId))
                        {
                            Kit_PlayerBehaviour pb = NetworkClient.spawned[ownerPlayerNetworkId].GetComponent<Kit_PlayerBehaviour>();
                            pb.weaponManagerNetworkData = this;
                            ownerPlayer = pb;
                        }
                    }
                }

                if (ownerPlayer)
                {
                    ownerPlayer.weaponManager.OnWeaponsInUseSyncCallback(ownerPlayer, op, itemIndex, oldItem, newItem);
                }
            }

            public Kit_WeaponRuntimeDataBase GetWeapon(int index)
            {
                if (weaponsInUseDataObjects.Length == 0)
                {
                    //Rebuild list
                    for (int i = 0; i < weaponsInUseSync.Count; i++)
                    {
                        NetworkIdentity id = NetworkClient.spawned[weaponsInUseSync[i]];

                        if (id)
                        {
                            weaponsInUseDataObjects[i] = id.GetComponent<Kit_WeaponRuntimeDataBase>();
                        }
                        else
                        {
                            //Didn't find it. This shouldn't happen!
                            weaponsInUseDataObjects[i] = null;
                        }
                    }
                }

                return weaponsInUseDataObjects[index];
            }

            private void OnDestroy()
            {
                if (NetworkServer.active)
                {
                    for (int i = 0; i < weaponsInUseDataObjects.Length; i++)
                    {
                        if (weaponsInUseDataObjects[i])
                        {
                            NetworkServer.Destroy(weaponsInUseDataObjects[i].gameObject);
                        }
                    }
                }
            }

            #region IK
            [System.NonSerialized]
            /// <summary>
            /// Weight of the left hand IK
            /// </summary>
            public float leftHandIKWeight;
            #endregion
        }
    }
}