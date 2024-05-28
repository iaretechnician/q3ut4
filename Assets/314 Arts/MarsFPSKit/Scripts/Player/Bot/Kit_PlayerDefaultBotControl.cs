
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace MarsFPSKit
{
    public class Kit_BotControlRuntimeData
    {
        /// <summary>
        /// Navmesh agent used for navigation
        /// </summary>
        public NavMeshAgent nma;
        /// <summary>
        /// Where the bot should look in world space
        /// </summary>
        public Vector3 lookTarget = new Vector3(0, 100, 0);
        /// <summary>   
        /// Smoothed out look target
        /// </summary>
        public Vector3 smoothedLookTarget;

        //Input
        public Vector3 walkInput;

        public bool isEngaging = false;

        public List<Kit_PlayerBehaviour> enemyPlayersAwareOff = new List<Kit_PlayerBehaviour>();

        public Kit_PlayerBehaviour playerToEngage;

        /// <summary>
        /// Max offset on aim
        /// </summary>
        public float aimSkill = 1f;
        /// <summary>
        /// When was the bot updated for the last time?
        /// </summary>
        public float lastRoutine;
        /// <summary>
        /// Did we reach our destination
        /// </summary>
        public bool reachedDestination;
        /// <summary>
        /// When did we force a new waypoint for the last time?
        /// </summary>
        public float lastForceNewWaypoint;

        public float lastRandomAimSet;

        public Vector3 randomAimAdd;

        public float lastShot;

        public float lastEngagePlayerSet;

        public float lastEngagePosSet;

        public float burstTime;
        /// <summary>
        /// Where SHOULD we be aiming at?
        /// </summary>
        public Vector3 directAimVector;
        /// <summary>
        /// When did the bot press the reload button the last time?
        /// </summary>
        public float lastReloadTry;
    }

    [CreateAssetMenu(menuName = "MarsFPSKit/Bots/Behaviour/Default Behaviour")]
    public class Kit_PlayerDefaultBotControl : Kit_PlayerBotControlBase
    {
        [Header("Spotting")]
        public LayerMask spottingLayer;
        public LayerMask spottingCheckLayers;
        public float spottingMaxDistance = 50f;
        public Vector2 spottingBoxExtents = new Vector2(30, 30);
        private Vector3 spottingBoxSize;
        public float spottingFov = 90f;
        public float spottingRayDistance = 200f;
        /// <summary>
        /// How fast does the bot look?
        /// </summary>
        public float lookingInputSmooth = 5f;

        [Header("Shooting")]
        public float shootingAngleDifference = 5f;

        public int useLength = 2;

        public override void InitializeControls(Kit_PlayerBehaviour pb)
        {
            Kit_BotControlRuntimeData kbcrd = new Kit_BotControlRuntimeData();
            //Assign runtime data
            pb.botControlsRuntimeData = kbcrd;
            //Add Navmesh Agent
            kbcrd.nma = pb.gameObject.AddComponent<NavMeshAgent>();
            //Setup NMA
            kbcrd.nma.updatePosition = false;
            kbcrd.nma.updateRotation = false;
            kbcrd.nma.updateUpAxis = false;
            kbcrd.nma.nextPosition = pb.transform.position;
            kbcrd.nma.SetDestination(pb.transform.position);
            kbcrd.nma.speed = 0f;

            kbcrd.aimSkill = Random.Range(-0.4f, 0.4f);
            kbcrd.enemyPlayersAwareOff = new List<Kit_PlayerBehaviour>();
            spottingBoxSize = new Vector3(spottingBoxExtents.x, spottingBoxExtents.y, spottingMaxDistance / 2f);

            kbcrd.lookTarget = pb.transform.position + pb.transform.forward * 1000f;

            pb.input.weaponSlotUses = new bool[useLength];
        }

        public override void WriteToPlayerInput(Kit_PlayerBehaviour pb)
        {
            Kit_BotControlRuntimeData kbcrd = pb.botControlsRuntimeData as Kit_BotControlRuntimeData;

            if (pb.enableInput)
            {
                kbcrd.nma.speed = 3;

                if (Time.time - 0.1f > kbcrd.lastRoutine)
                {
                    kbcrd.lastRoutine = Time.time;
                    CheckForEnemies(pb, kbcrd);
                    UpdateAimPosition(pb, kbcrd);
                    FiringDecision(pb, kbcrd);

                    if (!kbcrd.reachedDestination)
                    {
                        if (kbcrd.nma.isOnNavMesh && kbcrd.nma.remainingDistance <= 0.5f)
                        {
                            kbcrd.reachedDestination = true;
                            ReachedDestination(pb, kbcrd);
                        }
                    }

                    if (!kbcrd.playerToEngage)
                    {
                        if (kbcrd.nma.isOnNavMesh && kbcrd.nma.remainingDistance > 10f && !pb.input.lmb)
                        {
                            pb.input.sprint = true;
                        }
                        else
                        {
                            pb.input.sprint = false;
                        }
                    }
                    else
                    {
                        pb.input.sprint = false;
                    }

                    if (Time.time - 5f > kbcrd.lastForceNewWaypoint || !kbcrd.playerToEngage && (kbcrd.nma.remainingDistance < 1f || !kbcrd.nma.hasPath) || kbcrd.playerToEngage && (kbcrd.nma.remainingDistance < 1f))
                    {
                        kbcrd.lastForceNewWaypoint = Time.time;
                        if (!kbcrd.playerToEngage)
                        {
                            //Set new waypoint
                            kbcrd.nma.SetDestination(Kit_IngameMain.instance.botNavPoints[Random.Range(0, Kit_IngameMain.instance.botNavPoints.Length)].position);
                        }
                        else
                        {
                            if (Vector3.Distance(pb.transform.position, kbcrd.playerToEngage.transform.position) > 10f)
                            {
                                kbcrd.nma.SetDestination(kbcrd.playerToEngage.transform.position + (Random.insideUnitSphere * Random.Range(1, 5f)));
                            }
                            else
                            {
                                int nearestWaypoint = Random.Range(0, Kit_IngameMain.instance.botNavPoints.Length);
                                float maxDistance = float.MaxValue;

                                for (int i = 0; i < Kit_IngameMain.instance.botNavPoints.Length; i++)
                                {
                                    float dist = Vector3.Distance(kbcrd.playerToEngage.transform.position, Kit_IngameMain.instance.botNavPoints[i].transform.position);
                                    if (dist < maxDistance)
                                    {
                                        if (Vector3.Distance(pb.transform.position, Kit_IngameMain.instance.botNavPoints[i].transform.position) > 10f)
                                        {
                                            maxDistance = dist;
                                            int id = i;
                                            nearestWaypoint = id;
                                        }
                                    }
                                }
                                //Set new waypoint
                                kbcrd.nma.SetDestination(Kit_IngameMain.instance.botNavPoints[nearestWaypoint].position + (Random.insideUnitSphere * Random.Range(1, 3f)));
                            }
                        }
                    }
                }

                #region Looking
                //Smooth look target
                kbcrd.smoothedLookTarget = Vector3.Lerp(kbcrd.smoothedLookTarget, kbcrd.lookTarget, Time.deltaTime * lookingInputSmooth);
                //Look at target
                Vector3 target = kbcrd.smoothedLookTarget;
                //Only look on y axis
                target.y = 0;
                //Get current
                Vector3 current = pb.transform.position;
                //Only look on y axis
                current.y = 0;
                pb.transform.rotation = Quaternion.LookRotation(target - current);
                //Reset
                target = kbcrd.smoothedLookTarget;
                current = pb.mouseLookObject.position;
                pb.mouseLookObject.rotation = Quaternion.LookRotation(target - current);
                #endregion
                //Send NavMeshAgent input to character controller
                kbcrd.walkInput = pb.transform.InverseTransformDirection(kbcrd.nma.desiredVelocity.normalized);
                //Copy input and smooth it
                pb.input.hor = kbcrd.walkInput.x;
                pb.input.ver = kbcrd.walkInput.z;
                //copy velocity back
                kbcrd.nma.velocity = pb.cc.velocity;
                kbcrd.nma.nextPosition = pb.transform.position;
            }
            else
            {
                kbcrd.nma.speed = 0;

                //Reset all input
                pb.input.hor = 0f;
                pb.input.ver = 0f;

                pb.input.crouch = false;
                pb.input.sprint = false;
                pb.input.jump = false;
                pb.input.interact = false;

                pb.input.rmb = false;
                pb.input.reload = false;

                pb.input.leanLeft = false;
                pb.input.leanRight = false;
                pb.input.thirdPerson = false;
                pb.input.flashlight = false;
                pb.input.laser = false;

                pb.input.lmb = false;
            }

            //Set Camera (this is validated on the server)
            pb.input.clientCamPos = pb.playerCameraTransform.position;
            pb.input.clientCamForward = pb.playerCameraTransform.forward;
        }

        void ReachedDestination(Kit_PlayerBehaviour pb, Kit_BotControlRuntimeData kbcrd)
        {
            if (!kbcrd.playerToEngage)
            {
                //Set new waypoint
                kbcrd.nma.SetDestination(Kit_IngameMain.instance.botNavPoints[Random.Range(0, Kit_IngameMain.instance.botNavPoints.Length)].position);
                kbcrd.reachedDestination = false;
            }
            else
            {
                if (Vector3.Distance(pb.transform.position, kbcrd.playerToEngage.transform.position) > 10f)
                {
                    kbcrd.nma.SetDestination(kbcrd.playerToEngage.transform.position + (Random.insideUnitSphere * Random.Range(1, 5f)));
                }
                else
                {
                    int nearestWaypoint = Random.Range(0, Kit_IngameMain.instance.botNavPoints.Length);
                    float maxDistance = float.MaxValue;

                    for (int i = 0; i < Kit_IngameMain.instance.botNavPoints.Length; i++)
                    {
                        float dist = Vector3.Distance(kbcrd.playerToEngage.transform.position, Kit_IngameMain.instance.botNavPoints[i].transform.position);
                        if (dist < maxDistance)
                        {
                            if (Vector3.Distance(pb.transform.position, Kit_IngameMain.instance.botNavPoints[i].transform.position) > 10f)
                            {
                                maxDistance = dist;
                                int id = i;
                                nearestWaypoint = id;
                            }
                        }
                    }
                    //Set new waypoint
                    kbcrd.nma.SetDestination(Kit_IngameMain.instance.botNavPoints[nearestWaypoint].position + (Random.insideUnitSphere * Random.Range(1, 3f)));
                }
                kbcrd.reachedDestination = false;
                /*
                kbcrd.nma.SetDestination(kbcrd.playerToEngage.transform.position + (Random.insideUnitSphere * Random.Range(0, 3f)));
                kbcrd.reachedDestination = false;
                */
            }
            kbcrd.lastForceNewWaypoint = Time.time;
        }

        void CheckForEnemies(Kit_PlayerBehaviour pb, Kit_BotControlRuntimeData kbcrd)
        {
            Collider[] possiblePlayers = Physics.OverlapBox(pb.playerCameraTransform.position + pb.playerCameraTransform.forward * (spottingMaxDistance / 2), spottingBoxSize, pb.playerCameraTransform.rotation, spottingLayer.value);

            //Clean
            kbcrd.enemyPlayersAwareOff.RemoveAll(item => item == null);

            //Loop
            for (int i = 0; i < possiblePlayers.Length; i++)
            {
                //Check if it is a player
                Kit_PlayerBehaviour pnb = possiblePlayers[i].transform.root.GetComponent<Kit_PlayerBehaviour>();
                if (pnb && pnb != pb)
                {
                    if (CanSeePlayer(pb, kbcrd, pnb))
                    {
                        if (isEnemyPlayer(pb, kbcrd, pnb))
                        {
                            if (!kbcrd.enemyPlayersAwareOff.Contains(pnb))
                            {
                                //Add to our known list
                                kbcrd.enemyPlayersAwareOff.Add(pnb);
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

        bool CanSeePlayer(Kit_PlayerBehaviour pb, Kit_BotControlRuntimeData kbcrd, Kit_PlayerBehaviour enemyPlayer)
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

        bool isEnemyPlayer(Kit_PlayerBehaviour pb, Kit_BotControlRuntimeData kbcrd, Kit_PlayerBehaviour enemyPlayer)
        {
            if (pb)
            {
                if (!pb.syncSetup) return false;
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

        void UpdateAimPosition(Kit_PlayerBehaviour pb, Kit_BotControlRuntimeData kbcrd)
        {
            if (pb.input.sprint)
            {
                kbcrd.lookTarget = pb.transform.position + kbcrd.nma.desiredVelocity.normalized * 2000f;
            }
            else
            {
                if (kbcrd.playerToEngage)
                {
                    if (Time.time > kbcrd.lastRandomAimSet + 0.5f)
                    {
                        kbcrd.lastRandomAimSet = Time.time;
                        kbcrd.randomAimAdd = Random.insideUnitSphere * kbcrd.aimSkill;
                    }
                    kbcrd.lookTarget = kbcrd.playerToEngage.playerCameraTransform.position - new Vector3(0, 0.5f, 0f) + kbcrd.randomAimAdd;
                }
                else if (kbcrd.nma.desiredVelocity.sqrMagnitude > 1)
                {
                    kbcrd.lookTarget = Vector3.Lerp(kbcrd.lookTarget, pb.transform.position + kbcrd.nma.desiredVelocity.normalized * 2000f, Time.deltaTime * 100f);
                }
                else
                {
                    kbcrd.lookTarget = Vector3.Lerp(kbcrd.lookTarget, pb.transform.position + pb.transform.forward * 2000f, Time.deltaTime * 100f);
                }
            }
        }

        void FiringDecision(Kit_PlayerBehaviour pb, Kit_BotControlRuntimeData kbcrd)
        {
            int lastWeaponState = pb.weaponManager.WeaponState(pb);
            int lastWeaponType = pb.weaponManager.WeaponType(pb);

            if (lastWeaponState == 0)
            {
                if (Time.time - 5f > kbcrd.lastEngagePlayerSet)
                {
                    UpdatePlayerToEngage(pb, kbcrd);
                }
                else if (!kbcrd.playerToEngage)
                {
                    UpdatePlayerToEngage(pb, kbcrd);
                }

                if (kbcrd.playerToEngage)
                {
                    /*
                    if (Time.time > kbcrd.lastEngagePosSet)
                    {
                        kbcrd.nma.SetDestination(kbcrd.playerToEngage.transform.position + (Random.insideUnitSphere * Random.Range(0, 3f)));
                        kbcrd.lastEngagePosSet = Time.time + Random.Range(2.5f, 15f);
                    }
                    */

                    if (isAimingNearEngage(pb, kbcrd) && CanSeePlayer(pb, kbcrd, kbcrd.playerToEngage))
                    {
                        if (lastWeaponType == 0)
                        {
                            //Check distance
                            float distance = Vector3.Distance(pb.playerCameraTransform.position, kbcrd.playerToEngage.playerCameraTransform.position);

                            //Short distance - spray
                            if (distance < 7.5f)
                            {
                                FullAutoFire(pb, kbcrd);
                                pb.input.rmb = false;
                            }
                            //Medium distance - burst
                            else if (distance < 30f)
                            {
                                BurstFire(pb, kbcrd);
                                pb.input.rmb = true;
                            }
                            //Long distance - single shots
                            else
                            {
                                SingleFire(pb, kbcrd);
                                pb.input.rmb = true;
                            }
                            pb.input.reload = false;
                        }
                        else if (lastWeaponType == 1)
                        {
                            SingleFire(pb, kbcrd);
                            pb.input.reload = false;
                            pb.input.rmb = true;
                        }
                        else if (lastWeaponType == 2)
                        {
                            //Check distance
                            float distance = Vector3.Distance(pb.playerCameraTransform.position, kbcrd.playerToEngage.playerCameraTransform.position);

                            if (distance < 15f)
                            {
                                SingleFire(pb, kbcrd);
                                pb.input.rmb = true;
                            }
                            else
                            {
                                pb.input.rmb = false;
                            }

                            pb.input.reload = false;
                        }
                    }
                    else
                    {
                        pb.input.lmb = false;
                        pb.input.reload = false;
                        pb.input.rmb = false;
                    }
                }
                else
                {
                    pb.input.lmb = false;
                    pb.input.reload = false;
                    pb.input.rmb = false;
                }
            }
            else if (lastWeaponState == 1)
            {
                pb.input.lmb = false;
                if (Time.time > kbcrd.lastReloadTry)
                {
                    pb.input.reload = true;
                    if (Time.time - 0.1f > kbcrd.lastReloadTry)
                    {
                        kbcrd.lastReloadTry = Time.time + 0.2f;
                    }
                }
                else
                {
                    pb.input.reload = false;
                }
                pb.input.rmb = false;
            }
            else if (lastWeaponState == 2)
            {
                pb.input.weaponSlotUses[1] = true;
                pb.input.lmb = false;
                pb.input.reload = false;
                pb.input.rmb = false;
            }
        }

        void UpdatePlayerToEngage(Kit_PlayerBehaviour pb, Kit_BotControlRuntimeData kbcrd)
        {
            kbcrd.lastEngagePlayerSet = Time.time;
            kbcrd.enemyPlayersAwareOff = kbcrd.enemyPlayersAwareOff.OrderBy(x => Vector3.Distance(pb.transform.position, x.transform.position)).ToList();

            for (int i = 0; i < kbcrd.enemyPlayersAwareOff.Count; i++)
            {
                if (i < kbcrd.enemyPlayersAwareOff.Count)
                {
                    if (CanSeePlayer(pb, kbcrd, kbcrd.enemyPlayersAwareOff[i]))
                    {
                        kbcrd.playerToEngage = kbcrd.enemyPlayersAwareOff[i];
                        break;
                    }
                }
            }
        }

        void FullAutoFire(Kit_PlayerBehaviour pb, Kit_BotControlRuntimeData kbcrd)
        {
            pb.input.lmb = true;
        }

        void BurstFire(Kit_PlayerBehaviour pb, Kit_BotControlRuntimeData kbcrd)
        {
            if (Time.time > kbcrd.lastShot)
            {
                pb.input.lmb = true;
                kbcrd.burstTime += 0.1f;

                if (kbcrd.burstTime >= 0.5f)
                {
                    kbcrd.lastShot = Time.time + 0.8f;
                }
            }
            else
            {
                pb.input.lmb = false;
            }
        }

        void SingleFire(Kit_PlayerBehaviour pb, Kit_BotControlRuntimeData kbcrd)
        {
            if (Time.time > kbcrd.lastShot)
            {
                pb.input.lmb = false;
                kbcrd.lastShot = Time.time + 0.6f;
            }
            else
            {
                pb.input.lmb = true;
            }
        }

        bool isAimingNearEngage(Kit_PlayerBehaviour pb, Kit_BotControlRuntimeData kbcrd)
        {
            if (kbcrd.playerToEngage)
            {
                kbcrd.directAimVector = ((kbcrd.playerToEngage.playerCameraTransform.position - new Vector3(0, 0.2f, 0f)) - pb.playerCameraTransform.position);

                if (Vector3.Angle(kbcrd.directAimVector, pb.playerCameraTransform.forward) < shootingAngleDifference)
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
                return false;
            }
        }
    }
}
