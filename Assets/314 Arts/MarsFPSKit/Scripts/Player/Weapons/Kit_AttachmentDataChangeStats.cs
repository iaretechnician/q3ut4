using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        public enum StatChangeMode { SetAbsolute, Add, Multiply }

        [CreateAssetMenu(menuName = "MarsFPSKit/Weapons/Attachments/Change stats")]
        public class Kit_AttachmentDataChangeStats : Kit_AttachmentDataBase
        {
            public StatChangeMode statChangeMode = StatChangeMode.SetAbsolute;

            #region Settings
            [Header("Settings")]
            public bool useGlobalSettings;

            public FireMode fireMode = FireMode.Semi;
            /// <summary>
            /// How many rounds per minute does this weapon shoot?
            /// </summary>
            public float RPM = 600;
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
            [Header("Bullets")]
            public bool useBullets;
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
            [Header("Bullet Spread")]
            public bool useBulletSpread;
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
            [Header("Recoil")]
            public bool useRecoil;
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


            public override void ChangeStats(Kit_ModernWeaponScript ws, Kit_ModernWeaponScriptRuntimeData data)
            {
                if (statChangeMode == StatChangeMode.SetAbsolute)
                {
                    if (useGlobalSettings)
                    {
                        data.fireMode = fireMode;
                        data.fireRate = 60f / RPM;
                        data.bulletsPerMag = bulletsPerMag;
                        data.bulletsToReloadAtStart = bulletsToReloadAtStart;
                        data.baseDamage = baseDamage;
                        data.range = range;
                        data.damageDropoff = damageDropoff;
                        data.fireTypeMode = fireTypeMode;
                        data.burstBulletsPerShot = burstBulletsPerShot;
                        data.burstTimeBetweenShots = burstTimeBetweenShots;
                        data.amountOfPellets = amountOfPellets;
                        data.speedMultiplierBase = speedMultiplierBase;
                        data.ragdollForce = ragdollForce;
                    }

                    if (useBullets)
                    {
                        data.bulletsMode = bulletsMode;
                        data.bulletsPenetrationEnabled = bulletsPenetrationEnabled;
                        data.bulletsPenetrationForce = bulletsPenetrationForce;
                        if (bulletPrefab) data.bulletPrefab = bulletPrefab;
                        data.bulletSpeed = bulletSpeed;
                        data.bulletHideForFrames = bulletHideForFrames;
                        data.bulletGravityMultiplier = bulletGravityMultiplier;
                        data.bulletLifeTime = bulletLifeTime;
                        data.bulletStaysAliveAfterDeath = bulletStaysAliveAfterDeath;
                        data.bulletStaysAliveAfterDeathTime = bulletStaysAliveAfterDeathTime;
                    }

                    if (useBulletSpread)
                    {
                        data.bulletSpreadMode = bulletSpreadMode;
                        data.bulletSpreadHipBase = bulletSpreadHipBase;
                        data.bulletSpreadHipVelocityAdd = bulletSpreadHipVelocityAdd;
                        data.bulletSpreadHipVelocityReference = bulletSpreadHipVelocityReference;
                        data.bulletSpreadAimBase = bulletSpreadAimBase;
                        data.bulletSpreadAimVelocityAdd = bulletSpreadAimVelocityAdd;
                        data.bulletSpreadAimVelocityReference = bulletSpreadAimVelocityReference;

                        data.bulletSpreadSprayPattern = bulletSpreadSprayPattern;
                        data.bulletSpreadSprayPatternRecoverySpeed = bulletSpreadSprayPatternRecoverySpeed;
                    }

                    if (useRecoil)
                    {
                        data.recoilPerShotMin = recoilPerShotMin;
                        data.recoilPerShotMax = recoilPerShotMax;
                        data.recoilApplyTime = recoilApplyTime;
                        data.recoilReturnSpeed = recoilReturnSpeed;
                    }
                }
                else if (statChangeMode == StatChangeMode.Add)
                {
                    if (useGlobalSettings)
                    {
                        //data.fireMode = fireMode;
                        data.fireRate = 60f / (ws.RPM + RPM);
                        data.bulletsPerMag += bulletsPerMag;
                        data.bulletsToReloadAtStart += bulletsToReloadAtStart;
                        data.baseDamage += baseDamage;
                        data.range += range;
                        //data.damageDropoff = damageDropoff;
                        //data.fireTypeMode = fireTypeMode;
                        data.burstBulletsPerShot += burstBulletsPerShot;
                        data.burstTimeBetweenShots += burstTimeBetweenShots;
                        data.amountOfPellets += amountOfPellets;
                        data.speedMultiplierBase += speedMultiplierBase;
                        data.ragdollForce += ragdollForce;
                    }

                    if (useBullets)
                    {
                        //data.bulletsMode = bulletsMode;
                        //data.bulletsPenetrationEnabled = bulletsPenetrationEnabled;
                        data.bulletsPenetrationForce += bulletsPenetrationForce;
                        //data.bulletPrefab = bulletPrefab;
                        data.bulletSpeed += bulletSpeed;
                        data.bulletHideForFrames += bulletHideForFrames;
                        data.bulletGravityMultiplier += bulletGravityMultiplier;
                        data.bulletLifeTime += bulletLifeTime;
                        data.bulletStaysAliveAfterDeath = bulletStaysAliveAfterDeath;
                        data.bulletStaysAliveAfterDeathTime += bulletStaysAliveAfterDeathTime;
                    }

                    if (useBulletSpread)
                    {
                        //data.bulletSpreadMode = bulletSpreadMode;
                        data.bulletSpreadHipBase += bulletSpreadHipBase;
                        data.bulletSpreadHipVelocityAdd += bulletSpreadHipVelocityAdd;
                        data.bulletSpreadHipVelocityReference += bulletSpreadHipVelocityReference;
                        data.bulletSpreadAimBase += bulletSpreadAimBase;
                        data.bulletSpreadAimVelocityAdd += bulletSpreadAimVelocityAdd;
                        data.bulletSpreadAimVelocityReference += bulletSpreadAimVelocityReference;

                        //data.bulletSpreadSprayPattern = bulletSpreadSprayPattern;
                        data.bulletSpreadSprayPatternRecoverySpeed += bulletSpreadSprayPatternRecoverySpeed;
                    }

                    if (useRecoil)
                    {
                        data.recoilPerShotMin += recoilPerShotMin;
                        data.recoilPerShotMax += recoilPerShotMax;
                        data.recoilApplyTime += recoilApplyTime;
                        data.recoilReturnSpeed += recoilReturnSpeed;
                    }
                }
                else if (statChangeMode == StatChangeMode.Multiply)
                {
                    if (useGlobalSettings)
                    {
                        //data.fireMode = fireMode;
                        data.fireRate = 60f / (ws.RPM * RPM);
                        data.bulletsPerMag *= bulletsPerMag;
                        data.bulletsToReloadAtStart *= bulletsToReloadAtStart;
                        data.baseDamage *= baseDamage;
                        data.range *= range;
                        //data.damageDropoff = damageDropoff;
                        //data.fireTypeMode = fireTypeMode;
                        data.burstBulletsPerShot *= burstBulletsPerShot;
                        data.burstTimeBetweenShots *= burstTimeBetweenShots;
                        data.amountOfPellets *= amountOfPellets;
                        data.speedMultiplierBase *= speedMultiplierBase;
                        data.ragdollForce *= ragdollForce;
                    }

                    if (useBullets)
                    {
                        //data.bulletsMode = bulletsMode;
                        //data.bulletsPenetrationEnabled = bulletsPenetrationEnabled;
                        data.bulletsPenetrationForce *= bulletsPenetrationForce;
                        //data.bulletPrefab = bulletPrefab;
                        data.bulletSpeed *= bulletSpeed;
                        data.bulletHideForFrames *= bulletHideForFrames;
                        data.bulletGravityMultiplier *= bulletGravityMultiplier;
                        data.bulletLifeTime *= bulletLifeTime;
                        //data.bulletStaysAliveAfterDeath = bulletStaysAliveAfterDeath;
                        data.bulletStaysAliveAfterDeathTime *= bulletStaysAliveAfterDeathTime;
                    }

                    if (useBulletSpread)
                    {
                        //data.bulletSpreadMode = bulletSpreadMode;
                        data.bulletSpreadHipBase *= bulletSpreadHipBase;
                        data.bulletSpreadHipVelocityAdd *= bulletSpreadHipVelocityAdd;
                        data.bulletSpreadHipVelocityReference *= bulletSpreadHipVelocityReference;
                        data.bulletSpreadAimBase *= bulletSpreadAimBase;
                        data.bulletSpreadAimVelocityAdd *= bulletSpreadAimVelocityAdd;
                        data.bulletSpreadAimVelocityReference *= bulletSpreadAimVelocityReference;

                        //data.bulletSpreadSprayPattern = bulletSpreadSprayPattern;
                        data.bulletSpreadSprayPatternRecoverySpeed *= bulletSpreadSprayPatternRecoverySpeed;
                    }

                    if (useRecoil)
                    {
                        data.recoilPerShotMin *= recoilPerShotMin;
                        data.recoilPerShotMax *= recoilPerShotMax;
                        data.recoilApplyTime *= recoilApplyTime;
                        data.recoilReturnSpeed *= recoilReturnSpeed;
                    }
                }
            }
        }
    }
}