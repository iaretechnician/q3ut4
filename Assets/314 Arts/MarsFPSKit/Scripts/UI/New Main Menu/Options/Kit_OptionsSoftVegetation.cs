using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    namespace UI
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Options/Graphics/Soft Vegetation")]
        public class Kit_OptionsSoftVegetation : Kit_OptionBase
        {
            public override string GetDisplayName()
            {
                return "Soft Vegetation";
            }

            public override string GetHoverText()
            {
                return "Use a two-pass shader for the vegetation in the terrain engine. If enabled, vegetation will have smoothed edges, if disabled all plants will have hard edges but are rendered roughly twice as fast";
            }

            public override OptionType GetOptionType()
            {
                return OptionType.Toggle;
            }

            public override void OnToggleStart(TextMeshProUGUI txt, Toggle toggle)
            {
                bool value = PlayerPrefsExtended.GetBool("softVegetation", true);

                toggle.isOn = value;
                OnToggleChange(txt, value);
            }

            public override void OnToggleChange(TextMeshProUGUI txt, bool newValue)
            {
                QualitySettings.softParticles = newValue;
                PlayerPrefsExtended.SetBool("softVegetation", newValue);
            }
        }
    }
}