using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine.SceneManagement;

namespace TW
{
    public class NetworkGameManager : NetworkManager
    {
        [SerializeField]
        public GameObject newPlayerPrefab;

        [SerializeField]
        public GameObject spectatorPrefab;

        [SerializeField]
        public GameObject dialogue;

        public Dictionary<ulong, NetworkObject> players = new Dictionary<ulong, NetworkObject>();

        public Dictionary<ulong, NetworkObject> spectators = new Dictionary<ulong, NetworkObject>();

        public Dictionary<ulong, NetworkObject> mobs = new Dictionary<ulong, NetworkObject>();

        private void Start()
        {
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += (UnityEngine.SceneManagement.Scene scene1, UnityEngine.SceneManagement.Scene scene2) =>
            {
                ClearVariables();
            };
        }

        public void SpawnPlayers(Transform[] spawnPoints = null)
        {
            if (!IsServer) return;

            GameObject dialogueobj = Instantiate(dialogue);

            dialogueobj.GetComponent<NetworkObject>().Spawn(true);
            int i = 0;
            foreach (var client in ConnectedClientsIds)
            {
                Vector3 position = Vector3.zero;
                Quaternion rotation = Quaternion.identity;
                if (spawnPoints != null && spawnPoints.Length > 0)
                {
                    position = spawnPoints[i].transform.position;
                    rotation = spawnPoints[i].transform.rotation;
                    if (i >= spawnPoints.Length)
                        i = 0;
                    else
                        i++;
                }
                GameObject newPlayer = Instantiate(newPlayerPrefab, position, rotation);
                NetworkObject networkObject = newPlayer.GetComponent<NetworkObject>();
                networkObject.SpawnAsPlayerObject(client, true);
                players.Add(client, networkObject);

                if (ConnectedClients.Count > 1) networkObject.GetComponentInChildren<BaseHealth>().Dead += CheckAllPlayersDied;
            }
        }

        public void DespawnPlayer(NetworkObject player)
        {
            players.Remove(player.OwnerClientId);
            player.Despawn();
        }

        public void SpawnSpectator(ulong client)
        {
            GameObject newPlayer = Instantiate(spectatorPrefab);
            NetworkObject networkObject = newPlayer.GetComponent<NetworkObject>();
            networkObject.SpawnAsPlayerObject(client, true);
            spectators.Add(client, networkObject);
        }

        public void DespawnSpectator(NetworkObject spectator)
        {
            spectators.Remove(spectator.OwnerClientId);
            spectator.Despawn();
        }

        public GameObject SpawnMob(GameObject mob, Transform spawnPoint)
        {
            if (!IsServer) return null;
            GameObject newMob = Instantiate(mob, spawnPoint.position, spawnPoint.rotation);
            NetworkObject networkObject = newMob.GetComponent<NetworkObject>();
            networkObject.Spawn(true);
            mobs.Add(networkObject.NetworkObjectId, networkObject);
            newMob.GetComponent<EnemyHealth>().Dead += () => DespawnMob(networkObject, true);

            return newMob;
        }

        public void DespawnMob(NetworkObject network, bool destroy)
        {
            if (!IsServer) return;
            network.Despawn(destroy);
        }

        private void ClearVariables()
        {
            foreach (var item in players)
            {
                if (item.Value != null)
                {
                    item.Value.Despawn();
                    if (item.Value != null) Destroy(item.Value.gameObject);
                }
            }
            players = new Dictionary<ulong, NetworkObject>();

            foreach (var item in spectators)
            {
                if (item.Value != null)
                {
                    item.Value.Despawn();
                    if (item.Value != null) Destroy(item.Value.gameObject);
                }
            }
            spectators = new Dictionary<ulong, NetworkObject>();

            foreach (var item in mobs)
            {
                if (item.Value != null)
                {
                    item.Value.Despawn();
                    if (item.Value != null) Destroy(item.Value.gameObject);
                }
            }
            mobs = new Dictionary<ulong, NetworkObject>();
        }

        private void CheckAllPlayersDied()
        {
            if(!IsServer) return;

            Debug.Log("someone died");

            var aliveKeys = players
                .Where(p => p.Value != null && p.Value.GetComponentInChildren<PlayerController>() != null && p.Value.GetComponentInChildren<PlayerController>().enabled)
                .ToList();

            Debug.Log($"{aliveKeys.Count} aliveKeys.Count");

            if (aliveKeys.Count <= 0)
            {
                Invoke(nameof(RestartScene), 2);
            }
        }

        private void RestartScene()
        {
            SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }
    }
}

