using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    namespace UI
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Options/Controls/Crouch Toggle")]
        public class Kit_OptionsCrouchToggle : Kit_OptionBase
        {
            public override string GetDisplayName()
            {
                return "Crouch Toggle";
            }

            public override string GetHoverText()
            {
                return "Should crouching be toggle or hold?";
            }

            public override OptionType GetOptionType()
            {
                return OptionType.Toggle;
            }

            public override void OnToggleStart(TextMeshProUGUI txt, Toggle toggle)
            {
                bool value = PlayerPrefsExtended.GetBool("crouchingToggle", true);

                toggle.isOn = value;
                OnToggleChange(txt, value);
            }

            public override void OnToggleChange(TextMeshProUGUI txt, bool newValue)
            {
                Kit_GameSettings.isCrouchToggle = newValue;
                PlayerPrefsExtended.SetBool("crouchingToggle", newValue);
            }
        }
    }
}