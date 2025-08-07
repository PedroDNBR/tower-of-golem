using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace TW
{
    public class SpectatorNetwork : NetworkBehaviour
    {
        [SerializeField]
        NetworkObject networkObject;

        [SerializeField]
        PlayerCamera playerCamera;

        [SerializeField]
        SpectatorController spectatorController;

        [SerializeField]
        GameObject cameraObj;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            networkObject = GetComponent<NetworkObject>();

            spectatorController.enabled = IsOwner;
            playerCamera.enabled = IsOwner;
            cameraObj.SetActive(IsOwner);

            gameObject.name = gameObject.name + " " + NetworkObjectId;
        }
    }
}
