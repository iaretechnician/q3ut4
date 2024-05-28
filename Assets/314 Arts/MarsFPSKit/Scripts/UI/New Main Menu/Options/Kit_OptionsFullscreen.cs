using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    namespace UI
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Options/Graphics/Fullscreen")]
        public class Kit_OptionsFullscreen : Kit_OptionBase
        {
            public override string GetDisplayName()
            {
                return "Fullscreen";
            }

            public override string GetHoverText()
            {
                return "Fullscreen window or windowed mode?";
            }

            public override OptionType GetOptionType()
            {
                return OptionType.Toggle;
            }

            public override void OnToggleStart(TextMeshProUGUI txt, Toggle toggle)
            {
                bool value = PlayerPrefsExtended.GetBool("fullScreen", true);

                toggle.isOn = value;
                OnToggleChange(txt, value);
            }

            public override void OnToggleChange(TextMeshProUGUI txt, bool newValue)
            {
#if !UNITY_ANDROID && !UNITY_IOS
                if (Screen.fullScreenMode != (newValue ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed))
                {
                    Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, newValue ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed, Screen.currentResolution.refreshRate);
                }
#endif
                PlayerPrefsExtended.SetBool("fullScreen", newValue);
            }
        }
    }
}