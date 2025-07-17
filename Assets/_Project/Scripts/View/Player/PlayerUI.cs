using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
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

        public PlayerController playerController;

        public Transform BossHUD { get => bossHUD; }

        public TextMeshProUGUI BossNameText { get => bossNameText; }

        public Slider BossHealthSlider { get => bossHealthSlider; }

        bool quitted = false;

        private void Start()
        {
            canvas.gameObject.SetActive(true);
            settingsButton.onClick.AddListener(OpenSettings);
            continueButton.onClick.AddListener(ClosePauseMenu);
            quitButton.onClick.AddListener(QuitToMenu);
            NetworkManager.Singleton.OnClientDisconnectCallback += QuitWhenServerDisconnects;
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

        private void QuitToMenu()
        {
            quitted = true;
            NetworkManager.Singleton.Shutdown();
            if(GameManager.Instance != null) GameManager.Instance.QuitToMainMenuAndDestroyNetworkManager();
        }

        private void QuitWhenServerDisconnects(ulong id)
        {
            Debug.Log($"Player quitted {id}");
            Debug.Log($"{id} == 0");
            if ((id == 0 || id == NetworkManager.Singleton.LocalClientId) && !quitted)
            {
                QuitToMenu();
            }
        }

        public void SetBossUIInPlayerVisible(bool isVisible)
        {
            if (!GetComponent<NetworkObject>().IsLocalPlayer) return;
            if (playerController == null || BossArea.instance == null || BossArea.instance.boss == null) return;
            BossArea.instance.boss.EnemyUI.EnemyHealthSlider = playerController.PlayerUI.BossHealthSlider;
            BossArea.instance.boss.EnemyUI.EnemyHUD = playerController.PlayerUI.BossHUD;
            BossArea.instance.boss.EnemyUI.EnemyNameText = playerController.PlayerUI.BossNameText;
            BossArea.instance.boss.EnemyUI.SetEnemyStats(ref BossArea.instance.boss);
            if (isVisible)
            {
                BossArea.instance.boss.SetHealthListener();
                BossArea.instance.boss.SetHealthValuesInSlider();
                BossArea.instance.boss.EnemyHealth.InvokeHealthUpdateCallback();
                EnableEnemyUI();
            }
            else
            {
                BossArea.instance.boss.UnsetHealthListener();
                BossArea.instance.boss.EnemyUI.SetEnemyStatsVisible(false);
            }

            if (LevelManager.instance != null)
            {
                if (LevelManager.instance.bossArenaInsideWalls != null) LevelManager.instance.bossArenaInsideWalls.SetActive(isVisible);
                if (LevelManager.instance.bossArenaOusideWalls != null) LevelManager.instance.bossArenaOusideWalls.SetActive(!isVisible);
            }
        }

        private void EnableEnemyUI()
        {
            if (BossArea.instance == null) return;
            if (BossArea.instance.boss.EnemyHealth.health.Value <= 0)
                Invoke(nameof(EnableEnemyUI), .1f);
            else
                BossArea.instance.boss.EnemyUI.SetEnemyStatsVisible(true);
        }
    }
}
