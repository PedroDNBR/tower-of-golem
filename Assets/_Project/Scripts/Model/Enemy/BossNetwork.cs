using UnityEngine;
using Unity.Netcode;

namespace TW
{
    public class BossNetwork : NetworkBehaviour
    {
        EnemyController enemyController;
        EnemyUI enemyUI;
        EnemyHealth enemyHealth;


        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            enemyController = GetComponent<EnemyController>();
            enemyUI = GetComponent<EnemyUI>();
            enemyHealth = GetComponent<EnemyHealth>();

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
        public void DieServerRpc()
        {
            UnsetUI();
            DieClientRpc();
        }

        [ClientRpc]
        public void DieClientRpc()
        {
            Debug.Log("BOSS DIED");
            UnsetUI();
        }

        private void UnsetUI()
        {
            Destroy(enemyController.BossArea.gameObject);
            enemyUI.HealthValueToSliderValue(0, enemyHealth.MaxHealth);
            enemyController.UnsetHealthListener();
        }
    }

}