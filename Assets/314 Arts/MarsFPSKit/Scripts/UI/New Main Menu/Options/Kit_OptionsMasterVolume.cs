using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MarsFPSKit
{
    namespace UI
    {
        [CreateAssetMenu(menuName = "MarsFPSKit/Options/Audio/Master Volume")]
        public class Kit_OptionsMasterVolume : Kit_OptionBase
        {
            public override string GetDisplayName()
            {
                return "Master Volume";
            }

            public override string GetHoverText()
            {
                return "Volume of everything.";
            }

            public override OptionType GetOptionType()
            {
                return OptionType.Slider;
            }

            public override void OnSliderStart(TextMeshProUGUI txt, Slider slider)
            {
                float load = PlayerPrefs.GetFloat("audioVolume", 1f);
                load = Mathf.Clamp(load, 0, 1);

                slider.minValue = 0;
                slider.maxValue = 1;

                slider.value = load;
                OnSliderChange(txt, load);
            }

            public override void OnSliderChange(TextMeshProUGUI txt, float newValue)
            {
                AudioListener.volume = newValue;
                PlayerPrefs.SetFloat("audioVolume", newValue);

                txt.text = GetDisplayName() + ": " + (newValue * 100f).ToString("F0") + "%";
            }
        }
    }
}