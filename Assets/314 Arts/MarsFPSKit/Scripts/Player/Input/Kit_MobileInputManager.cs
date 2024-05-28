using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityStandardAssets.CrossPlatformInput;

namespace MarsFPSKit
{
    public class MobileInputData
    {
        /// <summary>
        /// When did we check for enemies the last time?
        /// </summary>
        public float lastScan;

        public List<Kit_PlayerBehaviour> enemyPlayersAwareOff = new List<Kit_PlayerBehaviour>();

        /// <summary>
        /// Index of the look touch
        /// </summary>
        public int lookTouch = -1;
        /// <summary>
        /// Index of the move touch
        /// </summary>
        public int moveTouch = -1;
        /// <summary>
        /// Where did the walk input start?
        /// </summary>
        public Vector2 moveStart;
        /// <summary>
        /// Used to calculate delta for direction / sprint
        /// </summary>
        public Vector2 moveDelta;
        /// <summary>
        /// Which weapon should be selected?
        /// </summary>
        public int currentWeaponActive = 0;
    }

    [System.Serializable]
    public class MobileWeaponSelectStates
    {
        public bool[] boolsToSet;
    }

    [CreateAssetMenu(menuName = "MarsFPSKit/Input Manager/Mobile")]
    /// <summary>
    /// This is the kit's mobile input manager
    /// </summary>
    public class Kit_MobileInputManager : Kit_InputManagerBase
    {
        /// <summary>
        /// How many seconds apart are our scans?
        /// </summary>
        public float scanFrequency = 1f;

        [Header("Spotting")]
        public LayerMask spottingLayer;
        public LayerMask spottingCheckLayers;
        public float spottingMaxDistance = 50f;
        public Vector2 spottingBoxExtents = new Vector2(30, 30);
        private Vector3 spottingBoxSize;
        public float spottingFov = 90f;
        public float spottingRayDistance = 200f;

        public MobileWeaponSelectStates[] weaponSelectStates;

        public override void InitializeServer(Kit_PlayerBehaviour pb)
        {
            MobileInputData did = new MobileInputData();
            pb.inputManagerData = did;

            did.enemyPlayersAwareOff = new List<Kit_PlayerBehaviour>();
            spottingBoxSize = new Vector3(spottingBoxExtents.x, spottingBoxExtents.y, spottingMaxDistance / 2f);

            pb.input.weaponSlotUses = weaponSelectStates[0].boolsToSet;
        }

        public override void InitializeClient(Kit_PlayerBehaviour pb)
        {
            MobileInputData did = new MobileInputData();
            pb.inputManagerData = did;

            did.enemyPlayersAwareOff = new List<Kit_PlayerBehaviour>();
            spottingBoxSize = new Vector3(spottingBoxExtents.x, spottingBoxExtents.y, spottingMaxDistance / 2f);

            pb.input.weaponSlotUses = weaponSelectStates[0].boolsToSet;
        }

