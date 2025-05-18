using TW;
using Unity.Netcode;
using UnityEngine;

public class SpawnMobs : MonoBehaviour
{
    [SerializeField] SpawnNetworkPlayers playerSpawner;

    [SerializeField] GameObject mobPrefab;
    [SerializeField] Transform spawnPoint;

    void OnEnable()
    {
        if (playerSpawner != null) 
            playerSpawner.OnSpawnedPlayers += SpawnEnemy;
    }

    private void SpawnEnemy()
    {
        if (NetworkManager.Singleton != null)
            ((NetworkGameManager)NetworkManager.Singleton).SpawnMob(mobPrefab, spawnPoint);
    }
}
