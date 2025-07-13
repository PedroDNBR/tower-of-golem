using Unity.Netcode;

namespace TW
{
    public class PlayerHealth : BaseHealth
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            Dead += StartDeath;
        }

        private void StartDeath()
        {
            DieServerRpc();
        }

        [ClientRpc(RequireOwnership = false)]
        private void DieClientRpc()
        {
            Die();
        }

        [ServerRpc(RequireOwnership = false)]
        private void DieServerRpc()
        {
            Die();
            DieClientRpc();
        }

        private void Die()
        {
            InvokeHealthUpdateCallback();
        }
    }
}
