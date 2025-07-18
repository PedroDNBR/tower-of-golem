using Unity.Netcode;

namespace TW
{
    public class BossNetwork : EnemyNetwork
    {
        private void UnsetUI()
        {
            BossController bossController = (BossController)enemyController;
            if (bossController.BossArea != null)
                Destroy(bossController.BossArea.gameObject);

            if (enemyUI == null) return;

            enemyUI.HealthValueToSliderValue(0, enemyHealth.MaxHealth);

            bossController.UnsetHealthListener();
        }

        protected override void Die()
        {
            base.Die();
            DieServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        public void DieServerRpc()
        {
            UnsetUI();
            DieClientRpc();
        }

        [ClientRpc(RequireOwnership = false)]
        public void DieClientRpc()
        {
            UnsetUI();
        }
    }
}
