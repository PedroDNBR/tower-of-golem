using System.Collections;
using Unity.Netcode;
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
        private BossNetwork bossNetwork;
        private BossArea bossArea;

        public BaseAI BaseAI { get => baseAI; }
        public EnemyHealth EnemyHealth { get => enemyHealth; }
        public EnemyUI EnemyUI { get => enemyUI; }
        public AnimatorController AnimatorController { get => animatorController; }
        public BossArea BossArea { get => bossArea; }

        private void OnEnable()
        {
            enemyUI = GetComponent<EnemyUI>();
            baseAI = GetComponent<BaseAI>();
            enemyHealth = GetComponent<EnemyHealth>();
            bossArea = FindObjectOfType<BossArea>();
            bossNetwork = GetComponent<BossNetwork>();
            animatorController = GetComponentInChildren<AnimatorController>();
            enemyAttackSpawner = GetComponentInChildren<EnemyAttackSpawner>();

            if (DialogueMenu.instance != null)
            {
                baseAI.enabled = false;
                DialogueMenu.instance.OnDialogStarted += () => baseAI.enabled = false;
                DialogueMenu.instance.OnDialogEnded += () => baseAI.enabled = true;
            }
        }

        private void Start()
        {

            enemyHealth.EnemyController = this;

            if (bossArea != null) bossArea.boss = this;


            if (!NetworkManager.Singleton.IsServer) return;

            enemyHealth.Dead += Die;

            baseAI.EnemyController = this;
            baseAI.Init();

            animatorController.Agent = baseAI.Agent;
            animatorController.Init();

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

            if (!NetworkManager.Singleton.IsServer) return;
            bossNetwork.Die();
            baseAI.Die();
            baseAI.enabled = false;
            enemyHealth.InvokeHealthUpdateCallback();
            enemyUI.SetEnemyStatsVisible(false);
            enemyHealth.enabled = false;
            enemyUI.enabled = false;
            animatorController.enabled = false;
            enemyAttackSpawner.enabled = false;
            Destroy(enemyHealth);
            Destroy(baseAI);
            animatorController.PlayTargetAnimation("Dead", true);
            Destroy(animatorController);
            Destroy(enemyAttackSpawner);
            // StartCoroutine(ResetScene());
        }

        IEnumerator ResetScene()
        {
            yield return new WaitForSeconds(3f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
