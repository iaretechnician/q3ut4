
using Mirror;
using System.Linq;
using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        /// <summary>
        /// Implements a synced flashlight (FP, TP)
        /// This needs to be on both, FP AND TP! Onesided will not work.
        /// </summary>
        public class Kit_AttachmentFlashlight : Kit_AttachmentVisualBase
        {
            public Renderer referenceRenderer;

            /// <summary>
            /// Light used for flashlight
            /// </summary>
            public Light flashlight;

            /// <summary>
            /// Get Input for flashlight!
            /// </summary>
            private bool lastFlashlightInput;

            [HideInInspector]
            public AttachmentUseCase myUse = AttachmentUseCase.Drop;
            [HideInInspector]
            public Kit_PlayerBehaviour myPlayer;

            public Kit_AttachmentSyncDataFlashlight syncData;

            public GameObject syncPrefab;

            private Kit_ModernWeaponScriptRuntimeData myData;
            private int mySlot;

            public override bool RequiresInteraction()
            {
                return true;
            }

            public override void Interaction(Kit_PlayerBehaviour pb)
            {
                if (lastFlashlightInput != pb.input.flashlight)
                {
                    lastFlashlightInput = pb.input.flashlight;
                    if (pb.input.flashlight)
                    {
                        if (!syncData)
                        {
                            //Try to fetch sync data
                            var spawnedId = myData.additionalDataBehaviors.Where(x => NetworkClient.spawned.ContainsKey(x) &&
                            NetworkClient.spawned[x].GetComponent<Kit_AttachmentSyncDataFlashlight>() &&
                            NetworkClient.spawned[x].GetComponent<Kit_AttachmentSyncDataFlashlight>().slot == mySlot).FirstOrDefault();

                            if (NetworkClient.spawned.ContainsKey(spawnedId))
                            {
                                syncData = NetworkClient.spawned[spawnedId].GetComponent<Kit_AttachmentSyncDataFlashlight>();
                            }
                        }

                        if (syncData)
                        {
                            //Switch...
                            syncData.on = !syncData.on;
                            //Manually call update once, for THIRD PERSON CASE!
                            Update();
                        }
                    }
                }
            }

            public override bool RequiresSyncing()
            {
                return true;
            }

            public override void Selected(Kit_PlayerBehaviour pb, AttachmentUseCase auc, Kit_ModernWeaponScript script, Kit_ModernWeaponScriptRuntimeData data, int slot)
            {
                //Assign use
                myUse = auc;
                myData = data;
                mySlot = slot;
                myPlayer = pb;
                enabled = true;

                if (pb)
                {
                    //Create sync data
                    if (pb.isServer)
                    {
                        //Checks if a sync object for this is already created
                        if (!data.additionalDataBehaviors.Any(x => NetworkServer.spawned.ContainsKey(x) &&
                        NetworkServer.spawned[x].GetComponent<Kit_AttachmentSyncDataFlashlight>() &&
                        NetworkServer.spawned[x].GetComponent<Kit_AttachmentSyncDataFlashlight>().slot == slot))
                        {
                            if (auc == AttachmentUseCase.FirstPerson)
                            {
                                GameObject go = Instantiate(syncPrefab);
                                NetworkIdentity nid = go.GetComponent<NetworkIdentity>();
                                NetworkServer.Spawn(go);
                                syncData = go.GetComponent<Kit_AttachmentSyncDataFlashlight>();
                                syncData.slot = mySlot;
                                data.additionalDataBehaviors.Add(syncData.netId);
                            }
                        }
                        else
                        {
                            //Try to fetch sync data
                            var spawnedId = data.additionalDataBehaviors.Where(x => NetworkClient.spawned.ContainsKey(x) &&
                            NetworkClient.spawned[x].GetComponent<Kit_AttachmentSyncDataFlashlight>() &&
                            NetworkClient.spawned[x].GetComponent<Kit_AttachmentSyncDataFlashlight>().slot == slot).FirstOrDefault();

                            if (NetworkClient.spawned.ContainsKey(spawnedId))
                            {
                                syncData = NetworkClient.spawned[spawnedId].GetComponent<Kit_AttachmentSyncDataFlashlight>();
                            }
                        }
                    }
                    else
                    {
                        //Try to fetch sync data
                        var spawnedId = data.additionalDataBehaviors.Where(x => NetworkClient.spawned.ContainsKey(x) &&
                        NetworkClient.spawned[x].GetComponent<Kit_AttachmentSyncDataFlashlight>() &&
                        NetworkClient.spawned[x].GetComponent<Kit_AttachmentSyncDataFlashlight>().slot == slot).FirstOrDefault();

                        if (NetworkClient.spawned.ContainsKey(spawnedId))
                        {
                            syncData = NetworkClient.spawned[spawnedId].GetComponent<Kit_AttachmentSyncDataFlashlight>();
                        }
                    }
                }
            }

            void Update()
            {
                if (myPlayer)
                {
                    if (syncData)
                    {
                        //If first person, only enable when third person is not active
                        if (myUse == AttachmentUseCase.FirstPerson)
                        {
                            if (myPlayer.looking.GetPerspective(myPlayer) == Kit_GameInformation.Perspective.ThirdPerson)
                            {
                                flashlight.enabled = false;
                            }
                            else
                            {
                                flashlight.enabled = syncData.on && referenceRenderer.enabled;
                            }
                        }
                        //If third person, only enable when third person mode is active
                        else if (myUse == AttachmentUseCase.ThirdPerson)
                        {
                            if (myPlayer.looking.GetPerspective(myPlayer) == Kit_GameInformation.Perspective.ThirdPerson)
                            {
                                flashlight.enabled = syncData.on && referenceRenderer.enabled;
                            }
                            else
                            {
                                flashlight.enabled = false;
                            }
                        }
                    }
                    else
                    {
                        //Try to fetch sync data
                        var spawnedId = myData.additionalDataBehaviors.Where(x => NetworkClient.spawned.ContainsKey(x) &&
                        NetworkClient.spawned[x].GetComponent<Kit_AttachmentSyncDataFlashlight>() &&
                        NetworkClient.spawned[x].GetComponent<Kit_AttachmentSyncDataFlashlight>().slot == mySlot).FirstOrDefault();

                        if (NetworkClient.spawned.ContainsKey(spawnedId))
                        {
                            syncData = NetworkClient.spawned[spawnedId].GetComponent<Kit_AttachmentSyncDataFlashlight>();
                        }

                        if (!syncData)
                        {
                            flashlight.enabled = false;
                        }
                    }
                }
            }

            public override void SetVisibility(Kit_PlayerBehaviour pb, AttachmentUseCase auc, bool visible)
            {
                if (visible)
                {
                    enabled = true;
                }
                else
                {
                    enabled = false;
                    flashlight.enabled = false;
                    if (syncData)
                    {
                        syncData.on = false;
                    }
                }
            }
        }
    }
}