        public override void WriteToPlayerInput(Kit_PlayerBehaviour pb)
        {
            if (pb.inputManagerData != null && pb.inputManagerData.GetType() == typeof(MobileInputData))
            {
                MobileInputData did = pb.inputManagerData as MobileInputData;

                if (did.moveTouch < 0)
                {
                    pb.input.ver = Mathf.Lerp(pb.input.ver, 0, Time.deltaTime * 7f);
                    pb.input.hor = Mathf.Lerp(pb.input.hor, 0, Time.deltaTime * 7f);
                }

                if (Input.touchCount > 0)
                {
                    for (int i = 0; i < Input.touchCount; i++)
                    {
                        Touch touch = Input.GetTouch(i);
                        if (touch.phase == TouchPhase.Began)
                        {
                            //Check which one it is
                            if (touch.position.x > Screen.width / 2)
                            {
                                //Look
                                int t = i;
                                did.lookTouch = t;
                                if (did.moveTouch == did.lookTouch) did.moveTouch++;
                            }
                            else
                            {
                                //Move
                                int t = i;
                                did.moveTouch = t;
                                //Set start
                                did.moveStart = touch.position;
                                if (did.lookTouch == did.moveTouch) did.lookTouch++;
                            }
                        }
                        else if (touch.phase == TouchPhase.Ended)
                        {
                            if (i == did.moveTouch)
                            {
                                if (did.lookTouch > did.moveTouch) did.lookTouch--;
                                did.moveTouch = -1;
                            }
                            else if (i == did.lookTouch)
                            {
                                if (did.moveTouch > did.lookTouch) did.moveTouch--;
                                did.lookTouch = -1;
                            }
                        }
                        else
                        {
                            if (i == did.moveTouch)
                            {
                                //Calc delta
                                did.moveDelta = touch.position - did.moveStart;
                                did.moveDelta /= 50;
                                //Set
                                pb.input.ver = Mathf.Lerp(pb.input.ver, Mathf.Clamp(did.moveDelta.y, -1f, 1f), Time.deltaTime * 7f);
                                pb.input.hor = Mathf.Lerp(pb.input.hor, Mathf.Clamp(did.moveDelta.x, -1f, 1f), Time.deltaTime * 7f);
                                //Sprinting
                                if (did.moveDelta.y > 3f) pb.input.sprint = true;
                                else pb.input.sprint = false;
                            }
                            else if (i == did.lookTouch)
                            {
                                if (pb.weaponManager.IsAiming(pb))
                                {
                                    pb.input.mouseX += touch.deltaPosition.x / 6;
                                    pb.input.mouseY += touch.deltaPosition.y / 6;
                                }
                                else
                                {
                                    pb.input.mouseX += touch.deltaPosition.x / 3;
                                    pb.input.mouseY += touch.deltaPosition.y / 3;
                                }
                            }
                        }
                    }
                }

                pb.input.mouseY = Mathf.Clamp(pb.input.mouseY, -90, 90);

                pb.input.crouch = CrossPlatformInputManager.GetButton("Crouch");
                pb.input.jump = CrossPlatformInputManager.GetButton("Jump");
                if (CrossPlatformInputManager.GetButtonDown("Switch"))
                {
                    //Select the other weapon
                    did.currentWeaponActive++;
                    if (did.currentWeaponActive >= weaponSelectStates.Length) did.currentWeaponActive = 0;
                }

                pb.input.weaponSlotUses = weaponSelectStates[did.currentWeaponActive].boolsToSet;

                pb.input.interact = CrossPlatformInputManager.GetButton("Use");
                pb.input.lmb = CrossPlatformInputManager.GetButton("Fire");
                pb.input.rmb = CrossPlatformInputManager.GetButton("Aim");
                pb.input.reload = CrossPlatformInputManager.GetButton("Reload");

                //Set Camera (this is validated on the server)
                pb.input.clientCamPos = pb.playerCameraTransform.position;
                pb.input.clientCamForward = pb.playerCameraTransform.forward;

                //Scan
                if (Time.time > did.lastScan)
                {
                    did.lastScan = Time.time + scanFrequency;
                    ScanForEnemies(pb, did);
                }
            }
        }

        void ScanForEnemies(Kit_PlayerBehaviour pb, MobileInputData did)
        {
            Collider[] possiblePlayers = Physics.OverlapBox(pb.playerCameraTransform.position + pb.playerCameraTransform.forward * (spottingMaxDistance / 2), spottingBoxSize, pb.playerCameraTransform.rotation, spottingLayer.value);

            //Clean
            did.enemyPlayersAwareOff.RemoveAll(item => item == null);

            //Loop
            for (int i = 0; i < possiblePlayers.Length; i++)
            {
                //Check if it is a player
                Kit_PlayerBehaviour pnb = possiblePlayers[i].transform.root.GetComponent<Kit_PlayerBehaviour>();
                if (pnb && pnb != pb)
                {
                    if (CanSeePlayer(pb, did, pnb))
                    {
                        if (isEnemyPlayer(pb, did, pnb))
                        {
                            if (!did.enemyPlayersAwareOff.Contains(pnb))
                            {
                                //Add to our known list
                                did.enemyPlayersAwareOff.Add(pnb);
                                //Call spotted
                                if (pb.voiceManager)
                                {
                                    pb.voiceManager.SpottedEnemy(pb, pnb);
                                }
                            }
                        }
                    }
                }
            }
        }

        bool CanSeePlayer(Kit_PlayerBehaviour pb, MobileInputData did, Kit_PlayerBehaviour enemyPlayer)
        {
            if (enemyPlayer)
            {
                RaycastHit hit;
                Vector3 rayDirection = enemyPlayer.playerCameraTransform.position - new Vector3(0, 0.2f, 0f) - pb.playerCameraTransform.position;

                if ((Vector3.Angle(rayDirection, pb.playerCameraTransform.forward)) < spottingFov)
                {
                    if (Physics.Raycast(pb.playerCameraTransform.position, rayDirection, out hit, spottingRayDistance, spottingCheckLayers.value))
                    {
                        if (hit.collider.transform.root == enemyPlayer.transform.root)
                        {
                            return true;
                        }
                        else
                        {

                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        bool isEnemyPlayer(Kit_PlayerBehaviour pb, MobileInputData did, Kit_PlayerBehaviour enemyPlayer)
        {
            if (pb)
            {
                if (Kit_IngameMain.instance.currentPvPGameModeBehaviour)
                {
                    if (!Kit_IngameMain.instance.currentPvPGameModeBehaviour.isTeamGameMode) return true;
                    else
                    {
                        if (pb.myTeam != enemyPlayer.myTeam) return true;
                        else return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }
}