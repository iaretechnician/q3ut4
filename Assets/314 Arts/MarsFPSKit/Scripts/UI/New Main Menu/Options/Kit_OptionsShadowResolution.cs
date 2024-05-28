using System.Linq;
using TMPro;
using UnityEngine;

namespace MarsFPSKit
{
    namespace UI
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Options/Graphics/Shadow Resolution")]
        public class Kit_OptionsShadowResolution : Kit_OptionBase
        {
            public string[] valueToString = { "Low", "Medium", "High", "Very High" };

            public override string GetDisplayName()
            {
                return "Shadow Resolution";
            }

            public override string GetHoverText()
            {
                return "The default resolution of the shadow maps.";
            }

            public override OptionType GetOptionType()
            {
                return OptionType.Dropdown;
            }

            public override void OnDropdownStart(TextMeshProUGUI txt, TMP_Dropdown dropdown)
            {
                //Load
                int selected = PlayerPrefs.GetInt("shadowResolution", 3);
                //Clamp
                selected = Mathf.Clamp(selected, 0, 3);
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
                QualitySettings.shadowResolution = (ShadowResolution)newValue;
                //Save
                PlayerPrefs.SetInt("shadowResolution", newValue);
            }
        }
    }
}