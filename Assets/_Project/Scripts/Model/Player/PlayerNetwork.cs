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
        PlayerCamera playerCamera;

        [SerializeField]
        PlayerSpell playerSpell;

        [SerializeField]
        PlayerHealth playerHealth;

        [SerializeField]
        PlayerUI playerUI;

        [SerializeField]
        PlayerDealDamageOnCollision onCollision;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            playerController.enabled = IsOwner;
            playerCamera.enabled = IsOwner;
            canvas.SetActive(IsOwner);
            cameraObj.SetActive(IsOwner);
            playerSpell.enabled = IsOwner || IsServer;
            playerMovement.enabled = IsOwner || IsServer;
            playerUI.enabled = IsOwner;
            playerHealth.enabled = IsOwner || IsServer;

            gameObject.name = gameObject.name + " " + NetworkObjectId;

            onCollision.enabled = IsServer;

            if(IsServer)
            {
                //playerController.SetComponentsVariables();
                //playerController.Init();
            }
        }
    }
}
