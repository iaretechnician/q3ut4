
using Mirror;
using UnityEngine;

namespace MarsFPSKit
{
    namespace Weapons
    {
        /// <summary>
        /// Expand upon this class to create a custom Weapon Manager. It should be using <see cref="Kit_WeaponBase"/> and <see cref="Kit_WeaponInformation"/> to be compatible with all setups
        /// </summary>
        public abstract class Kit_WeaponManagerBase : ScriptableObject
        {
            public GameObject networkData;

            /// <summary>
            /// Called before spawning to setup the spawn data for this player
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="loadout"></param>
            public abstract void SetupSpawnData(Kit_PlayerBehaviour pb, Loadout loadout, NetworkConnectionToClient sender);

            /// <summary>
            /// Called before spawning to setup the spawn data for this bot
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="loadout"></param>
            public abstract void SetupSpawnData(Kit_PlayerBehaviour pb, Loadout loadout);

            /// <summary>
            /// Called when our weapon's list is modified
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="op"></param>
            /// <param name="itemIndex"></param>
            /// <param name="oldItem"></param>
            /// <param name="newItem"></param>
            public abstract void OnWeaponsInUseSyncCallback(Kit_PlayerBehaviour pb, SyncList<uint>.Operation op, int itemIndex, uint oldItem, uint newItem);

            /// <summary>
            /// Called once the player object has been assigned to the actual network player
            /// </summary>
            /// <param name="pb"></param>
            public abstract void TakeControl(Kit_PlayerBehaviour pb);

            /// <summary>
            /// Called when the manager should be setup (server only)
            /// </summary>
            /// <param name="pb"></param>
            public abstract void InitializeServer(Kit_PlayerBehaviour pb);

            /// <summary>
            /// Called when the manager should be setup (client)
            /// </summary>
            /// <param name="pb"></param>
            public abstract void InitializeClient(Kit_PlayerBehaviour pb);

            /// <summary>
            /// Called when the player changed from first to third or third to first person view
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="isThirdPersonEnabled"></param>
            public abstract void FirstThirdPersonChanged(Kit_PlayerBehaviour pb, Kit_GameInformation.Perspective perspective);

            /// <summary>
            /// Selects another selectable weapon right fucking now!
            /// </summary>
            /// <param name="pb"></param>
            public abstract void ForceUnselectCurrentWeapon(Kit_PlayerBehaviour pb);

            /// <summary>
            /// Executed at 1 / send rate
            /// </summary>
            /// <param name="input"></param>
            public abstract void AuthorativeInput(Kit_PlayerBehaviour pb, Kit_PlayerInput input, float delta, double revertTime);

            /// <summary>
            /// Prediction input
            /// </summary>
            /// <param name="input"></param>
            public abstract void PredictionInput(Kit_PlayerBehaviour pb, Kit_PlayerInput input, float delta);

            #region Unity Callback
            /// <summary>
            /// Called in update for the local (controlling) player
            /// </summary>
            public abstract void CustomUpdate(Kit_PlayerBehaviour pb, double revertTime);

            /// <summary>
            /// Called just before the player dies
            /// </summary>
            /// <param name="pb"></param>
            public virtual void PlayerDead(Kit_PlayerBehaviour pb) { }

            /// <summary>
            /// OnControllerColliderHit relay
            /// </summary>
            /// <param name="hit"></param>
            public abstract void OnControllerColliderHitRelay(Kit_PlayerBehaviour pb, ControllerColliderHit hit);

            /// <summary>
            /// OnAnimatorIK relay
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="anim"></param>
            public abstract void OnAnimatorIKCallback(Kit_PlayerBehaviour pb, Animator anim);

            /// <summary>
            /// When the fall down effect should be played, this is called
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="runtimeData"></param>
            /// <param name="wasFallDamageApplied"></param>
            public abstract void FallDownEffect(Kit_PlayerBehaviour pb, bool wasFallDamageApplied);
            #endregion

            #region Values for other systems
            /// <summary>
            /// Get current weapon state
            /// 0 = loaded
            /// 1 = empty
            /// 2 = completely empty
            /// </summary>
            /// <param name="pb"></param>
            /// <returns></returns>
            public abstract int WeaponState(Kit_PlayerBehaviour pb);

