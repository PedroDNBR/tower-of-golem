using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace TW
{
    public class PlayerInAreaCounter : MonoBehaviour
    {
        protected List<PlayerController> playersInArea = new List<PlayerController>();

        protected void AddPlayerToArea(PlayerController player)
        {
            if (!playersInArea.Contains(player))
                playersInArea.Add(player);

            if (playersInArea.Count == ((NetworkGameManager)NetworkManager.Singleton).ConnectedClientsIds.Count)
            {
                AllPlayersInArena();
            }
        }

        protected void RemovePlayerFromArea(PlayerController player)
        {
            if (!playersInArea.Contains(player))
                playersInArea.Remove(player);
        }

        protected virtual void AllPlayersInArena()
        {

        }
    }
}
