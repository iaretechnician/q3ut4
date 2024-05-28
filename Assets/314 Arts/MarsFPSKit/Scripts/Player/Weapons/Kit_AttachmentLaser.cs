
using Mirror;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        /// <summary>
        /// Implements a synced laser (FP, TP)
        /// This needs to be on both, FP AND TP! Onesided will not work.
        /// </summary>
        public class Kit_AttachmentLaser : Kit_AttachmentVisualBase
        {
            public Renderer referenceRenderer;

            /// <summary>
            /// Light used for laser
            /// </summary>
            public Light laser;

            /// <summary>
            /// Line Renderer used for laser
            /// </summary>
            public LineRenderer laserLine;

            /// <summary>
            /// Game Object from which raycast is fired
            /// </summary>
            public Transform laserGO;

            /// <summary>
            /// Maximum distance for raycast
            /// </summary>
            public float maxLaserDistance = 500f;

            /// <summary>
            /// Layer mask for laser ;)
            /// </summary>
            public LayerMask laserMask;

            /// <summary>
            /// Raycast hit..
            /// </summary>
            public RaycastHit hit;

            /// <summary>
            /// Player reference
            /// </summary>
            private Kit_PlayerBehaviour myPlayer;

            /// <summary>
            /// Use
            /// </summary>
            private AttachmentUseCase myUse;

            /// <summary>
            /// Get input!;
            /// </summary>
            private bool lastLaserInput;


            public Kit_AttachmentSyncDataLaser syncData;

            public GameObject syncPrefab;

            private Kit_ModernWeaponScriptRuntimeData myData;
            private int mySlot;

            public override bool RequiresSyncing()
            {
                return true;
            }

            public override bool RequiresInteraction()
            {
                return true;
            }

            public override void SyncFromFirstPerson(object obj)
            {
            }

            public override void Selected(Kit_PlayerBehaviour pb, AttachmentUseCase auc, Kit_ModernWeaponScript script, Kit_ModernWeaponScriptRuntimeData data, int slot)
            {
                myPlayer = pb;
                myUse = auc;
                myData = data;
                mySlot = slot;
                SetVisibility(pb, auc, false);

                if (!pb) enabled = false; //Drop or loadout

                if (pb)
                {
                    //Create sync data
                    if (pb.isServer)
                    {
                        //Checks if a sync object for this is already created
                        if (!data.additionalDataBehaviors.Any(x => NetworkServer.spawned.ContainsKey(x) &&
                        NetworkServer.spawned[x].GetComponent<Kit_AttachmentSyncDataLaser>() &&
                        NetworkServer.spawned[x].GetComponent<Kit_AttachmentSyncDataLaser>().slot == slot))
                        {
                            if (auc == AttachmentUseCase.FirstPerson)
                            {
                                GameObject go = Instantiate(syncPrefab);
                                NetworkIdentity nid = go.GetComponent<NetworkIdentity>();
                                NetworkServer.Spawn(go);
                                syncData = go.GetComponent<Kit_AttachmentSyncDataLaser>();
                                syncData.slot = mySlot;
                                data.additionalDataBehaviors.Add(syncData.netId);
                            }
                        }
                        else
                        {
                            //Try to fetch sync data
                            var spawnedId = data.additionalDataBehaviors.Where(x => NetworkClient.spawned.ContainsKey(x) &&
                            NetworkClient.spawned[x].GetComponent<Kit_AttachmentSyncDataLaser>() &&
                            NetworkClient.spawned[x].GetComponent<Kit_AttachmentSyncDataLaser>().slot == slot).FirstOrDefault();

                            if (NetworkClient.spawned.ContainsKey(spawnedId))
                            {
                                syncData = NetworkClient.spawned[spawnedId].GetComponent<Kit_AttachmentSyncDataLaser>();
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
                            syncData = NetworkClient.spawned[spawnedId].GetComponent<Kit_AttachmentSyncDataLaser>();
                        }
                    }
                }
            }

            public override void Interaction(Kit_PlayerBehaviour pb)
            {
                if (lastLaserInput != myPlayer.input.laser)
                {
                    lastLaserInput = myPlayer.input.laser;
                    if (myPlayer.input.laser)
                    {
                        if (!syncData)
                        {
                            //Try to fetch sync data
                            var spawnedId = myData.additionalDataBehaviors.Where(x => NetworkClient.spawned.ContainsKey(x) &&
                            NetworkClient.spawned[x].GetComponent<Kit_AttachmentSyncDataFlashlight>() &&
                            NetworkClient.spawned[x].GetComponent<Kit_AttachmentSyncDataFlashlight>().slot == mySlot).FirstOrDefault();

                            if (NetworkClient.spawned.ContainsKey(spawnedId))
                            {
                                syncData = NetworkClient.spawned[spawnedId].GetComponent<Kit_AttachmentSyncDataLaser>();
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

            void Update()
            {
                UpdateLaser();
            }

            void LateUpdate()
            {
                UpdateLaser();
            }

            void UpdateLaser()
            {
                if (Physics.Raycast(laserGO.position, laserGO.forward, out hit, maxLaserDistance, laserMask, QueryTriggerInteraction.Ignore))
                {
                    laserLine.SetPosition(0, laserGO.position);
                    laserLine.SetPosition(1, hit.point);
                    laser.transform.position = hit.point + hit.normal * 0.03f;
                }
                else
                {
                    laserLine.SetPosition(0, laserGO.position);
                    laserLine.SetPosition(1, laserGO.position + laserGO.forward * maxLaserDistance);
                    laser.transform.position = laserGO.position + laserGO.forward * maxLaserDistance;
                }

                if (myPlayer)
                {
                    if (syncData)
                    {
                        //If first person, only enable when third person is not active
                        if (myUse == AttachmentUseCase.FirstPerson)
                        {
                            if (myPlayer.looking.GetPerspective(myPlayer) == Kit_GameInformation.Perspective.ThirdPerson)
                            {
                                laser.enabled = false;
                                laserLine.enabled = false;
                            }
                            else
                            {
                                laser.enabled = syncData.on && referenceRenderer.enabled;
                                laserLine.enabled = syncData.on && referenceRenderer.enabled;
                            }
                        }
                        //If third person, only enable when third person mode is active
                        else if (myUse == AttachmentUseCase.ThirdPerson)
                        {
                            if (myPlayer.looking.GetPerspective(myPlayer) == Kit_GameInformation.Perspective.ThirdPerson)
                            {
                                laser.enabled = syncData.on && referenceRenderer.enabled;
                                laserLine.enabled = syncData.on && referenceRenderer.enabled;
                            }
                            else
                            {
                                laser.enabled = false;
                                laserLine.enabled = false;
                            }
                        }
                    }
                    else
                    {
                        //Try to fetch sync data
                        var spawnedId = myData.additionalDataBehaviors.Where(x => NetworkClient.spawned.ContainsKey(x) &&
                        NetworkClient.spawned[x].GetComponent<Kit_AttachmentSyncDataLaser>() &&
                        NetworkClient.spawned[x].GetComponent<Kit_AttachmentSyncDataLaser>().slot == mySlot).FirstOrDefault();

                        if (NetworkClient.spawned.ContainsKey(spawnedId))
                        {
                            syncData = NetworkClient.spawned[spawnedId].GetComponent<Kit_AttachmentSyncDataLaser>();
                        }

                        if (!syncData)
                        {
                            laser.enabled = false;
                            laserLine.enabled = false;
                        }
                    }
                }
            }

            public override void SetVisibility(Kit_PlayerBehaviour pb, AttachmentUseCase auc, bool visible)
            {
                if (visible)
                {
                    UpdateLaser();
                }
                else
                {
                    laserLine.enabled = false;
                    laser.enabled = false;
                    if (syncData)
                    {
                        syncData.on = false;
                    }
                }
            }
        }
    }
}