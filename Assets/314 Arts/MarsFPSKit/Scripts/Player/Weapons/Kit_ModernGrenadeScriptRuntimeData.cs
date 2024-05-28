using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        public class Kit_ModernGrenadeScriptRuntimeData : Kit_WeaponRuntimeDataBase
        {
            public Kit_GrenadeRenderer grenadeRenderer;
            public Kit_ThirdPersonGrenadeRenderer tpGrenadeRenderer;
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

            /// <summary>
            /// How many grenades do we have left?
            /// </summary>
            public int amountOfGrenadesLeft;
            /// <summary>
            /// Are we currently throwing a grenade?
            /// </summary>
            public bool isThrowingGrenade;
            /// <summary>
            /// At which <see cref="Time.time"/> did we began throwing the grenade?
            /// </summary>
            public float beganThrowingGrenade;
            /// <summary>
            /// After pullpin, throw
            /// </summary>
            public bool hasThrownGrenadeAndIsWaitingForReturn;
            /// <summary>
            /// When was the grenade thrown last?
            /// </summary>
            public float grenadeThrownAt;
            /// <summary>
            /// Is redraw in progress?
            /// </summary>
            public bool isRedrawInProgress;

            #region Input
            public bool lastLmb;
            public bool lastRmb;
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
        }
    }
}