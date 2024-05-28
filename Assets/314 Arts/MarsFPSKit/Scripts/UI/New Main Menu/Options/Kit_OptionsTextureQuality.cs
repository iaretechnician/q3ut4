using System.Linq;
using TMPro;
using UnityEngine;

namespace MarsFPSKit
{
    namespace UI
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Options/Graphics/Texture Quality")]
        public class Kit_OptionsTextureQuality : Kit_OptionBase
        {
            public string[] textureQualitySettings = { "Very High", "High", "Medium", "Low" };

            public override string GetDisplayName()
            {
                return "Texture Quality";
            }

            public override string GetHoverText()
            {
                return "A texture size limit applied to all textures.";
            }

            public override OptionType GetOptionType()
            {
                return OptionType.Dropdown;
            }

            public override void OnDropdownStart(TextMeshProUGUI txt, TMP_Dropdown dropdown)
            {
                //Load
                int selectedResolution = PlayerPrefs.GetInt("textureResolution", 0);
                //Clamp
                selectedResolution = Mathf.Clamp(selectedResolution, 0, textureQualitySettings.Length - 1);
                //Clear
                dropdown.ClearOptions();
                //Add
                dropdown.AddOptions(textureQualitySettings.ToList());
                //Set default value
                dropdown.value = selectedResolution;
                //Use that value
                OnDropdowChange(txt, selectedResolution);
            }

            public override void OnDropdowChange(TextMeshProUGUI txt, int newValue)
            {
                //Set
                QualitySettings.globalTextureMipmapLimit = newValue;
                //Save
                PlayerPrefs.SetInt("textureResolution", newValue);
            }
        }
    }
}