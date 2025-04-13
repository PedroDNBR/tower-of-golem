using TW;
using Unity.Netcode;
using UnityEngine;

public class TEMPhostjoin : MonoBehaviour
{
    public GameObject bossPrefab;
    public Transform bossSpawner;

    public void Host() => NetworkManager.Singleton.StartHost();

    public void Join() => NetworkManager.Singleton.StartClient();

    public void SpawnBoss()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        var boss = Instantiate(bossPrefab, bossSpawner.position, bossSpawner.rotation);
        var bossNetworkObject = boss.GetComponent<NetworkObject>();
        bossNetworkObject.Spawn();
    }
}
