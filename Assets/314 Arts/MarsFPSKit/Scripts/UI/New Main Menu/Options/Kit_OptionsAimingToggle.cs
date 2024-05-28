using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    namespace UI
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Options/Controls/Aiming Toggle")]
        public class Kit_OptionsAimingToggle : Kit_OptionBase
        {
            public override string GetDisplayName()
            {
                return "Aiming Toggle";
            }

            public override string GetHoverText()
            {
                return "Should aiming be toggle or hold?";
            }

            public override OptionType GetOptionType()
            {
                return OptionType.Toggle;
            }

            public override void OnToggleStart(TextMeshProUGUI txt, Toggle toggle)
            {
                bool value = PlayerPrefsExtended.GetBool("aimingToggle", true);

                toggle.isOn = value;
                OnToggleChange(txt, value);
            }

            public override void OnToggleChange(TextMeshProUGUI txt, bool newValue)
            {
                Kit_GameSettings.isAimingToggle = newValue;
                PlayerPrefsExtended.SetBool("aimingToggle", newValue);
            }
        }
    }
}