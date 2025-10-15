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
        protected EnemyAttackEnableColliders enemyAttackEnableColliders;
        protected SwitchStateAnimator switchStateAnimator;
        protected ChangeHealthValuesAnimator changeHealthValuesAnimator;

        public BaseAI BaseAI { get => baseAI; }
        public EnemyHealth EnemyHealth { get => enemyHealth; }
        public EnemyUI EnemyUI { get => enemyUI; }
        public AnimatorController AnimatorController { get => animatorController; }
        public EnemyAttackEnableColliders EnemyAttackEnableColliders { get => enemyAttackEnableColliders; }
        public EnemyAttackSpawner EnemyAttackSpawner { get => enemyAttackSpawner; }
        public SwitchStateAnimator SwitchStateAnimator { get => switchStateAnimator; }
        public ChangeHealthValuesAnimator ChangeHealthValuesAnimator { get => changeHealthValuesAnimator; }

        protected virtual void OnEnable()
        {
            enemyUI = GetComponent<EnemyUI>();
            baseAI = GetComponent<BaseAI>();
            enemyHealth = GetComponent<EnemyHealth>();
            enemyNetwork = GetComponent<EnemyNetwork>();
            animatorController = GetComponentInChildren<AnimatorController>();
            enemyAttackEnableColliders = GetComponentInChildren<EnemyAttackEnableColliders>();
            enemyAttackSpawner = GetComponentInChildren<EnemyAttackSpawner>();
            switchStateAnimator = GetComponentInChildren<SwitchStateAnimator>();
            changeHealthValuesAnimator = GetComponentInChildren<ChangeHealthValuesAnimator>();

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

            baseAI.EnemyController = this;
            baseAI.Init();

            animatorController.Agent = baseAI.Agent;
            animatorController.Init();

            if(switchStateAnimator != null)
                switchStateAnimator.EnemyController = this;

            if (changeHealthValuesAnimator != null)
                changeHealthValuesAnimator.enemyHealth = enemyHealth;


            if (enemyAttackSpawner != null)
            {
                enemyAttackSpawner.OriginHealth = enemyHealth;
            }
        }

        public void SetHealthValuesInSlider()
        {
            enemyUI.HealthValueToSliderValue(enemyHealth.Health, enemyHealth.MaxHealth);
        }

        public void SetHealthListener() =>
            enemyHealth.onHealthChanged += enemyUI.HealthValueToSliderValue;

        public void UnsetHealthListener() =>
            enemyHealth.onHealthChanged -= enemyUI.HealthValueToSliderValue;

        //protected virtual void Die()
        //{
        //    if (!NetworkManager.Singleton.IsServer) return;

        //    baseAI.Die();
        //    baseAI.enabled = false;
        //    enemyAttackEnableColliders.DestroyAllColliders();
        //    if (enemyAttackSpawner != null)
        //    {
        //        enemyAttackSpawner.enabled = false;
        //    }
        //    animatorController.enabled = false;
        //    DieServerRpc();
        //    Destroy(enemyHealth);
        //}

        //[ServerRpc]
        //public void DieServerRpc()
        //{
        //    DieClientRpc();
        //}

        //[ClientRpc]
        //public void DieClientRpc()
        //{
        //    enemyHealth.InvokeHealthUpdateCallback();
        //    enemyHealth.enabled = false;
        //    if (enemyUI != null)
        //    {
        //        enemyUI.SetEnemyStatsVisible(false);
        //        enemyUI.enabled = false;
        //    }
        //}
    }
}
