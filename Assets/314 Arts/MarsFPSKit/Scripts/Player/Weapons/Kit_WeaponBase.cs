
using System.Collections.Generic;
using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        public class WeaponIKValues
        {
            public Transform leftHandIK;
            public bool canUseIK;
        }

        /// <summary>
        /// Helper class for the loadout menu which retrieves the weapon's stats
        /// </summary>
        public class WeaponStats
        {
            /// <summary>
            /// How high is this weapon's damage?
            /// </summary>
            public float damage;
            /// <summary>
            /// How high is this weapon's fire rate?
            /// </summary>
            public float fireRate;
            /// <summary>
            /// How much recoil does this weapon have?
            /// </summary>
            public float recoil;
            /// <summary>
            /// How far can this weapon shoot?
            /// </summary>
            public float reach;
        }

        public class WeaponDisplayData
        {
            /// <summary>
            /// Sprite for HUD
            /// </summary>
            public Sprite sprite;
            /// <summary>
            /// Weapon Name
            /// </summary>
            public string name;
            /// <summary>
            /// This bool will be set at draw time!
            /// </summary>
            public bool selected;
        }

        public class WeaponQuickUseDisplayData
        {
            /// <summary>
            /// Sprite for HUD!
            /// </summary>
            public Sprite sprite;
            /// <summary>
            /// Name of the weapon
            /// </summary>
            public string name;
            /// <summary>
            /// Amount!
            /// </summary>
            public int amount;
        }

        /// <summary>
        /// This script is executed when this weapon is active
        /// </summary>
        public abstract class Kit_WeaponBase : ScriptableObject
        {
            /// <summary>
            /// The name of this weapon as it will be displayed in the loadout menu / Killfeed
            /// </summary>
            public string weaponName;
            /// <summary>
            /// The sprite of this weapon
            /// </summary>
            public Sprite weaponPicture;
            /// <summary>
            /// The hud sprite of this weapon
            /// </summary>
            public Sprite weaponHudPicture;
            /// <summary>
            /// Quick use sprite!
            /// </summary>
            public Sprite weaponQuickUsePicture;
            /// <summary>
            /// Image used for the killfeed!
            /// </summary>
            public Sprite weaponKillfeedImage;

            /// <summary>
            /// The type of this weapon
            /// </summary>
            public string weaponType = "Primary";
            /// <summary>
            /// Category of this weapon in loadout
            /// </summary>
            public string weaponLoadoutSubCategory = "Assault Rifle";

            /// <summary>
            /// In which weapon manager slots can this fit?
            /// </summary>
            public int[] canFitIntoSlots = new int[1];

            [Tooltip("At which level should this weapon be unlocked at?")]
            /// <summary>
            /// At which level should this weapon be unlocked at?
            /// </summary>
            public int levelToUnlockAt = -1;
            [Tooltip("This image will be displayed when this is unlocked")]
            /// <summary>
            /// This image will be displayed when this is unlocked
            /// </summary>
            public Sprite unlockImage;

            public bool IsWeaponUnlocked(Kit_GameInformation game)
            {
                //Check if we use leveling
                if (game.leveling)
                {
                    //Check if we have met the required level
                    if (game.leveling.GetLevel() >= levelToUnlockAt)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                //If not, just have it unlocked
                return true;
            }

            public bool IsWeaponUnlocked(int lvl)
            {
                if (lvl >= levelToUnlockAt) return true;
                return false;
            }

            #region Prefabs
            [Header("Prefabs")]
            public GameObject firstPersonPrefab; //The prefab to use for first person
            public GameObject thirdPersonPrefab; //The prefab to use for third person
            public GameObject dropPrefab; //The prefab to use for drop

            public GameObject runtimeDataPrefab;
            #endregion

            /// <summary>
            /// Which third person animset to use
            /// </summary>
            public string thirdPersonAnimType = "Rifle";
            /// <summary>
            /// Time it takes to take this weapon out
            /// </summary>
            public float drawTime = 0.5f;
            /// <summary>
            /// Time it takes to put this weapon away
            /// </summary>
            public float putawayTime = 0.5f; 
            /// <summary>
            /// Which category ID do we use if we are killed by this weapon?
            /// </summary>
            public int deathSoundCategory;
            /// <summary>
            /// ID in weapon list
            /// </summary>
            [System.NonSerialized]
            public int gameGunID;


            /// <summary>
            /// Can this weapon be selected in the loadout menu?
            /// </summary>
            /// <returns></returns>
            public virtual bool CanBeSelectedInLoadout()
            {
                return true;
            }

            /// <summary>
            /// Does this script support customization?
            /// </summary>
            /// <returns></returns>
            public virtual bool SupportsCustomization()
            {
                return true;
            }

            /// <summary>
            /// Can this weapon be selected?
            /// </summary>
            public virtual bool CanBeSelected(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                return true;
            }

            /// <summary>
            /// Called when this weapon is equipped and another weapon begins quick use
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="runtimeData"></param>
            public virtual void QuickUseOnOtherWeaponBegin(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {

            }

            /// <summary>
            /// If a script supports quick use, this needs to return true!
            /// </summary>
            /// <returns></returns>
            public virtual bool SupportsQuickUse(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                return false;
            }

            /// <summary>
            /// Do we skip putaway?
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="runtimeData"></param>
            /// <returns></returns>
            public virtual bool QuickUseSkipsPutaway(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                return false;
            }

            /// <summary>
            /// Begin the quick use process. Returns the time this takes!
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="runtimeData"></param>
            /// <returns></returns>
            public virtual float BeginQuickUse(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, float revertBy)
            {
                return 0f;
            }

            /// <summary>
            /// Ends the quick use process. Returns the time this takes!
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="runtimeData"></param>
            /// <returns></returns>
            public virtual float EndQuickUse(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, float revertBy)
            {
                return 0f;
            }

            /// <summary>
            /// Called just before the weapon is redrawn!
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="runtimeData"></param>
            public virtual void EndQuickUseAfter(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, float revertBy)
            {

            }

            /// <summary>
            /// Should this quick use be a one go button press or do we want to wait until the button is released?
            /// </summary>
            /// <returns></returns>
            public virtual bool WaitForQuickUseButtonRelease(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                return true;
            }

            /// <summary>
            /// Executed at 1 / send rate
            /// </summary>
            /// <param name="input"></param>
            public abstract void AuthorativeInput(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, Kit_PlayerInput input, float delta, double revertTime);

            /// <summary>
            /// Prediction input
            /// </summary>
            /// <param name="input"></param>
            public abstract void PredictionInput(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, Kit_PlayerInput input, float delta);

            /// <summary>
            /// Calculate the weapon (Update)
            /// </summary>
            /// <param name="pb"></param>
            /// /// <param name="runtimeData"></param>
            public abstract void CalculateWeaponUpdate(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData);

            /// <summary>
            /// Calculate the weapon (Late Update)
            /// </summary>
            /// <param name="pb"></param>
            /// /// <param name="runtimeData"></param>
            public virtual void CalculateWeaponLateUpdate(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData) //This is optional
            {

            }

            public virtual WeaponDisplayData GetWeaponDisplayData(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                return null;
            }

            public virtual WeaponQuickUseDisplayData GetWeaponQuickUseDisplayData(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                return null;
            }

            /// <summary>
            /// Tells the weapon to play idle, walk or run animation
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="runtimeData"></param>
            /// <param name="id">The animation that should be played</param>
            /// <param name="speed">The speed at which the animation should be played</param>
            public abstract void AnimateWeapon(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, int id, float speed);

            /// <summary>
            /// When the fall down effect should be played, this is called
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="runtimeData"></param>
            /// <param name="wasFallDamageApplied"></param>
            public abstract void FallDownEffect(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, bool wasFallDamageApplied);

            /// <summary>
            /// Callback for OnControllerColliderHit
            /// </summary>
            /// <param name="hit"></param>
            /// /// <param name="runtimeData"></param>
            public virtual void OnControllerColliderHitCallback(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, ControllerColliderHit hit) //This is optional
            {

            }

            /// <summary>
            /// Called when this weapon should be taken out
            /// </summary>
            /// <param name="pb"></param>
            /// /// <param name="runtimeData"></param>
            public abstract void DrawWeapon(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData);

            /// <summary>
            /// Called when this weapon should be put away (not hidden)
            /// </summary>
            /// <param name="pb"></param>
            /// /// <param name="runtimeData"></param>
            public abstract void PutawayWeapon(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData);

            /// <summary>
            /// Called when the weapon should be hidden after putaway sequence is done
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="runtimeData"></param>
            public abstract void PutawayWeaponHide(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData);

            public virtual GameObject SetupSpawnData(Kit_PlayerBehaviour pb, int slot, LoadoutWeapon weapon)
            {
                //Get our ID
                gameGunID = weapon.weaponID;

                GameObject go = Instantiate(runtimeDataPrefab, null);
                Kit_WeaponRuntimeDataBase baseData = go.GetComponent<Kit_WeaponRuntimeDataBase>();
                baseData.slot = slot;
                baseData.id = weapon.weaponID;
                for (int i = 0; i < weapon.attachments.Length; i++)
                {
                    baseData.attachments.Add(weapon.attachments[i]);
                }
                return go;
            }

            /// <summary>
            /// Create First Person for this weapon
            /// </summary>
            /// <param name="pb"></param>
            public abstract void SetupFirstPerson(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData);

            /// <summary>
            /// Create Third Person for this weapon
            /// </summary>
            /// <param name="pb"></param>
            public abstract void SetupThirdPerson(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData);

            /// <summary>
            /// Called when the player changed from first to third or third to first person view
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="isThirdPersonEnabled"></param>
            public abstract void FirstThirdPersonChanged(Kit_PlayerBehaviour pb, Kit_GameInformation.Perspective perspective, Kit_WeaponRuntimeDataBase runtimeData);

            #region Relay
            /// <summary>
            /// Called when we enter a trigger
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="isThirdPersonEnabled"></param>
            public virtual void OnTriggerEnterRelay(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, Collider col) { }

            /// <summary>
            /// Called when we exit a trigger
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="isThirdPersonEnabled"></param>
            public virtual void OnTriggerExitRelay(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, Collider col) { }
            #endregion

            #region Weapon Network Relays
            /// <summary>
            /// We received a semi shot RPC
            /// </summary>
            public virtual void NetworkSemiRPCReceived(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {

            }

            /// <summary>
            /// We received a bolt action shot RPC
            /// </summary>
            public virtual void NetworkBoltActionRPCReceived(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, int state)
            {

            }

            /// <summary>
            /// We received a burst fire RPC
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="runtimeData"></param>
            public virtual void NetworkBurstRPCReceived(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, int burstLength)
            {

            }

            /// <summary>
            /// We received a reload RPC
            /// </summary>
            /// <param name="isEmpty"></param>
            public virtual void NetworkReloadRPCReceived(Kit_PlayerBehaviour pb, bool isEmpty, Kit_WeaponRuntimeDataBase runtimeData)
            {

            }

            /// <summary>
            /// We received a procedural reload RPC
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="stage"></param>
            /// <param name="runtimeData"></param>
            public virtual void NetworkProceduralReloadRPCReceived(Kit_PlayerBehaviour pb, int stage, Kit_WeaponRuntimeDataBase runtimeData)
            {

            }

            /// <summary>
            /// Fire (dummy) physical bullet.
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="pos"></param>
            /// <param name="dir"></param>
            /// <param name="runtimeData"></param>
            public virtual void NetworkPhysicalBulletFired(Kit_PlayerBehaviour pb, Vector3 pos, Vector3 dir, Kit_WeaponRuntimeDataBase runtimeData)
            {

            }

            /// <summary>
            /// Charge RPC received
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="id"></param>
            /// <param name="runtimeData"></param>
            public virtual void NetworkMeleeChargeRPCReceived(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, int id, int slot)
            {

            }

            /// <summary>
            /// Heal RPC received
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="id"></param>
            /// <param name="runtimeData"></param>
            public virtual void NetworkMeleeHealRPCReceived(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, int slot)
            {

            }

            /// <summary>
            /// Stab rpc received
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="runtimeData"></param>
            public virtual void NetworkMeleeStabRPCReceived(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, int state, int slot)
            {

            }

            /// <summary>
            /// PullPin RPC received
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="runtimeData"></param>
            public virtual void NetworkGrenadePullPinRPCReceived(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {

            }

            /// <summary>
            /// Throw RPC received
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="runtimeData"></param>
            public virtual void NetworkGrenadeThrowRPCReceived(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {

            }
            #endregion

            #region For Other Scripts
            /// <summary>
            /// Retrives if the weapon is currently aiming or not
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="runtimeData"></param>
            /// <returns></returns>
            public virtual bool IsWeaponAiming(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                return false;
            }

            public virtual float GetAimingPercentage(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {
                return 0f;
            }

            /// <summary>
            /// Retrives if the weapon forces the camera into first person
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="runtimeData"></param>
            /// <returns></returns>
            public abstract bool ForceIntoFirstPerson(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData);

            /// <summary>
            /// Aiming time
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="runtimeData"></param>
            /// <returns></returns>
            public abstract float AimInTime(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData);

            /// <summary>
            /// Retrieves the movement speed multiplier for this weapon
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="runtimeData"></param>
            /// <returns></returns>
            public abstract float SpeedMultiplier(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData);

            /// <summary>
            /// Returns the sensitivity for the weapon
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="runtimeData"></param>
            /// <returns></returns>
            public abstract float Sensitivity(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData);

            /// <summary>
            /// Returns the IK data for this weapon
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="runtimeData"></param>
            /// <returns></returns>
            public abstract WeaponIKValues GetIK(Kit_PlayerBehaviour pb, Animator anim, Kit_WeaponRuntimeDataBase runtimeData);

            /// <summary>
            /// Retrieve this weapon's stats
            /// </summary>
            /// <returns></returns>
            public abstract WeaponStats GetStats();

            /// <summary>
            /// Do we support the display of stats?
            /// </summary>
            /// <returns></returns>
            public virtual bool SupportsStats()
            {
                return true;
            }

            /// <summary>
            /// Get current weapon state
            /// 0 = loaded
            /// 1 = empty
            /// 2 = completely empty
            /// </summary>
            /// <param name="pb"></param>
            /// <returns></returns>
            public abstract int WeaponState(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData);

            /// <summary>
            /// Get current weapon type
            /// 0 = Full Auto
            /// 1 = Semi Auto
            /// 2 = Close up only semi
            /// </summary>
            /// <param name="pb"></param>
            /// <returns></returns>
            public abstract int GetWeaponType(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData);
            #endregion

            #region Other Functionality
            /// <summary>
            /// Called on ammo pickup. Implement if you want to be able to pickup ammo
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="pickup"></param>
            public virtual void OnAmmoPickup(Kit_PlayerBehaviour pb, Kit_AmmoPickup pickup, Kit_WeaponRuntimeDataBase runtimeData)
            {

            }

            /// <summary>
            /// Restock ammo for this gun
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="runtimeData"></param>
            public virtual void RestockAmmo(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {

            }

            /// <summary>
            /// Is this weapon full?
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="runtimeData"></param>
            /// <returns></returns>
            public virtual bool IsWeaponFull(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData)
            {

                return true;
            }

            /// <summary>
            /// Called when we are now being spectated
            /// </summary>
            /// <param name="pb"></param>
            /// /// <param name="runtimeData"></param>
            public abstract void BeginSpectating(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData, int[] attachments);

            /// <summary>
            /// Called when we are no longer being spectated
            /// </summary>
            /// <param name="pb"></param>
            /// /// <param name="runtimeData"></param>
            public abstract void EndSpectating(Kit_PlayerBehaviour pb, Kit_WeaponRuntimeDataBase runtimeData);
            #endregion

            #region Mirror Only
            /// <summary>
            /// Called when network prefabs are registered
            /// </summary>
            public virtual void RegisterNetworkPrefabs()
            {

            }
            #endregion
        }
    }
}