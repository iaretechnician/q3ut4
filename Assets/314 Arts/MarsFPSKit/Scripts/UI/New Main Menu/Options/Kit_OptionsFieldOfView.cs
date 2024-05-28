using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    namespace UI
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Options/Gameplay/Field of View")]
        public class Kit_OptionsFieldOfView : Kit_OptionBase
        {
            /// <summary>
            /// Minimum Value
            /// </summary>
            public float minValue = 60f;
            /// <summary>
            /// Maximum Value
            /// </summary>
            public float maxValue = 120f;

            public override string GetDisplayName()
            {
                return "Field of view";
            }

            public override string GetHoverText()
            {
                return "How much you actually see.";
            }

            public override OptionType GetOptionType()
            {
                return OptionType.Slider;
            }

            public override void OnSliderStart(TextMeshProUGUI txt, Slider slider)
            {
                float load = PlayerPrefs.GetFloat("fieldOfView", 60f);
                load = Mathf.Clamp(load, minValue, maxValue);

                slider.minValue = minValue;
                slider.maxValue = maxValue;

                slider.value = load;
                OnSliderChange(txt, load);
            }

            public override void OnSliderChange(TextMeshProUGUI txt, float newValue)
            {
                Kit_GameSettings.baseFov = newValue;
                PlayerPrefs.SetFloat("fieldOfView", newValue);
                txt.text = GetDisplayName() + ": " + newValue.ToString("F0") + "°";
            }
        }
    }
}