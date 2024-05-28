using System.Linq;
using TMPro;
using UnityEngine;

namespace MarsFPSKit
{
    namespace UI
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Options/Graphics/Shadows")]
        public class Kit_OptionsShadows : Kit_OptionBase
        {
            public string[] valueToString = { "Disable", "Hard", "Soft" };

            public override string GetDisplayName()
            {
                return "Shadows";
            }

            public override string GetHoverText()
            {
                return "Realtime Shadows type to be used. This determines which type of shadows should be used.The available options are Hard and Soft Shadows, Hard Shadows Only and Disable Shadows.";
            }

            public override OptionType GetOptionType()
            {
                return OptionType.Dropdown;
            }

            public override void OnDropdownStart(TextMeshProUGUI txt, TMP_Dropdown dropdown)
            {
                //Load
                int selected = PlayerPrefs.GetInt("shadows", 2);
                //Clamp
                selected = Mathf.Clamp(selected, 0, 2);
                //Clear
                dropdown.ClearOptions();
                //Add
                dropdown.AddOptions(valueToString.ToList());
                //Set default value
                dropdown.value = selected;
                //Use that value
                OnDropdowChange(txt, selected);
            }

            public override void OnDropdowChange(TextMeshProUGUI txt, int newValue)
            {
                //Set
                QualitySettings.shadows = (ShadowQuality)newValue;
                //Save
                PlayerPrefs.SetInt("shadows", newValue);
            }
        }
    }
}