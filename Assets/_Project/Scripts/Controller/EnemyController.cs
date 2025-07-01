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
        protected NetworkObject networkObject;
        protected EnemyAttackEnableColliders enemyAttackEnableColliders;

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
            networkObject = GetComponent<NetworkObject>();
            animatorController = GetComponentInChildren<AnimatorController>();
            enemyAttackEnableColliders = GetComponentInChildren<EnemyAttackEnableColliders>();
            enemyAttackSpawner = GetComponentInChildren<EnemyAttackSpawner>();

            if (DialogueMenu.instance != null)
            {
                baseAI.enabled = false;
                DialogueMenu.instance.OnDialogStarted += () => 
                { 
                    if(baseAI != null) baseAI.enabled = false; 
                };
                DialogueMenu.instance.OnDialogEnded += () =>
                {
                    if (baseAI != null) baseAI.enabled = true;
                };
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
            Destroy(baseAI);
            animatorController.PlayTargetAnimation("Dead", true);
            animatorController.enabled = false;
            Destroy(animatorController);
            enemyHealth.InvokeHealthUpdateCallback();
            if (enemyUI != null)
            {
                enemyUI.SetEnemyStatsVisible(false);
                enemyUI.enabled = false;
            }
            enemyHealth.enabled = false;
            Destroy(enemyHealth);
            enemyAttackEnableColliders.DestroyAllColliders();
            Destroy(enemyAttackEnableColliders);
            networkObject.Despawn(false);
            if (enemyAttackSpawner != null)
            {
                enemyAttackSpawner.enabled = false;
                Destroy(enemyAttackSpawner);
            }
        }
    }
}
