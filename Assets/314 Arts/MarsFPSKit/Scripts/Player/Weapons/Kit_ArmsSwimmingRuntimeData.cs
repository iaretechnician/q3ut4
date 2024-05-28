using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        public class Kit_ArmsSwimmingRuntimeData : Kit_WeaponRuntimeDataBase
        {
            public Kit_MeleeRenderer armsSwimmingRenderer;

            /// <summary>
            /// Is this weapon out and ready to shoot?
            /// </summary>
            public bool isSelectedAndReady = false;
            /// <summary>
            /// Is the weapon selected?
            /// </summary>
            public bool isSelected;

            public Animator genericAnimator;

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
            #endregion

            #region Weapon Fall
            public Transform weaponFallTransform;
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
            public AudioSource sounds;
            #endregion

            /// <summary>
            /// When did we ran the last time (using this weapon)
            /// </summary>
            public float lastRun;

            /// <summary>
            /// Which animation was played the last time? Used to only call CrossFade once so it transitions correctly.
            /// </summary>
            public int lastWeaponAnimationID;

            #region Attack
            /// <summary>
            /// Are we charging right now?
            /// </summary>
            public bool isCharging;
            /// <summary>
            /// Progress of charging
            /// </summary>
            public float chargingProgress = 0f;
            /// <summary>
            /// Is charging with primary?
            /// </summary>
            public bool chargingPrimary;
            /// <summary>
            /// Is charging with secondary?
            /// </summary>
            public bool chargingSecondary;
            /// <summary>
            /// When can we do another action?
            /// </summary>
            public float nextActionPossibleAt;
            /// <summary>
            /// Quick charge
            /// </summary>
            public float quickChargeStartedAt;
            #endregion

            #region Input
            public bool lastLmb;
            public bool lastRmb;
            #endregion
        }
    }
}