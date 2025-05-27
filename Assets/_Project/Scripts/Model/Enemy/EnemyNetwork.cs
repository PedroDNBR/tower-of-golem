using Unity.Netcode;

namespace TW
{
    public abstract class EnemyNetwork : NetworkBehaviour
    {
        protected EnemyController enemyController;
        protected EnemyUI enemyUI;
        protected EnemyHealth enemyHealth;

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