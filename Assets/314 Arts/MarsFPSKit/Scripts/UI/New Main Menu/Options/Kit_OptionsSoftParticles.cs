using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    namespace UI
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Options/Graphics/Soft Particles")]
        public class Kit_OptionsSoftParticles : Kit_OptionBase
        {
            public override string GetDisplayName()
            {
                return "Soft Particles";
            }

            public override string GetHoverText()
            {
                return "Should soft blending be used for particles?";
            }

            public override OptionType GetOptionType()
            {
                return OptionType.Toggle;
            }

            public override void OnToggleStart(TextMeshProUGUI txt, Toggle toggle)
            {
                bool value = PlayerPrefsExtended.GetBool("softParticles", true);

                toggle.isOn = value;
                OnToggleChange(txt, value);
            }

            public override void OnToggleChange(TextMeshProUGUI txt, bool newValue)
            {
                QualitySettings.softParticles = newValue;
                PlayerPrefsExtended.SetBool("softParticles", newValue);
            }
        }
    }
}