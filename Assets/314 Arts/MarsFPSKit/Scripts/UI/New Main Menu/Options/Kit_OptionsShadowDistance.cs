using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    namespace UI
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Options/Graphics/Shadow Distance")]
        public class Kit_OptionsShadowDistance : Kit_OptionBase
        {
            /// <summary>
            /// Minimum Value
            /// </summary>
            public float minValue = 0f;
            /// <summary>
            /// Maximum Value
            /// </summary>
            public float maxValue = 150f;

            public override string GetDisplayName()
            {
                return "Shadow Distance";
            }

            public override string GetHoverText()
            {
                return "Shadow drawing distance.";
            }

            public override OptionType GetOptionType()
            {
                return OptionType.Slider;
            }

            public override void OnSliderStart(TextMeshProUGUI txt, Slider slider)
            {
                float load = PlayerPrefs.GetFloat("shadowDistance", 50f);
                load = Mathf.Clamp(load, minValue, maxValue);

                slider.minValue = minValue;
                slider.maxValue = maxValue;

                slider.value = load;
                OnSliderChange(txt, load);
            }

            public override void OnSliderChange(TextMeshProUGUI txt, float newValue)
            {
                QualitySettings.shadowDistance = newValue;
                PlayerPrefs.SetFloat("shadowDistance", newValue);

                txt.text = GetDisplayName() + ": " + newValue.ToString("F0");
            }
        }
    }
}