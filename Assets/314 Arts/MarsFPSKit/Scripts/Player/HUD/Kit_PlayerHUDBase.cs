using UnityEngine;

namespace MarsFPSKit
{
    public abstract class Kit_PlayerHUDBase : MonoBehaviour
    {
        /// <summary>
        /// Shows or hides the HUD. Some parts (such as the hitmarker) will always be visible.
        /// </summary>
        /// <param name="visible"></param>
        public abstract void SetVisibility(bool visible);

        /// <summary>
        /// Are we underwater?
        /// </summary>
        /// <param name="isUnderwater"></param>
        public abstract void DisplayUnderwater(bool isUnderwater);

        /// <summary>
        /// Display "you are leaving the battlefield". Values less than 0 will hide it!
        /// </summary>
        /// <param name="timeLeft"></param>
        public abstract void DisplayLeavingBattlefield(float timeLeft);

        /// <summary>
        /// Should a 'Waiting for players' be shown?
        /// </summary>
        /// <param name="isWaiting"></param>
        public abstract void SetWaitingStatus(bool isWaiting);

        /// <summary>
        /// Called from the player when he spawns
        /// </summary>
        /// <param name="pb"></param>
        public abstract void PlayerStart(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called from the player when he is active in Update
        /// </summary>
        /// <param name="pb"></param>
        public abstract void PlayerUpdate(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Called from the player when he dies
        /// </summary>
        /// <param name="pb"></param>
        public abstract void PlayerEnd(Kit_PlayerBehaviour pb);

        /// <summary>
        /// Displays the hitmarker for <see cref="hitmarkerTime"/> seconds
        /// </summary>
        public abstract void DisplayHitmarker();

        /// <summary>
        /// Displays the spawn protected hitmarker for <see cref="hitmarkerTime"/> seconds
        /// </summary>
        public abstract void DisplayHitmarkerSpawnProtected();

        /// <summary>
        /// Display hit points in the HUD
        /// </summary>
        /// <param name="hp">Amount of hitpoints</param>
        public abstract void DisplayHealth(float hp);

        /// <summary>
        /// Display ammo count in the HUD
        /// </summary>
        /// <param name="bl">Bullets left (On the left side)</param>
        /// <param name="bltr">Bullets left to reload (On the right side)</param>
        public abstract void DisplayAmmo(int bl, int bltr, bool show = true);

        /// <summary>
        /// Displays the crosshair at given distance from center. 0 or smaller will hide it.
        /// </summary>
        /// <param name="size"></param>
        public abstract void DisplayCrosshair(float size, bool overrideShow);

        /// <summary>
        /// Moves the crosshair to this position
        /// </summary>
        /// <param name="pos"></param>
        public abstract void MoveCrosshairTo(Vector3 pos);

        /// <summary>
        /// Display our weapons and quick uses!
        /// </summary>
        /// <param name="weaponControllerRuntimeData"></param>
        public abstract void DisplayWeaponsAndQuickUses(Kit_PlayerBehaviour pb, Weapons.Kit_ModernWeaponManagerNetworkData weaponControllerRuntimeData);

        /// <summary>
        /// Displays the player hurt state (After being shot, for example). From 0 to 1 where 0 is display no hurt effect and 1 is full.
        /// </summary>
        /// <param name="state"></param>
        public abstract void DisplayHurtState(float state);

        /// <summary>
        /// Called when we were shot
        /// </summary>
        /// <param name="from"></param>
        public abstract void DisplayShot(Vector3 from);

        /// <summary>
        /// We were blinded!!!
        /// </summary>
        /// <param name="time"></param>
        public abstract void DisplayBlind(float time);

        /// <summary>
        /// Sets the state of the sniper scope
        /// </summary>
        /// <param name="display"></param>
        public abstract void DisplaySniperScope(bool display);

        /// <summary>
        /// Display movment state (standing / crouch icon)
        /// </summary>
        /// <param name="state"></param>
        public abstract void DisplayMovementState(int state);

        /// <summary>
        /// Sets the weapon pickup state
        /// </summary>
        /// <param name="displayed"></param>
        /// <param name="weapon"></param>
        public abstract void DisplayWeaponPickup(bool displayed, int weapon = -1);

        /// <summary>
        /// Should display interaction state
        /// </summary>
        /// <param name="display"></param>
        /// <param name="txt"></param>
        public abstract void DisplayInteraction(bool display, string txt = "");

        /// <summary>
        /// Displays the stamina
        /// </summary>
        /// <param name="stamina"></param>
        public abstract void DisplayStamina(float stamina);
        
        /// <summary>
        /// Displays the auxiliary progress bar
        /// </summary>
        /// <param name="stamina"></param>
        public abstract void DisplayAuxiliaryBar(float fill);

        /// <summary>
        /// Returns an unused player maker (for names)
        /// </summary>
        /// <returns></returns>
        public abstract int GetUnusedPlayerMarker();

        /// <summary>
        /// Releases a used player marker so a different player can use it
        /// </summary>
        /// <param name="id"></param>
        public abstract void ReleasePlayerMarker(int id);

        /// <summary>
        /// Positions a player marker at worldPos (on screen) with state and the given name
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="worldPos"></param>
        /// <param name="playerName"></param>
        public abstract void UpdatePlayerMarker(int id, PlayerNameState state, Vector3 worldPos, string playerName);

        /// <summary>
        /// Display the spawn protection with given values
        /// </summary>
        /// <param name="isActive">Is the spawn protection active?</param>
        /// <param name="timeLeft">How much time is left?</param>
        public abstract void UpdateSpawnProtection(bool isActive, float timeLeft);
    }
}
