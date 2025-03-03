using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TW
{
    public class EnemyController : MonoBehaviour
    {
        private BaseAI baseAI;
        private EnemyHealth enemyHealth;
        private EnemyUI enemyUI;
        protected AnimatorController animatorController;
        private EnemyAttackSpawner enemyAttackSpawner;

        public BaseAI BaseAI { get => baseAI; }
        public EnemyHealth EnemyHealth { get => enemyHealth; }
        public EnemyUI EnemyUI { get => enemyUI; }
        public AnimatorController AnimatorController { get => animatorController; }


        private void Awake()
        {
            baseAI = GetComponent<BaseAI>();
            baseAI.EnemyController = this;
            baseAI.Init();
            
            enemyHealth = GetComponent<EnemyHealth>();
            enemyHealth.EnemyController = this;
            enemyHealth.Dead += Die;

            enemyUI = GetComponent<EnemyUI>();
            animatorController = GetComponentInChildren<AnimatorController>();
            animatorController.Agent = baseAI.Agent;
            animatorController.Init();

            enemyAttackSpawner = GetComponentInChildren<EnemyAttackSpawner>();
            enemyAttackSpawner.OriginHealth = enemyHealth;
            baseAI.playerFound += enemyAttackSpawner.SetPlayerAsAttackTarget;
        }

        public void SetHealthValuesInSlider() => enemyUI.HealthValueToSliderValue(enemyHealth.Health, enemyHealth.MaxHealth);

        public void SetHealthListener() =>
            enemyHealth.HealthChanged += enemyUI.HealthValueToSliderValue;

        public void UnsetHealthListener() =>
            enemyHealth.HealthChanged -= enemyUI.HealthValueToSliderValue;

        private void Die()
        {
            baseAI.Die();
            baseAI.enabled = false;
            enemyHealth.enabled = false;
            enemyUI.SetEnemyStatsVisible(false);
            enemyUI.enabled = false;
            animatorController.enabled = false;
            enemyAttackSpawner.enabled = false;
            Destroy(enemyHealth);
            Destroy(baseAI);
            animatorController.PlayTargetAnimation("Dead", true);
            Destroy(animatorController);
            Destroy(enemyAttackSpawner);

            StartCoroutine(ResetScene());
        }

        IEnumerator ResetScene()
        {
            yield return new WaitForSeconds(3f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
