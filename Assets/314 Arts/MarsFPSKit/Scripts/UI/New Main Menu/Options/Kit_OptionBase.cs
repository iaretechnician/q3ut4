using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MarsFPSKit
{
    namespace UI
    {
        public enum OptionType { Slider, Dropdown, Toggle }

        public abstract class Kit_OptionBase : ScriptableObject
        {
            /// <summary>
            /// Returns the name that is displayed
            /// </summary>
            /// <returns></returns>
            public abstract string GetDisplayName();

            /// <summary>
            /// Returns the hover text
            /// </summary>
            /// <returns></returns>
            public abstract string GetHoverText();

            /// <summary>
            /// What type is this setting?
            /// </summary>
            /// <returns></returns>
            public abstract OptionType GetOptionType();

            public virtual void OnToggleChange(TextMeshProUGUI txt, bool newValue)
            {

            }

            public virtual void OnToggleStart(TextMeshProUGUI txt, Toggle toggle)
            {

            }

            public virtual void OnDropdowChange(TextMeshProUGUI txt, int newValue)
            {

            }

            public virtual void OnDropdownStart(TextMeshProUGUI txt, TMP_Dropdown dropdown)
            {

            }

            public virtual void OnSliderChange(TextMeshProUGUI txt, float newValue)
            {

            }

            public virtual void OnSliderStart(TextMeshProUGUI txt, Slider slider)
            {

            }
        }
    }
}