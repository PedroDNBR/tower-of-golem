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

        [ServerRpc]
        public override void DieServerRpc()
        {
            UnsetUI();
            DieClientRpc();
        }

        [ClientRpc]
        public override void DieClientRpc()
        {
            UnsetUI();
        }
    }
}
