using Unity.Netcode;
using UnityEngine;

namespace TW
{
    public abstract class EnemyNetwork : NetworkBehaviour
    {
        protected EnemyController enemyController;
        protected EnemyUI enemyUI;
        protected EnemyHealth enemyHealth;
        protected AnimatorController animatorController;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            enemyController = GetComponent<EnemyController>();
            enemyUI = GetComponent<EnemyUI>();
            enemyHealth = GetComponent<EnemyHealth>();
            animatorController = GetComponentInChildren<AnimatorController>();

            if(enemyUI != null) enemyUI.enabled = IsServer;
            enemyHealth.enabled = IsServer;
            animatorController.enabled = IsServer;

            if (IsServer) return;

            var damageColliders1 = GetComponentsInChildren<DealDamageWhenTriggerEnter>();
            var damageColliders2 = GetComponentsInChildren<ShouldReceiveDamage>();

            foreach (var damage in damageColliders1) 
                damage.enabled = false;

            foreach (var damage in damageColliders2)
                damage.enabled = false;
        }

        public void Die()
        {
            DieServerRpc();
        }

        [ServerRpc]
        public virtual void DieServerRpc()
        {
            DieClientRpc();
        }

        [ClientRpc]
        public virtual void DieClientRpc()
        {
        }
    }
}