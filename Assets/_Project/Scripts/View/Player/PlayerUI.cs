using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace TW
{
    public class PlayerUI : BaseUI
    {
        [Header("Player Stats")]
        [SerializeField]
        private Slider healthSlider;

        [SerializeField]
        private Slider staminaSlider;

        [Header("Boss Stats")]
        [SerializeField]
        private TextMeshProUGUI bossNameText;

        [SerializeField]
        private Slider bossHealthSlider;

        [SerializeField]
        private Transform bossHUD;

        [SerializeField]
        private Transform deathScreen;

        public Transform BossHUD { get => bossHUD; }

        public TextMeshProUGUI BossNameText { get => bossNameText; }

        public Slider BossHealthSlider { get => bossHealthSlider; }

        public PlayerController playerController;

        protected override void Start()
        {
            base.Start();
            playerController.PlayerHealth.Dead += () => deathScreen.gameObject.SetActive(true);
        }

        public void HealthValueToSliderValue(float current, float max)
        {
            UIUtils.ConvertToSliderValue(healthSlider, current, max);
        }

        public void StaminaTimeToSliderValue(float current, float max)
        {
            UIUtils.ConvertToSliderValue(staminaSlider, current, max);
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
