using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFPSKit
{
    public class Kit_Movement_BootsOnGroundNetworkData : Kit_MovementNetworkBase
    {
        [SyncVar(hook = "OnServerPositionChanged")]
        public Vector3 serverPosition;
        [SyncVar]
        public Quaternion serverRotation;
        [SyncVar]
        /// <summary>
        /// Are we grounded?
        /// </summary>
        public bool isGrounded;
        [SyncVar]
        /// <summary>
        /// Velocity
        /// </summary>
        public Vector3 velocity;
        [SyncVar]
        /// <summary>
        /// The character state.
        /// <para>0 = Standing</para>
        /// <para>1 = Crouching</para>
        /// <para>2 = Prone (not implemented right now.)</para>
        /// <para>3 = Swimming</para>
        /// </summary>
        public int state;
        /// <summary>
        /// Helper bool to see if we're currently swimming
        /// </summary>
        public bool isSwimming
        {
            get
            {
                return state == 3;
            }
        }
        [SyncVar]
        /// <summary>
        /// Are we underwater?
        /// </summary>
        public bool isUnderwater;
        [SyncVar]
        /// <summary>
        /// How much air do we have left?
        /// </summary>
        public float airLeft = 100f;
        /// <summary>
        /// When do we take the next damage?
        /// </summary>
        public float nextAirDamageAppliedAt;
        /// <summary>
        /// Current
        /// </summary>
        public Kit_SwimmingTrigger swimmingCurrent;
        /// <summary>
        /// When was the last swimming sound played?
        /// </summary>
        public float lastSwimmingSoundPlayed;

        /// <summary>
        /// Did we jump and have not been grounded since?
        /// </summary>
        public bool isJumping;
        /// <summary>
        /// How many times have we jumped since last ground?
        /// </summary>
        public int jumpCount;
        [SyncVar]
        /// <summary>
        /// Are we currently sprinting?
        /// </summary>
        public bool isSprinting;
        /// <summary>
        /// Were we swimming (true until grounded)
        /// </summary>
        public bool wasSwimming;
        /// <summary>
        /// Slot of swimming weapon :)
        /// </summary>
        public int swimmingWeaponSlot;
        /// <summary>
        /// This is where we return afterwards!
        /// </summary>
        public int swimmingPreviousWeaponSlot;
        [SyncVar]
        /// <summary>
        /// Material type for footsteps
        /// </summary>
        public string currentMaterial;
        /// <summary>
        /// When can we make our next footstep?
        /// </summary>
        public float nextFootstep;

        /// <summary>
        /// Should we play slow (aiming) walk animation?
        /// </summary>
        public bool playSlowWalkAnimation;

        #region Fall Damage
        /// <summary>
        /// Are we falling?
        /// </summary>
        public bool falling;
        /// <summary>
        /// How far have we fallen?
        /// </summary>
        public float fallDistance;
        /// <summary>
        /// From where did we fall?
        /// </summary>
        public float fallHighestPoint;
        #endregion

        #region Stamina
        [SyncVar]
        /// <summary>
        /// Our currently left stamina
        /// </summary>
        public float staminaLeft = 100f;
        /// <summary>
        /// After which time can we regenerate stamina?
        /// </summary>
        public float staminaRegenerationTime = 0f;
        /// <summary>
        /// If <see cref="Time.time"/> is smaller than this, sprinting is blocked because stamina is depleted
        /// </summary>
        public float staminaDepletedSprintingBlock;
        #endregion


        public Vector3 refVelocity;

        public Vector3 desiredMoveDirection = Vector3.zero;

        public Vector3 swimmingWorldMoveDirection = Vector3.zero;

        public Vector3 moveDirection = Vector3.zero;

        /// <summary>
        /// The local movement direction
        /// </summary>
        public Vector3 localMoveDirection = Vector3.zero;

        /// <summary>
        /// Last state of the crouch input
        /// </summary>
        public bool lastCrouch = false;
        /// <summary>
        /// Last state of the jump input
        /// </summary>
        public bool lastJump = false;

        /// <summary>
        /// Our historical posititons
        /// </summary>
        public List<Vector3> historicalPositions;
        /// <summary>
        /// When was the last position set
        /// </summary>
        public float historyLastPositionSetAt;

        public int amountOfSnapShots = 10;

        public Kit_PlayerBehaviour myPlayer;
        public Kit_Movement_BootsOnGround bogrd;
        public int snapShotCalculated;
        public Vector3 deltaToServer;

        public float compareToTime = 0.6f;

        public void OnServerPositionChanged(Vector3 was, Vector3 isNow)
        {
            if (!myPlayer)
            {
                if (isServer)
                {
                    if (NetworkServer.spawned.ContainsKey(ownerPlayerNetworkId))
                        myPlayer = NetworkServer.spawned[ownerPlayerNetworkId].GetComponent<Kit_PlayerBehaviour>();
                }
                else
                {
                    if (NetworkClient.spawned.ContainsKey(ownerPlayerNetworkId))
                        myPlayer = NetworkClient.spawned[ownerPlayerNetworkId].GetComponent<Kit_PlayerBehaviour>();
                }
            }
            else
            {
                if (!bogrd)
                {
                    bogrd = myPlayer.movement as Kit_Movement_BootsOnGround;
                }

                bogrd.OnServerPositionReceived(myPlayer, isNow);
            }
        }
    }
}