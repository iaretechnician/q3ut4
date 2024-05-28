using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MarsFPSKit
{
    public class Kit_LoadoutPlayerModelCategory : MonoBehaviour
    {
        /// <summary>
        /// Event trigger to get the hover!
        /// </summary>
        public EventTrigger eventTrigger;
        /// <summary>
        /// Dropdown button
        /// </summary>
        public Dropdown dropdown;
        /// <summary>
        /// Customize button
        /// </summary>
        public Button customizeButton;
        /// <summary>
        /// Team name
        /// </summary>
        public Text teamText;
    }
}