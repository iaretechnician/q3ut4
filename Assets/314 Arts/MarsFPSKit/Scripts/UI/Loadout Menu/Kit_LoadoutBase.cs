
using UnityEngine;

namespace MarsFPSKit
{
    [System.Serializable]
    /// <summary>
    /// Helper class that contains loadout info
    /// </summary>
    public class Loadout
    {
        /// <summary>
        /// Selected Weapons
        /// </summary>
        public LoadoutWeapon[] loadoutWeapons;
        /// <summary>
        /// Player Model Selection
        /// </summary>
        public TeamLoadout[] teamLoadout;
    }

    [System.Serializable]
    public class TeamLoadout
    {
        /// <summary>
        /// Player Model ID
        /// </summary>
        public int playerModelID;
        /// <summary>
        /// Customizations for player model
        /// </summary>
        public int[] playerModelCustomizations;
    }

    [System.Serializable]
    public class LoadoutWeapon
    {
        /// <summary>
        /// ID of this weapon in "allWeapons"
        /// </summary>
        public int weaponID;
        /// <summary>
        /// Attachments for this weapon
        /// </summary>
        public int[] attachments;
    }

    /// <summary>
    /// Use this to implement your own loadout menu
    /// </summary>
    public abstract class Kit_LoadoutBase : MonoBehaviour
    {
        /// <summary>
        /// Returns the currently selected loadout
        /// </summary>
        /// <returns></returns>
        public abstract Loadout GetCurrentLoadout();

        /// <summary>
        /// Called from <see cref="Kit_IngameMain"/> or from <see cref="UI.Kit_MenuManager"/> to initialize the Loadout menu at the beginning
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Called when the Loadout menu should be opened
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// The Loadout menu should usually close itself. When the game needs it to close, this is called.
        /// </summary>
        public abstract void ForceClose();

        /// <summary>
        /// Is the loadout menu currently open?
        /// </summary>
        /// <returns></returns>
        public bool isOpen;

        /// <summary>
        /// Screen ID for the menu
        /// </summary>
        public int menuScreenId;

        /// <summary>
        /// Called when the team has changed
        /// </summary>
        /// <param name="newTeam"></param>
        public virtual void TeamChanged(int newTeam)
        {

        }

        /// <summary>
        /// Called when the local player spawned
        /// </summary>
        /// <param name="pb"></param>
        public virtual void LocalPlayerSpawned(Kit_PlayerBehaviour pb)
        {

        }
    }
}
