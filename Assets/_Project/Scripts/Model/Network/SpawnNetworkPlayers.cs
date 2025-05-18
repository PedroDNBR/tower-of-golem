using Unity.Netcode;
using UnityEngine;
using System;

namespace TW
{
    public class SpawnNetworkPlayers : MonoBehaviour
    {
        [SerializeField] Transform[] spawnPoints;

        public Action OnSpawnedPlayers;

        void Start()
        {
            if(NetworkManager.Singleton != null)
            {
                ((NetworkGameManager)NetworkManager.Singleton).SpawnPlayers(spawnPoints);
                OnSpawnedPlayers?.Invoke();
            }
        }
    }
}