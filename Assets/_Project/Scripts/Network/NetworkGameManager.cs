using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
namespace TW
{
    public class NetworkGameManager : NetworkManager
    {
        [SerializeField]
        public GameObject newPlayerPrefab;

        public void SpawnPlayers()
        {
            if (!IsServer) return;
            foreach (var client in ConnectedClientsIds)
            {
                Debug.Log($"Spawn Client {client}");
                GameObject newPlayer = Instantiate(newPlayerPrefab, Vector3.zero, Quaternion.identity);
                newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(client, true);
            }
        }
    }
}

