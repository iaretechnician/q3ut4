using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MarsFPSKit
{
    namespace UI
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Options/Graphics/Resolution")]
        public class Kit_OptionsResolution : Kit_OptionBase
        {
            public override string GetDisplayName()
            {
                return "Resolution";
            }

            public override string GetHoverText()
            {
                return "Select your desired display resolution";
            }

            public override OptionType GetOptionType()
            {
                return OptionType.Dropdown;
            }

            public override void OnDropdownStart(TextMeshProUGUI txt, TMP_Dropdown dropdown)
            {
                //Load
                int selectedResolution = PlayerPrefs.GetInt("resolution", Screen.resolutions.Length - 1);
                //Clamp
                selectedResolution = Mathf.Clamp(selectedResolution, 0, Screen.resolutions.Length - 1);
                //Setup Dropdown
                dropdown.ClearOptions();
                //Create List
                List<string> resolutions = new List<string>();

                for (int i = 0; i < Screen.resolutions.Length; i++)
                {
                    //Create string
                    resolutions.Add(Screen.resolutions[i].width + "x" + Screen.resolutions[i].height + "@" + Screen.resolutions[i].refreshRate + "Hz");
                }

                //Add
                dropdown.AddOptions(resolutions);
                //Set default value
                dropdown.value = selectedResolution;
                //Use that value
                OnDropdowChange(txt, selectedResolution);
            }

            public override void OnDropdowChange(TextMeshProUGUI txt, int newValue)
            {
#if !UNITY_ANDROID && !UNITY_IOS
                if (Screen.currentResolution.width != Screen.resolutions[newValue].width || Screen.currentResolution.height != Screen.resolutions[newValue].height || Screen.currentResolution.refreshRate != Screen.resolutions[newValue].refreshRate)
                {
                    //Set resoltuion
                    Screen.SetResolution(Screen.resolutions[newValue].width, Screen.resolutions[newValue].height, Screen.fullScreenMode, Screen.resolutions[newValue].refreshRate);
                    //Save
                    PlayerPrefs.SetInt("resolution", newValue);
                }
#endif
            }
        }
    }
}