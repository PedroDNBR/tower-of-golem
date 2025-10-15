using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace TW
{
    public abstract class EnemyNetwork : NetworkBehaviour
    {
        protected EnemyController enemyController;
        protected EnemyUI enemyUI;
        protected EnemyHealth enemyHealth;
        protected AnimatorController animatorController;
        protected Animator animator;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            enemyController = GetComponent<EnemyController>();
            enemyUI = GetComponent<EnemyUI>();
            enemyHealth = GetComponent<EnemyHealth>();
            animatorController = GetComponentInChildren<AnimatorController>();
            animator= GetComponentInChildren<Animator>();

            if(enemyUI != null) enemyUI.enabled = IsServer;
            enemyHealth.enabled = IsServer;
            enemyHealth.InitOnHealthChangedAction();
            animatorController.enabled = IsServer;


            if (IsServer) return;

            var damageColliders1 = GetComponentsInChildren<DealDamageWhenTriggerEnter>();
            var damageColliders2 = GetComponentsInChildren<ShouldReceiveDamage>();

            foreach (var damage in damageColliders1) 
                damage.enabled = false;

            foreach (var damage in damageColliders2)
                damage.enabled = false;
        }

        public override void OnDestroy()
        {
            Destroy(GetComponent<NetworkAnimator>());
            animator.gameObject.name = "EU MORRI";
            animator.transform.SetParent(null);
            animator.enabled = true;
            animator.SetBool(Constants.isBusyString, true);
            animator.CrossFade("Dead", 0);
            Destroy(animatorController);
            Invoke(nameof(DisableAnimator), 3);

            base.OnDestroy();
        }

        public override void OnNetworkDespawn()
        {
            Die();
            base.OnNetworkDespawn();
        }

        protected virtual void Die()
        {
            enemyController.BaseAI.Die();
            enemyController.BaseAI.enabled = false;
            enemyController.EnemyAttackEnableColliders?.DestroyAllColliders();
            if (enemyController.EnemyAttackSpawner != null)
            {
                enemyController.EnemyAttackSpawner.enabled = false;
            }
        }

        private void DisableAnimator()
        {
            animatorController.enabled = false;
            animator.enabled = false;
        }
    }
}