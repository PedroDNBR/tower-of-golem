using UnityEngine;
using UnityEngine.UI;

namespace TW
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField]
        private Slider healthSlider;

        [SerializeField]
        private Slider staminaSlider;

        [SerializeField]
        private Canvas canvas;

        private void Start()
        {
            canvas.gameObject.SetActive(true);
        }

        public void ConvertHealthValueToSliderValue(float current, float max)
        {
            float sliderValue = Mathf.InverseLerp(0f, max, current);
            SetSliderValue(healthSlider, sliderValue);
        }

        public void ConvertStaminaTimeToSliderValue(float current, float max)
        {
            float sliderValue = Mathf.InverseLerp(0f, max, current);
            SetSliderValue(staminaSlider, sliderValue);
        }

        public void SetSliderValue(Slider slider, float value)
        {
            slider.value = value;
        }
    }
}
