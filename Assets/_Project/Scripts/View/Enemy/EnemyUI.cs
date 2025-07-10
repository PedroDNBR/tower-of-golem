using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TW
{
    public class EnemyUI : MonoBehaviour
    {
        [SerializeField]
        private Transform enemyHUD;

        [SerializeField]
        private TextMeshProUGUI enemyNameText;

        [SerializeField]
        private Slider enemyHealthSlider;

        public Transform EnemyHUD { get => enemyHUD; set => enemyHUD = value; }

        public TextMeshProUGUI EnemyNameText { get => enemyNameText; set => enemyNameText = value; }

        public Slider EnemyHealthSlider { get => enemyHealthSlider; set => enemyHealthSlider = value; }

        public void HealthValueToSliderValue(float current, float max)
        {
            if (enemyHealthSlider == null) return;
            UIUtils.ConvertToSliderValue(enemyHealthSlider, current, max);
        }

        public void SetEnemyStats(ref EnemyController enemy) => enemyNameText.text = enemy.name;

        public void SetEnemyStatsVisible(bool isVisible)
        {
            if(enemyHUD != null) enemyHUD.gameObject.SetActive(isVisible);
        }
    }
}
