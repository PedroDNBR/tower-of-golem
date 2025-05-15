using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
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
        private Transform optionsMenu;

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
            NetworkManager.Singleton.OnClientDisconnectCallback += QuitWhenLobbyHostDisconnects;
        }

        public void HealthValueToSliderValue(float current, float max)
        {
            UIUtils.ConvertToSliderValue(healthSlider, current, max);
        }

        public void StaminaTimeToSliderValue(float current, float max)
        {
            UIUtils.ConvertToSliderValue(staminaSlider, current, max);
        }

        public void TogglePauseMenu()
        {
            if(pauseMenu.gameObject.activeSelf)
                EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
            else
            {
                settingsMenu.gameObject.SetActive(false);
                optionsMenu.gameObject.SetActive(true);
            }

            pauseMenu.gameObject.SetActive(!pauseMenu.gameObject.activeSelf);
        }

        bool GetPauseMenuIsOpen() => pauseMenu.gameObject.activeSelf;

        public void OpenSettings()
        {
            optionsMenu.gameObject.SetActive(false);
            settingsMenu.gameObject.SetActive(true);
        }

        private void ClosePauseMenu()
        {
            pauseMenu.gameObject.SetActive(false);
            optionsMenu.gameObject.SetActive(true);
            settingsMenu.gameObject.SetActive(false);
        }

        private void QuitGame()
        {
            Destroy(NetworkManager.Singleton.gameObject);
            SceneManager.LoadScene("MainMenu");
        }

        private void QuitWhenLobbyHostDisconnects(ulong id)
        {
            Debug.Log($"{id} == {NetworkManager.Singleton.LocalClientId} {id == NetworkManager.Singleton.LocalClientId}");
            if(id == NetworkManager.Singleton.LocalClientId) QuitGame();
        }
    }
}
