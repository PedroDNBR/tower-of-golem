using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        [SerializeField]
        private Transform pauseMenu;

        [SerializeField]
        private Transform settingsMenu;

        [SerializeField]
        private Button continueButton;

        [SerializeField]
        private Button settingsButton;

        [SerializeField]
        private Button quitButton;

        public Transform BossHUD { get => bossHUD; }

        public TextMeshProUGUI BossNameText { get => bossNameText; }

        public Slider BossHealthSlider { get => bossHealthSlider; }

        private void Start()
        {
            canvas.gameObject.SetActive(true);
            settingsButton.onClick.AddListener(OpenSettings);
            continueButton.onClick.AddListener(ClosePauseMenu);
            quitButton.onClick.AddListener(QuitGame);
        }

        public void HealthValueToSliderValue(float current, float max)
        {
            UIUtils.ConvertToSliderValue(healthSlider, current, max);
        }

        public void StaminaTimeToSliderValue(float current, float max)
        {
            UIUtils.ConvertToSliderValue(staminaSlider, current, max);
        }

        public void TogglePauseMenu() => pauseMenu.gameObject.SetActive(!pauseMenu.gameObject.activeSelf);

        bool GetPauseMenuIsOpen() => pauseMenu.gameObject.activeSelf;

        public void OpenSettings() => settingsMenu.gameObject.SetActive(true);
        
        private void ClosePauseMenu() => pauseMenu.gameObject.SetActive(false);

        private void QuitGame() => SceneManager.LoadScene("MainMenu");
    }
}
