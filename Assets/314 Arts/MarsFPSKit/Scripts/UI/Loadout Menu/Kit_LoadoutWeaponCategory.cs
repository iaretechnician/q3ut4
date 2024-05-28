using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MarsFPSKit
{
    public class Kit_LoadoutWeaponCategory : MonoBehaviour
    {
        /// <summary>
        /// Event trigger to get the hover!
        /// </summary>
        public EventTrigger eventTrigger;
        /// <summary>
        /// Picture of the currently selected weapon
        /// </summary>
        public Image currentWeaponImage;
        /// <summary>
        /// Dropdown that has all weapons
        /// </summary>
        public Dropdown weaponsInDropdown;
        /// <summary>
        /// Converts <see cref="weaponsInDropdown"/> to global ones
        /// </summary>
        public List<int> dropdownLocalToGlobal = new List<int>();
        /// <summary>
        /// The button that triggers the customize
        /// </summary>
        public Button customizeWeaponButton;
        /// <summary>
        /// How this category is called
        /// </summary>
        public Text weaponCategoryName;
        /// <summary>
        /// Statistics root object (to hide or unhide dependent on weapon)
        /// </summary>
        public GameObject statsRoot;
        /// <summary>
        /// Damage stats
        /// </summary>
        public Image statsDamageFill;
        /// <summary>
        /// Fire Rate stats
        /// </summary>
        public Image statsFireRateFill;
        /// <summary>
        /// Recoil stats
        /// </summary>
        public Image statsRecoilFill;
        /// <summary>
        /// Reach stats
        /// </summary>
        public Image statsReachFill;
    }
}