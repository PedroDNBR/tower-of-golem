using UnityEngine;
using UnityEngine.UI;

namespace TW
{
    public static class UIUtils
    {
        public static void SetSliderValue(Slider slider, float value)
        {
            slider.value = value;
        }

        public static void ConvertToSliderValue(Slider slider, float current, float max)
        {
            float sliderValue = Mathf.InverseLerp(0f, max, current);
            SetSliderValue(slider, sliderValue);
        }
    }
}
