using Unity.Netcode;
using UnityEngine;

namespace TW
{
    public class PlayerNetwork : NetworkBehaviour
    {
        [SerializeField]
        GameObject canvas;

        [SerializeField]
        GameObject cameraObj;

        [SerializeField]
        PlayerMovement playerMovement;

        [SerializeField]
        PlayerController playerController;

        [SerializeField]
        public PlayerCamera playerCamera;

        [SerializeField]
        PlayerSpell playerSpell;

        [SerializeField]
        PlayerHealth playerHealth;

        [SerializeField]
        PlayerUI playerUI;

        [SerializeField]
        PlayerDealDamageOnCollision onCollision;

        [SerializeField]
        NetworkObject networkObject;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            networkObject = GetComponent<NetworkObject>();

            playerController.enabled = IsOwner;
            playerCamera.enabled = IsOwner;
            canvas.SetActive(IsOwner);
            cameraObj.SetActive(IsOwner);
            playerSpell.enabled = IsOwner || IsServer;
            playerMovement.enabled = IsOwner || IsServer;
            playerUI.enabled = IsOwner;
            playerHealth.enabled = IsOwner || IsServer;

            gameObject.name = gameObject.name + " " + NetworkObjectId;

            onCollision.enabled = true;

            //if(IsServer)
            //{
            //    //playerController.SetComponentsVariables();
            //    //playerController.Init();
            //}

            BossArea.instance.BossSpawned += () => EnableBossUIServerRpc(true);
        }

        [ServerRpc(RequireOwnership = false)]
        private void EnableBossUIServerRpc(bool isVisible)
        {
            EnableBossUI(isVisible);
            EnableBossUIClientRpc(isVisible);
        }

        [ClientRpc(RequireOwnership = false)]
        private void EnableBossUIClientRpc(bool isVisible)
        {
            EnableBossUI(isVisible);
        }

        private void EnableBossUI(bool isVisible)
        {
            if (networkObject.IsLocalPlayer)
            {
                if(BossArea.instance.boss == null)
                {
                    if (isVisible)
                        Invoke(nameof(SetBossUIInPlayerVisibleTrue), .2f);
                    else
                        Invoke(nameof(SetBossUIInPlayerVisibleFalse), .2f);
                }
                else
                {
                    playerUI.SetBossUIInPlayerVisible(isVisible);
                }
            }
        }

        private void SetBossUIInPlayerVisibleTrue()
        {
            if(BossArea.instance.boss == null)
                Invoke(nameof(SetBossUIInPlayerVisibleTrue), .2f);
            else
                playerUI.SetBossUIInPlayerVisible(true);
        }

        private void SetBossUIInPlayerVisibleFalse()
        {
            if (BossArea.instance.boss == null)
                Invoke(nameof(SetBossUIInPlayerVisibleFalse), .2f);
            else
                playerUI.SetBossUIInPlayerVisible(false);
        }

        public void HandlePlayerDestroyAndSpectator()
        {
            HandlePlayerDestroyAndSpectatorServerRpc();
        }

        [ServerRpc]
        public void HandlePlayerDestroyAndSpectatorServerRpc()
        {
            Debug.Log("HandlePlayerDeathAndSpectatorServerRpc");
            if (!IsServer) return;
            Debug.Log("HandlePlayerDeathAndSpectatorServerRpc is server");
            ulong id = networkObject.OwnerClientId;
            ((NetworkGameManager)NetworkManager.Singleton).SpawnSpectator(id);
            ((NetworkGameManager)NetworkManager.Singleton).DespawnPlayer(networkObject);
        }
    }
}
