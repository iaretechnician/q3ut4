using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    namespace UI
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Options/Graphics/Vsync")]
        public class Kit_OptionsVsync : Kit_OptionBase
        {
            public override string GetDisplayName()
            {
                return "VSync";
            }

            public override string GetHoverText()
            {
                return "Vertical Syncronization";
            }

            public override OptionType GetOptionType()
            {
                return OptionType.Toggle;
            }

            public override void OnToggleStart(TextMeshProUGUI txt, Toggle toggle)
            {
                bool value = PlayerPrefsExtended.GetBool("vsync", true);

                toggle.isOn = value;
                OnToggleChange(txt, value);
            }

            public override void OnToggleChange(TextMeshProUGUI txt, bool newValue)
            {
                QualitySettings.vSyncCount = newValue ? 1 : 0;
                PlayerPrefsExtended.SetBool("vsync", newValue);
            }
        }
    }
}