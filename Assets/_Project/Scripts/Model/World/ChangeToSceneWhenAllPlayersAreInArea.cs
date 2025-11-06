using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace TW
{
    public class ChangeToSceneWhenAllPlayersAreInArea : PlayerInAreaCounter
    {
        public string sceneName;

        private void OnTriggerEnter(Collider other)
        {
            PlayerController player = other.gameObject.GetComponentInParent<PlayerController>();
            if (player == null) return;

            AddPlayerToArea(player);
        }

        protected override void AllPlayersInArena()
        {
            if (((NetworkGameManager)NetworkManager.Singleton).IsServer)
            {
                NetworkManager.Singleton.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            PlayerController player = other.gameObject.GetComponentInParent<PlayerController>();
            if (player == null) return;

            RemovePlayerFromArea(player);
        }
    }
}
