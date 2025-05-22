using UnityEngine;
using Unity.Netcode;

namespace TW
{
    public class NetworkGameManager : NetworkManager
    {
        [SerializeField]
        public GameObject newPlayerPrefab;

        [SerializeField]
        public GameObject dialogue;

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
                newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(client, true);
            }
        }

        public void SpawnMob(GameObject mob, Transform spawnPoint)
        {
            if (!IsServer) return;
            GameObject newMob = Instantiate(mob, spawnPoint.position, spawnPoint.rotation);
            newMob.GetComponent<NetworkObject>().Spawn(true);
        }
    }
}