            /// <summary>
            /// Must the camera be forced into first person? E.g. fullscreen aiming (sniper)
            /// </summary>
            /// <param name="pb"></param>
            /// <returns></returns>
            public abstract bool ForceIntoFirstPerson(Kit_PlayerBehaviour pb);

            /// <summary>
            /// Aiming time
            /// </summary>
            /// <param name="pb"></param>
            /// <returns></returns>
            public abstract float AimInTime(Kit_PlayerBehaviour pb);

            /// <summary>
            /// Get current weapon type
            /// 0 = Full Auto
            /// 1 = Semi Auto
            /// 2 = Close up only semi
            /// </summary>
            /// <param name="pb"></param>
            /// <returns></returns>
            public abstract int WeaponType(Kit_PlayerBehaviour pb);

            /// <summary>
            /// Retrives whether we are currently aiming or not
            /// </summary>
            /// <param name="pb"></param>
            /// <returns></returns>
            public abstract bool IsAiming(Kit_PlayerBehaviour pb);

            /// <summary>
            /// Retrieves the percentage (0-1) of our current aim progress
            /// </summary>
            /// <param name="pb"></param>
            /// <returns></returns>
            public abstract float GetAimingPercentage(Kit_PlayerBehaviour pb);

            /// <summary>
            /// Retrives the current movement multiplier
            /// </summary>
            /// <param name="pb"></param>
            /// <returns></returns>
            public abstract float CurrentMovementMultiplier(Kit_PlayerBehaviour pb);

            /// <summary>
            /// Returns the current sensitivty for the mouse look
            /// </summary>
            /// <param name="pb"></param>
            /// <returns></returns>
            public abstract float CurrentSensitivity(Kit_PlayerBehaviour pb);

            /// <summary>
            /// Does the weapon manager currently allow us to run?
            /// </summary>
            /// <param name="pb"></param>
            /// <returns></returns>
            public abstract bool CanRun(Kit_PlayerBehaviour pb);

            /// <summary>
            /// Returns the currently selected weapon ID
            /// </summary>
            /// <param name="pb"></param>
            /// <returns></returns>
            public abstract int GetCurrentWeapon(Kit_PlayerBehaviour pb);

            /// <summary>
            /// Returns the runtime data of the given slot
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="slot"></param>
            /// <returns></returns>
            public abstract Kit_WeaponRuntimeDataBase GetRuntimeDataOfWeapon(Kit_PlayerBehaviour pb, int slot);

            /// <summary>
            /// Tells us if we can buy the supplied weapon id
            /// Essentially: If the weapon is in our inventory
            /// Special case grenades: If grenade amount is under maximum (start) amount, it also returns true
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="id"></param>
            /// <returns></returns>
            public abstract bool CanBuyWeapon(Kit_PlayerBehaviour pb, int id);

            /// <summary>
            /// Tells us if our weapon is full or not
            /// </summary>
            /// <param name="pb"></param>
            /// <returns></returns>
            public abstract bool IsCurrentWeaponFull(Kit_PlayerBehaviour pb);
            #endregion

            #region Weapon Network Relays
            /// <summary>
            /// We received a semi shot RPC
            /// </summary>
            public abstract void NetworkSemiRPCReceived(Kit_PlayerBehaviour pb);

            /// <summary>
            /// We received a bolt action RPC
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="state">The state of this bolt action</param>
            public abstract void NetworkBoltActionRPCReceived(Kit_PlayerBehaviour pb, int state);

            /// <summary>
            /// We received a burst RPC
            /// </summary>
            /// <param name="pb"></param>
            public abstract void NetworkBurstRPCReceived(Kit_PlayerBehaviour pb, int burstLength);

            /// <summary>
            /// We received a reload RPC
            /// </summary>
            /// <param name="isEmpty"></param>
            public abstract void NetworkReloadRPCReceived(Kit_PlayerBehaviour pb, bool isEmpty);

            /// <summary>
            /// We received a procedural reload RPC
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="stage"></param>
            public abstract void NetworkProceduralReloadRPCReceived(Kit_PlayerBehaviour pb, int stage);

            /// <summary>
            /// Hit
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="hit"></param>
            public abstract void NetworkMeleeStabRPCReceived(Kit_PlayerBehaviour pb, int state, int slot);

