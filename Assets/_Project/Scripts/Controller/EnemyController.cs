using Unity.Netcode;
using UnityEngine;

namespace TW
{
    public abstract class EnemyController : MonoBehaviour
    {
        protected BaseAI baseAI;
        protected EnemyHealth enemyHealth;
        protected EnemyUI enemyUI;
        protected AnimatorController animatorController;
        protected EnemyAttackSpawner enemyAttackSpawner;
        protected EnemyNetwork enemyNetwork;

        public BaseAI BaseAI { get => baseAI; }
        public EnemyHealth EnemyHealth { get => enemyHealth; }
        public EnemyUI EnemyUI { get => enemyUI; }
        public AnimatorController AnimatorController { get => animatorController; }

        protected virtual void OnEnable()
        {
            enemyUI = GetComponent<EnemyUI>();
            baseAI = GetComponent<BaseAI>();
            enemyHealth = GetComponent<EnemyHealth>();
            enemyNetwork = GetComponent<EnemyNetwork>();
            animatorController = GetComponentInChildren<AnimatorController>();
            enemyAttackSpawner = GetComponentInChildren<EnemyAttackSpawner>();

            if (DialogueMenu.instance != null)
            {
                baseAI.enabled = false;
                DialogueMenu.instance.OnDialogStarted += () => baseAI.enabled = false;
                DialogueMenu.instance.OnDialogEnded += () => baseAI.enabled = true;
            }
        }

        protected virtual void Start()
        {
            enemyHealth.EnemyController = this;


            if (!NetworkManager.Singleton.IsServer) return;

            enemyHealth.Dead += Die;

            baseAI.EnemyController = this;
            baseAI.Init();

            animatorController.Agent = baseAI.Agent;
            animatorController.Init();

            if(enemyAttackSpawner != null)
            {
                enemyAttackSpawner.OriginHealth = enemyHealth;
            }
        }

        public void SetHealthValuesInSlider() => enemyUI.HealthValueToSliderValue(enemyHealth.Health, enemyHealth.MaxHealth);

        public void SetHealthListener() =>
            enemyHealth.HealthChanged += enemyUI.HealthValueToSliderValue;

        public void UnsetHealthListener() =>
            enemyHealth.HealthChanged -= enemyUI.HealthValueToSliderValue;

        protected virtual void Die()
        {

            if (!NetworkManager.Singleton.IsServer) return;
            enemyNetwork.Die();
            baseAI.Die();
            baseAI.enabled = false;
            enemyHealth.InvokeHealthUpdateCallback();
            if (enemyUI != null)
            {
                enemyUI.SetEnemyStatsVisible(false);
                enemyUI.enabled = false;
            }
            enemyHealth.enabled = false;
            animatorController.enabled = false;
            if (enemyAttackSpawner != null)
            {
                enemyAttackSpawner.enabled = false;
                Destroy(enemyAttackSpawner);
            }
            Destroy(enemyHealth);
            Destroy(baseAI);
            animatorController.PlayTargetAnimation("Dead", true);
            Destroy(animatorController);
        }
    }
}
