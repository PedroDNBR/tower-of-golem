using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;

public class TEMPhostjoin : MonoBehaviour
{
    public GameObject bossPrefab;
    public Transform bossSpawner;

    public TMP_InputField input;
    public TMP_InputField joinCodeInput;

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void Host()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Join Code: " + joinCode);
            joinCodeInput.text = joinCode;
            GUIUtility.systemCopyBuffer = joinCode;

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

            SpawnBoss();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
        NetworkManager.Singleton.StartHost();
    }

    public async void Join()
    {
        string joinCode = input.text;
        try
        {
            Debug.Log("Joining Relay with " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }

    public void SpawnBoss()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        var boss = Instantiate(bossPrefab, bossSpawner.position, bossSpawner.rotation);
        var bossNetworkObject = boss.GetComponent<NetworkObject>();
        bossNetworkObject.Spawn();
    }
}
