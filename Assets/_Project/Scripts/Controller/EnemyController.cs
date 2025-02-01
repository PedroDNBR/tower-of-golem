using System;
using UnityEngine;

namespace TW
{
    public class EnemyController : MonoBehaviour
    {
        private BaseAI baseAI;
        private EnemyHealth enemyHealth;
        private EnemyUI enemyUI;
        protected AnimatorController animatorController;

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

            enemyUI = GetComponent<EnemyUI>();
            animatorController = GetComponentInChildren<AnimatorController>();
            animatorController.Agent = baseAI.Agent;
            animatorController.Init();
        }

        public void SetHealthValuesInSlider() => enemyUI.HealthValueToSliderValue(enemyHealth.Health, enemyHealth.MaxHealth);

        public void SetHealthListener() =>
            enemyHealth.HealthChanged += enemyUI.HealthValueToSliderValue;

        public void UnsetHealthListener() =>
            enemyHealth.HealthChanged -= enemyUI.HealthValueToSliderValue;
    }
}
