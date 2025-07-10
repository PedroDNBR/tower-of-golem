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
            if (playerController == null || boss == null) return;
            if (!playerController.GetComponentInParent<PlayerNetwork>().IsLocalPlayer) return;
            boss.EnemyUI.EnemyHealthSlider = playerController.PlayerUI.BossHealthSlider;
            boss.EnemyUI.EnemyHUD = playerController.PlayerUI.BossHUD;
            boss.EnemyUI.EnemyNameText = playerController.PlayerUI.BossNameText;
            boss.EnemyUI.SetEnemyStats(ref boss);
            Debug.Log($"isVisible {isVisible}");
            if (isVisible)
            {
                boss.SetHealthListener();
                boss.SetHealthValuesInSlider();
                boss.EnemyHealth.InvokeHealthUpdateCallback();
            }
            else 
                boss.UnsetHealthListener();

            if (isVisible)
                EnableEnemyUI();
            else
                boss.EnemyUI.SetEnemyStatsVisible(false);
        }

        private void EnableEnemyUI()
        {
            if (boss.EnemyHealth.health.Value <= 0)
                Invoke(nameof(EnableEnemyUI), .1f);
            else
                boss.EnemyUI.SetEnemyStatsVisible(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerController player = other.gameObject.GetComponentInParent<PlayerController>();
            if (player == null) return;

            if(!playersInArea.Contains(player))
                playersInArea.Add(player);

            SetBossUIInPlayerVisible(ref player, true);

            if (!boss.gameObject.activeSelf)
            {
                boss.gameObject.SetActive(true);
                boss.enabled = true;
                boss.BaseAI.enabled = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            PlayerController player = other.gameObject.GetComponentInParent<PlayerController>();
            if (player == null) return;

            if (!playersInArea.Contains(player))
                playersInArea.Remove(player);

            SetBossUIInPlayerVisible(ref player, false);
        }

        private void OnDestroy()
        {
            for (int i = 0; i < playersInArea.Count; i++)
            {
                PlayerController playerController = playersInArea[i];
                SetBossUIInPlayerVisible(ref playerController, false);
            }
            if(boss != null) boss.UnsetHealthListener();
        }
    }
}
