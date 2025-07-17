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
            Debug.Log($"IsServer {IsServer}");
            if (IsServer)
            {
                Debug.Log($"AICommander.Instance.allEnemies.Count {AICommander.Instance.allEnemies.Count}");
                for (int i = 0; i < AICommander.Instance.allEnemies.Count; i++)
                {
                    AICommander.Instance.allEnemies[i].enemyController.EnemyHealth.Dead += MinionDied;
                    Debug.Log($"add listener {AICommander.Instance.allEnemies[i]}");
                }
            }

            BossArea.instance.gameObject.SetActive(false);
        }

        private void MinionDied()
        {
            currentDeadMinions++;

            if(currentDeadMinions == minMinionDeath)
            {
                Debug.Log("Server Starts boss area");
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
            Debug.Log("Client Starts boss area");
            BossArea.instance.gameObject.SetActive(true);
            //LevelManager.instance.bossArenaLeftDoor.SetActive(false);
            //LevelManager.instance.bossArenaRightDoor.SetActive(false);
            await LevelManager.instance.OpenDoorsLerp(1f);
        }

    }
}
