using Mirror;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        /// <summary>
        /// Input that is allowed for the given weapon slot
        /// </summary>
        [System.Serializable]
        public class WeaponManagerSlot
        {
            /// <summary>
            /// Can weapons in this slot be equipped?
            /// </summary>
            public bool enableEquipping = true;
            /// <summary>
            /// ID of <see cref="Kit_PlayerInput.weaponSlotUses"/>
            /// </summary>
            public int equippingInputID;
            /// <summary>
            /// Can weapons in this slot be quick used (e.g. quick grenades, quick knife)
            /// </summary>
            public bool enableQuickUse;
            /// <summary>
            /// ID of <see cref="Kit_PlayerInput.weaponSlotUses"/>
            /// </summary>
            public int quickUseInputID;
        }

        public enum DeadDrop { None, Selected, All }

        /// <summary>
        /// This is a modern, generic weapon manager. Weapons are put away and then taken out, like in COD or Battlefield. Supports an "infinite" amount of weapons
        /// </summary>
        [CreateAssetMenu(menuName = ("MarsFPSKit/Weapons/Modern Weapon Manager"))]
        public class Kit_ModernWeaponManager : Kit_WeaponManagerBase
        {
            public WeaponManagerSlot[] slotConfiguration;
            /// <summary>
            /// Main drop prefab!
            /// </summary>
            public GameObject dropPrefab;
            /// <summary>
            /// Layers that will be hit by the pickup raycast
            /// </summary>
            public LayerMask pickupLayers;
            /// <summary>
            /// Distance for the pickup raycast
            /// </summary>
            public float pickupDistance = 3f;
            /// <summary>
            /// Which weapons should be dropped upon death?
            /// </summary>
            public DeadDrop uponDeathDrop;
            /// <summary>
            /// How fast does the weapon position change?
            /// </summary>
            public float weaponPositionChangeSpeed = 5f;
            /// <summary>
            /// Can we switch while we are running?
            /// </summary>
            public bool allowSwitchingWhileRunning;
            /// <summary>
            /// Can we do quick use while running?
            /// </summary>
            public bool allowQuickUseWhileRunning;

            public override void OnWeaponsInUseSyncCallback(Kit_PlayerBehaviour pb, SyncList<uint>.Operation op, int itemIndex, uint oldItem, uint newItem)
            {
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;

                switch (op)
                {
                    case SyncList<uint>.Operation.OP_SET:
                        //Cleanup old data first
                        for (int i = 0; i < runtimeData.weaponsInUseDataObjects[itemIndex].instantiatedObjects.Count; i++)
                        {
                            if (runtimeData.weaponsInUseDataObjects[itemIndex].instantiatedObjects[i])
                            {
                                NetworkIdentity nid = runtimeData.weaponsInUseDataObjects[itemIndex].instantiatedObjects[i].GetComponentInChildren<NetworkIdentity>();

                                if (nid)
                                {
                                    if (NetworkServer.active)
                                    {
                                        NetworkServer.Destroy(nid.gameObject);
                                    }
                                }
                                else
                                {
                                    Destroy(runtimeData.weaponsInUseDataObjects[itemIndex].instantiatedObjects[i]);
                                }
                            }
                        }

                        runtimeData.weaponsInUseDataObjects[itemIndex].instantiatedObjects.Clear();

                        //We may need to wait a bit for the network
                        Kit_ScriptableObjectCoroutineHelper.instance.StartCoroutine(Kit_ScriptableObjectCoroutineHelper.instance.NetworkReplaceWeapon(pb, runtimeData, itemIndex, newItem));
                        break;
                }
            }

            public override void SetupSpawnData(Kit_PlayerBehaviour pb, Loadout loadout, NetworkConnectionToClient sender)
            {
                GameObject data = Instantiate(networkData, Vector3.zero, Quaternion.identity);
                Kit_ModernWeaponManagerNetworkData runtimeData = data.GetComponent<Kit_ModernWeaponManagerNetworkData>();
                pb.weaponManagerNetworkData = runtimeData;

                for (int i = 0; i < loadout.loadoutWeapons.Length; i++)
                {
                    GameObject spawnObject = Kit_IngameMain.instance.gameInformation.allWeapons[loadout.loadoutWeapons[i].weaponID].SetupSpawnData(pb, i, loadout.loadoutWeapons[i]);
                    NetworkServer.Spawn(spawnObject, sender);
                    NetworkIdentity id = spawnObject.GetComponent<NetworkIdentity>();

                    //Add to our weapon list
                    runtimeData.weaponsInUseSync.Add(id.netId);
                }
            }

            public override void SetupSpawnData(Kit_PlayerBehaviour pb, Loadout loadout)
            {
                GameObject data = Instantiate(networkData, Vector3.zero, Quaternion.identity);
                Kit_ModernWeaponManagerNetworkData runtimeData = data.GetComponent<Kit_ModernWeaponManagerNetworkData>();
                pb.weaponManagerNetworkData = runtimeData;

                for (int i = 0; i < loadout.loadoutWeapons.Length; i++)
                {
                    GameObject spawnObject = Kit_IngameMain.instance.gameInformation.allWeapons[loadout.loadoutWeapons[i].weaponID].SetupSpawnData(pb, i, loadout.loadoutWeapons[i]);
                    NetworkServer.Spawn(spawnObject);
                    NetworkIdentity id = spawnObject.GetComponent<NetworkIdentity>();

                    //Add to our weapon list
                    runtimeData.weaponsInUseSync.Add(id.netId);
                }
            }

            public override void InitializeServer(Kit_PlayerBehaviour pb)
            {
                //Is created in SetupSpawnData
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                runtimeData.ownerPlayerNetworkId = pb.netId;
                //Spawn it here
                if (pb.isBot)
                    NetworkServer.Spawn(runtimeData.gameObject);
                else
                    NetworkServer.Spawn(runtimeData.gameObject, pb.gameObject);

                //Setup input IDs
                runtimeData.lastInputIDs = new bool[pb.input.weaponSlotUses.Length];

                //Initially build list
                runtimeData.weaponsInUseDataObjects = new Kit_WeaponRuntimeDataBase[runtimeData.weaponsInUseSync.Count];

                for (int i = 0; i < runtimeData.weaponsInUseSync.Count; i++)
                {
                    NetworkIdentity id = null;

                    if (NetworkServer.active)
                    {
                        id = NetworkServer.spawned[runtimeData.weaponsInUseSync[i]];
                    }
                    else
                    {
                        id = NetworkClient.spawned[runtimeData.weaponsInUseSync[i]];
                    }

                    if (id)
                    {
                        runtimeData.weaponsInUseDataObjects[i] = id.GetComponent<Kit_WeaponRuntimeDataBase>();
                    }
                    else
                    {
                        //Didn't find it. This shouldn't happen!
                        runtimeData.weaponsInUseDataObjects[i] = null;
                    }
                }

                for (int i = 0; i < runtimeData.weaponsInUseDataObjects.Length; i++)
                {
                    int slot = i;
                    int id = runtimeData.weaponsInUseDataObjects[slot].id;
                    //Get their behaviour modules
                    Kit_WeaponBase weaponBehaviour = pb.gameInformation.allWeapons[id];
                    //Assign Behaviour
                    runtimeData.weaponsInUseDataObjects[slot].behaviour = weaponBehaviour;
                    //Setup FP
                    weaponBehaviour.SetupFirstPerson(pb, runtimeData.weaponsInUseDataObjects[slot]);
                    //Setup TP
                    weaponBehaviour.SetupThirdPerson(pb, runtimeData.weaponsInUseDataObjects[slot]);
                }

                SelectDefaultWeapon(pb, runtimeData);

                //Set time
                runtimeData.switchNextEnd = Time.time + runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.drawTime;
                //Set phase
                runtimeData.switchPhase = 1;
                //Set switching
                runtimeData.switchInProgress = true;
            }

            public override void InitializeClient(Kit_PlayerBehaviour pb)
            {
                if (!pb.weaponManagerNetworkData)
                {
                    //Find ours
                    pb.weaponManagerNetworkData = FindObjectsOfType<Kit_ModernWeaponManagerNetworkData>().Where(x => x.ownerPlayerNetworkId == pb.netId).FirstOrDefault();
                }

                if (pb.isClientOnly)
                {
                    Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;

                    //Setup input IDs
                    runtimeData.lastInputIDs = new bool[pb.input.weaponSlotUses.Length];

                    //Initially build list
                    runtimeData.weaponsInUseDataObjects = new Kit_WeaponRuntimeDataBase[runtimeData.weaponsInUseSync.Count];

                    for (int i = 0; i < runtimeData.weaponsInUseSync.Count; i++)
                    {
                        NetworkIdentity id = null;

                        if (NetworkServer.active)
                        {
                            id = NetworkServer.spawned[runtimeData.weaponsInUseSync[i]];
                        }
                        else
                        {
                            id = NetworkClient.spawned[runtimeData.weaponsInUseSync[i]];
                        }

                        if (id)
                        {
                            runtimeData.weaponsInUseDataObjects[i] = id.GetComponent<Kit_WeaponRuntimeDataBase>();
                        }
                        else
                        {
                            //Didn't find it. This shouldn't happen!
                            runtimeData.weaponsInUseDataObjects[i] = null;
                        }
                    }

                    for (int i = 0; i < runtimeData.weaponsInUseDataObjects.Length; i++)
                    {
                        int slot = i;
                        int id = runtimeData.weaponsInUseDataObjects[slot].id;
                        //Get their behaviour modules
                        Kit_WeaponBase weaponBehaviour = pb.gameInformation.allWeapons[id];
                        //Assign Behaviour
                        runtimeData.weaponsInUseDataObjects[slot].behaviour = weaponBehaviour;
                        //Setup FP
                        weaponBehaviour.SetupFirstPerson(pb, runtimeData.weaponsInUseDataObjects[slot]);
                        //Setup TP
                        weaponBehaviour.SetupThirdPerson(pb, runtimeData.weaponsInUseDataObjects[slot]);
                    }

                    SelectDefaultWeapon(pb, runtimeData);

                    //Set time
                    runtimeData.switchNextEnd = Time.time + runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.drawTime;
                    //Set phase
                    runtimeData.switchPhase = 1;
                    //Set switching
                    runtimeData.switchInProgress = true;
                }
            }

            /// <summary>
            /// Selects the first weapon
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="runtimeData"></param>
            void SelectDefaultWeapon(Kit_PlayerBehaviour pb, Kit_ModernWeaponManagerNetworkData runtimeData)
            {
                //Select default weapon
                for (int i = 0; i < runtimeData.weaponsInUseDataObjects.Length; i++)
                {
                    if (i < slotConfiguration.Length && slotConfiguration[i].enableEquipping)
                    {
                        if (runtimeData.weaponsInUseDataObjects[i].behaviour.CanBeSelected(pb, runtimeData.weaponsInUseDataObjects[i]))
                        {
                            int id = i;
                            //Select current weapon
                            runtimeData.weaponsInUseDataObjects[i].behaviour.DrawWeapon(pb, runtimeData.weaponsInUseDataObjects[i]);
                            //Play Third person animation
                            pb.thirdPersonPlayerModel.PlayWeaponChangeAnimation(runtimeData.weaponsInUseDataObjects[i].behaviour.thirdPersonAnimType, true, runtimeData.weaponsInUseDataObjects[i].behaviour.drawTime);
                            //Set current weapon
                            runtimeData.desiredWeapon = runtimeData.currentWeapon = id;
                            return;
                        }

                    }
                }
            }

            public override void TakeControl(Kit_PlayerBehaviour pb)
            {
                //Hide crosshair
                Kit_IngameMain.instance.hud.DisplayCrosshair(0f, false);

            }

            public override void ForceUnselectCurrentWeapon(Kit_PlayerBehaviour pb)
            {
                Debug.Log("[Weapon Manager] Forcing unselect of current weapon!");
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                bool foundWeapon = false;
                //Try to find next weapon
                int next = -1;
                for (int i = 0; i < runtimeData.weaponsInUseDataObjects.Length; i++)
                {
                    if (foundWeapon) break;


                    if (!runtimeData.weaponsInUseDataObjects[i].isInjectedFromPlugin)
                    {
                        //Check if this one works!
                        if (runtimeData.weaponsInUseDataObjects[i].behaviour.CanBeSelected(pb, runtimeData.weaponsInUseDataObjects[i]))
                        {
                            int id = i;
                            next = id;
                            //We found one
                            foundWeapon = true;
                            break;
                        }
                    }

                }

                //This should ALWAYS be true!
                if (next >= 0)
                {
                    runtimeData.desiredWeapon = next;
                    //Begin switch and skip putaway
                    runtimeData.switchInProgress = true;
                    //Set time (Because here we cannot use a coroutine)
                    runtimeData.switchNextEnd = 0f;
                    //Set phase
                    runtimeData.switchPhase = 0;
                    if (pb.isFirstPersonActive)
                    {
                        //Hide crosshair
                        Kit_IngameMain.instance.hud.DisplayCrosshair(0f, false);
                    }
                    //Set current one too!
                    runtimeData.currentWeapon = next;
                }
                else
                {
                    Debug.LogError("Could not find next weapon! This is not allowed!");
                }
            }

            public override void AuthorativeInput(Kit_PlayerBehaviour pb, Kit_PlayerInput input, float delta, double revertTime)
            {
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;

                if (Physics.Raycast(pb.playerCameraTransform.position, pb.playerCameraTransform.forward, out runtimeData.hit, pickupDistance, pickupLayers.value))
                {
                    if (runtimeData.hit.transform.root.GetComponent<Kit_DropBehaviour>())
                    {
                        Kit_DropBehaviour drop = runtimeData.hit.transform.root.GetComponent<Kit_DropBehaviour>();
                        if (pb.isFirstPersonActive)
                        {
                            Kit_IngameMain.instance.hud.DisplayWeaponPickup(true, drop.weaponID);
                            Kit_IngameMain.instance.hud.DisplayInteraction(false);
                        }

                        if (runtimeData.lastInteract != input.interact && pb.isServer)
                        {
                            runtimeData.lastInteract = input.interact;
                            if (input.interact && (allowSwitchingWhileRunning || !pb.movement.IsRunning(pb)))
                            {
                                int slot = 0;

                                if (Kit_IngameMain.instance.gameInformation.allWeapons[drop.weaponID].canFitIntoSlots.Contains(runtimeData.currentWeapon))
                                {
                                    slot = runtimeData.currentWeapon;
                                }
                                else
                                {
                                    slot = Kit_IngameMain.instance.gameInformation.allWeapons[drop.weaponID].canFitIntoSlots[0];
                                }

                                //Check if we can drop
                                if (!drop.isSceneOwned || drop.isSceneOwned && Kit_IngameMain.instance.gameInformation.enableDropWeaponOnSceneSpawnedWeapons)
                                {
                                    //First drop our weapon
                                    DropWeapon(pb, slot, drop.transform);
                                }

                                pb.ReplaceWeapon(slot, drop.weaponID, drop.bulletsLeft, drop.bulletsLeftToReload, drop.attachments.ToArray());

                                drop.PickedUp();
                            }
                        }
                    }
                    else if (runtimeData.hit.transform.GetComponentInParent<Kit_InteractableObject>())
                    {
                        Kit_InteractableObject io = runtimeData.hit.transform.GetComponentInParent<Kit_InteractableObject>();

                        if (io.CanInteract(pb))
                        {
                            if (pb.isFirstPersonActive)
                            {
                                Kit_IngameMain.instance.hud.DisplayWeaponPickup(false);
                                Kit_IngameMain.instance.hud.DisplayInteraction(true, io.interactionText);
                            }

                            if (io.IsHold())
                            {
                                if (input.interact)
                                {
                                    //Store object
                                    runtimeData.holdingInteractableObject = io;

                                    //Tell object we want to interact
                                    io.Interact(pb);
                                }
                                else
                                {
                                    if (runtimeData.holdingInteractableObject)
                                    {
                                        runtimeData.holdingInteractableObject.InteractHoldEnd(pb);
                                        runtimeData.holdingInteractableObject = null;
                                    }
                                }
                            }
                            else
                            {
                                if (runtimeData.lastInteract != input.interact)
                                {
                                    runtimeData.lastInteract = input.interact;
                                    if (input.interact && NetworkServer.active)
                                    {
                                        //Tell object we want to interact
                                        io.Interact(pb);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (pb.isFirstPersonActive)
                            {
                                Kit_IngameMain.instance.hud.DisplayWeaponPickup(false);
                                Kit_IngameMain.instance.hud.DisplayInteraction(false);
                            }

                            if (runtimeData.holdingInteractableObject)
                            {
                                runtimeData.holdingInteractableObject.InteractHoldEnd(pb);
                                runtimeData.holdingInteractableObject = null;
                            }
                        }
                    }
                    else
                    {
                        if (pb.isFirstPersonActive)
                        {
                            Kit_IngameMain.instance.hud.DisplayWeaponPickup(false);
                            Kit_IngameMain.instance.hud.DisplayInteraction(false);
                        }

                        if (runtimeData.holdingInteractableObject)
                        {
                            runtimeData.holdingInteractableObject.InteractHoldEnd(pb);
                            runtimeData.holdingInteractableObject = null;
                        }
                    }
                }
                else
                {
                    if (pb.isFirstPersonActive)
                    {
                        Kit_IngameMain.instance.hud.DisplayWeaponPickup(false);
                        Kit_IngameMain.instance.hud.DisplayInteraction(false);
                    }

                    if (runtimeData.holdingInteractableObject)
                    {
                        runtimeData.holdingInteractableObject.InteractHoldEnd(pb);
                        runtimeData.holdingInteractableObject = null;
                    }
                }

                if (!runtimeData.isDesiredWeaponLocked)
                {
                    for (int i = 0; i < runtimeData.weaponsInUseDataObjects.Length; i++)
                    {
                        if (!runtimeData.weaponsInUseDataObjects[i].isInjectedFromPlugin)
                        {
                            if (slotConfiguration.Length > i)
                            {
                                if (slotConfiguration[i].enableEquipping && slotConfiguration[i].equippingInputID >= 0 && !runtimeData.quickUseInProgress)
                                {
                                    if (runtimeData.lastInputIDs[slotConfiguration[i].equippingInputID] != input.weaponSlotUses[slotConfiguration[i].equippingInputID])
                                    {
                                        runtimeData.lastInputIDs[slotConfiguration[i].equippingInputID] = input.weaponSlotUses[slotConfiguration[i].equippingInputID];
                                        //Check for input
                                        if (input.weaponSlotUses[slotConfiguration[i].equippingInputID] && (allowSwitchingWhileRunning || !pb.movement.IsRunning(pb)))
                                        {
                                            int id = i;
                                            if (runtimeData.desiredWeapon != id)
                                            {
                                                if (runtimeData.weaponsInUseDataObjects[i].behaviour.CanBeSelected(pb, runtimeData.weaponsInUseDataObjects[i]))
                                                {
                                                    runtimeData.desiredWeapon = id;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //Check if we can do a quick use!
                    if (runtimeData.currentWeapon == runtimeData.desiredWeapon && !runtimeData.switchInProgress)
                    {
                        if (!runtimeData.quickUseInProgress)
                        {
                            for (int i = 0; i < runtimeData.weaponsInUseDataObjects.Length; i++)
                            {
                                if (!runtimeData.weaponsInUseDataObjects[i].isInjectedFromPlugin)
                                {
                                    int slot = i;
                                    if (slotConfiguration.Length > slot)
                                    {
                                        if (slotConfiguration[slot].enableQuickUse)
                                        {
                                            if (slotConfiguration[slot].quickUseInputID >= 0)
                                            {
                                                if (runtimeData.lastInputIDs[slotConfiguration[slot].quickUseInputID] != input.weaponSlotUses[slotConfiguration[slot].quickUseInputID])
                                                {
                                                    runtimeData.lastInputIDs[slotConfiguration[slot].quickUseInputID] = input.weaponSlotUses[slotConfiguration[slot].quickUseInputID];
                                                    //Check for input
                                                    if (input.weaponSlotUses[slotConfiguration[slot].quickUseInputID] && (allowQuickUseWhileRunning || !pb.movement.IsRunning(pb)))
                                                    {
                                                        if (runtimeData.weaponsInUseDataObjects[slot].behaviour.SupportsQuickUse(pb, runtimeData.weaponsInUseDataObjects[slot]))
                                                        {
                                                            runtimeData.desiredQuickUse = slot;
                                                            runtimeData.quickUseInProgress = true;
                                                            //Also reset these!
                                                            runtimeData.quickUseState = 0;
                                                            runtimeData.quickUseOverAt = Time.time;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (runtimeData.quickUseState == 2)
                            {
                                //Check if quick use button needs to be released
                                if (runtimeData.weaponsInUseDataObjects[runtimeData.desiredQuickUse].behaviour.WaitForQuickUseButtonRelease(pb, runtimeData.weaponsInUseDataObjects[runtimeData.desiredQuickUse]) && (!pb.input.weaponSlotUses[slotConfiguration[runtimeData.desiredQuickUse].quickUseInputID]))
                                {
                                    runtimeData.quickUseSyncButtonWaitOver = true;
                                }
                            }
                        }
                    }
                }

                if (!runtimeData.switchInProgress && !runtimeData.quickUseInProgress)
                {
                    runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.AuthorativeInput(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon], input, delta, revertTime);
                }
            }

            public override void PredictionInput(Kit_PlayerBehaviour pb, Kit_PlayerInput input, float delta)
            {
                AuthorativeInput(pb, input, delta, 0f);
            }

            public override void CustomUpdate(Kit_PlayerBehaviour pb, double revertBy)
            {
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;

                //Quick use has priority!
                if ((runtimeData.quickUseInProgress || runtimeData.quickUseState > 0) && runtimeData.desiredQuickUse >= 0)
                {
                    //Update weapon animation
                    runtimeData.weaponsInUseDataObjects[runtimeData.desiredQuickUse].behaviour.AnimateWeapon(pb, runtimeData.weaponsInUseDataObjects[runtimeData.desiredQuickUse], pb.movement.GetCurrentWeaponMoveAnimation(pb), pb.movement.GetCurrentWalkAnimationSpeed(pb));

                    if (Time.time >= runtimeData.quickUseOverAt)
                    {
                        //First, put away current weapon!
                        if (runtimeData.quickUseState == 0)
                        {
                            if (!runtimeData.weaponsInUseDataObjects[runtimeData.desiredQuickUse].behaviour.QuickUseSkipsPutaway(pb, runtimeData.weaponsInUseDataObjects[runtimeData.desiredQuickUse]))
                            {
                                //Set time (Because here we cannot use a coroutine)
                                runtimeData.quickUseOverAt = Time.time + runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.putawayTime;
                                //Set phase
                                runtimeData.quickUseState = 1;
                                //Start putaway
                                runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.PutawayWeapon(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]);
                                //Play Third person animation
                                pb.thirdPersonPlayerModel.PlayWeaponChangeAnimation(runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.thirdPersonAnimType, false, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.putawayTime);
                                if (pb.isFirstPersonActive)
                                {
                                    //Hide crosshair
                                    Kit_IngameMain.instance.hud.DisplayCrosshair(0f, false);
                                }
                            }
                            else
                            {
                                //Set phase
                                runtimeData.quickUseState = 1;
                                if (pb.isFirstPersonActive)
                                {
                                    //Hide crosshair
                                    Kit_IngameMain.instance.hud.DisplayCrosshair(0f, false);
                                }
                            }
                        }
                        else if (runtimeData.quickUseState == 1)
                        {
                            //Weapon has been put away, hide weapon
                            runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.PutawayWeaponHide(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]);
                            runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.QuickUseOnOtherWeaponBegin(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]);

                            //Set state
                            runtimeData.quickUseState = 2;

                            //Begin quick use....
                            runtimeData.quickUseOverAt = Time.time + runtimeData.weaponsInUseDataObjects[runtimeData.desiredQuickUse].behaviour.BeginQuickUse(pb, runtimeData.weaponsInUseDataObjects[runtimeData.desiredQuickUse], (float)revertBy);
                        }
                        else if (runtimeData.quickUseState == 2)
                        {
                            //Check if weapon wants to abort quick use
                            if (!runtimeData.weaponsInUseDataObjects[runtimeData.desiredQuickUse].behaviour.SupportsQuickUse(pb, runtimeData.weaponsInUseDataObjects[runtimeData.desiredQuickUse]) ||
                                //Check if we don't need to wait for the button
                                (!runtimeData.weaponsInUseDataObjects[runtimeData.desiredQuickUse].behaviour.WaitForQuickUseButtonRelease(pb, runtimeData.weaponsInUseDataObjects[runtimeData.desiredQuickUse])) ||
                                //Or button was released via sync
                                (runtimeData.quickUseSyncButtonWaitOver))
                            {
                                runtimeData.quickUseSyncButtonWaitOver = true;
                                //Set State
                                runtimeData.quickUseState = 3;
                                //End quick use...
                                runtimeData.quickUseOverAt = Time.time + runtimeData.weaponsInUseDataObjects[runtimeData.desiredQuickUse].behaviour.EndQuickUse(pb, runtimeData.weaponsInUseDataObjects[runtimeData.desiredQuickUse], (float)revertBy);
                            }
                        }
                        else if (runtimeData.quickUseState == 3)
                        {
                            //Hide Quick Use!
                            runtimeData.weaponsInUseDataObjects[runtimeData.desiredQuickUse].behaviour.EndQuickUseAfter(pb, runtimeData.weaponsInUseDataObjects[runtimeData.desiredQuickUse], (float)revertBy);
                            //Check if currently selected  weapon is valid.
                            if (runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.CanBeSelected(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]))
                            {
                                //Set weapon
                                runtimeData.currentWeapon = runtimeData.desiredWeapon;
                            }
                            else
                            {
                                //Its not, find a new one
                                int next = 0;
                                for (int i = 0; i < runtimeData.weaponsInUseDataObjects.Length; i++)
                                {
                                    //Check if this one works!
                                    if (runtimeData.weaponsInUseDataObjects[i].behaviour.CanBeSelected(pb, runtimeData.weaponsInUseDataObjects[i]))
                                    {
                                        int id = i;
                                        next = id;
                                        //We found one
                                        break;
                                    }
                                }

                                //This should ALWAYS be true!
                                if (next >= 0)
                                {
                                    runtimeData.desiredWeapon = next;
                                    //Set current one too!
                                    runtimeData.currentWeapon = next;
                                }
                                else
                                {
                                    Debug.LogError("Could not find next weapon! This is not allowed!");
                                }
                            }

                            //Draw that weapon
                            runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.DrawWeapon(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]);
                            //Play Third person animation
                            pb.thirdPersonPlayerModel.PlayWeaponChangeAnimation(runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.thirdPersonAnimType, true, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.drawTime);
                            //Set phase
                            runtimeData.quickUseState = 4;
                            //Set time
                            runtimeData.quickUseOverAt = Time.time + runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.drawTime;
                            //Done, now wait
                        }
                        else if (runtimeData.quickUseState == 4)
                        {
                            //End quick use
                            runtimeData.quickUseInProgress = false;
                            runtimeData.desiredQuickUse = -1;
                            runtimeData.quickUseSyncButtonWaitOver = false;
                            runtimeData.quickUseState = 0;
                            runtimeData.quickUseOverAt = Time.time;

                            //Also reset switching just to be sure!
                            runtimeData.switchPhase = 0;
                            runtimeData.switchNextEnd = 0f;
                            runtimeData.switchInProgress = false;
                        }
                    }
                }
                else
                {
                    //Update weapon animation
                    runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.AnimateWeapon(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon], pb.movement.GetCurrentWeaponMoveAnimation(pb), pb.movement.GetCurrentWalkAnimationSpeed(pb));

                    if (!runtimeData.switchInProgress)
                    {
                        //If we aren't switching weapons, update weapon behaviour
                        runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.CalculateWeaponUpdate(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]);

                        //Check if we want to select a different weapon
                        if (runtimeData.desiredWeapon != runtimeData.currentWeapon)
                        {
                            //If not, start to switch
                            runtimeData.switchInProgress = true;
                            //Set time (Because here we cannot use a coroutine)
                            runtimeData.switchNextEnd = Time.time + runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.putawayTime;
                            //Set phase
                            runtimeData.switchPhase = 0;
                            //Start putaway
                            runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.PutawayWeapon(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]);
                            //Play Third person animation
                            pb.thirdPersonPlayerModel.PlayWeaponChangeAnimation(runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.thirdPersonAnimType, false, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.putawayTime);
                            if (pb.isFirstPersonActive)
                            {
                                //Hide crosshair
                                Kit_IngameMain.instance.hud.DisplayCrosshair(0f, false);
                            }
                        }
                    }
                    else
                    {
                        //Switching, courtine less
                        #region Switching
                        //Check for time
                        if (Time.time >= runtimeData.switchNextEnd)
                        {
                            //Time is over, check which phase is next
                            if (runtimeData.switchPhase == 0)
                            {
                                //Weapon has been put away, hide weapon
                                runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.PutawayWeaponHide(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]);

                                runtimeData.currentWeapon = runtimeData.desiredWeapon;

                                //Draw that weapon
                                runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.DrawWeapon(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]);
                                //Set phase
                                runtimeData.switchPhase = 1;
                                //Set time
                                runtimeData.switchNextEnd = Time.time + runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.drawTime;
                                //Play Third person animation
                                pb.thirdPersonPlayerModel.PlayWeaponChangeAnimation(runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.thirdPersonAnimType, true, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.drawTime);
                                //Done, now wait
                            }
                            else if (runtimeData.switchPhase == 1)
                            {
                                //Switching is over
                                runtimeData.switchPhase = 0;
                                runtimeData.switchNextEnd = 0f;
                                runtimeData.switchInProgress = false;
                            }
                        }
                        #endregion
                    }
                }

                //Move weapons transform
                pb.weaponsGo.localPosition = Vector3.Lerp(pb.weaponsGo.localPosition, Vector3.zero + pb.looking.GetWeaponOffset(pb), Time.deltaTime * weaponPositionChangeSpeed);

                //Move weapons transform
                pb.weaponsGo.localRotation = Quaternion.Slerp(pb.weaponsGo.localRotation, pb.looking.GetWeaponRotationOffset(pb), Time.deltaTime * weaponPositionChangeSpeed);

                if (pb.isFirstPersonActive)
                {
                    Kit_IngameMain.instance.hud.DisplayWeaponsAndQuickUses(pb, runtimeData);
                }
            }

            public override void PlayerDead(Kit_PlayerBehaviour pb)
            {
                if (NetworkServer.active && (Kit_IngameMain.instance.currentPvPGameModeBehaviour && Kit_IngameMain.instance.currentPvPGameModeBehaviour.CanDropWeapons()) || (Kit_IngameMain.instance.currentPvEGameModeBehaviour && Kit_IngameMain.instance.currentPvEGameModeBehaviour.CanDropWeapons()))
                {
                    Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                    if (uponDeathDrop == DeadDrop.Selected)
                    {
                        DropWeaponDead(pb, runtimeData.currentWeapon);
                    }
                    else if (uponDeathDrop == DeadDrop.All)
                    {
                        for (int i = 0; i < runtimeData.weaponsInUseDataObjects.Length; i++)
                        {
                            if (!runtimeData.weaponsInUseDataObjects[i].isInjectedFromPlugin)
                            {
                                DropWeaponDead(pb, i);
                            }
                        }
                    }

                }
            }

            public override void OnAnimatorIKCallback(Kit_PlayerBehaviour pb, Animator anim)
            {
                //Get runtime data
                if (pb.isLocalPlayer)
                {
                    Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                    if (anim)
                    {
                        //Get Weapon IK
                        WeaponIKValues ikv = null;

                        if (runtimeData.quickUseInProgress && runtimeData.quickUseState > 0 && runtimeData.quickUseState < 4)
                        {
                            ikv = runtimeData.weaponsInUseDataObjects[runtimeData.desiredQuickUse].behaviour.GetIK(pb, anim, runtimeData.weaponsInUseDataObjects[runtimeData.desiredQuickUse]);
                        }
                        else
                        {
                            ikv = runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.GetIK(pb, anim, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]);
                        }

                        if (ikv != null)
                        {
                            if (ikv.leftHandIK)
                            {
                                anim.SetIKPosition(AvatarIKGoal.LeftHand, ikv.leftHandIK.position);
                                anim.SetIKRotation(AvatarIKGoal.LeftHand, ikv.leftHandIK.rotation);
                            }
                            if (!runtimeData.switchInProgress && ikv.canUseIK && ikv.leftHandIK)
                            {
                                runtimeData.leftHandIKWeight = Mathf.Lerp(runtimeData.leftHandIKWeight, 1f, Time.deltaTime * 3);
                            }
                            else
                            {
                                runtimeData.leftHandIKWeight = Mathf.Lerp(runtimeData.leftHandIKWeight, 0f, Time.deltaTime * 20);
                            }
                            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, runtimeData.leftHandIKWeight);
                            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, runtimeData.leftHandIKWeight);
                        }
                        else
                        {
                            runtimeData.leftHandIKWeight = Mathf.Lerp(runtimeData.leftHandIKWeight, 0f, Time.deltaTime * 20);
                            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, runtimeData.leftHandIKWeight);
                            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, runtimeData.leftHandIKWeight);
                        }
                    }

                }
                else
                {
                    Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                    if (anim)
                    {
                        //Get Weapon IK
                        WeaponIKValues ikv = null;

                        if (runtimeData.quickUseInProgress && runtimeData.quickUseState > 0 && runtimeData.quickUseState < 4)
                        {
                            ikv = runtimeData.weaponsInUseDataObjects[runtimeData.desiredQuickUse].behaviour.GetIK(pb, anim, runtimeData.weaponsInUseDataObjects[runtimeData.desiredQuickUse]);
                        }
                        else
                        {
                            ikv = runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.GetIK(pb, anim, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]);
                        }

                        if (ikv != null)
                        {
                            if (ikv.leftHandIK)
                            {
                                anim.SetIKPosition(AvatarIKGoal.LeftHand, ikv.leftHandIK.position);
                                anim.SetIKRotation(AvatarIKGoal.LeftHand, ikv.leftHandIK.rotation);
                            }
                            if (!runtimeData.switchInProgress && ikv.canUseIK && ikv.leftHandIK)
                            {
                                runtimeData.leftHandIKWeight = Mathf.Lerp(runtimeData.leftHandIKWeight, 1f, Time.deltaTime * 3);
                            }
                            else
                            {
                                runtimeData.leftHandIKWeight = Mathf.Lerp(runtimeData.leftHandIKWeight, 0f, Time.deltaTime * 20);
                            }
                            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, runtimeData.leftHandIKWeight);
                            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, runtimeData.leftHandIKWeight);
                        }
                        else
                        {
                            runtimeData.leftHandIKWeight = Mathf.Lerp(runtimeData.leftHandIKWeight, 0f, Time.deltaTime * 20);
                            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, runtimeData.leftHandIKWeight);
                            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, runtimeData.leftHandIKWeight);
                        }
                    }

                }
            }

            public override void FallDownEffect(Kit_PlayerBehaviour pb, bool wasFallDamageApplied)
            {
                if (pb.isBot) return;
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                if (runtimeData.weaponsInUseDataObjects.Length <= runtimeData.currentWeapon) return;
                runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.FallDownEffect(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon], wasFallDamageApplied);

            }

            public override void OnControllerColliderHitRelay(Kit_PlayerBehaviour pb, ControllerColliderHit hit)
            {
                if (!pb.weaponManagerNetworkData) return;
                //Get runtime data
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                if (runtimeData.weaponsInUseDataObjects.Length <= runtimeData.currentWeapon) return;
                runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.OnControllerColliderHitCallback(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon], hit);
            }

            public override void NetworkSemiRPCReceived(Kit_PlayerBehaviour pb)
            {
                if (!pb.weaponManagerNetworkData) return;
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                if (runtimeData.weaponsInUseDataObjects.Length <= runtimeData.currentWeapon) return;
                //Relay to weapon script
                runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.NetworkSemiRPCReceived(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]);
            }

            public override void NetworkBoltActionRPCReceived(Kit_PlayerBehaviour pb, int state)
            {
                if (!pb.weaponManagerNetworkData) return;
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                if (runtimeData.weaponsInUseDataObjects.Length <= runtimeData.currentWeapon) return;
                //Relay to weapon script
                runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.NetworkBoltActionRPCReceived(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon], state);
            }

            public override void NetworkBurstRPCReceived(Kit_PlayerBehaviour pb, int burstLength)
            {
                if (!pb.weaponManagerNetworkData) return;
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                if (runtimeData.weaponsInUseDataObjects.Length <= runtimeData.currentWeapon) return;
                //Relay to weapon script
                runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.NetworkBurstRPCReceived(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon], burstLength);
            }

            public override void NetworkReloadRPCReceived(Kit_PlayerBehaviour pb, bool isEmpty)
            {
                if (!pb.weaponManagerNetworkData) return;
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                if (runtimeData.weaponsInUseDataObjects.Length <= runtimeData.currentWeapon) return;
                //Relay to weapon script
                runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.NetworkReloadRPCReceived(pb, isEmpty, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]);

            }

            public override void NetworkProceduralReloadRPCReceived(Kit_PlayerBehaviour pb, int stage)
            {
                if (!pb.weaponManagerNetworkData) return;
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                if (runtimeData.weaponsInUseDataObjects.Length <= runtimeData.currentWeapon) return;
                //Relay to weapon script
                runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.NetworkProceduralReloadRPCReceived(pb, stage, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]);

            }

            public override void NetworkMeleeChargeRPCReceived(Kit_PlayerBehaviour pb, int state, int slot)
            {
                if (!pb.weaponManagerNetworkData) return;
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                if (runtimeData.weaponsInUseDataObjects.Length <= runtimeData.currentWeapon) return;
                //Relay to weapon script
                runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.NetworkMeleeChargeRPCReceived(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon], state, slot);

            }

            public override void NetworkMeleeHealRPCReceived(Kit_PlayerBehaviour pb, int slot)
            {
                if (!pb.weaponManagerNetworkData) return;
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                if (runtimeData.weaponsInUseDataObjects.Length <= runtimeData.currentWeapon) return;
                //Relay to weapon script
                runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.NetworkMeleeHealRPCReceived(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon], slot);
            }

            public override void NetworkMeleeStabRPCReceived(Kit_PlayerBehaviour pb, int state, int slot)
            {
                if (!pb.weaponManagerNetworkData) return;
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                if (runtimeData.weaponsInUseDataObjects.Length <= runtimeData.currentWeapon) return;
                //Relay to weapon script
                runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.NetworkMeleeStabRPCReceived(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon], state, slot);
            }

            public override void NetworkGrenadePullPinRPCReceived(Kit_PlayerBehaviour pb)
            {
                if (!pb.weaponManagerNetworkData) return;
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                if (runtimeData.weaponsInUseDataObjects.Length <= runtimeData.currentWeapon) return;
                //Relay to weapon script
                runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.NetworkGrenadePullPinRPCReceived(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]);
            }

            public override void NetworkGrenadeThrowRPCReceived(Kit_PlayerBehaviour pb)
            {
                if (!pb.weaponManagerNetworkData) return;
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                if (runtimeData.weaponsInUseDataObjects.Length <= runtimeData.currentWeapon) return;
                //Relay to weapon script
                runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.NetworkGrenadeThrowRPCReceived(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]);
            }

            public override float GetAimingPercentage(Kit_PlayerBehaviour pb)
            {
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                return runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.GetAimingPercentage(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]); //Relay to weapon script
            }

            public override bool IsAiming(Kit_PlayerBehaviour pb)
            {
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                return runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.IsWeaponAiming(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]); //Relay to weapon script
            }

            public override float AimInTime(Kit_PlayerBehaviour pb)
            {
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                return runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.AimInTime(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]); //Relay to weapon script
            }

            public override bool ForceIntoFirstPerson(Kit_PlayerBehaviour pb)
            {
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                return runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.ForceIntoFirstPerson(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]); //Relay to weapon script
            }

            public override bool CanRun(Kit_PlayerBehaviour pb)
            {
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                if (allowSwitchingWhileRunning) return true;
                else return !runtimeData.switchInProgress;
            }

            public override float CurrentMovementMultiplier(Kit_PlayerBehaviour pb)
            {
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                return runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.SpeedMultiplier(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]); //Relay to weapon script
            }

            public override float CurrentSensitivity(Kit_PlayerBehaviour pb)
            {
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                return runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.Sensitivity(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]); //Relay to weapon script
            }

            public override int GetCurrentWeapon(Kit_PlayerBehaviour pb)
            {
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                //Just return ID
                return runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].id;
            }

            public override int HasWeapon(Kit_PlayerBehaviour pb, int id)
            {
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;

                for (int i = 0; i < runtimeData.weaponsInUseDataObjects.Length; i++)
                {
                    if (runtimeData.weaponsInUseDataObjects[i].id == id)
                    {
                        return i;
                    }
                }

                return -1;
            }

            public override Kit_WeaponRuntimeDataBase GetRuntimeDataOfWeapon(Kit_PlayerBehaviour pb, int slot)
            {
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                //Just return ID
                return runtimeData.weaponsInUseDataObjects[slot];
            }

            public override bool CanBuyWeapon(Kit_PlayerBehaviour pb, int id)
            {
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;

                for (int i = 0; i < runtimeData.weaponsInUseDataObjects.Length; i++)
                {
                    if (runtimeData.weaponsInUseDataObjects[i].id == id)
                    {
                        //Special case for grenades - let us buy more grenades if we are below that #
                        if (runtimeData.weaponsInUseDataObjects[i] != null && runtimeData.weaponsInUseDataObjects[i].GetType() == typeof(Kit_ModernGrenadeScriptRuntimeData))
                        {
                            Kit_ModernGrenadeScriptRuntimeData gcrd = runtimeData.weaponsInUseDataObjects[i] as Kit_ModernGrenadeScriptRuntimeData;
                            Kit_ModernGrenadeScript grenadeData = runtimeData.weaponsInUseDataObjects[i].behaviour as Kit_ModernGrenadeScript;

                            if (gcrd.amountOfGrenadesLeft < grenadeData.amountOfGrenadesAtStart)
                            {
                                return true;
                            }

                        }

                        return false;
                    }

                }

                //Can buy
                return true;
            }

            public override void NetworkReplaceWeapon(Kit_PlayerBehaviour pb, int slot, int weapon, int bulletsLeft, int bulletsLeftToReload, int[] attachments)
            {
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;

                GameObject spawnObject = Kit_IngameMain.instance.gameInformation.allWeapons[weapon].SetupSpawnData(pb, slot, new LoadoutWeapon { weaponID = weapon, attachments = attachments });

                Kit_WeaponRuntimeDataBase wrdb = spawnObject.GetComponent<Kit_WeaponRuntimeDataBase>();

                if (wrdb is Kit_ModernWeaponScriptRuntimeData)
                {
                    Kit_ModernWeaponScriptRuntimeData mwsrd = wrdb as Kit_ModernWeaponScriptRuntimeData;
                    mwsrd.bulletsLeft = bulletsLeft;
                    mwsrd.bulletsLeftToReload = bulletsLeftToReload;
                }

                NetworkServer.Spawn(spawnObject, pb.connectionToClient);
                NetworkIdentity id = spawnObject.GetComponent<NetworkIdentity>();

                NetworkIdentity oldIdentity = NetworkServer.spawned[runtimeData.weaponsInUseSync[slot]];

                if (oldIdentity)
                {
                    NetworkServer.Destroy(oldIdentity.gameObject);
                }

                runtimeData.weaponsInUseSync[slot] = id.netId;
            }

            public override void NetworkPhysicalBulletFired(Kit_PlayerBehaviour pb, Vector3 pos, Vector3 dir)
            {
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                //Relay to weapon script
                runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.NetworkPhysicalBulletFired(pb, pos, dir, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]);
            }

            public void DropWeapon(Kit_PlayerBehaviour pb, int slot)
            {
                if ((Kit_IngameMain.instance.currentPvPGameModeBehaviour && Kit_IngameMain.instance.currentPvPGameModeBehaviour.CanDropWeapons()) || (Kit_IngameMain.instance.currentPvEGameModeBehaviour && Kit_IngameMain.instance.currentPvEGameModeBehaviour.CanDropWeapons()))
                {
                    if (pb.weaponManagerNetworkData != null && pb.weaponManagerNetworkData.GetType() == typeof(Kit_ModernWeaponManagerNetworkData))
                    {
                        //Get the manager's runtime data
                        Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                        if (runtimeData.weaponsInUseDataObjects[slot] != null && runtimeData.weaponsInUseDataObjects[slot].GetType() == typeof(Kit_ModernWeaponScriptRuntimeData))
                        {
                            //Get the weapon's runtime data
                            Kit_ModernWeaponScriptRuntimeData wepData = runtimeData.weaponsInUseDataObjects[slot] as Kit_ModernWeaponScriptRuntimeData;
                            //Get the Scriptable object
                            Kit_ModernWeaponScript wepScript = runtimeData.weaponsInUseDataObjects[slot].behaviour as Kit_ModernWeaponScript;
                            //Instantiate
                            GameObject go = Instantiate(dropPrefab, pb.playerCameraTransform.position, pb.playerCameraTransform.rotation);
                            Kit_DropBehaviour dp = go.GetComponent<Kit_DropBehaviour>();
                            dp.isSceneOwned = true;
                            dp.weaponID = wepScript.gameGunID;
                            dp.bulletsLeft = wepData.bulletsLeft;
                            dp.bulletsLeftToReload = wepData.bulletsLeftToReload;

                            for (int i = 0; i < wepData.attachments.Count; i++)
                            {
                                dp.attachments.Add(wepData.attachments[i]);
                            }

                            NetworkServer.Spawn(go);
                        }
                        else if (runtimeData.weaponsInUseDataObjects[slot] != null && runtimeData.weaponsInUseDataObjects[slot].GetType() == typeof(Kit_ModernMeleeScriptRuntimeData))
                        {
                            //Get the weapon's runtime data
                            Kit_ModernMeleeScriptRuntimeData wepData = runtimeData.weaponsInUseDataObjects[slot] as Kit_ModernMeleeScriptRuntimeData;
                            //Get the Scriptable object
                            Kit_ModernMeleeScript wepScript = runtimeData.weaponsInUseDataObjects[slot].behaviour as Kit_ModernMeleeScript;
                            //Instantiate
                            GameObject go = Instantiate(dropPrefab, pb.playerCameraTransform.position, pb.playerCameraTransform.rotation);
                            Kit_DropBehaviour dp = go.GetComponent<Kit_DropBehaviour>();
                            dp.isSceneOwned = true;
                            dp.weaponID = wepScript.gameGunID;
                            dp.bulletsLeft = 0;
                            dp.bulletsLeftToReload = 0;

                            for (int i = 0; i < wepData.attachments.Count; i++)
                            {
                                dp.attachments.Add(wepData.attachments[i]);
                            }

                            NetworkServer.Spawn(go);
                        }
                        else if (runtimeData.weaponsInUseDataObjects[slot] != null && runtimeData.weaponsInUseDataObjects[slot].GetType() == typeof(Kit_ModernGrenadeScriptRuntimeData))
                        {
                            //Get the weapon's runtime data
                            Kit_ModernGrenadeScriptRuntimeData wepData = runtimeData.weaponsInUseDataObjects[slot] as Kit_ModernGrenadeScriptRuntimeData;
                            if (wepData.amountOfGrenadesLeft <= 0) return;
                            //Get the Scriptable object
                            Kit_ModernGrenadeScript wepScript = runtimeData.weaponsInUseDataObjects[slot].behaviour as Kit_ModernGrenadeScript;
                            //Instantiate
                            GameObject go = Instantiate(dropPrefab, pb.playerCameraTransform.position, pb.playerCameraTransform.rotation);
                            Kit_DropBehaviour dp = go.GetComponent<Kit_DropBehaviour>();
                            dp.isSceneOwned = true;
                            dp.weaponID = wepScript.gameGunID;
                            dp.bulletsLeft = 0;
                            dp.bulletsLeftToReload = 0;

                            for (int i = 0; i < wepData.attachments.Count; i++)
                            {
                                dp.attachments.Add(wepData.attachments[i]);
                            }

                            NetworkServer.Spawn(go);
                        }
                    }
                }
            }

            /// <summary>
            /// Drops a weapon and applies the ragdoll force!
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="slot"></param>
            public void DropWeaponDead(Kit_PlayerBehaviour pb, int slot)
            {
                if ((Kit_IngameMain.instance.currentPvPGameModeBehaviour && Kit_IngameMain.instance.currentPvPGameModeBehaviour.CanDropWeapons()) || (Kit_IngameMain.instance.currentPvEGameModeBehaviour && Kit_IngameMain.instance.currentPvEGameModeBehaviour.CanDropWeapons()))
                {
                    if (pb.weaponManagerNetworkData != null && pb.weaponManagerNetworkData.GetType() == typeof(Kit_ModernWeaponManagerNetworkData))
                    {
                        //Get the manager's runtime data
                        Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                        if (runtimeData.weaponsInUseDataObjects[slot] != null && runtimeData.weaponsInUseDataObjects[slot].GetType() == typeof(Kit_ModernWeaponScriptRuntimeData))
                        {
                            //Get the weapon's runtime data
                            Kit_ModernWeaponScriptRuntimeData wepData = runtimeData.weaponsInUseDataObjects[slot] as Kit_ModernWeaponScriptRuntimeData;
                            //Get the Scriptable object
                            Kit_ModernWeaponScript wepScript = runtimeData.weaponsInUseDataObjects[slot].behaviour as Kit_ModernWeaponScript;
                            //Instantiate
                            GameObject go = Instantiate(dropPrefab, pb.playerCameraTransform.position, pb.playerCameraTransform.rotation);
                            Kit_DropBehaviour dp = go.GetComponent<Kit_DropBehaviour>();
                            dp.isSceneOwned = true;
                            dp.weaponID = wepScript.gameGunID;
                            dp.bulletsLeft = wepData.bulletsLeft;
                            dp.bulletsLeftToReload = wepData.bulletsLeftToReload;

                            for (int i = 0; i < wepData.attachments.Count; i++)
                            {
                                dp.attachments.Add(wepData.attachments[i]);
                            }

                            NetworkServer.Spawn(go);
                            Rigidbody body = go.GetComponent<Rigidbody>();
                            if (body)
                            {
                                body.velocity = pb.movement.GetVelocity(pb);
                                body.AddForceNextFrame(pb.ragdollForward * pb.ragdollForce / 10);
                            }
                        }
                        else if (runtimeData.weaponsInUseDataObjects[slot] != null && runtimeData.weaponsInUseDataObjects[slot].GetType() == typeof(Kit_ModernMeleeScriptRuntimeData))
                        {
                            //Get the weapon's runtime data
                            Kit_ModernMeleeScriptRuntimeData wepData = runtimeData.weaponsInUseDataObjects[slot] as Kit_ModernMeleeScriptRuntimeData;
                            //Get the Scriptable object
                            Kit_ModernMeleeScript wepScript = runtimeData.weaponsInUseDataObjects[slot].behaviour as Kit_ModernMeleeScript;
                            //Instantiate
                            GameObject go = Instantiate(dropPrefab, pb.playerCameraTransform.position, pb.playerCameraTransform.rotation);
                            Kit_DropBehaviour dp = go.GetComponent<Kit_DropBehaviour>();
                            dp.isSceneOwned = true;
                            dp.weaponID = wepScript.gameGunID;
                            dp.bulletsLeft = 0;
                            dp.bulletsLeftToReload = 0;

                            for (int i = 0; i < wepData.attachments.Count; i++)
                            {
                                dp.attachments.Add(wepData.attachments[i]);
                            }

                            NetworkServer.Spawn(go);
                            Rigidbody body = go.GetComponent<Rigidbody>();
                            if (body)
                            {
                                body.velocity = pb.movement.GetVelocity(pb);
                                body.AddForceNextFrame(pb.ragdollForward * pb.ragdollForce / 10);
                            }
                        }
                        else if (runtimeData.weaponsInUseDataObjects[slot] != null && runtimeData.weaponsInUseDataObjects[slot].GetType() == typeof(Kit_ModernGrenadeScriptRuntimeData))
                        {
                            //Get the weapon's runtime data
                            Kit_ModernGrenadeScriptRuntimeData wepData = runtimeData.weaponsInUseDataObjects[slot] as Kit_ModernGrenadeScriptRuntimeData;
                            if (wepData.amountOfGrenadesLeft <= 0) return;
                            //Get the Scriptable object
                            Kit_ModernGrenadeScript wepScript = runtimeData.weaponsInUseDataObjects[slot].behaviour as Kit_ModernGrenadeScript;
                            //Instantiate
                            GameObject go = Instantiate(dropPrefab, pb.playerCameraTransform.position, pb.playerCameraTransform.rotation);
                            Kit_DropBehaviour dp = go.GetComponent<Kit_DropBehaviour>();
                            dp.isSceneOwned = true;
                            dp.weaponID = wepScript.gameGunID;
                            dp.bulletsLeft = 0;
                            dp.bulletsLeftToReload = 0;

                            for (int i = 0; i < wepData.attachments.Count; i++)
                            {
                                dp.attachments.Add(wepData.attachments[i]);
                            }

                            NetworkServer.Spawn(go);
                            Rigidbody body = go.GetComponent<Rigidbody>();
                            if (body)
                            {
                                body.velocity = pb.movement.GetVelocity(pb);
                                body.AddForceNextFrame(pb.ragdollForward * pb.ragdollForce / 10);
                            }
                        }
                    }
                }
            }

            public void DropWeapon(Kit_PlayerBehaviour pb, int slot, Transform replace)
            {
                if (pb.weaponManagerNetworkData != null && pb.weaponManagerNetworkData.GetType() == typeof(Kit_ModernWeaponManagerNetworkData))
                {
                    //Get the manager's runtime data
                    Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                    if (runtimeData.weaponsInUseDataObjects[slot] != null && runtimeData.weaponsInUseDataObjects[slot].GetType() == typeof(Kit_ModernWeaponScriptRuntimeData))
                    {
                        //Get the weapon's runtime data
                        Kit_ModernWeaponScriptRuntimeData wepData = runtimeData.weaponsInUseDataObjects[slot] as Kit_ModernWeaponScriptRuntimeData;
                        //Get the Scriptable object
                        Kit_ModernWeaponScript wepScript = runtimeData.weaponsInUseDataObjects[slot].behaviour as Kit_ModernWeaponScript;
                        //Instantiate
                        GameObject go = Instantiate(dropPrefab, replace.position, replace.rotation);
                        Kit_DropBehaviour dp = go.GetComponent<Kit_DropBehaviour>();
                        dp.isSceneOwned = true;
                        dp.weaponID = wepScript.gameGunID;
                        dp.bulletsLeft = wepData.bulletsLeft;
                        dp.bulletsLeftToReload = wepData.bulletsLeftToReload;

                        for (int i = 0; i < wepData.attachments.Count; i++)
                        {
                            dp.attachments.Add(wepData.attachments[i]);
                        }

                        NetworkServer.Spawn(go);
                    }
                    else if (runtimeData.weaponsInUseDataObjects[slot] != null && runtimeData.weaponsInUseDataObjects[slot].GetType() == typeof(Kit_ModernMeleeScriptRuntimeData))
                    {
                        //Get the weapon's runtime data
                        Kit_ModernMeleeScriptRuntimeData wepData = runtimeData.weaponsInUseDataObjects[slot] as Kit_ModernMeleeScriptRuntimeData;
                        //Get the Scriptable object
                        Kit_ModernMeleeScript wepScript = runtimeData.weaponsInUseDataObjects[slot].behaviour as Kit_ModernMeleeScript;
                        //Instantiate
                        GameObject go = Instantiate(dropPrefab, replace.position, replace.rotation);
                        Kit_DropBehaviour dp = go.GetComponent<Kit_DropBehaviour>();
                        dp.isSceneOwned = true;
                        dp.weaponID = wepScript.gameGunID;
                        dp.bulletsLeft = 0;
                        dp.bulletsLeftToReload = 0;

                        for (int i = 0; i < wepData.attachments.Count; i++)
                        {
                            dp.attachments.Add(wepData.attachments[i]);
                        }

                        NetworkServer.Spawn(go);
                    }
                    else if (runtimeData.weaponsInUseDataObjects[slot] != null && runtimeData.weaponsInUseDataObjects[slot].GetType() == typeof(Kit_ModernGrenadeScriptRuntimeData))
                    {
                        //Get the weapon's runtime data
                        Kit_ModernGrenadeScriptRuntimeData wepData = runtimeData.weaponsInUseDataObjects[slot] as Kit_ModernGrenadeScriptRuntimeData;
                        if (wepData.amountOfGrenadesLeft <= 0) return;
                        //Get the Scriptable object
                        Kit_ModernGrenadeScript wepScript = runtimeData.weaponsInUseDataObjects[slot].behaviour as Kit_ModernGrenadeScript;
                        //Instantiate
                        GameObject go = Instantiate(dropPrefab, replace.position, replace.rotation);
                        Kit_DropBehaviour dp = go.GetComponent<Kit_DropBehaviour>();
                        dp.isSceneOwned = true;
                        dp.weaponID = wepScript.gameGunID;
                        dp.bulletsLeft = 0;
                        dp.bulletsLeftToReload = 0;

                        for (int i = 0; i < wepData.attachments.Count; i++)
                        {
                            dp.attachments.Add(wepData.attachments[i]);
                        }

                        NetworkServer.Spawn(go);
                    }
                }
            }

            public override int WeaponState(Kit_PlayerBehaviour pb)
            {
                //Get the manager's runtime data
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                return runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.WeaponState(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]);
            }

            public override int WeaponType(Kit_PlayerBehaviour pb)
            {
                //Get the manager's runtime data
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                return runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.GetWeaponType(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]);
            }

            public override void FirstThirdPersonChanged(Kit_PlayerBehaviour pb, Kit_GameInformation.Perspective perspective)
            {
                if (pb.syncSetup)
                {
                    //Forward to currently selected weapon
                    Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                    if (runtimeData.quickUseInProgress && runtimeData.quickUseState > 0 && runtimeData.quickUseState < 4)
                    {
                        runtimeData.weaponsInUseDataObjects[runtimeData.desiredQuickUse].behaviour.FirstThirdPersonChanged(pb, perspective, runtimeData.weaponsInUseDataObjects[runtimeData.desiredQuickUse]);
                    }
                    else
                    {
                        runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.FirstThirdPersonChanged(pb, perspective, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]);
                    }
                }
            }

            public override void OnAmmoPickup(Kit_PlayerBehaviour pb, Kit_AmmoPickup pickup)
            {
                //Forward to currently selected weapon
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.OnAmmoPickup(pb, pickup, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]);
            }

            public override void RestockAmmo(Kit_PlayerBehaviour pb, bool allWeapons)
            {
                if (pb.isServer)
                {
                    if (pb.weaponManagerNetworkData != null && pb.weaponManagerNetworkData.GetType() == typeof(Kit_ModernWeaponManagerNetworkData))
                    {
                        if (allWeapons)
                        {
                            Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                            for (int i = 0; i < runtimeData.weaponsInUseDataObjects.Length; i++)
                            {
                                runtimeData.weaponsInUseDataObjects[i].behaviour.RestockAmmo(pb, runtimeData.weaponsInUseDataObjects[i]);
                            }
                        }
                        else
                        {
                            //Forward to currently selected weapon
                            Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                            runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.RestockAmmo(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]);
                        }
                    }
                }
            }

            public override bool IsCurrentWeaponFull(Kit_PlayerBehaviour pb)
            {
                //Forward to currently selected weapon
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                return runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon].behaviour.IsWeaponFull(pb, runtimeData.weaponsInUseDataObjects[runtimeData.currentWeapon]);
            }

            public override void PluginSelectWeapon(Kit_PlayerBehaviour pb, int slot, bool locked = true)
            {
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                //Just set, thats all!
                runtimeData.desiredWeapon = slot;
                runtimeData.isDesiredWeaponLocked = locked;
            }

            public override int GetCurrentlyDesiredWeapon(Kit_PlayerBehaviour pb)
            {
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                return runtimeData.desiredWeapon;
            }

            public override int GetCurrentlyDesiredQuickUse(Kit_PlayerBehaviour pb)
            {
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                return runtimeData.desiredQuickUse;
            }

            public override int GetCurrentlySelectedWeapon(Kit_PlayerBehaviour pb)
            {
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                return runtimeData.currentWeapon;
            }

            public override void SetDesiredWeapon(Kit_PlayerBehaviour pb, int desiredWeapon)
            {
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;
                if (runtimeData.weaponsInUseDataObjects[desiredWeapon].behaviour.CanBeSelected(pb, runtimeData.weaponsInUseDataObjects[desiredWeapon]))
                {
                    runtimeData.desiredWeapon = desiredWeapon;

                }
            }

            public override int[] GetSlotsWithEmptyWeapon(Kit_PlayerBehaviour pb)
            {
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;

                List<int> emptySlots = new List<int>();

                for (int i = 0; i < runtimeData.weaponsInUseDataObjects.Length; i++)
                {
                    //Check if that slot contains a placehodler weapon
                    if (runtimeData.weaponsInUseDataObjects[i].behaviour.GetType() == typeof(Kit_WeaponUnselectable))
                    {
                        int id = i;
                        //Add it to list of empty slots
                        emptySlots.Add(id);
                    }
                }

                return emptySlots.ToArray();
            }

            public override void BeginSpectating(Kit_PlayerBehaviour pb)
            {
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;

                for (int i = 0; i < runtimeData.weaponsInUseDataObjects.Length; i++)
                {
                    //Relay
                    runtimeData.weaponsInUseDataObjects[i].behaviour.BeginSpectating(pb, runtimeData.weaponsInUseDataObjects[i], runtimeData.weaponsInUseDataObjects[i].attachments.ToArray());
                }
            }

            public override void EndSpectating(Kit_PlayerBehaviour pb)
            {
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;

                for (int i = 0; i < runtimeData.weaponsInUseDataObjects.Length; i++)
                {
                    //Relay
                    runtimeData.weaponsInUseDataObjects[i].behaviour.EndSpectating(pb, runtimeData.weaponsInUseDataObjects[i]);
                }
            }

            public override void OnTriggerEnterRelay(Kit_PlayerBehaviour pb, Collider col)
            {
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;

                for (int i = 0; i < runtimeData.weaponsInUseDataObjects.Length; i++)
                {
                    //Relay
                    runtimeData.weaponsInUseDataObjects[i].behaviour.OnTriggerEnterRelay(pb, runtimeData.weaponsInUseDataObjects[i], col);
                }
            }

            public override void OnTriggerExitRelay(Kit_PlayerBehaviour pb, Collider col)
            {
                Kit_ModernWeaponManagerNetworkData runtimeData = pb.weaponManagerNetworkData as Kit_ModernWeaponManagerNetworkData;

                for (int i = 0; i < runtimeData.weaponsInUseDataObjects.Length; i++)
                {
                    //Relay
                    runtimeData.weaponsInUseDataObjects[i].behaviour.OnTriggerExitRelay(pb, runtimeData.weaponsInUseDataObjects[i], col);
                }
            }
        }
    }
}
