using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    namespace UI
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Options/Controls/Sensitivity")]
        public class Kit_OptionsSensitivity : Kit_OptionBase
        {
            public enum SensType { Hip, Aim, Fullscreen }
            /// <summary>
            /// Our sensitivity type
            /// </summary>
            public SensType type;

            /// <summary>
            /// Minimum Value
            /// </summary>
            public float minValue = 0f;
            /// <summary>
            /// Maximum Value
            /// </summary>
            public float maxValue = 10f;

            public override string GetDisplayName()
            {
                if (type == SensType.Hip) return "Hip Sensitivity";
                else if (type == SensType.Aim) return "Aim Sensitivity";
                else if (type == SensType.Fullscreen) return "Fullscreen Sensitivity";

                return "";
            }

            public override string GetHoverText()
            {
                if (type == SensType.Hip) return "Hip Sensitivity";
                else if (type == SensType.Aim) return "Aim Sensitivity";
                else if (type == SensType.Fullscreen) return "Fullscreen Sensitivity";

                return "";
            }

            public override OptionType GetOptionType()
            {
                return OptionType.Slider;
            }

            public override void OnSliderStart(TextMeshProUGUI txt, Slider slider)
            {
                float load = PlayerPrefs.GetFloat("sens" + type.ToString(), 1f);
                load = Mathf.Clamp(load, minValue, maxValue);

                slider.minValue = minValue;
                slider.maxValue = maxValue;

                slider.value = load;
                OnSliderChange(txt, load);
            }

            public override void OnSliderChange(TextMeshProUGUI txt, float newValue)
            {
                if (type == SensType.Hip) Kit_GameSettings.hipSensitivity = newValue;
                else if (type == SensType.Aim) Kit_GameSettings.aimSensitivity = newValue;
                else if (type == SensType.Fullscreen) Kit_GameSettings.fullScreenAimSensitivity = newValue;
                PlayerPrefs.SetFloat("sens" + type.ToString(), newValue);

                txt.text = GetDisplayName() + ": " + newValue.ToString("F1");
            }
        }
    }
}