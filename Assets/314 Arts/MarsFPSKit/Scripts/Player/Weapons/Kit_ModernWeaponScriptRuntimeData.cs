using Mirror;
using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        public class Kit_ModernWeaponScriptRuntimeData : Kit_WeaponRuntimeDataBase
        {
            [SyncVar]
            /// <summary>
            /// Is this weapon out and ready to shoot?
            /// </summary>
            public bool isSelectedAndReady = false;
            [SyncVar]
            /// <summary>
            /// Is the weapon selected?
            /// </summary>
            public bool isSelected;
            [SyncVar]
            /// <summary>
            /// How many bullets are left in the magazine?
            /// </summary>
            public int bulletsLeft;
            [SyncVar]
            /// <summary>
            /// How many bullets do we have left to reload
            /// </summary>
            public int bulletsLeftToReload;

            [SyncVar]
            /// <summary>
            /// Are we currently reloading?
            /// </summary>
            public bool reloadInProgress;

            public Kit_WeaponRenderer weaponRenderer;
            public Kit_ThirdPersonWeaponRenderer tpWeaponRenderer;

            public Animator genericAnimator;

            #region Stats
            [SyncVar]
            /// <summary>
            /// Are we currently firing full auto?
            /// </summary>
            public bool isFiring;
            /// <summary>
            /// When did we fire for the last time? Compare with <see cref="Time.time"/>
            /// </summary>
            public float lastFire;

            /// <summary>
            /// When did we ran the last time (using this weapon)
            /// </summary>
            public float lastRun;
            #endregion

            #region Reload
            /// <summary>
            /// When is the next reloading phase over?
            /// </summary>
            public float reloadNextEnd; //This is only so we don't have to use a coroutine
            /// <summary>
            /// The current phase of reloading
            /// </summary>
            public int reloadPhase;
            #endregion

            #region Procedural Reload
            /// <summary>
            /// Set to true if the player attemps to fire during the reload
            /// </summary>
            public bool cancelProceduralReload = false;
            #endregion

            #region Aiming
            /// <summary>
            /// Are we currently aiming?
            /// </summary>
            public bool isAiming;

            /// <summary>
            /// So we can see if it changed, based on bools only
            /// </summary>
            public bool lastIsAiming;

            /// <summary>
            /// Float between 0 and 1 that indicates how far we have aimed in
            /// </summary>
            public float aimingProgress;

            /// <summary>
            /// Are we aimed in?
            /// </summary>
            public bool isAimedIn
            {
                get
                {
                    return Mathf.Approximately(aimingProgress, 1f);
                }
            }

            /// <summary>
            /// If we are using the sniper scope, this helps us to only hide the weapon once
            /// </summary>
            public bool sniperWeaponHidden;

            /// <summary>
            /// Transform used to move the weapon to aiming position
            /// </summary>
            public Transform aimingTransform;
            #endregion

            #region Weapon Delay
            /// <summary>
            /// The transform to apply our delay effect to
            /// </summary>
            public Transform weaponDelayTransform;
            /// <summary>
            /// Current weapon delay target
            /// </summary>
            public Vector3 weaponDelayCur;
            /// <summary>
            /// Current Mouse X input for weapon delay
            /// </summary>
            public float weaponDelayCurrentX;
            /// <summary>
            /// Current Mouse Y input for weapon delay
            /// </summary>
            public float weaponDelayCurrentY;
            /// <summary>
            /// This is the difference from last frame
            /// </summary>
            public Quaternion weaponDelayLastDifference;
            /// <summary>
            /// To get difference, we use quaternion of look root
            /// </summary>
            public Quaternion weaponDelayLastRotation;
            #endregion

            #region Weapon Fall
            public Transform weaponFallTransform;
            #endregion

            #region Shell Ejection
            /// <summary>
            /// Should we check if we should eject a shell?
            /// </summary>
            public bool shellEjectEnabled = false;
            /// <summary>
            /// At which point in time are we going to eject the shell?
            /// </summary>
            public float shellEjectNext;
            #endregion

            #region  Bolt Action
            /// <summary>
            /// 0 = Nothing; 1 = Eject next; 2 = Eject next (last shot)
            /// </summary>
            public int boltActionState;
            /// <summary>
            /// When is the eject definied in <see cref="boltActionState"/> going to happen?
            /// </summary>
            public float boltActionTime;
            #endregion

            /// <summary>
            /// Which animation was played the last time? Used to only call CrossFade once so it transitions correctly.
            /// </summary>
            public int lastWeaponAnimationID;

            #region Spray Pattern
            /// <summary>
            /// Current state of the spray pattern
            /// </summary>
            public float sprayPatternState = 0f;
            #endregion

            #region Run Animation
            /// <summary>
            /// Is the running animation (using non generic mecanim) currently playing?
            /// </summary>
            public bool startedRunAnimation;
            #endregion

            #region Sound
            /// <summary>
            /// Audio source used for fire sounds
            /// </summary>
            public AudioSource soundFire;

            /// <summary>
            /// Audio Source used for reload sounds
            /// </summary>
            public AudioSource soundReload;

            /// <summary>
            /// Audio Source used for other sounds
            /// </summary>
            public AudioSource soundOther;
            #endregion

            #region Input
            public bool lastLmb;
            public bool lastRmb;
            public bool lastReload;
            #endregion

            #region Spring
            /// <summary>
            /// Positional spring
            /// </summary>
            public Kit_Spring springPos;
            /// <summary>
            /// Rotational spring
            /// </summary>
            public Kit_Spring springRot;
            #endregion

            //In order to modify the stats, we copy them here first
            #region Modifyable stats
            #region Settings
            [Header("Settings")]
            public FireMode fireMode = FireMode.Semi;
            public float fireRate = 0.1f;
            /// <summary>
            /// How many bullets do we have per mag
            /// </summary>
            public int bulletsPerMag = 30;
            /// <summary>
            /// With how many <see cref="WeaponControllerRuntimeData.bulletsLeftToReload"/> do we start?
            /// </summary>
            public int bulletsToReloadAtStart = 60;
            /// <summary>
            /// How much damage does this weapon deal?
            /// </summary>
            public float baseDamage = 30f;
            /// <summary>
            /// The max. range of this weapon
            /// </summary>
            public float range = 500f;
            /// <summary>
            /// This is the damage drop off relative to <see cref="range"/>. It should be a value between 0 and 1 that multiplies with <see cref="baseDamage"/>
            /// </summary>
            public AnimationCurve damageDropoff = AnimationCurve.Linear(0, 1, 500f, 0.8f);
            /// <summary>
            /// Should we shoot one bullet at a time or pellets?
            /// </summary>
            public FireTypeMode fireTypeMode = FireTypeMode.Simple;
            /// <summary>
            /// If we are firing using burst, how many bullets should be fired in one burst?
            /// </summary>
            public int burstBulletsPerShot = 3;
            /// <summary>
            /// How many seconds need to pass between individual shots in burst fire mode?
            /// </summary>
            public float burstTimeBetweenShots = 0.1f;
            /// <summary>
            /// If we should shoot pellets, how many should we shoot?
            /// </summary>
            public int amountOfPellets = 12;
            /// <summary>
            /// How much should the weapon slow us down (base)?  Other values are multiplier by this.
            /// </summary>
            public float speedMultiplierBase = 1f;
            /// <summary>
            /// How much force is applied to the ragdoll?
            /// </summary>
            public float ragdollForce = 500f;
            #endregion

            #region Bullets
            /// <summary>
            /// Determines whether we should use raycast or physical bullets
            /// </summary>
            public BulletMode bulletsMode = BulletMode.Raycast;
            /// <summary>
            /// Is bullet penetration enabled?
            /// </summary>
            public bool bulletsPenetrationEnabled = true;
            /// <summary>
            /// This is a value that determines how many objects can be penetrated with this bullet. Think of it as the money you have available to pay the penetration cost here <see cref="Kit_PenetrateableObject.cost"/>
            /// </summary>
            public int bulletsPenetrationForce = 3;
            /// <summary>
            /// If <see cref="bulletsMode"/> is set to <see cref="BulletMode.Physical"/>, this prefab will be instantiated instead of raycast
            /// </summary>
            public GameObject bulletPrefab;
            /// <summary>
            /// Speed of physical bullet in m/s
            /// </summary>
            public float bulletSpeed = 200;
            /// <summary>
            /// How many frames after shooting should the bullet be hidden?
            /// </summary>
            public int bulletHideForFrames = 3;
            /// <summary>
            /// Multiplier of gravity applied to bullet (1 = realistic). Multiplies with <see cref="Physics.gravity"/>
            /// </summary>
            public float bulletGravityMultiplier = 1f;
            /// <summary>
            /// How long will this weapon's bullets exist?
            /// </summary>
            public float bulletLifeTime = 10f;
            /// <summary>
            /// Should the bullet parent itself to its hit thing and stay alive? Useful for things like nails and arrows.
            /// </summary>
            public bool bulletStaysAliveAfterDeath = false;
            /// <summary>
            /// If bullet stays alive after death, this is how long
            /// </summary>
            public float bulletStaysAliveAfterDeathTime = 10f;
            #endregion

            #region Bullet Spread
            /// <summary>
            /// Which spray m ode does this weapon use?
            /// </summary>
            public SpreadMode bulletSpreadMode = SpreadMode.Simple;
            /// <summary>
            /// Base value for hip bullet spread
            /// </summary>
            public float bulletSpreadHipBase = 0.1f;
            /// <summary>
            /// How much spread should be added for our current velocity in reference to <see cref="bulletSpreadHipVelocityReference"/>
            /// </summary>
            public float bulletSpreadHipVelocityAdd = 0.1f;
            /// <summary>
            /// Reference velocity for hip spread
            /// </summary>
            public float bulletSpreadHipVelocityReference = 6f;

            /// <summary>
            /// Base value for aiming bullet spread
            /// </summary>
            public float bulletSpreadAimBase = 0.01f;
            /// <summary>
            /// How much spread should be added for our current velocity in reference to <see cref="bulletSpreadAimVelocityReference"/>
            /// </summary>
            public float bulletSpreadAimVelocityAdd = 0.02f;
            /// <summary>
            /// Reference velocity for aim spread
            /// </summary>
            public float bulletSpreadAimVelocityReference = 6f;

            //SPRAY PATTERN
            /// <summary>
            /// Offset values for the spray pattern mode
            /// </summary>
            public Vector3[] bulletSpreadSprayPattern = new Vector3[30];
            /// <summary>
            /// How fast does the spray pattern recover?
            /// </summary>
            public float bulletSpreadSprayPatternRecoverySpeed = 1f;
            //END
            #endregion

            #region Recoil
            /// <summary>
            /// Minimum amount of recoil per shot
            /// </summary>
            public Vector2 recoilPerShotMin = new Vector2 { x = -0.1f, y = 0.5f };
            /// <summary>
            /// Max amount of recoil per shot
            /// </summary>
            public Vector2 recoilPerShotMax = new Vector2 { x = 0.1f, y = 1f };
            /// <summary>
            /// How fast is our recoil going to be applied
            /// </summary>
            public float recoilApplyTime = 0.1f;
            /// <summary>
            /// How fast are we going to return to our normal position?
            /// </summary>
            public float recoilReturnSpeed = 6f;
            #endregion
            #endregion
        }
    }
}