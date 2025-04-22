using Unity.Netcode;
using UnityEngine;

namespace TW
{
    public class SpawnNetworkPlayers : MonoBehaviour
    {
        void Start()
        {
            ((NetworkGameManager)NetworkManager.Singleton).SpawnPlayers();
        }
    }
}