            /// <summary>
            /// Charge
            /// </summary>
            public abstract void NetworkMeleeChargeRPCReceived(Kit_PlayerBehaviour pb, int state, int slot);

            /// <summary>
            /// Heal
            /// </summary>
            public abstract void NetworkMeleeHealRPCReceived(Kit_PlayerBehaviour pb, int slot);

            /// <summary>
            /// Grenade Pull Pin!
            /// </summary>
            /// <param name="pb"></param>
            public abstract void NetworkGrenadePullPinRPCReceived(Kit_PlayerBehaviour pb);

            /// <summary>
            /// Grenade Throw!
            /// </summary>
            /// <param name="pb"></param>
            public abstract void NetworkGrenadeThrowRPCReceived(Kit_PlayerBehaviour pb);

            /// <summary>
            /// We received a weapon replace RPC
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="slot"></param>
            /// <param name="weapon"></param>
            /// <param name="bulletsLeft"></param>
            /// <param name="bulletsLeftToReload"></param>
            /// <param name="attachments"></param>
            public abstract void NetworkReplaceWeapon(Kit_PlayerBehaviour pb, int slot, int weapon, int bulletsLeft, int bulletsLeftToReload, int[] attachments);

            /// <summary>
            /// A (other) player has fired a physical bullet and is now telling us to do the same.
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="pos"></param>
            /// <param name="dir"></param>
            public abstract void NetworkPhysicalBulletFired(Kit_PlayerBehaviour pb, Vector3 pos, Vector3 dir);
            #endregion

            #region Relays
            /// <summary>
            /// Called when we enter a trigger
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="isThirdPersonEnabled"></param>
            public abstract void OnTriggerEnterRelay(Kit_PlayerBehaviour pb, Collider col);

            /// <summary>
            /// Called when we exit a trigger
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="isThirdPersonEnabled"></param>
            public abstract void OnTriggerExitRelay(Kit_PlayerBehaviour pb, Collider col);
            #endregion

            #region Other Functionality
            /// <summary>
            /// Called on ammo pickup. Implement if you want to be able to pickup ammo
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="pickup"></param>
            public virtual void OnAmmoPickup(Kit_PlayerBehaviour pb, Kit_AmmoPickup pickup)
            {

            }

            /// <summary>
            /// Restocks  ammo
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="allWeapons"></param>
            public abstract void RestockAmmo(Kit_PlayerBehaviour pb, bool allWeapons);

            /// <summary>
            /// Start spectating
            /// </summary>
            /// <param name="pb"></param>
            public abstract void BeginSpectating(Kit_PlayerBehaviour pb);

            /// <summary>
            /// End spectator mode
            /// </summary>
            /// <param name="pb"></param>
            public abstract void EndSpectating(Kit_PlayerBehaviour pb);
            #endregion

            #region Plugin System Calls
            public abstract int GetCurrentlySelectedWeapon(Kit_PlayerBehaviour pb);

            public abstract int GetCurrentlyDesiredWeapon(Kit_PlayerBehaviour pb);

            public abstract int GetCurrentlyDesiredQuickUse(Kit_PlayerBehaviour pb);

            /// <summary>
            /// Returns id of weapon in array if it exists, otherwise -1
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="id"></param>
            /// <returns></returns>
            public abstract int HasWeapon(Kit_PlayerBehaviour pb, int id);

            /// <summary>
            /// Set the weapon that the player wants to select
            /// </summary>
            /// <param name="pb"></param>
            /// <param name="desiredWeapon"></param>
            public abstract void SetDesiredWeapon(Kit_PlayerBehaviour pb, int desiredWeapon);

            /// <summary>
            /// Returns all slots that have an unselectable (placedholer) weapon in them
            /// </summary>
            /// <param name="pb"></param>
            /// <returns></returns>
            public abstract int[] GetSlotsWithEmptyWeapon(Kit_PlayerBehaviour pb);

            /// <summary>
            /// For plugins so that they can force select weapons
            /// </summary>
            /// <param name="slot"></param>
            /// <param name="id"></param>
            /// <param name="locked"></param>
            public abstract void PluginSelectWeapon(Kit_PlayerBehaviour pb, int slot, bool locked = true);
            #endregion
        }
    }
}