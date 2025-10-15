using Unity.Netcode;
using UnityEngine;

namespace TW
{
    public class EnableBossfightOnDefeatingMinions : NetworkBehaviour
    {
        [SerializeField]
        int minMinionDeath = 10;

        int currentDeadMinions = 0;

        private void Start()
        {
            Invoke(nameof(SetListeners), 0.1f);
        }

        private void SetListeners()
        {
            if (IsServer)
            {
                NetworkGameManager networkGameManager = (NetworkGameManager)NetworkManager.Singleton;
                foreach (var item in networkGameManager.mobs)
                {
                    networkGameManager.mobs[item.Key].GetComponent<EnemyController>().EnemyHealth.onDeath += MinionDied;
                }
            }

            BossArea.instance.gameObject.SetActive(false);
        }

        private void MinionDied()
        {
            currentDeadMinions++;

            if(currentDeadMinions == minMinionDeath)
            {
                EnableBossfightServerRpc();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void EnableBossfightServerRpc()
        {
            EnableBossfight();
            EnableBossfightClientRpc();
        }

        [ClientRpc(RequireOwnership = false)]
        private void EnableBossfightClientRpc()
        {
            EnableBossfight();
        }

        private async void EnableBossfight()
        {
            BossArea.instance.gameObject.SetActive(true);
            //LevelManager.instance.bossArenaLeftDoor.SetActive(false);
            //LevelManager.instance.bossArenaRightDoor.SetActive(false);
            await LevelManager.instance.OpenDoorsLerp(1f);
        }

    }
}
