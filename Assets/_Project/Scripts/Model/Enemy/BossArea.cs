using System.Collections.Generic;
using UnityEngine;

namespace TW
{
    public class BossArea : MonoBehaviour
    {
        [SerializeField]
        public EnemyController boss;

        private List<PlayerController> playersInArea = new List<PlayerController>();

        private void SetBossUIInPlayerVisible(ref PlayerController playerController, bool isVisible)
        {
            boss.EnemyUI.EnemyHealthSlider = playerController.PlayerUI.BossHealthSlider;
            boss.EnemyUI.EnemyHUD = playerController.PlayerUI.BossHUD;
            boss.EnemyUI.EnemyNameText = playerController.PlayerUI.BossNameText;
            boss.EnemyUI.SetEnemyStatsVisible(isVisible);
            boss.EnemyUI.SetEnemyStats(ref boss);
            if (isVisible)
            {
                boss.SetHealthListener();
                boss.SetHealthValuesInSlider();
            }
            else 
                boss.UnsetHealthListener();
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerController player = other.gameObject.GetComponentInParent<PlayerController>();
            if (player == null) return;

            if(!playersInArea.Contains(player))
                playersInArea.Add(player);

            SetBossUIInPlayerVisible(ref player, true);
        }

        private void OnTriggerExit(Collider other)
        {
            PlayerController player = other.gameObject.GetComponentInParent<PlayerController>();
            if (player == null) return;

            if (!playersInArea.Contains(player))
                playersInArea.Remove(player);

            SetBossUIInPlayerVisible(ref player, false);
        }
    }
}
