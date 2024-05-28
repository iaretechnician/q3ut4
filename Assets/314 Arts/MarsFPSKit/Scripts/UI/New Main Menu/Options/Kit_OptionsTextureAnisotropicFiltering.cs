using System.Linq;
using TMPro;
using UnityEngine;

namespace MarsFPSKit
{
    namespace UI
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Options/Graphics/Anisotropic Filtering")]
        public class Kit_OptionsTextureAnisotropicFiltering : Kit_OptionBase
        {
            public string[] valueToString = { "Disable", "Enable", "Force Enable" };

            public override string GetDisplayName()
            {
                return "Anisotropic Filtering";
            }

            public override string GetHoverText()
            {
                return "Global anisotropic filtering mode.";
            }

            public override OptionType GetOptionType()
            {
                return OptionType.Dropdown;
            }

            public override void OnDropdownStart(TextMeshProUGUI txt, TMP_Dropdown dropdown)
            {
                //Load
                int selected = PlayerPrefs.GetInt("anisotropicFiltering", 2);
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
                QualitySettings.anisotropicFiltering = (AnisotropicFiltering)newValue;
                //Save
                PlayerPrefs.SetInt("anisotropicFiltering", newValue);
            }
        }
    }
}