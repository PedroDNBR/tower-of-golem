using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TW
{
    public class PlayerUI : MonoBehaviour
    {

        [SerializeField]
        private Canvas canvas;

        [SerializeField]
        private Slider healthSlider;

        [SerializeField]
        private Slider staminaSlider;

        [SerializeField]
        private Transform bossHUD;

        [SerializeField]
        private TextMeshProUGUI bossNameText;

        [SerializeField]
        private Slider bossHealthSlider;

        public Transform BossHUD { get => bossHUD; }

        public TextMeshProUGUI BossNameText { get => bossNameText; }

        public Slider BossHealthSlider { get => bossHealthSlider; }

        private void Start()
        {
            canvas.gameObject.SetActive(true);
        }

        public void HealthValueToSliderValue(float current, float max)
        {
            UIUtils.ConvertToSliderValue(healthSlider, current, max);
        }

        public void StaminaTimeToSliderValue(float current, float max)
        {
            UIUtils.ConvertToSliderValue(staminaSlider, current, max);
        }
    }
}
